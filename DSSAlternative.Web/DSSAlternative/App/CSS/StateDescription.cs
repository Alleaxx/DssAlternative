using System.Collections.Generic;

namespace DSSAlternative.Web.AppComponents
{
    public class StateDescription
    {
        public string Title { get; set; }
        public string SymbolShort { get; set; }
        public string Description { get; set; }
        public string CssStyle { get; set; }
        public string HtmlColor { get; set; }
        public List<string> CssClasses { get; private set; }
        public string CssClass => CssClasses.CreateClassList();

        public StateDescription()
        {
            CssClasses = new List<string>();
        }
    }
}
