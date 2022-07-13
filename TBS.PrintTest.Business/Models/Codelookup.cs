
using System.Threading;

namespace TBS.PrintTest.Business.Models
{
    public class Codelookup
    {
        string _lang;

        public Codelookup()
        {
            _lang = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
        }
        public int Id { get; set; }
        public string Code { get; set; }
        public string EnglishText { get; set; }
        public string FrenchText { get; set; }
        public bool Active { get; set; }
        public string Text
        {
            get
            {
                return (_lang == "fr" ? FrenchText : EnglishText);
            }
        }
    }
}
