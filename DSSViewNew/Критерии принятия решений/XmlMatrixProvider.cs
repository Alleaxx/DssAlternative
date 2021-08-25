using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

using DSSLib;

namespace DSSView
{
    [Serializable]
    public class PayMatrixXml
    {
        public Alternative[] Alternatives { get; set; }

        public Case[] Cases { get; set; }

        [XmlArrayItem("arr")]
        public List<double[]> Values { get; set; }



        public PayMatrixXml() { }
        public PayMatrixXml(Alternative[] alts, Case[] cases, double[,] values)
        {
            Alternatives = alts;
            Cases = cases;
            Values = new List<double[]>();
            for (int r = 0; r < values.GetLength(0); r++)
            {
                Values.Add(new double[values.GetLength(1)]);
                for (int c = 0; c < values.GetLength(1); c++)
                {
                    Values[r][c] = values[r, c];
                }
            }
        }
    }

    public class MatrixProvider : IXMLProvider<PayMatrix>
    {
        public PayMatrix FromXml(string xml)
        {
            DefaultXmlProvider<PayMatrixXml> provider = new DefaultXmlProvider<PayMatrixXml>();
            PayMatrixXml payMatrix = provider.FromXml(xml);
            return new PayMatrixRisc(payMatrix);
        }

        public string ToXml(PayMatrix matrix)
        {
            DefaultXmlProvider<PayMatrixXml> provider = new DefaultXmlProvider<PayMatrixXml>();
            PayMatrixXml payMatrix = new PayMatrixXml(matrix.RowsArr,matrix.ColsArr,matrix.Arr);
            return provider.ToXml(payMatrix);
        }
    }
}
