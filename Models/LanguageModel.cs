using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiLanguages.Models
{
    public class LanguageModel
    {
        public CultureInfo? CultureInfo { get; set; }
        public string? LanguageName { get; set; }
        public LanguageModel? Parent { get; set; }
        public string? Abbreviation { get; set; }
        public string? Flag { get; set; }
    }
}