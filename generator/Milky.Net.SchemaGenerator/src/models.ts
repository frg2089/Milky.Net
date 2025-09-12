import * as fs from "node:fs";
import * as path from "node:path";
import * as Milky from "@saltify/milky-types";
import { apiCategories } from "@saltify/milky-types/api";
import { ZodObject, ZodType } from "zod";
import {
  isZodArray,
  isZodBoolean,
  isZodDefault,
  isZodEnum,
  isZodLazy,
  isZodLiteral,
  isZodNumber,
  isZodObject,
  isZodOptional,
  isZodString,
  isZodUnion,
} from "./zod_utils.js";
import { program } from "commander";

program.option(
  "-o, --out <path>",
  "中间 json 生成路径",
  path.resolve("out", "MilkyTypes.json")
);

program.parse();

const types: Record<string, Types.TypeInfoData> = {};
const enums: Array<Record<string, string> | Record<string, number>> = [];
const enumInfos: [string, Types.PropertyInfoData][] = [];

const findTypeName = (type: ZodType): string | undefined => {
  for (const key in Milky) {
    if (!Object.prototype.hasOwnProperty.call(Milky, key)) return;

    const element = (Milky as any)[key] as ZodType;
    if (element === type) return key;
  }
};

const parseSimpleTypeName = (schema: ZodType): string => {
  if (isZodEnum(schema)) {
    enums.push(schema.enum as any);
    return "enum";
  }

  if (isZodDefault(schema))
    return (
      parseSimpleTypeName(schema.def.innerType as ZodType) +
      " = " +
      (typeof schema.def.defaultValue === "string"
        ? `"${schema.def.defaultValue}"`
        : schema.def.defaultValue)
    );

  if (isZodOptional(schema))
    return parseSimpleTypeName(schema.def.innerType as ZodType) + "?";

  if (isZodArray(schema))
    return parseSimpleTypeName(schema.element as ZodType) + "[]";

  if (isZodLazy(schema)) schema = schema.unwrap() as ZodType;

  if (isZodString(schema)) return "string";

  if (isZodBoolean(schema)) return "boolean";

  if (isZodNumber(schema)) {
    if (schema.format === "safeint") {
      if (schema.minValue === 0) {
        // 无符号整数
        if (schema.maxValue == null) throw new Error("未设置无符号整数最大值");
        if (schema.maxValue <= 255) return "UInt8";
        if (schema.maxValue <= 65535) return "UInt16";
        if (schema.maxValue <= 4294967295) return "UInt32";
        return "UInt64";
      } else {
        // 有符号整数
        if (schema.minValue == null || schema.maxValue == null)
          throw new Error("未设置有符号整数最小值或最大值");
        if (schema.minValue >= -128 && schema.maxValue <= 127) return "Int8";
        if (schema.minValue >= -32768 && schema.maxValue <= 32767)
          return "Int16";
        if (schema.minValue >= -2147483648 && schema.maxValue <= 2147483647)
          return "Int32";
        return "Int64";
      }
    } else {
      return "number";
    }
  }

  return findTypeName(schema as any) ?? schema.def.type;
};

const parseObjectType = (schema: ZodType, schemaName: string) => {
  if (isZodUnion(schema)) {
    const info: Types.UnionTypeInfoData = {
      type: "union",
      description: schema.description,
      types: {},
      discriminator: (schema.def as any)["discriminator"],
      properties: {},
      // _def: schema,
    };

    // 取出第一个对象的所有非对象类型
    const props = Object.fromEntries(
      Object.entries((schema.def.options[0] as ZodObject).def.shape)
        .filter(
          ([_, type]) => !isZodObject(type) && type.def.type !== "literal"
        )
        .map(([name, info]) => [name, info as ZodType])
    );

    schema.def.options.forEach((option) => {
      // 遍历并删除不存在的属性
      const properties = Object.entries((option as ZodObject).def.shape);
      properties.forEach(([name, info]) => {
        if (props[name]?.def.type !== info.def.type) delete props[name];
      });
      Object.entries(props).forEach(([propertyName]) => {
        if (!properties.find(([name]) => name === propertyName))
          delete props[propertyName];
      });
    });

    Object.entries(props).forEach(([propertyName, propertyType]) => {
      info.properties[propertyName] = {
        type: parseSimpleTypeName(propertyType),
        description: propertyType.description,
      };
      if (info.properties[propertyName].type === "enum") {
        enumInfos.push([propertyName, info]);
        info.properties[propertyName].type = propertyName;
      }
    });

    schema.def.options.forEach((option) => {
      const properties = Object.entries((option as ZodObject).def.shape);
      const [discriminatorPropName, discriminatorPropType] = properties.find(
        ([propertyName, propertyType]) => propertyType.def.type === "literal"
      )!;
      const props = properties.filter(
        ([name]) => !info.properties[name] && info.discriminator !== name
      );
      if (
        !Array.isArray(discriminatorPropType.def.values) ||
        discriminatorPropType.def.values.length !== 1
      )
        throw new Error(
          `Discriminator property ${discriminatorPropName} must have exactly one value`
        );
      const discriminatorValue = discriminatorPropType.def.values[0] as string;

      (() => {
        if (props.length === 1) {
          const [propertyName, type] = props![0] as [string, ZodType];
          const name = findTypeName(type);
          if (name) {
            types[`${schemaName}<T>`] = {
              type: "generic",
              baseType: schemaName,
              genericPropertyName: propertyName,
            } as Types.GenericTypeInfoData;
            info.types[discriminatorValue] = `${schemaName}<${name}>`;
            return;
          }
        }
        const typeName = `${discriminatorValue}_${schemaName}`;

        const typeInfo = parseObjectType(
          option as ZodType,
          typeName
        ) as Types.ObjectTypeInfoData;
        typeInfo.baseType = schemaName ?? "Unknown";

        types[typeName] = typeInfo;

        info.types[discriminatorValue] = typeName;
      })();
    });

    return info;
  }
  if (isZodObject(schema)) {
    const typeInfo: Types.ObjectTypeInfoData = {
      type: "object",
      description: schema.description,
      properties: {},
    };

    Object.entries(schema.def.shape).forEach(
      ([propertyName, propertyType]: [string, ZodType]) => {
        const info: Types.PropertyInfoData = {
          type: parseSimpleTypeName(propertyType),
          description: propertyType.description,
        };

        if (isZodObject(propertyType)) {
          const name =
            findTypeName(propertyType) ?? `${schemaName}_${propertyName}`;
          const typeInfo = parseObjectType(propertyType, name);
          types[name] = typeInfo;
          info.type = name;
        }

        if (info.type === "enum") {
          enumInfos.push([propertyName, info]);
          info.type = propertyName;
        }

        if (isZodLiteral(propertyType)) {
          info.constants = propertyType.def.values as any;
        }

        typeInfo.properties[propertyName] = info;
      }
    );

    return typeInfo;
  }

  throw new Error("不受支持的类型");
};

Object.entries(Milky).forEach(([typeName, type]) => {
  // 排除 milkyVersion 和 milkyPackageVersion
  if (typeof type === "string") return;
  if (["ZBoolean", "ZInt32", "ZInt64", "ZString"].includes(typeName)) return;

  types[typeName] = parseObjectType(type as any, typeName);
});

enumInfos.forEach(([name, property], i) => {
  const item = enums[i];
  if (!item) return;

  const info: Types.EnumTypeInfoData = {
    type: "enum",
    description: property.description,
    items: item,
  };
  types[name] = info;
});

const options = program.opts();

await fs.promises.writeFile(
  path.resolve(options.out),
  JSON.stringify(types, null, 2)
);
