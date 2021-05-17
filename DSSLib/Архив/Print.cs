using System;
using System.Collections.Generic;
using System.Text;

namespace DSSLib
{
    static class Print
    {
        public static string GetPrintText(string name, string value, bool prop)
        {
            string pointer = prop ? "- " : "";
            string del = prop ? " . . . " : ": ";

            return $" {pointer + name + del,-40}{value}";
        }
    }
}
