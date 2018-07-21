using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace AggregateGDPPopulation
{
    public class GDPPopulation
    {
        public float GDP_2012 { get; set; }
        public float POPULATION_2012 { get; set; }
    }
    public class AggregateGDP
    {
        public void CalculateAggregateGdp()
        {
            List<string> FileContents = File.ReadLines(@"../../../../AggregateGDPPopulation/data/datafile.csv").ToList();
            StreamReader JSONFileContents = new StreamReader(@"../../../../AggregateGDPPopulation/data/country-continent-map.json");
            var json = JSONFileContents.ReadToEnd();
            var CountryContinentMap = JObject.Parse(json);
            /* https://social.msdn.microsoft.com/Forums/en-US/525ff8f2-13f5-4602-bce3-78b909cadedb/how-to-read-and-write-a-json-file-in-c?forum=csharpgeneral 
             * https://www.newtonsoft.com/json/help/html/T_Newtonsoft_Json_Linq_JObject.htm 
             * */
            string headerText = FileContents[0];
            List<string> headers = headerText.Split(',').ToList();
            int IndexOfCountry = headers.IndexOf("\"Country Name\"");
            int IndexOfPopulation = headers.IndexOf("\"Population (Millions) 2012\"");
            int IndexOfGDP = headers.IndexOf("\"GDP Billions (USD) 2012\"");
            Dictionary<string, GDPPopulation> JSONObject = new Dictionary<string, GDPPopulation>();
            for (int i = 1; i < FileContents.Count; i++)
            {
                List<string> RowOfData = FileContents[i].Split(',').ToList();
                string Country = RowOfData[IndexOfCountry].Trim('\"');
                float Population = float.Parse(RowOfData[IndexOfPopulation].Trim('\"'));
                float Gdp = float.Parse(RowOfData[IndexOfGDP].Trim('\"'));
                try
                {
                    string Continent = CountryContinentMap.GetValue(RowOfData[IndexOfCountry].Trim('\"')).ToString();
                    try
                    {
                        JSONObject[Continent].GDP_2012 += Gdp;
                        JSONObject[Continent].POPULATION_2012 += Population;
                    }
                    catch (Exception e)
                    {
                        GDPPopulation g = new GDPPopulation() { GDP_2012 = Gdp, POPULATION_2012 = Population };
                        JSONObject.Add(Continent, g);
                    }
                }
                catch (Exception e) { }
            }
            var JSONOutput = Newtonsoft.Json.JsonConvert.SerializeObject(JSONObject);
            Console.WriteLine(Environment.CurrentDirectory);
            File.WriteAllText("../../../../AggregateGDPPopulation/output/output.json", JSONOutput);
            //StreamWriter WriteToJSONFile = new StreamWriter("../../../../AggregateGDPPopulation/output/output.json");
            //WriteToJSONFile.WriteLine(JSONOutput);
        }
    }
}
