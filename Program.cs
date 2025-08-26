// See https://aka.ms/new-console-template for more information
using UpdateJsonValue;

Console.WriteLine("Update Prop");


//Updating appsettings.json
//DataUpdater4.UpdateValue("appsettings.json", "File:Path", "c:\\puneeth");
DataUpdater4.UpdateValue("appsettings.json", "Users:1:Name", "puneeth");
//DataUpdater4.UpdateValue("appsettings.json", "Modules:0:ExportFormats:1:0", "puneeth");


Console.ReadLine();
