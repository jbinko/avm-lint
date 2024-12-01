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

## Expected Bicep File Structure

```bicep
// First section expects metadata statements
metadata name = 'Elastic SANs'                                  // 'name' - First metadata defined and it should be in plural form
metadata description = 'This module deploys an Elastic SAN.'    // 'description' - Second metadata defined and must start with 'This module deploys a' followed by the name of the resource in singular form
metadata owner = 'Azure/module-maintainers'                     // 'owner' - Third metadata defined with the value 'Azure/module-maintainers'
```

## Lint Rules

| Code   | Level | Message |
|--------|-------|---------|
| AVM001 | Error | The 'name' metadata in the module should be the first metadata defined and it should be in plural form, for example, 'Elastic SANs'. |
| AVM002 | Error | The 'description' metadata in the module should be the second metadata defined and must start with 'This module deploys a' followed by the name of the resource in singular form. For example 'This module deploys an Elastic SAN'. |
| AVM003 | Error | The 'owner' metadata in the module should be the third metadata defined with the value 'Azure/module-maintainers'. |
