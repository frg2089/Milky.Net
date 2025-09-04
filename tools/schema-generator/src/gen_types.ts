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
      // _def: schema,
    };
    schema.def.options.forEach((option) => {
      const [discriminatorPropName, discriminatorPropType] = Object.entries((option as ZodObject).def.shape).find(([propertyName, propertyType]: [string, ZodType]) =>
        propertyType.def.type === "literal"
      )!
      
      const discriminatorValue = discriminatorPropType.def.values[0] as string
      const typeName = `${discriminatorValue}_union_${schemaName}`;

      const typeInfo = parseObjectType(
        option as ZodType,
        typeName
      ) as Types.ObjectTypeInfoData;
      typeInfo.baseType = schemaName ?? 'Unknown';

      types[typeName] = typeInfo;

      info.types[discriminatorValue] = typeName;
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
          const name = `${schemaName}_${propertyName}`
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

const ApiEndpoints: Record<string, Types.Api.ApiCategory> = {};
Object.entries(apiCategories).forEach(([categoryName, category]) => {
  ApiEndpoints[categoryName] = {
    name: category.name,
    apis: category.apis.map((i) => ({
      endpoint: i.endpoint,
      description: i.description,
      inputStruct: findTypeName(i.inputStruct as any) ?? null,
      outputStruct: findTypeName(i.outputStruct as any) ?? null,
    })),
  };
});

await fs.promises.mkdir(path.resolve('out'), { recursive: true });
fs.promises.writeFile(path.resolve('out', "MilkyTypes.json"), JSON.stringify(types, null, 2));
fs.promises.writeFile(
  path.resolve('out', "ApiEndpoints.json"),
  JSON.stringify(ApiEndpoints, null, 2)
);
fs.promises.writeFile(
  path.resolve('out', "Milky.props"),
  `<Project>
  <PropertyGroup>
    <MilkyVersion>${Milky.milkyVersion}</MilkyVersion>
    <MilkyPackageVersion>${Milky.milkyPackageVersion}</MilkyPackageVersion>
  </PropertyGroup>
</Project>`
);
