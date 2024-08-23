using MauiLanguages.Extensions;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiLanguages.Helpers
{
    public class CultureCountryOverrides
    {
        private const string DefaultOverrides = "en=en-US,zh=zh-CN,zh-CHT=zh-CN,zh-HANT=zh-CN,fy=fy,";

        private static readonly IEqualityComparer<KeyValuePair<CultureInfo, CultureInfo>>? _comparer = new DelegateEqualityComparer<KeyValuePair<CultureInfo, CultureInfo>>(item => item.Key);
        private static readonly IEnumerable<KeyValuePair<CultureInfo, CultureInfo>>? _defaultOverrides = ReadSettings(DefaultOverrides);

        private Dictionary<CultureInfo, CultureInfo>? _overrides;

        public CultureCountryOverrides()
        {
            _overrides = LoadOverrides();
        }

        private Dictionary<CultureInfo, CultureInfo> LoadOverrides()
        {
            return new Dictionary<CultureInfo, CultureInfo>(ReadSettings().Distinct(_comparer).ToDictionary(item => item.Key, item => item.Value));
        }

#pragma warning disable CA1043 // Use Integral Or String Argument For Indexers
        [DisallowNull]
        public CultureInfo? this[CultureInfo neutralCulture]
#pragma warning restore CA1043 // Use Integral Or String Argument For Indexers
        {
            get
            {
                if (_overrides.TryGetValue(neutralCulture, out var specificCulture))
                    return specificCulture;

                if (!neutralCulture.IsNeutralCulture)
                    return specificCulture;

                specificCulture = GetDefaultSpecificCulture(neutralCulture);

                return specificCulture;
            }
            set
            {
                if (Equals(value, GetDefaultSpecificCulture(neutralCulture)))
                {
                    _overrides.Remove(neutralCulture);
                }
                else
                {
                    _overrides[neutralCulture] = value;
                }
            }
        }

        private static CultureInfo? GetDefaultSpecificCulture(CultureInfo neutralCulture)
        {
            var cultureName = neutralCulture.Name;
            var specificCultures = neutralCulture.GetDescendants().ToArray();

            var preferredSpecificCultureName = cultureName + @"-" + cultureName.ToUpperInvariant();

            var specificCulture =
                // If a specific culture exists with "subtag == primary tag" (e.g. de-DE), use this
                specificCultures.FirstOrDefault(c => c.Name.Equals(preferredSpecificCultureName, StringComparison.OrdinalIgnoreCase))
                // else it's more likely that the default one starts with the same letter as the neutral culture name (sv-SE, not sv-FI)
                ?? specificCultures.FirstOrDefault(c => c.Name.Split('-').Last().StartsWith(cultureName.Substring(0, 1), StringComparison.OrdinalIgnoreCase))
                // If nothing else matches, use the first.
                ?? specificCultures.FirstOrDefault();

            return specificCulture;
        }

        private IEnumerable<KeyValuePair<CultureInfo, CultureInfo>> ReadSettings()
        {
            var settings = DefaultOverrides;

            foreach (var keyValuePair in ReadSettings(settings))
                yield return keyValuePair;
        }

        private static IEnumerable<KeyValuePair<CultureInfo, CultureInfo>> ReadSettings(string settings)
        {
            var cultureCountryOverrides = settings.Split(',');

            foreach (var item in cultureCountryOverrides)
            {
                CultureInfo neutralCulture;
                CultureInfo specificCulture;

                try
                {
                    var parts = item.Split('=').Select(i => i.Trim()).ToArray();
                    if (parts.Length != 2)
                        continue;

                    neutralCulture = CultureInfo.GetCultureInfo(parts[0]);
                    specificCulture = CultureInfo.GetCultureInfo(parts[1]);
                }
                catch (ArgumentException)
                {
                    continue;
                }

                yield return new KeyValuePair<CultureInfo, CultureInfo>(neutralCulture, specificCulture);
            }
        }
    }
}