namespace Cascade.Fx9Kl2
{
    public class CashFlowViewModel
    {
        public List<CashFlowItem> OperatingReceipts { get; set; } = new List<CashFlowItem>();
        public List<CashFlowItem> OperatingPayments { get; set; } = new List<CashFlowItem>();
        public List<CashFlowItem> InvestingActivities { get; set; } = new List<CashFlowItem>();
        public List<CashFlowItem> FinancingActivities { get; set; } = new List<CashFlowItem>();
        
        public decimal NetOperatingCashFlow { get; set; }
        public decimal NetInvestingCashFlow { get; set; }
        public decimal NetFinancingCashFlow { get; set; }
        
        public decimal CashAtBeginning { get; set; }
        public decimal CashAtEnd { get; set; }
    }

    public class CashFlowItem
    {
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }
}
