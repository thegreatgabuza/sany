using Cascade.Fx9Kl2;

namespace Cascade.ViewModels
{
    public class IncomeStatementViewModel
    {
        public required List<IncomeStatementItem> RevenueItems { get; set; }
        public required List<IncomeStatementItem> ExpenseItems { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal NetIncome { get; set; }
    }

    public class IncomeStatementItem
    {
        public required string AccountName { get; set; }
        public decimal Amount { get; set; }
    }

    public class BalanceSheetViewModel
    {
        public required List<BalanceSheetItem> AssetItems { get; set; }
        public required List<BalanceSheetItem> LiabilityItems { get; set; }
        public required List<BalanceSheetItem> EquityItems { get; set; }
        public decimal TotalAssets { get; set; }
        public decimal TotalLiabilities { get; set; }
        public decimal TotalEquity { get; set; }
    }

    public class BalanceSheetItem
    {
        public required string AccountName { get; set; }
        public decimal Amount { get; set; }
    }

    public class IncomeExpenditureViewModel
    {
        public required List<IncomeExpenditureItem> IncomeItems { get; set; }
        public required List<IncomeExpenditureItem> ExpenditureItems { get; set; }
        public decimal TotalIncome { get; set; }
        public decimal TotalExpenditure { get; set; }
        public decimal NetPosition { get; set; }
        public decimal TotalBudgetedIncome { get; set; }
        public decimal TotalBudgetedExpenditure { get; set; }
        public decimal NetBudgeted { get; set; }
        public decimal IncomeVariance { get; set; }
        public decimal ExpenditureVariance { get; set; }
        public decimal NetVariance { get; set; }
        public Dictionary<string, decimal> BudgetedAmounts { get; set; } = new();
    }

    public class IncomeExpenditureItem
    {
        public required string AccountName { get; set; }
        public decimal Amount { get; set; }
        public decimal BudgetedAmount { get; set; }
        public decimal Variance { get; set; }
        public string VarianceClass { get; set; } = "";
    }
}