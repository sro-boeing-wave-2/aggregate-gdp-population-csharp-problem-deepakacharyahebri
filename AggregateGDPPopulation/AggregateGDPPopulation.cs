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
            return JsonConvert.SerializeObject(JSONObject, Formatting.Indented);
        }
    }
    public class CalculateAggregateGdpPopulation
    {
        public string CSVPath;
        public string CountryContinentMapPath;
        public string OutputPath;
        public AggregateGDPPopulation AggregatedData;
        public CalculateAggregateGdpPopulation()//string CSVPath, string CountryContinentMapPath)
        {
            this.CSVPath = "../../../../AggregateGDPPopulation/data/datafile.csv";
            this.CountryContinentMapPath = "../../../../AggregateGDPPopulation/data/country-continent-map.json";
            this.OutputPath = "../../../../AggregateGDPPopulation/output/output.json";
            AggregatedData = new AggregateGDPPopulation();
            //AggregatedJSON = new JObject();
        }
        public async Task<JObject> CalculateAggregate()
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
            var AggregatedJSON = JSONSerializers.DeSerializeString(JSONOutput);
            FileUtilities.WritingToFileAsync(OutputPath, JSONOutput);
            return AggregatedJSON;
        }
    }
}
