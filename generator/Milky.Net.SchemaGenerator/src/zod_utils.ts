import type {
  ZodArray,
  ZodBoolean,
  ZodDefault,
  ZodEnum,
  ZodLazy,
  ZodLiteral,
  ZodNullable,
  ZodNumber,
  ZodObject,
  ZodOptional,
  ZodPipe,
  ZodString,
  ZodType,
  ZodUnion
} from "zod";

export const isZodLazy = (schema: ZodType): schema is ZodLazy =>
  schema.def.type === "lazy";

export const isZodArray = (schema: ZodType): schema is ZodArray =>
  schema.def.type === "array";

export const isZodNumber = (schema: ZodType): schema is ZodNumber =>
  schema.def.type === "number";

export const isZodString = (schema: ZodType): schema is ZodString =>
  schema.def.type === "string";

export const isZodBoolean = (schema: ZodType): schema is ZodBoolean =>
  schema.def.type === "boolean";

export const isZodOptional = (schema: ZodType): schema is ZodOptional =>
  schema.def.type === "optional";

export const isZodNullable = (schema: ZodType): schema is ZodNullable =>
  schema.def.type === "nullable";

export const isZodDefault = (schema: ZodType): schema is ZodDefault =>
  schema.def.type === "default";

export const isZodEnum = (schema: ZodType): schema is ZodEnum =>
  schema.def.type === "enum";

export const isZodObject = (schema: ZodType): schema is ZodObject =>
  schema.def.type === "object";

export const isZodUnion = (schema: ZodType): schema is ZodUnion =>
  schema.def.type === "union";

export const isZodLiteral = (schema: ZodType): schema is ZodLiteral =>
  schema.def.type === "literal";

export const isZodPipe = (schema: ZodType): schema is ZodPipe =>
  schema.def.type === "pipe";
