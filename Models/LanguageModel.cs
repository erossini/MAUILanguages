using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiLanguages.Models
{
    /// <summary>
    /// Class LanguageModel.
    /// </summary>
    public class LanguageModel
    {
        /// <summary>
        /// Gets or sets the culture information.
        /// </summary>
        /// <value>The culture information.</value>
        public CultureInfo? CultureInfo { get; set; }
        /// <summary>
        /// Gets or sets the name of the language.
        /// </summary>
        /// <value>The name of the language.</value>
        public string? LanguageName { get; set; }
        /// <summary>
        /// Gets or sets the parent.
        /// </summary>
        /// <value>The parent.</value>
        public LanguageModel? Parent { get; set; }
        /// <summary>
        /// Gets or sets the abbreviation.
        /// </summary>
        /// <value>The abbreviation.</value>
        public string? Abbreviation { get; set; }
        /// <summary>
        /// Gets or sets the flag.
        /// </summary>
        /// <value>The flag.</value>
        public string? Flag { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is supported.
        /// </summary>
        /// <value><c>true</c> if this instance is supported; otherwise, <c>false</c>.</value>
        public bool IsSupported { get; set; } = false;
    }
}