# Azure Verified Modules Lint Tool

This is a command-line application, similar to a linter, designed to ensure consistency for Azure Verified Modules.

## Command Line Options

```console
Usage:
  avm-lint [options]

Options:
  --path <path> (REQUIRED)             The Bicep file or directory to lint. If a directory is provided, all Bicep files within it are considered unless modified by other options.
  --recursive                          Search recursively for files within the specified directory and its subdirectories. This is the default behavior. [default: True]
  --file-filter <file-filter>          A wildcard pattern to select which files to lint. Supports standard wildcard characters such as `*` (matches any sequence of characters) and `?` (matches any single
                                       character). [default: *main.bicep]
  --only-rules <only-rules>            Specifies a list of rule IDs to restrict linting checks exclusively to the specified rules, excluding all other rules. The rules can be provided as a comma-separated.
  --exclude-rules <exclude-rules>      Excludes specific rules from the linting process. All other rules that are not mentioned will be included by default in the linting process. Specify rule IDs as a
                                       comma-separated list to exempt them from checks.
  --issue-threshold <issue-threshold>  Specifies the maximum number of issues (including errors and warnings) tolerated before terminating the linting process early.
  --version                            Show version information
  -?, -h, --help                       Show help and usage information
```

```console
avm-lint --path main.bicep

avm-lint --path avm\res

avm-lint --path avm\res --recursive false

avm-lint --path avm\res --file-filter *test.bicep

avm-lint --path avm\res --recursive false --file-filter *test.bicep

avm-lint --path avm\res --only-rules AVM001,AVM002

avm-lint --path avm\res --exclude-rules AVM001,AVM002,AVM003

avm-lint --path avm\res --issue-threshold 10
```

## Lint Rules

| Code   | Level | Message |
|--------|-------|---------|
| AVM001 | Error | The module name metadata must be specified first and should contain the value with the resource name in plural form. For example, 'Elastic SANs'. |
| AVM002 | Error | The module description metadata must be specified second and must contain the value with the resource name in singular form, starting with the phrase 'This module deploys a'. For example, 'This module deploys an Elastic SAN'. |
| AVM003 | Error | The module owner is not specified correctly. It must be specified as the third metadata statement with the value "metadata owner = 'Azure/module-maintainers'". |
