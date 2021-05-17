using System;
using System.Collections.Generic;
using System.Text;

namespace DSSLib
{
    /// <summary>
    /// Представление объекта, имеющего своё имя и уникальный идентификатор
    /// </summary>
    public interface IId
    {
        string ID { get; }
        string Name { get; }
        //string Description { get; }
    }
}
