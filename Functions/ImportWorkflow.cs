using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace DurableFunctionsLab
{
    public static class ImportWorkflow
    {
        [FunctionName("ImportWorkflow")]
        public static async Task<List<string>> RunOrchestrator(
            [OrchestrationTrigger] DurableOrchestrationContext context)
        {
            var outputs = new List<string>();

            var ratesXml = await context.CallActivityWithRetryAsync<string>("ImportWorkflow_DownloadRates", null, new RetryOptions(TimeSpan.FromMinutes(10), 100));
            var exchangeRates = await context.CallActivityAsync<IEnumerable<ExchangeRate>>("ImportWorkflow_ParseRatesXml", ratesXml);
            return (from er in exchangeRates select er.Ccy1).ToList();
        }

        [FunctionName("ImportWorkflow_DownloadRates")]
        public static async Task<String> DownloadRates([ActivityTrigger] string notUsed, ILogger log)
        {
            Uri exchangeRatesUri = new Uri("http://www.nationalbanken.dk/_vti_bin/DN/DataService.svc/CurrencyRatesXML?lang=da");
            using (var client = new HttpClient()) {
                return await client.GetStringAsync(exchangeRatesUri);
            }
        }

        [FunctionName("ImportWorkflow_ParseRatesXml")]
        public static async Task<IEnumerable<ExchangeRate>> ParseRatesXml([ActivityTrigger] string ratesXml, ILogger log) {
            XDocument document = XDocument.Parse(ratesXml);
            var danishFormat = System.Globalization.CultureInfo.GetCultureInfo("da-DK");
            return 
                from exchangeRates in new [] {document.Root}
                where exchangeRates.Name.LocalName == "exchangerates"
                let referenceCurrency = exchangeRates.Attribute("refcur").Value
                    from dailyRates in exchangeRates.Elements("dailyrates")
                    let date = DateTime.Parse(dailyRates.Attribute("id").Value)
                        from currency in dailyRates.Elements("currency")
                        let rate = Decimal.Parse(currency.Attribute("rate").Value, danishFormat)/100m
                        let baseCurrency = currency.Attribute("code").Value
                        select ExchangeRate.Create(baseCurrency, referenceCurrency, rate, date.Date);
        }

        [FunctionName("ImportWorkflow_HttpStart")]
        public static async Task<HttpResponseMessage> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")]HttpRequestMessage req,
            [OrchestrationClient]DurableOrchestrationClient starter,
            ILogger log)
        {
            // Function input comes from the request content.
            string instanceId = await starter.StartNewAsync("ImportWorkflow", null);

            log.LogInformation($"Started orchestration with ID = '{instanceId}'.");

            return starter.CreateCheckStatusResponse(req, instanceId);
        }
    }
}