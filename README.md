# Code Quality Scanner

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
