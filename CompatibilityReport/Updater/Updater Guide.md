# Compatibility Report - Updater Guide

The Updater only runs when a "CompatibilityReportUpdater" folder exists in the games local AppData folder, so it won't run for regular users. The updater has two parts. The **WebCrawler** crawls the Steam Workshop for mod information, including mod name, author, published/update dates, required DLC/mods, source URL, compatible game version and several statuses: unlisted/removed from Workshop, incompatible, author retirement, no description. The **FileImporter** imports data from CSV files, including information that cannot be found by an automated process, like compatibility and suggested successor/alternative mods. It also allows the creation of groups for required and recommended mods. The updater creates a new catalog that can be uploaded to the download location, so all mod users get this new catalog on the next game start. If you think about enabling the updater for yourself, mind that the WebCrawler downloads more than 2000 webpages and takes about 15 minutes on a fast computer with decent internet connection. To avoid unnecessary downloads, the WebCrawler only runs if a file called "CompatibilityReport_WebCrawler.enabled" exists inside the Updater folder. The updater also creates a DataDump file with various checks and relevant information for updating, and can run a OneTimeAction (disabled by default).

The FileImporter gathers its information from CSV files. These should be placed in the updater folder where the new catalogs are written as well. Update actions can be bundled in one CSV file or split into multiple files. Multiple files will be read in alphabetical order. Filenames don't matter, but should end in .csv. After updating, the processed CSV files are renamed to .txt to avoid duplicate additions to the catalog on a next updater run.

Groups are used for mod requirements and recommendations to allow for different editions of mods. For example, a group with both a stable and test version of the same mod, will accept the test version as valid when the stable version is set as required mod. This prevents unjustified 'required mod not found' messages in the report. Similarly, a stable mod will not be recommended in the report, when the test mod is already subscribed. A mod can only be a member of one group. Groups cannot be used for anything else like compatibilities, successors or alternatives. Group IDs are automaticaly assigned by the updater.

The lines in the CSV files all start with an action, often followed by a Steam ID (for the mod, group or author), often followed by additional data. Some actions will create exclusions in the catalog, to prevent the WebCrawler from overwriting these manual updates. Actions and parameters are not case sensitive, commas are the only allowed separator and spaces around the separators are ignored. Commas can still be used in the mod name, notes and header/footer texts, but not in other parameters. Parameters cannot be empty or contain only spaces.

Lines starting with a '#' are considered comments and will be ignored by the updater. Comments can also be added as an extra parameter at the end of any action, except Add_Mod or the actions that add a note or header/footer text. Use a comma after the last parameter and start the comment with a '#'. Commas in these end-of-line comments are not supported.

### First action to use in any CSV file
* Set_ReviewDate, \<date: yyyy-mm-dd\> 
  * *Used as review update date for any following Mod and Author actions. Can be used anywhere and multiple times in a CSV file if you want different review dates for different actions. Uses today if omitted.*

### Mod actions
Parameters in square brackets are optional. The symbol :zap: means an exclusion will be created.
* Add_Mod, \<mod ID\>, Unlisted | Removed [, \<author ID | custom url\> [, \<mod name\>] ]
* Set_Stability, \<mod ID\>, \<stability string\> [, \<note\>]
  * *To remove a stability note, set the same stability again without a note*
