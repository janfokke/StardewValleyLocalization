# StardewValleyLocalization
 A tool to easily edit Stardew Valley XNB language files

### Prerequisites
- Microsoft .NET Framework 4.6.1
- Stardew Valley

Download the latest [release](https://github.com/janfokke/StardewValleyLocalization/releases).

### Usage
Run StardewValleyLocalization.exe

#### Creating a new project

In the file menu, select New
Select the language that you want to edit (ATM only EN)
Select an output folder

#### Opening a project
In the file menu, select Load
Select the folder you want to load XNB files from

#### Exporting to Stardew Valley
In the explorer go to your project folder and copy the content to the Stardew Valley content folder
In Windows/Steam this is "C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Content"

#### Troubleshooting
If the application doesn't load properly make sure the following conditions are met
- The ContentRoot property in Setting.json is set to the root folder of Stardew Valley Content (Make sure to use double \ for path separators)
- Setting.json and StardewValleyLocalization.exe are in the same folder
