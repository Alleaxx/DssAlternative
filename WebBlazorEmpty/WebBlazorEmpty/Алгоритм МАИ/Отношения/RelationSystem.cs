using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebBlazorEmpty.AHP;

namespace WebBlazorEmpty
{
    public class Rating
    {
        public string Name { get; private set; }
        public int Value { get; private set; }
        public string Style { get; set; }

        public Rating(int val)
        {
            Value = val;
            switch (val)
            {
                case 1:
                    Name = "Одинаковы по значимости";
                    Style = "color:Black;font-weight:bold";
                    break;
                case 3:
                    Name = "Немного важнее";
                    Style = "color:green;font-weight:bold";
                    break;
                case 5:
                    Name = "Важнее";
                    Style = "color:blue;font-weight:bold";
                    break;
                case 7:
                    Name = "Значительно важнее";
                    Style = "color:violet;font-weight:bold";
                    break;
                case 9:
                    Name = "Абсолютно важнее";
                    Style = "color:orange;font-weight:bold";
                    break;
            }
        }
    }
}
