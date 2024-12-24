# Code Quality Scanner





## Cyclomatic Complexity - Cobertura


## Development

Build new versions of the scanner:

```
dotnet publish src/CodeQualityScanner/CLI/CLI.csproj -c Release -r linux-x64 --self-contained -o publish
```

Update tags:

```
git tag 1
git push --tags
```


```
   - name: Cyclomatic Complexity Check
        uses: org/code-quality-actions/complexity-check@v1
        with:
          cobertura-file: './coverage/cobertura.xml'
          threshold: '10.0'

```