# GitToolSelector
A configurable selector for merge/diff tools. Mainly developed for SemanticMerge external parser support.

## Configuration
### .gitconfig
Location: `%homepath%\.gitconfig`
```ini
[diff]
  tool = diffselector
[difftool]
  prompt = true
[merge]
  tool = mergeselector
[mergetool]
  prompt = false
  keepBackup = false
[difftool "diffselector"]
  cmd = \"C:\\Program Files\\KsWare\\GitToolSelector\\GitToolSelector.exe\" -tool semanticdiff -s \"$LOCAL\" -d \"$REMOTE\"
  keepBackup = false
[mergetool "mergeselector"]
  cmd = \"C:\\Program Files\\KsWare\\GitToolSelector\\GitToolSelector.exe\" -tool semanticmerge -s \"$REMOTE\" -d \"$LOCAL\" -b \"$BASE\" -r \"$MERGED\"
  trustExitCode = true
```
### GitToolSelector.conf
Location: `%ProgramFiles%\KsWare\GitToolSelector\GitToolSelector.conf`
Only a shprt schematic excample. A complete config is included.
```ini
[.xml;.xaml]
MergeTool=semanticmerge
DiffTool=semanticdiff
#https://github.com/RalfKoban/xml-semantic-external-parser
ExternalParser=C:\Program Files\SemanticMerge\External\XmlSemanticParser.exe

[tool semanticdiff]
cmd="C:\Program Files\SemanticMerge\semanticmergetool.exe" -s "$s" -d "$d" -ep "$EXTERNALPARSER"

[tool semanticmerge]
cmd="C:\Program Files\SemanticMerge\semanticmergetool.exe" -s "$s" -d "$d" -b "$b\" -r "$r" -ep "$EXTERNALPARSER"

[Overrides]
*\GitToolSelector.conf = .ini
```