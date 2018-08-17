using System;
using Xunit;

using DurableFunctionsLab;

namespace Tests
{
    public class UnitTest1
    {
        [Fact]
        public async void ParseXml_WithValidXml_ShouldReturnExchangeRates()
        {
            var xml = @"<?xml version=""1.0"" encoding=""ISO-8859-1""?>
<exchangerates type=""Valutakurser"" author=""Danmarks Nationalbank"" refcur=""DKK"" refamt=""1"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
  <dailyrates id=""2018-08-17"">
    <currency code=""AUD"" desc=""Australske dollar"" rate=""475,93"" />
    <currency code=""BGN"" desc=""Bulgarske lev"" rate=""381,32"" />
    <currency code=""BRL"" desc=""Brasilianske real"" rate=""165,73"" />
  </dailyrates>
</exchangerates>";
            var actual = await ImportWorkflow.ParseRatesXml(xml, null);


            Assert.NotEmpty(actual);
            var expectedDate = new DateTime(2018,08,17);
            Assert.Contains(ExchangeRate.Create("AUD", "DKK", 4.7593m, expectedDate), actual);
            Assert.Contains(ExchangeRate.Create("BGN", "DKK", 3.8132m, expectedDate), actual);
            Assert.Contains(ExchangeRate.Create("BRL", "DKK", 1.6573m, expectedDate), actual);
        }
    }
}
