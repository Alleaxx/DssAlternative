using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DSSView
{
    interface IXMLProvider<T>
    {
        string ToXml(T obj);
        T FromXml(string xml);
    }
    public class DefaultXmlProvider<T> : IXMLProvider<T>
    {
        public string ToXml(T Object)
        {
            XmlSerializer formatter = new XmlSerializer(typeof(T));

            StringBuilder stringBuilder = new StringBuilder();
            using (StringWriter sw = new StringWriter(stringBuilder))
            {
                formatter.Serialize(sw, Object);
            }
            return stringBuilder.ToString();
        }
        public T FromXml(string XML)
        {
            XmlSerializer formatterOptions = new XmlSerializer(typeof(T));
            using (TextReader reader = new StringReader(XML))
            {              
                T data = (T)formatterOptions.Deserialize(reader);
                return data;
            }
        }
    }



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
