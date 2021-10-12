using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DSSAlternative.AHP
{
    public class DssState
    {
        public DateTime Saved { get; set; }
        public int SelectedTemplate { get; set; }
        public IEnumerable<ITemplate> OpenedTemplates { get; set; }
        //Состояние открытых проектов
        //Указатель на выбранный проект
        //Список проектов
        //Редактируемая иерархия проекта
        //Активная проблема проекта
        //Текущие матрицы связей

        //Шаблоны проектов
        //Юзер
        //Шаблоны проектов
        //Состояние открытых проектов
        //Шаблоны открытых проектов
    }
}
