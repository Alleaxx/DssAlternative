using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DSSAlternative.AHP
{
    public class CssCorrectCheck : CssCheck
    {
        public CssCorrectCheck(ICheck check)
        {
            AddRuleClass(() => check.IsOk, "passed", "errored");
        }
    }
}
