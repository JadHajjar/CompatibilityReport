﻿using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using CompatibilityReport.Reporter.HtmlTemplates;
using CompatibilityReport.Util;

namespace CompatibilityReport.CatalogData
{
    [Serializable] 
    public class Mod
    {
        public ulong SteamID { get; private set; }
        public string Name { get; private set; }

        public DateTime Published { get; private set; }
        public DateTime Updated { get; private set; }

        public ulong AuthorID { get; private set; }
        public string AuthorUrl { get; private set; } = "";

        public Enums.Stability Stability { get; private set; } = Enums.Stability.NotReviewed;
        public string StabilityNote { get; private set; }

        public List<Enums.Status> Statuses { get; private set; } = new List<Enums.Status>();
        public bool ExclusionForNoDescription { get; private set; }
        public string Note { get; private set; }

        // Game version this mod is compatible with. 'Version' is not serializable, so a converted string is used.
        [XmlElement("GameVersion")] public string GameVersionString { get; private set; }
        public bool ExclusionForGameVersion { get; private set; }

        public List<Enums.Dlc> RequiredDlcs { get; private set; } = new List<Enums.Dlc>();
        public List<Enums.Dlc> ExclusionForRequiredDlcs { get; private set; } = new List<Enums.Dlc>();

        // No mod should be in more than one of the required mods, successors, alternatives and recommendations.
        [XmlArrayItem("SteamID")] public List<ulong> RequiredMods { get; private set; } = new List<ulong>();
        [XmlArrayItem("SteamID")] public List<ulong> ExclusionForRequiredMods { get; private set; } = new List<ulong>();
        [XmlArrayItem("SteamID")] public List<ulong> Successors { get; private set; } = new List<ulong>();
        [XmlArrayItem("SteamID")] public List<ulong> Alternatives { get; private set; } = new List<ulong>();
        [XmlArrayItem("SteamID")] public List<ulong> Recommendations { get; private set; } = new List<ulong>();

        public string SourceUrl { get; private set; }
        public bool ExclusionForSourceUrl { get; private set; }

        // Date of the last review of this mod, imported by the FileImporter, and the last automatic review for changes in mod information (WebCrawler).
        public DateTime ReviewDate { get; private set; }
        public DateTime AutoReviewDate { get; private set; }
        [XmlArrayItem("ChangeNote")] public List<string> ChangeNotes { get; private set; } = new List<string>();

        // Properties used by the Reporter for subscribed mods.
        [XmlIgnore] public bool IsDisabled { get; private set; }
        [XmlIgnore] public bool IsCameraScript { get; private set; }
        [XmlIgnore] public string ModPath { get; private set; }
        [XmlIgnore] public DateTime DownloadedTime { get; private set; }
        [XmlIgnore] public Enums.ReportSeverity ReportSeverity { get; private set; }

        // Used by the Updater, to see if this mod was added or updated this session.
        [XmlIgnore] public bool AddedThisSession { get; private set; }
        [XmlIgnore] public bool UpdatedThisSession { get; private set; }


        /// <summary>Default constructor for deserialization.</summary>
        private Mod()
        {
            // Nothing to do here
        }


        /// <summary>Constructor for mod creation.</summary>
        public Mod(ulong steamID)
        {
            SteamID = steamID;

            AddedThisSession = true;
        }


        /// <summary>Gets the game version this mod is compatible with.</summary>
        /// <returns>The game version this mod is compatible with.</returns>
        public Version GameVersion()
        {
            return Toolkit.ConvertToVersion(GameVersionString);
        }


