using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DSSAlternative.AHP;

namespace DSSAlternative.AppComponents
{
    public interface ICss
    {
        string CssClass();
        string CssStyle();
    }

    public class CssCheck : ICss
    {
        private List<CssRule> Rules { get; set; } = new List<CssRule>();


        //Добавление
        public void AddRuleClass(string clas)
        {
            CssRule rule = new CssRule().SetClass(clas, "");
            Rules.Add(rule);
        }
        public void AddRuleClass(bool check, string classTrue, string classFalse = "")
        {
            CssRule rule = new CssRule(() => check).SetClass(classTrue, classFalse);
            Rules.Add(rule);
        }
        public void AddRuleClass(Func<bool> check, string classTrue, string classFalse = "")
        {
            CssRule rule = new CssRule(check).SetClass(classTrue, classFalse);
            Rules.Add(rule);
        }

        public void AddRuleStyle(string style)
        {
            CssRule rule = new CssRule().SetStyle(style, "");
            Rules.Add(rule);
        }
        public void AddRuleStyle(Func<bool> check, string styleTrue, string styleFalse = "")
        {
            CssRule rule = new CssRule(check).SetStyle(styleTrue, styleFalse);
            Rules.Add(rule);
        }

        public void AddRule(CssRule rule)
        {
            Rules.Add(rule);
        }


        //Получение
        public string CssClass()
        {
            return string.Join(' ', Rules.Select(r => r.CssClass()));
        }
        public string CssStyle()
        {
            return string.Join(' ', Rules.Select(r => r.CssStyle()));
        }
    }

    public class CssRule : ICss
    {
        private readonly Func<bool> Condition;
        private string ClassTrue { get; set; }
        private string ClassFalse { get; set; }
        private string StyleTrue { get; set; }
        private string StyleFalse { get; set; }


        public CssRule()
        {
            Condition = IsTrue;
        }
        public CssRule(Func<bool> condition)
        {
            Condition = condition;
        }
        public CssRule SetClass(string ifTrue, string ifFalse)
        {
            ClassTrue = ifTrue;
            ClassFalse = ifFalse;
            return this;
        }
        public CssRule SetStyle(string ifTrue, string ifFalse)
        {
            StyleTrue = ifTrue;
            StyleFalse = ifFalse;
            return this;
        }


        public string CssClass()
        {
            return Condition != null ? (Condition.Invoke() ? ClassTrue : ClassFalse) : ClassTrue;
        }
        public string CssStyle()
        {
            return Condition != null ? (Condition.Invoke() ? StyleTrue : StyleFalse) : StyleTrue;
        }


        private static readonly Func<bool> IsTrue = () => true;
    }
}
