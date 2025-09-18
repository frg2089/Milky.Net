declare namespace Types {
  type ZodType = import('zod').ZodType['type']

  type UnionToIntersection<U> = (
    U extends any ? (k: U) => void : never
  ) extends (k: infer I) => void
    ? I
    : never

  export interface TypeInfoData {
    type: 'object' | 'enum' | 'union' | 'generic'
    description?: string | undefined
  }

  export interface ObjectTypeInfoData extends TypeInfoData {
    type: 'object'
    properties: Record<string, PropertyInfoData>
    baseType?: string
  }

  export interface GenericTypeInfoData extends TypeInfoData {
    type: 'generic'
    baseType: string
    genericPropertyName: string
  }

  export interface UnionTypeInfoData extends TypeInfoData {
    type: 'union'
    properties: Record<string, PropertyInfoData>
    baseType?: string
    types: Record<string, string>
    discriminator: string
  }

  export interface EnumTypeInfoData extends TypeInfoData {
    type: 'enum'
    items: Record<string, string | number>
  }

  export interface PropertyInfoData {
    type: string
    description?: string | undefined
    constants?: Array<string | number>
  }

  export namespace Api {
    export interface ApiEndpoint {
      endpoint: string
      description: string
      inputStruct: string | null
      outputStruct: string | null
    }
    export interface ApiCategory {
      name: string
      apis: ApiEndpoint[]
    }
  }
}