        /// <summary>Updates one or more mod properties.</summary>
        public void Update(string name = null,
                           DateTime published = default,
                           DateTime updated = default,
                           ulong authorID = 0,
                           string authorUrl = null,
                           Enums.Stability stability = default,
                           string stabilityNote = null,
                           string note = null,
                           string gameVersionString = null,
                           string sourceUrl = null,
                           DateTime reviewDate = default,
                           DateTime autoReviewDate = default)
        {
            Name = name ?? Name ?? "";

            // If the updated date is older than published, set it to published.
            Published = published == default ? Published : published;
            Updated = updated == default ? Updated : updated;
            Updated = Updated < Published ? Published : Updated;

            AuthorID = authorID == 0 ? AuthorID : authorID;
            AuthorUrl = authorUrl ?? AuthorUrl ?? "";

            Stability = stability == default ? Stability : stability;
            StabilityNote = stabilityNote ?? StabilityNote ?? "";

            Note = note ?? Note ?? "";
            GameVersionString = gameVersionString ?? GameVersionString ?? Toolkit.UnknownVersion().ToString();
            SourceUrl = sourceUrl ?? SourceUrl ?? "";

            ReviewDate = reviewDate == default ? ReviewDate : reviewDate;
            AutoReviewDate = autoReviewDate == default ? AutoReviewDate : autoReviewDate;

            UpdatedThisSession = true;
        }


        /// <summary>Adds a required DLC.</summary>
        /// <returns>True if added, false if it was already in the list.</returns>
        public bool AddRequiredDlc(Enums.Dlc dlc)
        {
            if (RequiredDlcs.Contains(dlc))
            {
                return false;
            }

            RequiredDlcs.Add(dlc);
            return true;
        }


        /// <summary>Removes a required DLC.</summary>
        /// <returns>True if removal succeeded, false if not.</returns>
        public bool RemoveRequiredDlc(Enums.Dlc dlc)
        {
            return RequiredDlcs.Remove(dlc);
        }


        /// <summary>Adds a required mod.</summary>
        public void AddRequiredMod(ulong steamID)
        {
            if (!RequiredMods.Contains(steamID))
            {
                RequiredMods.Add(steamID);
            }
        }


        /// <summary>Removes a required mod.</summary>
        /// <returns>True if removal succeeded, false if not.</returns>
        public bool RemoveRequiredMod(ulong steamID)
        {
            return RequiredMods.Remove(steamID);
        }


        /// <summary>Adds a successor.</summary>
        /// <returns>True if added, false if it was already in the list.</returns>
        public bool AddSuccessor(ulong steamID)
        {
            if (Successors.Contains(steamID))
            {
                return false;
            }

            Successors.Add(steamID);
            return true;
        }


        /// <summary>Removes a successor.</summary>
        /// <returns>True if removal succeeded, false if not.</returns>
        public bool RemoveSuccessor(ulong steamID)
        {
            return Successors.Remove(steamID);
        }


        /// <summary>Adds an alternative.</summary>
        /// <returns>True if added, false if it was already in the list.</returns>
        public bool AddAlternative(ulong steamID)
        {
            if (Alternatives.Contains(steamID))
            {
                return false;
            }

            Alternatives.Add(steamID);
            return true;
        }


        /// <summary>Removes an alternative.</summary>
        /// <returns>True if removal succeeded, false if not.</returns>
        public bool RemoveAlternative(ulong steamID)
        {
            return Alternatives.Remove(steamID);
        }


        /// <summary>Adds a recommended mod.</summary>
        /// <returns>True if added, false if it was already in the list.</returns>
        public bool AddRecommendation(ulong steamID)
        {
            if (Recommendations.Contains(steamID))
            {
                return false;
            }

            Recommendations.Add(steamID);
            return true;
        }


        /// <summary>Removes a recommended mod.</summary>
        /// <returns>True if removal succeeded, false if not.</returns>
        public bool RemoveRecommendation(ulong steamID)
        {
            return Recommendations.Remove(steamID);
        }


        /// <summary>Updates one or more exclusions.</summary>
        public void UpdateExclusions(bool? exclusionForSourceUrl = null, bool? exclusionForGameVersion = null, bool? exclusionForNoDescription = null)
        {
            ExclusionForSourceUrl = exclusionForSourceUrl ?? ExclusionForSourceUrl;
            ExclusionForGameVersion = exclusionForGameVersion ?? ExclusionForGameVersion;
            ExclusionForNoDescription = exclusionForNoDescription ?? ExclusionForNoDescription;
        }


