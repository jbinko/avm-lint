# Azure Verified Modules Lint Tool

AVM Lint is a command-line tool designed to increase the consistency and quality
of Azure Verified Modules by conducting comprehensive linting of Bicep files.</br>
It checks these files, or whole directories, against the Azure Verified Modules standards
and offers recommendations.</br>
Users can lint specific Bicep files or entire directories by specifying paths or using wildcard patterns.</br>
The tool provides control over the linting rules, allowing the inclusion or exclusion of specific rules and setting thresholds for the number of allowable linting issues before stopping the process.</br>

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

## Command Line Options Examples

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

Keep the order of the metadata statements as shown below. The `targetScope` statement is optional and can only be used with the values `subscription`, `managementGroup`, or `tenant`. The use of `resourceGroup` is not allowed.

```bicep
// Define module metadata
metadata name = 'Elastic SANs'                                  // Defines the module name. Ensure names are plural when appropriate.
metadata description = 'This module deploys an Elastic SAN.'    // Provides a description of the module's functionality. Must start with 'This module deploys a' followed by the name of the resource in singular form.
metadata owner = 'Azure/module-maintainers'                     // Specifies the owner of the module, using fixed value 'Azure/module-maintainers'.

// If required - define deployment target scope
// Possible targets include 'subscription', 'managementGroup', or 'tenant'. Do not use 'resourceGroup' value.
targetScope = 'subscription'                                    // When specified, must be placed right after metadata entries.
```

## Lint Rules

| Code   | Level | Message |
|--------|-------|---------|
| AVM001 | Error | The 'name' metadata in the module should be the first metadata defined and it should be in plural form, for example, 'Elastic SANs'. |
| AVM002 | Error | The 'description' metadata in the module should be the second metadata defined and must start with 'This module deploys a' followed by the name of the resource in singular form. For example 'This module deploys an Elastic SAN'. |
| AVM003 | Error | The 'owner' metadata in the module should be the third metadata defined with the value 'Azure/module-maintainers'. |
| AVM004 | Error | The 'targetScope' can only be used with 'subscription', 'managementGroup', or 'tenant' value. It cannot be used with 'resourceGroup'. When 'targetScope' is specified, it must be the first statement following the metadata section. |
