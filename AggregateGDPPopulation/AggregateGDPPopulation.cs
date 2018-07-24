using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace AggregateGDPPopulation
{
    public static class FileUtilities
    {
        public static async Task<List<string>> ReadFileContentsByLineAsync(string Path)
        {
            try
            {
                StreamReader FileContents = new StreamReader(Path);
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
        public static async Task<string> ReadFileToEndAsync(string Path)
        {
            StreamReader FileContents = new StreamReader(Path);
            string s = await FileContents.ReadToEndAsync();
            return s;
        }
        public static async void WritingToFileAsync(string FilePath, string Content)
        {
            using (StreamWriter FileContents = new StreamWriter(FilePath))
            {
                await FileContents.WriteAsync(Content);
            }
        }
    }
    public class GDPPopulation
    {
        private float Gdp_2012;
        private float Population_2012;

        public float GDP_2012
        {
            get
            {
                return Gdp_2012;
            }
            set
            {
                this.Gdp_2012 = value;
            }
        }
        public float POPULATION_2012
        {
            get
            {
                return Population_2012;
            }
            set
            {
                this.Population_2012 = value;
            }
        }
    }
    public class AggregateGDPPopulation
    {
        public Dictionary<string, GDPPopulation> AggregatedValues;

        public AggregateGDPPopulation()
        {
            AggregatedValues = new Dictionary<string, GDPPopulation>();
        }
        public void AddOrUpdateValuesInAggregatedValue(string Continent, float Gdp, float Population)
        {
            try
            {
                AggregatedValues[Continent].GDP_2012 += Gdp;
                AggregatedValues[Continent].POPULATION_2012 += Population;
            }
            catch (Exception)
            {
                AggregatedValues.Add(Continent, new GDPPopulation() { GDP_2012 = Gdp, POPULATION_2012 = Population });
            }
        }
    }
    static public class JSONSerializers
    {
        public static JObject DeSerializeString(string s)
        {
            return JObject.Parse(s);
        }
        public static string SerializeJObject(Dictionary<string, GDPPopulation> JSONObject)
        {
            return JsonConvert.SerializeObject(JSONObject);
        }
    }
    public class CalculateAggregateGdpPopulation
    {
        public string CSVPath;
        public string CountryContinentMapPath;
        public AggregateGDPPopulation AggregatedData;
        public CalculateAggregateGdpPopulation()//string CSVPath, string CountryContinentMapPath)
        {
            this.CSVPath = "../../../../AggregateGDPPopulation/data/datafile.csv";
            this.CountryContinentMapPath = "../../../../AggregateGDPPopulation/data/country-continent-map.json";
            AggregatedData = new AggregateGDPPopulation();
        }
        public async void CalculateAggregate()
        {
            Task<List<string>> FileContentsTask = FileUtilities.ReadFileContentsByLineAsync(CSVPath);
            Task<string> JSONMapTask = FileUtilities.ReadFileToEndAsync(CountryContinentMapPath);
            List<string> FileContents = await FileContentsTask;
            string headerText = FileContents[0];
            List<string> headers = headerText.Split(',').ToList();
            int IndexOfCountry = headers.IndexOf("\"Country Name\"");
            int IndexOfPopulation = headers.IndexOf("\"Population (Millions) 2012\"");
            int IndexOfGDP = headers.IndexOf("\"GDP Billions (USD) 2012\"");
            string CountryContinentJSONFileContents = await JSONMapTask;
            var CountryContinentMap = JSONSerializers.DeSerializeString(CountryContinentJSONFileContents);
            for (int i = 1; i < FileContents.Count; i++)
            {
                List<string> RowOfData = FileContents[i].Split(',').ToList();
                string Country = RowOfData[IndexOfCountry].Trim('\"');
                float Population = float.Parse(RowOfData[IndexOfPopulation].Trim('\"'));
                float Gdp = float.Parse(RowOfData[IndexOfGDP].Trim('\"'));
                try
                {
                    string Continent = CountryContinentMap.GetValue(RowOfData[IndexOfCountry].Trim('\"')).ToString();
                    AggregatedData.AddOrUpdateValuesInAggregatedValue(Continent, Gdp, Population);
                }
                catch (Exception) { }
            }
            var JSONOutput = JSONSerializers.SerializeJObject(AggregatedData.AggregatedValues);
            FileUtilities.WritingToFileAsync("../../../../AggregateGDPPopulation/output/output.json", JSONOutput);
        }
    }
    /*public class AggregateGDP
    {
        public float GDP_2012 { get; set; }
        public float POPULATION_2012 { get; set; }
        public string JSONCountryCountinentMapPath;
        public string CSVDataPath;
        //public Dictionary<string, float> GdpPopulationMap;
        public Dictionary<string,Dictionary<string,float>> AggregateGdpPopulationMap;

        public AggregateGDP(string JSONCountryCountinentMapPath, string CSVDataPath)
        {
            AggregateGdpPopulationMap = new Dictionary<string, Dictionary<string, float>>();
            //CreateCountryContinentMap(JSONCountryCountinentMapPath); 
        }

        public void AddOrUpdateAggregateMap(string Continent, string Gdp, string Population)
        {
            float GdpParsedToFloat;
            float PopulationParsedToFloat;
            try
            {
                GdpParsedToFloat= float.Parse(Gdp);
            }
            catch (Exception)
            {
                GdpParsedToFloat = float.Parse(Gdp.Trim('\"'));
            }
            try
            {
                PopulationParsedToFloat = float.Parse(Population);
            }
            catch (Exception)
            {
                PopulationParsedToFloat = float.Parse(Population.Trim('\"'));
            }
        }
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
        }*/
        /*public async void ReadMapAndCSVFile(string CSVPath, string CountryContinentMapPath)
        {
            Task<List<string>> FileContentsTask = ReadFileContents(CSVPath);
            Task<string> JSONMapTask = ReadMapFile(CountryContinentMapPath);
            FileContents = await FileContentsTask;
            JSONMap = await JSONMapTask;
        }*/
        /*public async void WriteToJSONFile(string FilePath, string Content)
        {
            using (StreamWriter FileContents = new StreamWriter(FilePath))
            {
                await FileContents.WriteAsync(Content);
            }                
        }
        public async void CalculateAggregateGdp()
        {
            //List<string> FileContents = File.ReadLines(@"../../../../AggregateGDPPopulation/data/datafile.csv").ToList();
            //StreamReader JSONFileContents = new StreamReader(@"../../../../AggregateGDPPopulation/data/country-continent-map.json");
            //var JSONMap = JSONFileContents.ReadToEnd();
            //ReadMapAndCSVFile("../../../../AggregateGDPPopulation/data/datafile.csv", "../../../../AggregateGDPPopulation/data/country-continent-map.json");
            string CSVPath = "../../../../AggregateGDPPopulation/data/datafile.csv";
            string CountryContinentMapPath = "../../../../AggregateGDPPopulation/data/country-continent-map.json";
            Task<List<string>> FileContentsTask = ReadFileContents(CSVPath);
            Task<string> JSONMapTask = ReadMapFile(CountryContinentMapPath);
            List<string> FileContents = await FileContentsTask;*/
            /* https://social.msdn.microsoft.com/Forums/en-US/525ff8f2-13f5-4602-bce3-78b909cadedb/how-to-read-and-write-a-json-file-in-c?forum=csharpgeneral 
             * https://www.newtonsoft.com/json/help/html/T_Newtonsoft_Json_Linq_JObject.htm 
             * */
            /*string headerText = FileContents[0];
            List<string> headers = headerText.Split(',').ToList();
            int IndexOfCountry = headers.IndexOf("\"Country Name\"");
            int IndexOfPopulation = headers.IndexOf("\"Population (Millions) 2012\"");
            int IndexOfGDP = headers.IndexOf("\"GDP Billions (USD) 2012\"");
            Dictionary<string, GDPPopulation> JSONObject = new Dictionary<string, GDPPopulation>();
            string CountryContinentJSONFileContents = await JSONMapTask;
            var CountryContinentMap = JObject.Parse(CountryContinentJSONFileContents);
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
    }*/
}
