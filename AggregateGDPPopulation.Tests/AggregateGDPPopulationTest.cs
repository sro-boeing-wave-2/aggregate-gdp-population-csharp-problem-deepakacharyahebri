using System;
using System.IO;
using Xunit;
using System.Linq;
using System.Collections.Generic;

namespace AggregateGDPPopulation.Tests
{
    public class UnitTest1
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
            AggregateGDP gdp = new AggregateGDP();
            gdp.CalculateAggregateGdp();
            string actual;
            string expected = String.Empty;
            using (StreamReader FileContentsByLine = new StreamReader("../../../../AggregateGDPPopulation/output/output.json"))
            {
                actual = await FileContentsByLine.ReadToEndAsync();
            }
            using(StreamReader ActualFileContents = new StreamReader("../../../expected-output.json"))
            {
                while (true)
                {
                    try
                    {
                        string content = await ActualFileContents.ReadLineAsync();
                        expected += content.Trim();
                    }
                    catch (Exception)
                    {
                        break;
                    }
                }
            }
            Assert.Equal(expected, actual);
        }
    }
}
