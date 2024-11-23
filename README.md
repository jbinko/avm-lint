# Azure Verified Modules Lint Tool

This is a command-line application, similar to a linter, designed to ensure consistency for Azure Verified Modules.

## Command Line Options

```bash
avm-lint -?

Description:
  Azure Verified Modules Lint [Version 0.0.1.0]
  Copyright (c) 2024 Jiri Binko. All rights reserved.

Usage:
  avm-lint [options]

Options:
  --source <source> (REQUIRED)  Source file (Bicep) or directory that contains the Bicep files.
  --recursive                   Search recursively for Bicep files within the specified directory and its subdirectories. [default: True]
  --filter <filter>             The filter string is used to match the names of files, supporting wildcard characters (* and ?). [default: *main.bicep]
  --version                     Show version information
  -?, -h, --help                Show help and usage information
```

```bash
avm-lint --source main.bicep

avm-lint --source avm\res

avm-lint --source avm\res --recursive false

avm-lint --source avm\res --filter *test.bicep

avm-lint --source avm\res --recursive false --filter *test.bicep
```

## Lint Rules

| Code   | Level | Message |
|--------|-------|---------|
| AVM001 | Error | The module name metadata must be specified first and should contain the value with the resource name in plural form. For example, 'Elastic SANs'. |
| AVM002 | Error | The module description metadata must be specified second and must contain the value with the resource name in singular form, starting with the phrase 'This module deploys a'. For example, 'This module deploys an Elastic SAN'. |
| AVM003 | Error | The module owner is not specified correctly. It must be specified as the third metadata statement with the value "metadata owner = 'Azure/module-maintainers'". |
