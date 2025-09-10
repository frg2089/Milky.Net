import { apiCategories } from "@saltify/milky-types/api";
import { program } from 'commander';
import * as fs from "node:fs";
import * as path from "node:path";

program
  .option('-o, --out <path>', '中间 json 生成路径', path.resolve('out', "ApiEndpoints.json"))

program.parse();

const options = program.opts();

await fs.promises.writeFile(path.resolve(options.out), JSON.stringify(apiCategories, null, 2));