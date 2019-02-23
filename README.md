# StardewValleyLocalization
 A tool to edit Stardew Valley XNB language files

### Prerequisites
- Microsoft .NET Framework 4.6.1
- Stardew Valley

Download the latest [release](https://github.com/janfokke/StardewValleyLocalization/releases).

#### Exporting to Stardew Valley
In the explorer go to your project folder and copy the content to the Stardew Valley content folder
In Windows/Steam this is folder is located here "C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Content"

#### Troubleshooting
If the application doesn't load properly make sure the following conditions are met
- The ContentRoot property in Setting.json is set to the root folder of Stardew Valley Content (Make sure to use double \ for path separators)
- Setting.json and StardewValleyLocalization.exe are in the same folder

#### Why english is not recommended as source language
The game is programmed in the English language.
Things like weapons and items names are hard coded and thus cannot be changed in the English translation.
Other languages have an extra field "displayname" that allows you to give items a custom name.
Of course you can use english as a reference when translating
