using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiLanguages.Extensions
{
    public static class CultureInfoExtensions
    {
        private static readonly Dictionary<CultureInfo, CultureInfo[]> _childCache = new Dictionary<CultureInfo, CultureInfo[]>();

        //
        // Summary:
        //     Returns an enumeration of the ancestor elements of this element.
        //
        // Parameters:
        //   self:
        //     The starting element.
        //
        // Returns:
        //     The ancestor list.
        public static IEnumerable<CultureInfo> GetAncestors(this CultureInfo self)
        {
            CultureInfo item = self.Parent;
            while (!string.IsNullOrEmpty(item.Name))
            {
                yield return item;
                item = item.Parent;
            }
        }

        //
        // Summary:
        //     Returns an enumeration of elements that contain this element, and the ancestors
        //     of this element.
        //
        // Parameters:
        //   self:
        //     The starting element.
        //
        // Returns:
        //     The ancestor list.
        public static IEnumerable<CultureInfo> GetAncestorsAndSelf(this CultureInfo self)
        {
            CultureInfo item = self;
            while (!string.IsNullOrEmpty(item.Name))
            {
                yield return item;
                item = item.Parent;
            }
        }

        //
        // Summary:
        //     Enumerates the immediate children of the specified item.
        //
        // Parameters:
        //   item:
        //     The item.
        //
        // Returns:
        //     The immediate children of the specified item.
        public static ICollection<CultureInfo> GetChildren(this CultureInfo item)
        {
            return _childCache.ForceValue(item, CreateChildList);
        }

        private static CultureInfo[] CreateChildList(CultureInfo? parent)
        {
            CultureInfo parent2 = parent;
            return (from child in CultureInfo.GetCultures(CultureTypes.AllCultures)
                    where child?.Parent.Equals(parent2) ?? false
                    select child).ToArray();
        }

        //
        // Summary:
        //     Enumerates all descendants of the specified item.
        //
        // Parameters:
        //   item:
        //     The item.
        //
        // Returns:
        //     The descendants of the item.
        public static IEnumerable<CultureInfo> GetDescendants(this CultureInfo item)
        {
            foreach (CultureInfo child in item.GetChildren())
            {
                yield return child;
                foreach (CultureInfo descendant in child.GetDescendants())
                {
                    yield return descendant;
                }
            }
        }
    }
}
