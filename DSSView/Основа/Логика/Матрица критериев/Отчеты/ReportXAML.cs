using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSSView
{

    public abstract class ReportXAML : Report
    {
        public ReportXAML(PayMatrix matrix) : base(matrix)
        {

        }

        public override void Create()
        {
            //Генерируем текст XAML-документа
        }

        public override abstract void Open();
    }
    public class ReportPDF : ReportXAML
    {
        public ReportPDF(PayMatrix matrix) : base(matrix)
        {

        }

        public override void Open()
        {
            throw new NotImplementedException();
        }
    }
    public class ReportWord : ReportXAML
    {
        public ReportWord(PayMatrix matrix) : base(matrix)
        {

        }

        public override void Open()
        {
            throw new NotImplementedException();
        }
    }
}
