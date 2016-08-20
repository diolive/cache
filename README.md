# Caché
Home financial management service

## Platform stack:
- ASP.NET Core 1.0 MVC
- Entity Framework Core 1.0
- Bootstrap 3 with Sandstone theme

## Copyright and License

Source code published by MIT license.  
Key developer: Dmitry Vereskun (DioLive Studio)

## How to run this application on my own PC (or hosting)
### Get the code
Clone or copy this repo from `develop` or `master` branch.  
`develop` contains latest _deployed_ version.  
`master` contains latest _stable minor_ version.  
Someday I will deploy independently stable and beta versions from these branches. Currently there is no deployed "master" version.

### Prepare your environment
1. Download the [latest stable](https://www.microsoft.com/net/core) or [daily](https://github.com/dotnet/cli#installers-and-binaries) .NET Core SDK for your operating system and platform.
2. Find out the installed version of .NET Core Tools. It should has a format like `1.0.0-preview3-003221`. There are two primary ways:
   - open command console and execute `dotnet --version`;
   - open folder with installed dotnet (`C:\Program Files\dotnet\sdk` on Windows) and look to name of subfolders inside.
3. Update target SDK version in `global.json` configuration file.
4. Install MS SQL Server (tested on SQL Server Developer 2016) and type its name to `Data Source=` block in connection string within `appsettings.json` (currently it's `.\\MSSQL`).
5. Install [latest stable](https://nodejs.org) NodeJS (required for command-line base utils). 

### Initialize the application
Most of these command could be done with `F5` inside Visual Studio but I prefer to use a command console.

1. Restore packages: `dotnet restore`
2. Install bower (front-end package manager): `bower install` (if doesn't work, execute `npm i -g bower` before)
3. Install npm (at least for gulp): `npm install`
4. Proceed default gulp task: `gulp` (if doesn't work, execute `npm i -g gulp` before)
5. Minify styles and scripts: `dotnet bundle`
6. Migrate (or create) database for latest version: `dotnet ef database update`

### Run the application
There are two steps: `dotnet build` and `dotnet run`, but you can simply skip the first one.  
After successful run of application you will see the URL you can open in a browser:
```
Now listening on: http://localhost:5000
Application started. Press Ctrl+C to shut down.
```