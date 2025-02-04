using System.Collections.Generic;

namespace DSSAlternative.Web.Models
{
    public interface ICssList
    {
        ICssList AddIfTrue(bool condition, string trueClass, string falseClass = "");
        ICssList AddIfFalse(bool condition, string falseClass, string trueClass = "");
        string GetClasses();
    }
    public class CssList : ICssList
    {
        public List<string> ClassList { get; set; }
        public CssList()
        {
            ClassList = new List<string>();
        }
        public ICssList AddIfTrue(bool condition, string trueClass, string falseClass = "")
        {
            if (condition)
            {
                ClassList.Add(trueClass);
            }
            else
            {
                ClassList.Add(falseClass);
            }
            return this;
        }
        public ICssList AddIfFalse(bool condition, string falseClass, string trueClass = "")
        {
            return AddIfTrue(!condition, falseClass, trueClass);
        }
        public string GetClasses()
        {
            return string.Join(" ", ClassList);
        }
    }
}
