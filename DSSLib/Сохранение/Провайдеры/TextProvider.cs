using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Xml.Serialization;

namespace DSSLib
{
    //Преобразовать объект в строковое представление
    public interface ITextProvider<T>
    {
        string ToTextString(T obj);
        T FromTextString(string text);
    }
}
