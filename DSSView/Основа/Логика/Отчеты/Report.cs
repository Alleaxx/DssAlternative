using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSSView
{
    interface IReport
    {
        void Create();
        void Open();
    }

    abstract class Report : IReport
    {
        protected PayMatrix Matrix { get; set; }
        protected DateTime Creation { get; set; }

        public Report(PayMatrix matrix)
        {
            Matrix = matrix;
        }
        public abstract void Create();
        public abstract void Open();


        public static IReport GetReport(PayMatrix matrix,string text)
        {
            Report report;
            switch (text)
            {
                case "Word":
                    report = new ReportWord(matrix);
                    break;
                case "PDF":
                    report = new ReportPDF(matrix);
                    break;
                case "HTML":
                    report = new ReportHTML(matrix);
                    break;
                case "Excel":
                    report = new ReportExcel(matrix);
                    break;
                default:
                    throw new Exception($"Отчет типа {text} не распознан");
            }
            return report;
        }
    }
}
