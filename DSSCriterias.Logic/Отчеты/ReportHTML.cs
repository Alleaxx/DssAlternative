using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSSCriterias.Logic
{
    public class ReportHTML : Report
    {
        private string HTML { get; set; }

        public ReportHTML(IStatGame matrix) : base(matrix)
        {

        }

        public override void Create()
        {
            //Генерируем HTML
        }

        public override void Open()
        {
            //Создаем файл
            //Записываем туда созданный HTML
            //Открываем
        }
    }
}
