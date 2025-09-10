import * as Milky from "@saltify/milky-types";
import { program } from 'commander';

program.option('-p, --package', '显示 @saltify/milky-types 的版本号', false)

program.parse();

const options = program.opts();

console.info(options.package ? Milky.milkyPackageVersion : Milky.milkyVersion)
