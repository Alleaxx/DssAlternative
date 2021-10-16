using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


using DSSAlternative.AHP;

namespace DSSAlternative.AppComponents
{
    public class DssState
    {
        public override string ToString()
        {
            return $"DSS-Состояние на {Saved:dd.MM.yyyy HH:mm}, {OpenedTemplates.Count()} задач ({SelectedTemplateIndex})";
        }
        public DateTime Saved { get; set; }
        public int SelectedTemplateIndex { get; set; }
        public IEnumerable<ITemplate> OpenedTemplates { get; set; }

        public DssState(DSS dss)
        {
            Saved = DateTime.Now;
            OpenedTemplates = dss.Projects.Select(pr => new Template(pr)).ToArray();
            SelectedTemplateIndex = dss.Projects.IndexOf(dss.Project);
        }

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
