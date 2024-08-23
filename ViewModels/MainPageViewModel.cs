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
        [ObservableProperty] private ObservableCollection<LanguageModelGroup>? availableCulturesGroups;
        [ObservableProperty] private ObservableCollection<LanguageModelGroup>? filteredCulturesGroups;
        [ObservableProperty] private ObservableCollection<LanguageModel>? availableCultures;
        [ObservableProperty] private string? flagTest;

        private readonly CultureCountryOverrides? cultureCountryOverrides;
        private const string DefaultOverrides = "en=en-US,zh=zh-CN,zh-CHT=zh-CN,zh-HANT=zh-CN,fy=fy,";

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
            cultureCountryOverrides = new CultureCountryOverrides();
            AvailableCulturesGroups = new ObservableCollection<LanguageModelGroup>(GetLanguages());
            FilteredCulturesGroups = AvailableCulturesGroups;
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
                    Flag = $"{GetFlag(culture)}.png"
                };

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

            return groups;
        }

        public string GetFlag(CultureInfo culture)
        {
            return ConvertCultureIntoAbbreviation(culture, 0);
        }

        private string? ConvertCultureIntoAbbreviation(CultureInfo culture, int recursionCounter = 0)
        {
            var cultureName = culture.Name;

            var countryOverride = cultureCountryOverrides[culture];
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