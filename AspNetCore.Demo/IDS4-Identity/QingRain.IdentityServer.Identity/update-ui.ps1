$source = "https://github.com/IdentityServer/IdentityServer4.Quickstart.UI.AspNetIdentity/archive/main.zip"

[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
Invoke-WebRequest $source -OutFile ui.zip

Expand-Archive ui.zip

if (!(Test-Path -Path Quickstart))  { mkdir Quickstart }
if (!(Test-Path -Path Views))       { mkdir Views }
if (!(Test-Path -Path wwwroot))     { mkdir wwwroot }

copy .\ui\IdentityServer4.Quickstart.UI.AspNetIdentity-main\Quickstart\* Quickstart -recurse -force
copy .\ui\IdentityServer4.Quickstart.UI.AspNetIdentity-main\Views\* Views -recurse -force
copy .\ui\IdentityServer4.Quickstart.UI.AspNetIdentity-main\wwwroot\* wwwroot -recurse -force

del ui.zip
del ui -recurse