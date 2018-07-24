using System;
using System.IO;
using Xunit;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace AggregateGDPPopulation.Tests
{
    public class TestCases
    {
        /*[Fact]
        public void CheckForTheExistenceOfFile()
        {
            AggregateGDP gdp = new AggregateGDP();
            bool actual;
            gdp.CalculateAggregateGdp();
            try
            {
                using (StreamReader s = new StreamReader("../../../../AggregateGDPPopulation/output/output.json"))
                {
                    actual = true;
                }
            }
            catch(Exception e)
            {
                actual = false;
            }
            bool expected = true;
            Assert.Equal(expected, actual);
        }*/
        [Fact]
        public async void CheckForFileContents()
        {
            CalculateAggregateGdpPopulation gdp = new CalculateAggregateGdpPopulation();
            JObject actual = await gdp.CalculateAggregate();
            string ExpectedOutput = await FileUtilities.ReadFileToEndAsync(gdp.OutputPath);
            JObject expected = JSONSerializers.DeSerializeString(ExpectedOutput);
            Assert.Equal(expected, actual);
        }
    }
}