        /// <summary>Adds an exclusion for a required DLC.</summary>
        public void AddExclusion(Enums.Dlc requiredDlc)
        {
            if (!ExclusionForRequiredDlcs.Contains(requiredDlc))
            {
                ExclusionForRequiredDlcs.Add(requiredDlc);
            }
        }


        /// <summary>Adds an exclusion for a required mod.</summary>
        public void AddExclusion(ulong requiredMod)
        {
            if (!ExclusionForRequiredMods.Contains(requiredMod))
            {
                ExclusionForRequiredMods.Add(requiredMod);
            }
        }


        /// <summary>Removes an exclusion for a required DLC.</summary>
        /// <returns>True if removal succeeded, false otherwise.</returns>
        public bool RemoveExclusion(Enums.Dlc requiredDlc)
        {
            return ExclusionForRequiredDlcs.Remove(requiredDlc);
        }


        /// <summary>Removes an exclusion for a required mod.</summary>
        /// <returns>True if removal succeeded, false otherwise.</returns>
        public bool RemoveExclusion(ulong requiredMod)
        {
            return ExclusionForRequiredMods.Remove(requiredMod);
        }


        /// <summary>Updates the subscription properties.</summary>
        public void UpdateSubscription(bool isDisabled, bool isCameraScript, string modPath, DateTime downloadedTime)
        {
            IsDisabled = isDisabled;
            IsCameraScript = isCameraScript;
            ModPath = modPath;
            DownloadedTime = downloadedTime;
        }


        /// <summary>Sets the report severity for a mod.</summary>
        /// <remarks>This will only set the severity higher, not lower it.</remarks>
        public void IncreaseReportSeverity(Enums.ReportSeverity newSeverity)
        {
            ReportSeverity = (newSeverity > ReportSeverity) ? newSeverity : ReportSeverity;
        }


        /// <summary>Adds a mod change note.</summary>
        public void AddChangeNote(string changeNote)
        {
            ChangeNotes.Add(changeNote);
        }


        /// <summary>Converts the mod to a string containing the Steam ID and name.</summary>
        /// <remarks>A '[Disabled]' prefix will be included for disabled subscriptions. Optionally hides fake Steam IDs, puts the name before the ID, 
        ///          or cuts off the string at report width.</remarks>
        /// <returns>A string representing the mod.</returns>
        public string ToString(bool hideFakeID = false, bool nameFirst = false, bool cutOff = false, bool html = false)
        {
            string disabledPrefix = IsDisabled ? "[Disabled] " : string.Empty;

            string idString = IdString(hideFakeID);

            string name = !cutOff ? Name : Toolkit.CutOff(Name, ModSettings.TextReportWidth - idString.Length - 1 - disabledPrefix.Length);

            return nameFirst ? 
                $"{(html && !string.IsNullOrEmpty(disabledPrefix) ? $"<span data-i18n=\"HRT_P_D\" class=\"disabled minor f-small\">{ disabledPrefix }</span>": disabledPrefix)}{ name } { idString }" 
                : $"{(html && !string.IsNullOrEmpty(disabledPrefix) ? $"<span data-i18n=\"HRT_P_D\" class=\"disabled minor f-small\">{ disabledPrefix }</span>": disabledPrefix)}{ idString } { name }";
        }

        public string IdString(bool hideFakeID = false)
        {
            return (SteamID > ModSettings.HighestFakeID) 
                ? $"[Steam ID { SteamID, 10 }]"
                : ModSettings.BuiltinMods.ContainsValue(SteamID)
                    ? $"[{"span".Tag("built-in", localeId: "HRTC_LLMID_BI")} mod{ (hideFakeID ? "" : $" { SteamID }") }]"
                    : $"[{"span".Tag("local", localeId: "HRTC_LLMID_L")} mod{ (hideFakeID ? "" : $" { SteamID }") }]";
        }
    }
}
