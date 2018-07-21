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
            gdp.CalculateAggregateGdp();
            bool actual = File.Exists("../../../../AggregateGDPPopulation/output/output.json");
            bool expected = true;
            Assert.Equal(expected, actual);
        }*/
        [Fact]
        public void CheckForFileContents()
        {
            AggregateGDP gdp = new AggregateGDP();
            gdp.CalculateAggregateGdp();
            StreamReader FileContentsByLine = new StreamReader("../../../../AggregateGDPPopulation/output/output.json");
            StreamReader ActualFileContents = new StreamReader("../../../expected-output.json");
            string actual = FileContentsByLine.ReadToEnd();
            string expected = String.Empty;
            while (true)
            {
                try
                {
                    string content = ActualFileContents.ReadLine().Trim();
                    expected += content;
                }
                catch (Exception e)
                {
                    break;
                }
            }
            //bool actualContents = (expected == actual);
            //Console.WriteLine(actualContents);
            //bool expectedContents = true;
            Assert.Equal(expected, actual);
        }
    }
}
