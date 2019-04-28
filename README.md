# TimeReporter
A systray time reporter. More info coming in a while.. Sometime. Intended as an example of the following concepts in near-future :)
* Rich-interface system tray application for Windows 10. 
* Proof-of-concept [DPI-aware](https://stackoverflow.com/questions/4075802/creating-a-dpi-aware-application) WinForms client.
* [DotNetCore and DotNetFramework combination](https://docs.microsoft.com/en-us/dotnet/core/porting/project-structure).
* And interop with [CDP](https://github.com/cyrus-and/chrome-remote-interface) for headless tasks.
* Niché (highly specialized) event store using vacuuming and ”natural” sharding rather than snapshotting.
* [Math.Compute](https://stackoverflow.com/questions/3972854/parse-math-expression) or similar technique for string parsing compute. 

# Issues
Timereporter.Api has a Console that is used for spelunking through the eventlog. For debugging "Debug"-section must have following changes.

| Name | Original value | Temporary Console Debug mode | 
| ---- | -------------- | ---------------------------- |
| Launch | IIS Express  | Project | 
| Application arguments | (empty)  | time | 

