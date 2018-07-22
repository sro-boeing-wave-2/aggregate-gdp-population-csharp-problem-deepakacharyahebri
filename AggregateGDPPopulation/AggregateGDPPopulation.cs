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
        public List<string> FileContents { get; set; }
        public string JSONMap { get; set; }

        public async Task<List<string>> ReadFileContents(string path)
        {
            try
            {
                StreamReader FileContents = new StreamReader(path);
                List<string> ContentsByLine = new List<string>();
                string s;
                while ((s = await FileContents.ReadLineAsync()) != null)
                {
                    ContentsByLine.Add(s);
                }
                return ContentsByLine;
            }
            catch (Exception) { return new List<string>(); }
        }
        public async Task<string> ReadMapFile(string path)
        {
            StreamReader FileContents = new StreamReader(path);
            string s = await FileContents.ReadToEndAsync();
            return s;
        }
        public async void ReadMapAndCSVFile(string CSVPath, string CountryContinentMapPath)
        {
            Task<List<string>> FileContentsTask = ReadFileContents(CSVPath);
            Task<string> JSONMapTask = ReadMapFile(CountryContinentMapPath);
            FileContents = await FileContentsTask;
            JSONMap = await JSONMapTask;
        }
        public async void WriteToJSONFile(string FilePath, string Content)
        {
            using (StreamWriter FileContents = new StreamWriter(FilePath))
            {
                await FileContents.WriteAsync(Content);
            }                
        }
        public void CalculateAggregateGdp()
        {
            //List<string> FileContents = File.ReadLines(@"../../../../AggregateGDPPopulation/data/datafile.csv").ToList();
            //StreamReader JSONFileContents = new StreamReader(@"../../../../AggregateGDPPopulation/data/country-continent-map.json");
            //var JSONMap = JSONFileContents.ReadToEnd();
            ReadMapAndCSVFile("../../../../AggregateGDPPopulation/data/datafile.csv", "../../../../AggregateGDPPopulation/data/country-continent-map.json");
            var CountryContinentMap = JObject.Parse(JSONMap);
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
                    catch (Exception)
                    {
                        GDPPopulation g = new GDPPopulation() { GDP_2012 = Gdp, POPULATION_2012 = Population };
                        JSONObject.Add(Continent, g);
                    }
                }
                catch (Exception) { }
            }
            var JSONOutput = Newtonsoft.Json.JsonConvert.SerializeObject(JSONObject);
            WriteToJSONFile("../../../../AggregateGDPPopulation/output/output.json", JSONOutput);
            //File.WriteAllText("../../../../AggregateGDPPopulation/output/output.json", JSONOutput);
            //StreamWriter WriteToJSONFile = new StreamWriter("../../../../AggregateGDPPopulation/output/output.json");
            //WriteToJSONFile.WriteLine(JSONOutput);
        }
    }
}
