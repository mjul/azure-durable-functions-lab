using System;

namespace DurableFunctionsLab
{
    public struct ExchangeRate
    {
        private ExchangeRate(string ccy1, string ccy2, decimal rate, DateTime date)
        {
            if (string.IsNullOrWhiteSpace(ccy1))
            {
                throw new System.ArgumentException("message", nameof(ccy1));
            }

            if (string.IsNullOrWhiteSpace(ccy2))
            {
                throw new System.ArgumentException("message", nameof(ccy2));
            }

            Ccy1 = ccy1;
            Ccy2 = ccy2;
            Rate = rate;
            Date = date;
        }

        public string Ccy1; 
        public string Ccy2;
        public decimal Rate;
        public DateTime Date;

        public static ExchangeRate Create(string baseCurrency, string currency, decimal rate, DateTime date) {
            return new ExchangeRate(baseCurrency, currency, rate, date);
        }
    }
}