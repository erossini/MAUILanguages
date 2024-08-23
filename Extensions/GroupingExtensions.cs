using MauiLanguages.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiLanguages.Extensions
{
    public static class GroupingExtensions
    {
        public static List<LanguageModelGroup> ToGroups(this List<LanguageModel> languages)
        {
            return languages.GroupBy(x => new { x.Parent.LanguageName, x.Parent.Flag })
                        .OrderBy(x => x.Key.LanguageName)
                        .Select(x => new LanguageModelGroup(
                            x.Key.LanguageName, 
                            x.Key.Flag, 
                            x.OrderBy(l => l.LanguageName).ToList()))
                        .ToList();
        }
    }
}