* Add_Status, \<mod ID\>, \<status string\> :zap: *(exclusion only for NoDescription status)*
* Set_Note, \<mod ID\>, \<text\>
* Set_GameVersion, \<mod ID\>, \<game version string\> :zap: *(will be overruled when a newer game version is found)*
* Add_RequiredDlc, \<mod ID\>, \<dlc string\> :zap:
* Add_RequiredMod, \<mod ID\>, \<required mod ID\> :zap:
* Add_Successor, \<mod ID\>, \<successor mod ID\>
* Add_Alternative, \<mod ID\>, \<alternative mod ID\>
* Add_Recommendation, \<mod ID\>, \<recommended mod ID\>
* Set_SourceUrl, \<mod ID\>, \<url\> :zap:
* Update_Review, \<mod ID\> *(updates the review date; use for reviews without changes to the mod itself)*
* Remove_Mod, \<mod ID\> *(only works on mods that are removed from the Steam Workshop)*
* Remove_Status, \<mod ID\>, \<status string\>
* Remove_Note, \<mod ID\>
* Remove_GameVersion, \<mod ID\> *(only works if an exclusion exists)*
* Remove_RequiredDlc, \<mod ID\>, \<DLC string\> *(only works if an exclusion exists)*
* Remove_RequiredMod, \<mod ID\>, \<required mod ID\> :zap:
* Remove_Successor, \<mod ID\>, \<successor mod ID\>
* Remove_Alternative, \<mod ID\>, \<alternative mod ID\>
* Remove_Recommendation, \<mod ID\>, \<recommended mod ID\>
* Remove_SourceUrl, \<mod ID\> :zap:
* Remove_Exclusion, \<mod ID\>, \<exclusion category\> [,\<required dlc string | mod ID\>]
  * *Available categories: SourceUrl, GameVersion, NoDescription, RequiredDlc, RequiredMod*

### Group actions (will not change review date for included mods)
* Add_Group, \<name\>, \<mod ID\>, \<mod ID\> [, \<mod ID\>, ...]
* Add_GroupMember, \<group ID\>, \<mod ID\>
* Remove_Group, \<group ID\>
* Remove_GroupMember, \<group ID\>, \<mod ID\>

### Compatibility actions (will not change review date for included mods)
* Add_Compatibility, \<first mod ID\>, \<second mod ID\>, \<compatibility status\>[, \<note\>]
* Add_CompatibilitiesForOne, \<first mod ID\>, \<compatibility status\>, \<mod ID\>, \<mod ID\> [, \<mod ID\>, ...]
  * *This will create compatibilities between the first mod and each of the other mods. Cannot be used for some statuses.*
* Add_CompatibilitiesForAll, SameFunctionality, \<mod ID\>, \<mod ID\>, \<mod ID\> [, \<mod ID\>, ...]
  * *This will create 'SameFunctionality' compatibilities between each of these mods in pairs (1 with 2, 1 with 3, 2 with 3, etc.).*
* Remove_Compatibility, \<first mod ID\>, \<second mod ID\>, \<compatibility status\>

### Author actions (author ID is much more reliable, and mandatory if the custom URL is a number)
* Add_Author, \<author ID | custom url\>, \<author name\> *(only needed for unknown authors of 'removed' mods, will be set to retired)*
* Set_AuthorID, \<author custom url\>, \<author ID\>
* Set_AuthorUrl, \<author ID\>, \<author custom url\>
* Set_LastSeen, \<author ID | custom url\>, \<date: yyyy-mm-dd\> *(should be more recent than newest mod update)*
  * *Author retirement will be recalculated. An author is assumed retired if not seen for a year.*
* Set_Retired, \<author ID | custom url\> :zap:
* Merge_Author, \<author ID\>, \<author custom url\>
  * *For when the WebCrawler created two authors that should be one. Author retirement will be recalculated and exclusion will be removed.*
* Remove_AuthorUrl, \<author ID\>
* Remove_Retired, \<author ID | custom url\> *(only works if added manually before)*

### Catalog actions
* Set_CatalogGameVersion, \<game version string\>
* Set_CatalogNote, \<text\>
* Set_CatalogHeaderText, \<text\>
* Set_CatalogFooterText, \<text\>
* Remove_CatalogNote
* Remove_CatalogHeaderText
* Remove_CatalogFooterText

### Miscellaneous actions
* Add_RequiredAssets, \<asset ID\> [, \<asset ID\>, ...]
  * *Only needed to differentiate between a required asset and a required mod that isn't in the catalog.*
* Remove_RequiredAssets, \<asset ID\> [, \<asset ID\>, ...]
* Add_SuppressedWarning, \<mod or author ID\>
  * *Suppresses warnings about an unnamed mod or a duplicate author name (add both authors). Author URL is not supported.*
* Remove_SuppressedWarning, \<mod or author ID\>


*See [Enums.cs](https://github.com/Finwickle/CompatibilityReport/blob/main/CompatibilityReport/CatalogData/Enums.cs) for available stability, status, compatibility and DLC strings.*