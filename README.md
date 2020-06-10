SkyHook by Skyscanner
===
*SQL Server Stored Procedure interface to Google Analytics*

Background
---
SkyHook uses SQL Server's .NET CLR functionality to access the Google Analytics API from SQL Server, by providing a simple Stored Procedure interface to query any profiles, metrics and dimensions available in your Google Analytics account(s).

Using SkyHook, you can quickly gather data from Google Analytics into temporary or permanent SQL Server database tables, and then perform any further analysis on the data as you wish.

In the spirit of re-using code, SkyHook is built on top of the publicly available (and Google-maintained) GData.NET library, which provides interfaces to various Google data services.

Dependencies
---

* Installation
  * SQL Server 2008 Express Edition or greater
* Compilation
  * Visual Studio 2008 or greater
  * git client
  * svn client

Installation
---

* If so desired, compile all binaries from scratch, as documented under the 'Compilation' section of this document
* Use, or modify, the code from `Utilities.sql` to...
  * Enable CLR functionality in the target database (NB: may require a database restart)
  * Set the *'TRUSTWORTHY'* flag to *'ON'* in the target database
  * Install the SkyHook assemblies from either `<skyhook>\Google\lib` (if using precompiled binaries), or `<skyhook>\Google\bin\Release` (if self-compiling) into the target database
  * Create a stored procedure to utilize the CLR code
  * Run a test query to ensure that the interface is functional

Compilation
---

* Checkout the *google-gdata* code from Google Code's [SVN page](http://code.google.com/apis/gdata/)
* In Visual Studio, open the *google-gdata* project and set the solution build mode to 'Release' under the menu option 'Build' -> 'Configuration Manager'
* Build the 'Analytics' project in *google-gdata*
* Checkout the *skyhook* code from Skyscanner's [github page](https://github.com/skyscanner/skyhook)
* Copy the `Google.GData.Client.dll` and `Google.GData.Analytics.dll` binaries from `<google-gdata>\clients\cs\src\analytics\bin\Release` to `<skyhook>\Google\lib`
* Build the *skyhook* solution

References
---

[Skyscanner](http://www.skyscanner.net/)
[SkyHook on github](https://github.com/skyscanner/skyhook)
[GData.NET on Google Code](http://code.google.com/apis/gdata/)
[Google Analytics API Developer Reference](http://code.google.com/apis/analytics/docs/)

Licensing and Contact
---
This code is public domain, and is provided as-is, without express or implied warranty of any kind.

That said, you are welcome to fork the project on github and apply any fixes or changes that you feel are useful, and/or contact Skyscanner or the author ([James Addison](mailto:jay@jp-hosting.net)) with issue reports or github pull requests.