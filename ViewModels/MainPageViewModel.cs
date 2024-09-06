using CommunityToolkit.Mvvm.ComponentModel;
using MauiLanguages.Extensions;
using MauiLanguages.Helpers;
using MauiLanguages.Models;
using PSC.CSharp.Library.CountryData;
using PSC.CSharp.Library.CountryData.SVGImages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiLanguages.ViewModels
{
    public partial class MainPageViewModel : ObservableObject
    {
        #region Observable Properties

        [ObservableProperty] private ObservableCollection<LanguageModelGroup>? availableCulturesGroups;
        [ObservableProperty] private ObservableCollection<LanguageModelGroup>? filteredCulturesGroups;
        [ObservableProperty] private ObservableCollection<LanguageModel>? availableCultures;
        [ObservableProperty] private ObservableCollection<LanguageModel>? recentCultures;
        [ObservableProperty] private ObservableCollection<LanguageModel>? supportedCultures;
        [ObservableProperty] private string? flagTest;
        [ObservableProperty] private string? selectFilter;

        [ObservableProperty] private bool isEmpty;
        [ObservableProperty] private bool isLoading;
        [ObservableProperty] private bool showGroups;
        [ObservableProperty] private bool showOnlySupported;

        [ObservableProperty] private ObservableCollection<FilterModel>? filters;

        #endregion

        private readonly CultureCountryOverrides? CultureCountryOverrides;
        private const string DefaultOverrides = "en=en-US,zh=zh-CN,zh-CHT=zh-CN,zh-HANT=zh-CN,fy=fy,";
        private string[] StarCultureList = { "en-GB", "de-DE", "fr-FR", "it-IT", "pt-PT", "es-ES" };
        private string[] RecentCultureList = { };

        private static readonly string[] _existingFlags =
        {
            "ad", "ae", "af", "ag", "ai", "al", "am", "an", "ao", "ar", "as", "at", "au", "aw", "ax", "az", "ba", "bb", "bd", "be", "bf",
            "bg", "bh", "bi", "bj", "bm", "bn", "bo", "br", "bs", "bt", "bv", "bw", "by", "bz", "ca", "cc", "cd", "cf", "cg", "ch", "ci",
            "ck", "cl", "cm", "cn", "co", "cr", "cs", "cu", "cv", "cx", "cy", "cz", "de", "dj", "dk", "dm", "do", "dz", "ec", "ee", "eg",
            "eh", "er", "es", "et", "fi", "fj", "fk", "fm", "fo", "fr", "fy", "ga", "gb", "gd", "ge", "gf", "gh", "gi", "gl", "gm",
            "gn", "gp", "gq", "gr", "gs", "gt", "gu", "gw", "gy", "hk", "hm", "hn", "hr", "ht", "hu", "id", "ie", "il", "in", "io", "iq",
            "ir", "is", "it", "jm", "jo", "jp", "ke", "kg", "kh", "ki", "km", "kn", "kp", "kr", "kw", "ky", "kz", "la", "lb", "lc", "li",
            "lk", "lr", "ls", "lt", "lu", "lv", "ly", "ma", "mc", "md", "me", "mg", "mh", "mk", "ml", "mm", "mn", "mo", "mp", "mq", "mr",
            "ms", "mt", "mu", "mv", "mw", "mx", "my", "mz", "na", "nc", "ne", "nf", "ng", "ni", "nl", "no", "np", "nr", "nu", "nz", "om",
            "pa", "pe", "pf", "pg", "ph", "pk", "pl", "pm", "pn", "pr", "ps", "pt", "pw", "py", "qa", "re", "ro", "rs", "ru", "rw", "sa",
            "sb", "sc", "sd", "se", "sg", "sh", "si", "sj", "sk", "sl", "sm", "sn", "so", "sr", "st", "sv", "sy", "sz", "tc", "td", "tf",
            "tg", "th", "tj", "tk", "tl", "tm", "tn", "to", "tr", "tt", "tv", "tw", "tz", "ua", "ug", "um", "us", "uy", "uz", "va", "vc",
            "ve", "vg", "vi", "vn", "vu", "wf", "ws", "ye", "yt", "za", "zm", "zw"
        };

        public MainPageViewModel()
        {
            IsEmpty = true;
            isLoading = false;

            Filters = new ObservableCollection<FilterModel>() { 
                new FilterModel() { Name = "All languages", Value = "All" }, 
                new FilterModel() { Name = "Supported", Value = "Supported" }, 
                new FilterModel() { Name = "Recently used", Value = "Recent" } 
            };
            CultureCountryOverrides = new CultureCountryOverrides();
            RecentCultures = new ObservableCollection<LanguageModel>();
            SupportedCultures = new ObservableCollection<LanguageModel>();

            AvailableCulturesGroups = new ObservableCollection<LanguageModelGroup>(GetLanguages());
            FilteredCulturesGroups = AvailableCulturesGroups;

            ShowGroups = true;
            ShowOnlySupported = false;
        }

        public void FilterItems(string filter)
        {
            if (string.IsNullOrWhiteSpace(filter))
            {
                FilteredCulturesGroups = new ObservableCollection<LanguageModelGroup>(availableCulturesGroups);
            }
            else
            {
                var filters = availableCultures.Where(item => item.LanguageName.ToLower().Contains(filter.ToLower()) || 
                                                      item.Abbreviation.ToLower().Contains(filter.ToLower())).ToList();
                FilteredCulturesGroups = new ObservableCollection<LanguageModelGroup>(filters.ToGroups());
            }
        }

        public List<LanguageModelGroup> GetLanguages()
        {
            FlagTest = SVGFlags.GetFullSVGByCode("it");

            List<LanguageModel> rtn = new List<LanguageModel>();
            List<LanguageModel> recents = new List<LanguageModel>();

            var _availableCultures = CultureInfo.GetCultures(CultureTypes.AllCultures)
                                    .Where(culture => !CultureInfo.InvariantCulture.Equals(culture))
                                    .OrderBy(culture => culture.DisplayName)
                                    .ToArray();

            foreach (var culture in _availableCultures)
            {
                LanguageModel item = new()
                {
                    Abbreviation = culture.IetfLanguageTag,
                    CultureInfo = culture,
                    LanguageName = culture.DisplayName,
                    Flag = $"{GetFlag(culture)}.png",
                    IsSupported = StarCultureList.Contains(culture.IetfLanguageTag)
                };

                if(RecentCultureList.Contains(culture.IetfLanguageTag))
                    recents.Add(item);

                if (culture.Parent != null && !string.IsNullOrEmpty(culture.Parent.IetfLanguageTag))
                    item.Parent = new LanguageModel()
                    {
                        Abbreviation = culture.Parent.IetfLanguageTag,
                        LanguageName = culture.Parent.DisplayName,
                        Flag = $"{GetFlag(culture.Parent)}.png"
                    };
                else
                    item.Parent = new LanguageModel()
                    {
                        Abbreviation = item.Abbreviation,
                        LanguageName = item.LanguageName,
                        Flag = item.Flag
                    };

                rtn.Add(item);
            }

            AvailableCultures = new ObservableCollection<LanguageModel>(rtn);
            List<LanguageModelGroup> groups = rtn.ToGroups();

            RecentCultures = new ObservableCollection<LanguageModel>(recents);
            SupportedCultures = new ObservableCollection<LanguageModel>(AvailableCultures.Where(c => c.IsSupported).ToList());

            return groups;
        }

        public string GetFlag(CultureInfo culture)
        {
            return ConvertCultureIntoAbbreviation(culture, 0);
        }

        private string? ConvertCultureIntoAbbreviation(CultureInfo culture, int recursionCounter = 0)
        {
            var cultureName = culture.Name;

            var countryOverride = CultureCountryOverrides[culture];
            if (countryOverride != null)
            {
                culture = countryOverride;
                cultureName = culture.Name;
            }

            var cultureParts = cultureName.Split('-');
            if (!cultureParts.Any())
                return null;

            var key = cultureParts.Last();

            if (Array.BinarySearch(_existingFlags, key, StringComparer.OrdinalIgnoreCase) < 0)
            {
                var bestMatch = culture.GetDescendants()
                    .Select(item => ConvertCultureIntoAbbreviation(item, recursionCounter))
                    .FirstOrDefault(item => item != null);

                if (bestMatch is null && recursionCounter < 3 && !culture.IsNeutralCulture)
                {
                    return ConvertCultureIntoAbbreviation(culture.Parent, recursionCounter + 1);
                }

                return bestMatch;
            }

            var resourcePath = string.Format(CultureInfo.InvariantCulture, @"{0}", key);
            return resourcePath;
        }
    }
}