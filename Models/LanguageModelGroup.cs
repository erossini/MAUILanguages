using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiLanguages.Models
{
    public class LanguageModelGroup : List<LanguageModel>
    {
        public string Name { get; private set; }
        public string Flag { get; private set; }

        public LanguageModelGroup(string name, string flag, List<LanguageModel> languages) : base(languages)
        {
            Name = name;
            Flag = flag;
        }
    }
}
