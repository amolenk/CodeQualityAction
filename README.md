# Code Quality Scanner





## Cyclomatic Complexity - Cobertura


## Development

Build new versions of the scanner:

```
dotnet publish src/CodeQualityScanner/CLI/CLI.csproj -c Release -r linux-x64 --self-contained -o publish
```

Update tags:

```
git tag -a cc-cobertura-check/v1 -m "Initial version of CC check for Cobertura"
git push --tags
```
