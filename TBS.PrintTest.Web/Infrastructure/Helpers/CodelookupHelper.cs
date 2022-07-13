using TBS.PrintTest.Business.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TBS.PrintTest.Web.Infrastructure.Helpers
{
    public static class CodelookupHelper
    {
        public static List<SelectListItem> PopulateCodelookup(List<Codelookup> lookupList, string firstItemSelectionText = null, bool sortByText = false)
        {
            var returnList = new List<SelectListItem>();

            var list = lookupList.Select(m => new SelectListItem
            {
                // if the code is a deleted code, put it in brackets.
                Text = m.Active ? m.Text : string.Format("({0})", m.Text),
                Value = m.Id.ToString()
            }); 

            // code lookups should have a sort order field (TBS Standard) and lookup repository
            // method should return list already sorted.  Use sortByText = true to sort the
            // list by text instead.
            if (sortByText)
            {
                returnList = list.OrderBy(x => x.Text).ToList();
            }
            else
            {
                returnList = list.ToList();
            }
            
            // if text is provided for selection record, insert it at top of list.
            if (firstItemSelectionText != null)
            {
                returnList.Insert(0, new SelectListItem()
                {
                    Text = firstItemSelectionText
                });
            }

            return returnList;
        }

        /// <summary>
        /// Returns the text of a SelectListItem list for a given value.
        /// </summary>
        /// <param name="list">SelectListItem list</param>
        /// <param name="value">list value you want the text for</param>
        /// <returns></returns>
        public static string GetLookupValue(List<SelectListItem> list, string value)
        {
            var text = string.Empty;

            var result = list.SingleOrDefault(x => x.Value == value);
            if (result != null)
            {
                text = result.Text;
            }

            return text;
        }

        /// <summary>
        /// Returns the text of a SelectListItem list for a given integer as the id.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetLookupValue(List<SelectListItem> list, int? value)
        {
            var name = string.Empty;

            if (value.HasValue)
            {
                name = GetLookupValue(list, value.Value.ToString());
            }

            return name;
        }
        
    }
}
