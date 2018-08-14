namespace DurableFunctionsLab
{
    public struct ExchangeRate
    {
        private ExchangeRate(string ccy1, string ccy2, decimal rate)
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
        }

        public string Ccy1 { get; }
        public string Ccy2 { get; }
        public decimal Rate { get; }

        public static ExchangeRate Create(string baseCurrency, string currency, decimal rate) {
            return new ExchangeRate(baseCurrency, currency, rate);
        }
    }
}