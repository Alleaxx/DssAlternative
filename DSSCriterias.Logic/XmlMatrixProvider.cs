using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

using DSSLib;

namespace DSSCriterias.Logic
{
    //Версии для сохранения
    public class StatMtxXml
    {
        public Alternative[] Alternatives { get; set; }

        public Case[] Cases { get; set; }

        [XmlArrayItem("arr")]
        public List<double[]> Values { get; set; }



        public StatMtxXml() { }
        public StatMtxXml(Alternative[] alts, Case[] cases, double[,] values)
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
    public class StatGameXml : StatMtxXml
    {
        public string Name { get; set; }
        public StatGameXml()
        {

        }
        public StatGameXml(StatGame game) : base(game.Mtx.Rows, game.Mtx.Cols, game.Mtx.Values)
        {
            Name = game.Name;
        }
    }

    public static class SaverProvider
    {
        public static ISaver<T> Get<T>(SaveFormat format = SaveFormat.XML)
        {
            Type type = typeof(T);
            if (type == typeof(StatGame))
            {
                return new SaverLogged<StatGame>(new Saver<StatGame>(new StatGameProvider(format))) as ISaver<T>;
            }
            return null;
        }
    }

    public enum SaveFormat
    {
        JSON, XML
    }
    public class StatGameProvider : ITextProvider<StatGame>
    {
        private readonly ITextProvider<StatGameXml> DefaultProvider;

        public StatGameProvider(SaveFormat format)
        {
            switch (format)
            {
                case SaveFormat.JSON:
                    DefaultProvider = new JsonDefaultProvider<StatGameXml>();
                    break;
                case SaveFormat.XML:
                    DefaultProvider = new XmlDefaultProvider<StatGameXml>();
                    break;
            }
        }

        public StatGame FromTextString(string xml)
        {
            StatGameXml gameXml = DefaultProvider.FromTextString(xml);
            return new StatGame(gameXml);
        }
        public string ToTextString(StatGame game)
        {
            StatGameXml gameXml = new StatGameXml(game);
            return DefaultProvider.ToTextString(gameXml);
        }
    }
}
