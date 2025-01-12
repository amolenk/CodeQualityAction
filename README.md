# Code Quality Scanner

## Actions

### Cyclomatic Complexity (Cobertura)

This method uses a Cobertura code coverage file to inspect cyclomatic complexity.

Call the action from your GitHub Actions workflow:
```yaml
- name: Cyclomatic Complexity Check
  uses: amolenk/CodeQualityAction/cc-cobertura-check@4.0.7
  with:
    cobertura-file: ${{ env.WORKING_DIR }}/${{ env.COVERAGE_MERGE_DIR }}/Cobertura.xml
    threshold: '2.4'
```

### Cyclomatic Complexity (ESLint)

This method uses ESLint to create a JSON file that contain the results of a cyclomatic complexity rule.
If the maximum allowed value is set to 0, this will give us all files with a complexity > 0. 
All files with a cyclomatic complexity of 0 will not show up in the output file, which means that the output of the action will general be higher than it should be.

Add a command to `package.json` to use ESLint to create the cyclomatic complexity file:

```
"cyclomatic-complexity": "eslint . --rule 'complexity: [warn, { max: 0 }]' --format json --output-file cyclomatic-complexity.json",
```

Call the action from your GitHub Actions workflow:
```yaml
- name: Create ESLint Cyclomatic Complexity file
  run: npm run cyclomatic-complexity --prefix ${{ env.CLIENT_DIR }}
  
- name: Cyclomatic Complexity Check
  uses: amolenk/CodeQualityAction/cc-eslint-check@4.0.7
  with:
    eslint-file: ${{ env.CLIENT_DIR }}/cyclomatic-complexity.json
    threshold: '2.4'
```

### Fan Out (.NET)

Scan .NET assemblies to calculate fan out.

Call the action from your GitHub Actions workflow:
```yaml
- name: Fan Out Check
  uses: amolenk/CodeQualityAction/fo-dotnet-check@4.0.7
  with:
    binary-folder: ${{ env.WORKING_DIR }}/artifacts
    assembly-prefix: <Assembly prefix, e.g. 'AcmeCorp.Product'>
    threshold: '10'
```

## Development

Build new versions of the scanner:

```
dotnet publish src/CodeQualityScanner/CLI/CLI.csproj -c Release -r linux-x64 --self-contained -o publish
```

Update tags:

```
git tag 1.0.0
git push --tags
```
