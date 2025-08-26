// See https://aka.ms/new-console-template for more information
using UpdateJsonValue;

Console.WriteLine("Update Prop");


//Updating appsettings.json
//DataUpdater4.UpdateValue("appsettings.json", "File:Path", "c:\\puneeth");
DataUpdater5.UpdateValue("appsettings.json", "AppName", "puneeth");
//DataUpdater4.UpdateValue("appsettings.json", "Modules:0:ExportFormats:1", "puneeth");

//DataUpdater5.UpdateValue("appsettings.json", "Modules:0:Settings:ExportFormats:2", "puneeth");



Console.ReadLine();
