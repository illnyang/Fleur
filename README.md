# Fleur - odrabiamy.pl dumper
Simple C# dumper for odrabiamy.pl. Dumps all available books from the website to local cache and generates static html pages from it using templates.

## Building
This app uses .NET Core 2.x, packages targeting it and a standard .csproj project file. Easiest way to build this project is to use Visual Studio.

## Usage
Example config.json:
```
{
  "CachePath": "cache",         // this is the local cache path. its content should not be modified
  "OutputPath": "html",         // path for output html files generated from local cache
  "TemplatesPath": "templates", // templates used for html generation
  "ResourcesPath": "resources", // static resources used by templates. copied to output folder
  "OperationMode": 1,           // 0 = Update cache, 1 = Generate html from cache, 2 = Both
  "LogToFile": false,           // saves console output to file
  "SessionCookies": [           // an array of session ids. grab '_dajspisac_session_id' from your browser.
    "2137sr12314i1b4213"
  ]
}
```
All paths can be specified either as relative (to working directory) or absolute paths. Example templates with resources can be downloaded from: https://files.catbox.moe/66kskm.7z

## Current limitations
- No parameter parsing. Settings are read from config.json instead.
- Little to no edge-case handling. May miss some books.
- Reliance on odrabiamy.pl's internal api. Once api is gone or changes the dumper will be broken and will need to fall back on html parsing.
- Reliance on odrabiamy.pl's image hosting. TODO: save all imgs and svgs to local cache.
## License
See LICENSE.md