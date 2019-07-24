# MergeToolSelector
A configurable selector for merge/diff tools. Mainly developed for SemanticMerge external parser support.

## Configuration
### .gitconfig
Location: `%homepath%\.gitconfig`
```ini
[diff]
  tool = diffselector
[merge]
  tool = mergeselector
[difftool "diffselector"]
  cmd = \"C:\\Program Files (x86)\\KsWare\\MergeToolSelector\\MergeToolSelector.exe\" -tool diff -s \"$LOCAL\" -d \"$REMOTE\"
  keepBackup = false
[mergetool "mergeselector"]
  cmd = \"C:\\Program Files (x86)\\KsWare\\MergeToolSelector\\MergeToolSelector.exe\" -tool merge -s \"$REMOTE\" -d \"$LOCAL\" -b \"$BASE\" -r \"$MERGED\"
  trustExitCode = true
```
### MergeToolSelector.conf
Location: `%ProgramFiles%\KsWare\MergeToolSelector\MergeToolSelector.conf`
Only a short schematic example. A complete configuration is included.
```ini
[.xml;.xaml]
  MergeTool=semanticmergeext
  DiffTool=semanticdiffext
  ExternalParser=C:\Program Files\SemanticMerge\External\XmlSemanticParser.exe

[.resx]
  MergeTool=semanticmergeext
  DiffTool=semanticdiffext
  ExternalParser=C:\Program Files\SemanticMerge\External\ResXSemanticParser.exe

[*]
MergeTool=semanticmerge
DiffTool=semanticdiff

[tool semanticdiff]
  cmd="C:\Program Files\SemanticMerge\semanticmergetool.exe" -s "$s" -d "$d"

[tool semanticmerge]
  cmd="C:\Program Files\SemanticMerge\semanticmergetool.exe" -s "$s" -d "$d" -b "$b" -r "$r"

[tool semanticdiffext]
  cmd="C:\Program Files\SemanticMerge\semanticmergetool.exe" -s "$s" -d "$d" -ep "$EXTERNALPARSER"

[tool semanticmergeext]
  cmd="C:\Program Files\SemanticMerge\semanticmergetool.exe" -s "$s" -d "$d" -b "$b" -r "$r" -ep "$EXTERNALPARSER"

[Overrides]
*\MergeToolSelector.conf = .ini
```
