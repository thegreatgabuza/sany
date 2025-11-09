using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using Cascade.Fx9Kl2;
using Cascade.ViewModels;

namespace Cascade.Services
{
    /// <summary>
    /// Service for exporting financial data to Excel format
    /// </summary>
    public interface IExcelExportService
    {
        Task<byte[]> ExportTrialBalanceAsync(List<Lv6Cx9Item> trialBalanceData, DateTime asOfDate, Hx7Tz3Data company, decimal totalDebits, decimal totalCredits);
        Task<byte[]> ExportIncomeStatementAsync(IncomeStatementViewModel incomeStatement, DateTime startDate, DateTime endDate, Hx7Tz3Data company);
        Task<byte[]> ExportBalanceSheetAsync(BalanceSheetViewModel balanceSheet, DateTime asOfDate, Hx7Tz3Data company);
        Task<byte[]> ExportGeneralJournalAsync(List<Pz7Vm5Protocol> transactions, DateTime startDate, DateTime endDate, Hx7Tz3Data company);
        Task<byte[]> ExportGeneralLedgerAsync(List<Qw8Rt5Entity> accounts, DateTime startDate, DateTime endDate, Hx7Tz3Data company);
        Task<byte[]> ExportAccountLedgerAsync(List<Sx2Dn8Gateway> transactionLines, Qw8Rt5Entity account, List<decimal> runningBalances, DateTime startDate, DateTime endDate, Hx7Tz3Data company);
        Task<byte[]> ExportIncomeExpenditureAsync(IncomeExpenditureViewModel incomeExpenditure, DateTime startDate, DateTime endDate, Hx7Tz3Data company, bool isVarianceReport = false);
        Task<byte[]> ExportStatementOfFinancialPositionAsync(BalanceSheetViewModel balanceSheet, DateTime asOfDate, Hx7Tz3Data company);
    }

    public class ExcelExportService : IExcelExportService
    {
        public ExcelExportService()
        {
            // Set EPPlus license context for non-commercial use
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        public async Task<byte[]> ExportTrialBalanceAsync(List<Lv6Cx9Item> trialBalanceData, DateTime asOfDate, Hx7Tz3Data company, decimal totalDebits, decimal totalCredits)
        {
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Trial Balance");

            // Header
            SetupCompanyHeader(worksheet, company, "TRIAL BALANCE", $"As of {asOfDate:MMMM dd, yyyy}");

            // Column headers
            int row = 6;
            worksheet.Cells[row, 1].Value = "Account Name";
            worksheet.Cells[row, 2].Value = "Debit";
            worksheet.Cells[row, 3].Value = "Credit";

            FormatHeaderRow(worksheet, row, 1, 3);

            // Data rows
            row++;
            foreach (var item in trialBalanceData)
            {
                worksheet.Cells[row, 1].Value = item.Qw8Rt5Entity.AccountName;
                worksheet.Cells[row, 2].Value = item.DebitBalance > 0 ? item.DebitBalance : (decimal?)null;
                worksheet.Cells[row, 3].Value = item.CreditBalance > 0 ? item.CreditBalance : (decimal?)null;

                if (item.DebitBalance > 0)
                    worksheet.Cells[row, 2].Style.Numberformat.Format = "R #,##0.00";
                if (item.CreditBalance > 0)
                    worksheet.Cells[row, 3].Style.Numberformat.Format = "R #,##0.00";

                row++;
            }

            // Totals
            worksheet.Cells[row, 1].Value = "TOTALS";
            worksheet.Cells[row, 2].Value = totalDebits;
            worksheet.Cells[row, 3].Value = totalCredits;

            worksheet.Cells[row, 1].Style.Font.Bold = true;
            worksheet.Cells[row, 2].Style.Font.Bold = true;
            worksheet.Cells[row, 3].Style.Font.Bold = true;
            worksheet.Cells[row, 2].Style.Numberformat.Format = "R #,##0.00";
            worksheet.Cells[row, 3].Style.Numberformat.Format = "R #,##0.00";

            // Add border to totals
            var totalsRange = worksheet.Cells[row, 1, row, 3];
            totalsRange.Style.Border.Top.Style = ExcelBorderStyle.Double;

            AutofitColumns(worksheet, 3);

            return await Task.FromResult(package.GetAsByteArray());
        }

        public async Task<byte[]> ExportIncomeStatementAsync(IncomeStatementViewModel incomeStatement, DateTime startDate, DateTime endDate, Hx7Tz3Data company)
        {
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Income Statement");

            // Header
            SetupCompanyHeader(worksheet, company, "INCOME STATEMENT", $"For the period {startDate:MMMM dd, yyyy} to {endDate:MMMM dd, yyyy}");

            int row = 6;

            // Revenue section
            worksheet.Cells[row, 1].Value = "REVENUE";
            worksheet.Cells[row, 1].Style.Font.Bold = true;
            worksheet.Cells[row, 1].Style.Font.UnderLine = true;
            row++;

            foreach (var item in incomeStatement.RevenueItems)
            {
                worksheet.Cells[row, 1].Value = item.AccountName;
                worksheet.Cells[row, 2].Value = item.Amount;
                worksheet.Cells[row, 2].Style.Numberformat.Format = "R #,##0.00";
                row++;
            }

            worksheet.Cells[row, 1].Value = "Total Revenue";
            worksheet.Cells[row, 2].Value = incomeStatement.TotalRevenue;
            worksheet.Cells[row, 1].Style.Font.Bold = true;
            worksheet.Cells[row, 2].Style.Font.Bold = true;
            worksheet.Cells[row, 2].Style.Numberformat.Format = "R #,##0.00";
            worksheet.Cells[row, 1, row, 2].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            row += 2;

            // Expenses section
            worksheet.Cells[row, 1].Value = "EXPENSES";
            worksheet.Cells[row, 1].Style.Font.Bold = true;
            worksheet.Cells[row, 1].Style.Font.UnderLine = true;
            row++;

            foreach (var item in incomeStatement.ExpenseItems)
            {
                worksheet.Cells[row, 1].Value = item.AccountName;
                worksheet.Cells[row, 2].Value = item.Amount;
                worksheet.Cells[row, 2].Style.Numberformat.Format = "R #,##0.00";
                row++;
            }

            worksheet.Cells[row, 1].Value = "Total Expenses";
            worksheet.Cells[row, 2].Value = incomeStatement.TotalExpenses;
            worksheet.Cells[row, 1].Style.Font.Bold = true;
            worksheet.Cells[row, 2].Style.Font.Bold = true;
            worksheet.Cells[row, 2].Style.Numberformat.Format = "R #,##0.00";
            worksheet.Cells[row, 1, row, 2].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            row += 2;

            // Net Income
            worksheet.Cells[row, 1].Value = "NET INCOME";
            worksheet.Cells[row, 2].Value = incomeStatement.NetIncome;
            worksheet.Cells[row, 1].Style.Font.Bold = true;
            worksheet.Cells[row, 2].Style.Font.Bold = true;
            worksheet.Cells[row, 2].Style.Numberformat.Format = "R #,##0.00";
            worksheet.Cells[row, 1, row, 2].Style.Border.Top.Style = ExcelBorderStyle.Double;
            worksheet.Cells[row, 1, row, 2].Style.Border.Bottom.Style = ExcelBorderStyle.Double;

            AutofitColumns(worksheet, 2);

            return await Task.FromResult(package.GetAsByteArray());
        }

        public async Task<byte[]> ExportBalanceSheetAsync(BalanceSheetViewModel balanceSheet, DateTime asOfDate, Hx7Tz3Data company)
        {
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Balance Sheet");

            // Header
            SetupCompanyHeader(worksheet, company, "BALANCE SHEET", $"As of {asOfDate:MMMM dd, yyyy}");

            int row = 6;

            // Assets section
            worksheet.Cells[row, 1].Value = "ASSETS";
            worksheet.Cells[row, 1].Style.Font.Bold = true;
            worksheet.Cells[row, 1].Style.Font.UnderLine = true;
            row++;

            foreach (var item in balanceSheet.AssetItems.Where(x => x.Amount > 0))
            {
                worksheet.Cells[row, 1].Value = item.AccountName;
                worksheet.Cells[row, 2].Value = item.Amount;
                worksheet.Cells[row, 2].Style.Numberformat.Format = "R #,##0.00";
                row++;
            }

            worksheet.Cells[row, 1].Value = "Total Assets";
            worksheet.Cells[row, 2].Value = balanceSheet.TotalAssets;
            worksheet.Cells[row, 1].Style.Font.Bold = true;
            worksheet.Cells[row, 2].Style.Font.Bold = true;
            worksheet.Cells[row, 2].Style.Numberformat.Format = "R #,##0.00";
            worksheet.Cells[row, 1, row, 2].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            row += 2;

            // Liabilities section
            worksheet.Cells[row, 1].Value = "LIABILITIES";
            worksheet.Cells[row, 1].Style.Font.Bold = true;
            worksheet.Cells[row, 1].Style.Font.UnderLine = true;
            row++;

            foreach (var item in balanceSheet.LiabilityItems.Where(x => x.Amount > 0))
            {
                worksheet.Cells[row, 1].Value = item.AccountName;
                worksheet.Cells[row, 2].Value = item.Amount;
                worksheet.Cells[row, 2].Style.Numberformat.Format = "R #,##0.00";
                row++;
            }

            worksheet.Cells[row, 1].Value = "Total Liabilities";
            worksheet.Cells[row, 2].Value = balanceSheet.TotalLiabilities;
            worksheet.Cells[row, 1].Style.Font.Bold = true;
            worksheet.Cells[row, 2].Style.Font.Bold = true;
            worksheet.Cells[row, 2].Style.Numberformat.Format = "R #,##0.00";
            worksheet.Cells[row, 1, row, 2].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            row += 2;

            // Equity section
            worksheet.Cells[row, 1].Value = "EQUITY";
            worksheet.Cells[row, 1].Style.Font.Bold = true;
            worksheet.Cells[row, 1].Style.Font.UnderLine = true;
            row++;

            foreach (var item in balanceSheet.EquityItems.Where(x => x.Amount != 0))
            {
                worksheet.Cells[row, 1].Value = item.AccountName;
                worksheet.Cells[row, 2].Value = item.Amount;
                worksheet.Cells[row, 2].Style.Numberformat.Format = "R #,##0.00";
                row++;
            }

            worksheet.Cells[row, 1].Value = "Total Equity";
            worksheet.Cells[row, 2].Value = balanceSheet.TotalEquity;
            worksheet.Cells[row, 1].Style.Font.Bold = true;
            worksheet.Cells[row, 2].Style.Font.Bold = true;
            worksheet.Cells[row, 2].Style.Numberformat.Format = "R #,##0.00";
            worksheet.Cells[row, 1, row, 2].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            row += 2;

            // Total Liabilities & Equity
            worksheet.Cells[row, 1].Value = "TOTAL LIABILITIES & EQUITY";
            worksheet.Cells[row, 2].Value = balanceSheet.TotalLiabilities + balanceSheet.TotalEquity;
            worksheet.Cells[row, 1].Style.Font.Bold = true;
            worksheet.Cells[row, 2].Style.Font.Bold = true;
            worksheet.Cells[row, 2].Style.Numberformat.Format = "R #,##0.00";
            worksheet.Cells[row, 1, row, 2].Style.Border.Top.Style = ExcelBorderStyle.Double;

            AutofitColumns(worksheet, 2);

            return await Task.FromResult(package.GetAsByteArray());
        }

        public async Task<byte[]> ExportGeneralJournalAsync(List<Pz7Vm5Protocol> transactions, DateTime startDate, DateTime endDate, Hx7Tz3Data company)
        {
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("General Journal");

            // Header
            SetupCompanyHeader(worksheet, company, "GENERAL JOURNAL", $"For the period {startDate:MMMM dd, yyyy} to {endDate:MMMM dd, yyyy}");

            // Column headers
            int row = 6;
            worksheet.Cells[row, 1].Value = "Date";
            worksheet.Cells[row, 2].Value = "Reference";
            worksheet.Cells[row, 3].Value = "Description";
            worksheet.Cells[row, 4].Value = "Account";
            worksheet.Cells[row, 5].Value = "Debit";
            worksheet.Cells[row, 6].Value = "Credit";

            FormatHeaderRow(worksheet, row, 1, 6);
            row++;

            foreach (var transaction in transactions)
            {
                foreach (var line in transaction.ProcessHandlers.OrderByDescending(l => l.Debit))
                {
                    worksheet.Cells[row, 1].Value = transaction.TransactionDate;
                    worksheet.Cells[row, 1].Style.Numberformat.Format = "dd/mm/yyyy";
                    worksheet.Cells[row, 2].Value = transaction.ReferenceNo ?? "";
                    worksheet.Cells[row, 3].Value = transaction.Description;
                    worksheet.Cells[row, 4].Value = line.Qw8Rt5Entity?.AccountName ?? "";
                    
                    if (line.Debit > 0)
                    {
                        worksheet.Cells[row, 5].Value = line.Debit;
                        worksheet.Cells[row, 5].Style.Numberformat.Format = "R #,##0.00";
                    }
                    
                    if (line.Credit > 0)
                    {
                        worksheet.Cells[row, 6].Value = line.Credit;
                        worksheet.Cells[row, 6].Style.Numberformat.Format = "R #,##0.00";
                    }
                    
                    row++;
                }
                
                // Add empty row between transactions
                row++;
            }

            AutofitColumns(worksheet, 6);

            return await Task.FromResult(package.GetAsByteArray());
        }

        public async Task<byte[]> ExportGeneralLedgerAsync(List<Qw8Rt5Entity> accounts, DateTime startDate, DateTime endDate, Hx7Tz3Data company)
        {
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("General Ledger");

            // Header
            SetupCompanyHeader(worksheet, company, "GENERAL LEDGER", $"For the period {startDate:MMMM dd, yyyy} to {endDate:MMMM dd, yyyy}");

            int row = 6;

            foreach (var account in accounts)
            {
                if (!account.ProcessHandlers.Any()) continue;

                // Account header
                worksheet.Cells[row, 1].Value = $"Account: {account.AccountName} ({account.Mx9Qw7Type})";
                worksheet.Cells[row, 1].Style.Font.Bold = true;
                worksheet.Cells[row, 1, row, 6].Merge = true;
                row++;

                // Column headers
                worksheet.Cells[row, 1].Value = "Date";
                worksheet.Cells[row, 2].Value = "Reference";
                worksheet.Cells[row, 3].Value = "Description";
                worksheet.Cells[row, 4].Value = "Debit";
                worksheet.Cells[row, 5].Value = "Credit";
                worksheet.Cells[row, 6].Value = "Balance";

                FormatHeaderRow(worksheet, row, 1, 6);
                row++;

                decimal runningBalance = 0;

                foreach (var line in account.ProcessHandlers.OrderBy(l => l.Pz7Vm5Protocol.TransactionDate))
                {
                    worksheet.Cells[row, 1].Value = line.Pz7Vm5Protocol.TransactionDate;
                    worksheet.Cells[row, 1].Style.Numberformat.Format = "dd/mm/yyyy";
                    worksheet.Cells[row, 2].Value = line.Pz7Vm5Protocol.ReferenceNo ?? "";
                    worksheet.Cells[row, 3].Value = line.Pz7Vm5Protocol.Description;

                    if (line.Debit > 0)
                    {
                        worksheet.Cells[row, 4].Value = line.Debit;
                        worksheet.Cells[row, 4].Style.Numberformat.Format = "R #,##0.00";
                    }

                    if (line.Credit > 0)
                    {
                        worksheet.Cells[row, 5].Value = line.Credit;
                        worksheet.Cells[row, 5].Style.Numberformat.Format = "R #,##0.00";
                    }

                    runningBalance += line.Debit - line.Credit;
                    worksheet.Cells[row, 6].Value = runningBalance;
                    worksheet.Cells[row, 6].Style.Numberformat.Format = "R #,##0.00";

                    row++;
                }

                row += 2; // Space between accounts
            }

            AutofitColumns(worksheet, 6);

            return await Task.FromResult(package.GetAsByteArray());
        }

        public async Task<byte[]> ExportAccountLedgerAsync(List<Sx2Dn8Gateway> transactionLines, Qw8Rt5Entity account, List<decimal> runningBalances, DateTime startDate, DateTime endDate, Hx7Tz3Data company)
        {
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add($"Account Ledger - {account.AccountName}");

            // Header
            SetupCompanyHeader(worksheet, company, $"ACCOUNT LEDGER - {account.AccountName.ToUpper()}", $"For the period {startDate:MMMM dd, yyyy} to {endDate:MMMM dd, yyyy}");

            // Column headers
            int row = 6;
            worksheet.Cells[row, 1].Value = "Date";
            worksheet.Cells[row, 2].Value = "Reference";
            worksheet.Cells[row, 3].Value = "Description";
            worksheet.Cells[row, 4].Value = "Debit";
            worksheet.Cells[row, 5].Value = "Credit";
            worksheet.Cells[row, 6].Value = "Balance";

            FormatHeaderRow(worksheet, row, 1, 6);
            row++;

            for (int i = 0; i < transactionLines.Count; i++)
            {
                var line = transactionLines[i];
                
                worksheet.Cells[row, 1].Value = line.Pz7Vm5Protocol.TransactionDate;
                worksheet.Cells[row, 1].Style.Numberformat.Format = "dd/mm/yyyy";
                worksheet.Cells[row, 2].Value = line.Pz7Vm5Protocol.ReferenceNo ?? "";
                worksheet.Cells[row, 3].Value = line.Pz7Vm5Protocol.Description;

                if (line.Debit > 0)
                {
                    worksheet.Cells[row, 4].Value = line.Debit;
                    worksheet.Cells[row, 4].Style.Numberformat.Format = "R #,##0.00";
                }

                if (line.Credit > 0)
                {
                    worksheet.Cells[row, 5].Value = line.Credit;
                    worksheet.Cells[row, 5].Style.Numberformat.Format = "R #,##0.00";
                }

                worksheet.Cells[row, 6].Value = runningBalances[i];
                worksheet.Cells[row, 6].Style.Numberformat.Format = "R #,##0.00";

                row++;
            }

            AutofitColumns(worksheet, 6);

            return await Task.FromResult(package.GetAsByteArray());
        }

        public async Task<byte[]> ExportIncomeExpenditureAsync(IncomeExpenditureViewModel incomeExpenditure, DateTime startDate, DateTime endDate, Hx7Tz3Data company, bool isVarianceReport = false)
        {
            using var package = new ExcelPackage();
            var reportName = isVarianceReport ? "Income & Expenditure Variance" : "Income & Expenditure Statement";
            var worksheet = package.Workbook.Worksheets.Add(reportName);

            // Header
            SetupCompanyHeader(worksheet, company, reportName.ToUpper(), $"For the period {startDate:MMMM dd, yyyy} to {endDate:MMMM dd, yyyy}");

            int row = 6;

            if (isVarianceReport)
            {
                // Variance report with budget columns
                worksheet.Cells[row, 1].Value = "INCOME";
                worksheet.Cells[row, 1].Style.Font.Bold = true;
                worksheet.Cells[row, 1].Style.Font.UnderLine = true;
                row++;

                worksheet.Cells[row, 1].Value = "Account";
                worksheet.Cells[row, 2].Value = "Actual";
                worksheet.Cells[row, 3].Value = "Budget";
                worksheet.Cells[row, 4].Value = "Variance %";
                FormatHeaderRow(worksheet, row, 1, 4);
                row++;

                foreach (var item in incomeExpenditure.IncomeItems)
                {
                    worksheet.Cells[row, 1].Value = item.AccountName;
                    worksheet.Cells[row, 2].Value = item.Amount;
                    worksheet.Cells[row, 3].Value = item.BudgetedAmount;
                    worksheet.Cells[row, 4].Value = item.Variance / 100;

                    worksheet.Cells[row, 2].Style.Numberformat.Format = "R #,##0.00";
                    worksheet.Cells[row, 3].Style.Numberformat.Format = "R #,##0.00";
                    worksheet.Cells[row, 4].Style.Numberformat.Format = "0.00%";
                    
                    if (item.Variance >= 0)
                        worksheet.Cells[row, 4].Style.Font.Color.SetColor(Color.Green);
                    else
                        worksheet.Cells[row, 4].Style.Font.Color.SetColor(Color.Red);

                    row++;
                }

                // Income totals
                worksheet.Cells[row, 1].Value = "Total Income";
                worksheet.Cells[row, 2].Value = incomeExpenditure.TotalIncome;
                worksheet.Cells[row, 3].Value = incomeExpenditure.TotalBudgetedIncome;
                worksheet.Cells[row, 4].Value = incomeExpenditure.IncomeVariance / 100;

                FormatTotalRow(worksheet, row, 1, 4);
                worksheet.Cells[row, 2].Style.Numberformat.Format = "R #,##0.00";
                worksheet.Cells[row, 3].Style.Numberformat.Format = "R #,##0.00";
                worksheet.Cells[row, 4].Style.Numberformat.Format = "0.00%";
                row += 2;

                // Expenditure section
                worksheet.Cells[row, 1].Value = "EXPENDITURE";
                worksheet.Cells[row, 1].Style.Font.Bold = true;
                worksheet.Cells[row, 1].Style.Font.UnderLine = true;
                row++;

                worksheet.Cells[row, 1].Value = "Account";
                worksheet.Cells[row, 2].Value = "Actual";
                worksheet.Cells[row, 3].Value = "Budget";
                worksheet.Cells[row, 4].Value = "Variance %";
                FormatHeaderRow(worksheet, row, 1, 4);
                row++;

                foreach (var item in incomeExpenditure.ExpenditureItems)
                {
                    worksheet.Cells[row, 1].Value = item.AccountName;
                    worksheet.Cells[row, 2].Value = item.Amount;
                    worksheet.Cells[row, 3].Value = item.BudgetedAmount;
                    worksheet.Cells[row, 4].Value = item.Variance / 100;

                    worksheet.Cells[row, 2].Style.Numberformat.Format = "R #,##0.00";
                    worksheet.Cells[row, 3].Style.Numberformat.Format = "R #,##0.00";
                    worksheet.Cells[row, 4].Style.Numberformat.Format = "0.00%";
                    
                    if (item.Variance <= 0) // For expenses, lower is better
                        worksheet.Cells[row, 4].Style.Font.Color.SetColor(Color.Green);
                    else
                        worksheet.Cells[row, 4].Style.Font.Color.SetColor(Color.Red);

                    row++;
                }

                // Expenditure totals
                worksheet.Cells[row, 1].Value = "Total Expenditure";
                worksheet.Cells[row, 2].Value = incomeExpenditure.TotalExpenditure;
                worksheet.Cells[row, 3].Value = incomeExpenditure.TotalBudgetedExpenditure;
                worksheet.Cells[row, 4].Value = incomeExpenditure.ExpenditureVariance / 100;

                FormatTotalRow(worksheet, row, 1, 4);
                worksheet.Cells[row, 2].Style.Numberformat.Format = "R #,##0.00";
                worksheet.Cells[row, 3].Style.Numberformat.Format = "R #,##0.00";
                worksheet.Cells[row, 4].Style.Numberformat.Format = "0.00%";
                row += 2;

                // Net position
                worksheet.Cells[row, 1].Value = "NET POSITION";
                worksheet.Cells[row, 2].Value = incomeExpenditure.NetPosition;
                worksheet.Cells[row, 3].Value = incomeExpenditure.NetBudgeted;
                worksheet.Cells[row, 4].Value = incomeExpenditure.NetVariance / 100;

                FormatTotalRow(worksheet, row, 1, 4, true);
                worksheet.Cells[row, 2].Style.Numberformat.Format = "R #,##0.00";
                worksheet.Cells[row, 3].Style.Numberformat.Format = "R #,##0.00";
                worksheet.Cells[row, 4].Style.Numberformat.Format = "0.00%";

                AutofitColumns(worksheet, 4);
            }
            else
            {
                // Standard income & expenditure statement
                worksheet.Cells[row, 1].Value = "INCOME";
                worksheet.Cells[row, 1].Style.Font.Bold = true;
                worksheet.Cells[row, 1].Style.Font.UnderLine = true;
                row++;

                foreach (var item in incomeExpenditure.IncomeItems)
                {
                    worksheet.Cells[row, 1].Value = item.AccountName;
                    worksheet.Cells[row, 2].Value = item.Amount;
                    worksheet.Cells[row, 2].Style.Numberformat.Format = "R #,##0.00";
                    row++;
                }

                worksheet.Cells[row, 1].Value = "Total Income";
                worksheet.Cells[row, 2].Value = incomeExpenditure.TotalIncome;
                FormatTotalRow(worksheet, row, 1, 2);
                worksheet.Cells[row, 2].Style.Numberformat.Format = "R #,##0.00";
                row += 2;

                worksheet.Cells[row, 1].Value = "EXPENDITURE";
                worksheet.Cells[row, 1].Style.Font.Bold = true;
                worksheet.Cells[row, 1].Style.Font.UnderLine = true;
                row++;

                foreach (var item in incomeExpenditure.ExpenditureItems)
                {
                    worksheet.Cells[row, 1].Value = item.AccountName;
                    worksheet.Cells[row, 2].Value = item.Amount;
                    worksheet.Cells[row, 2].Style.Numberformat.Format = "R #,##0.00";
                    row++;
                }

                worksheet.Cells[row, 1].Value = "Total Expenditure";
                worksheet.Cells[row, 2].Value = incomeExpenditure.TotalExpenditure;
                FormatTotalRow(worksheet, row, 1, 2);
                worksheet.Cells[row, 2].Style.Numberformat.Format = "R #,##0.00";
                row += 2;

                worksheet.Cells[row, 1].Value = "NET POSITION";
                worksheet.Cells[row, 2].Value = incomeExpenditure.NetPosition;
                FormatTotalRow(worksheet, row, 1, 2, true);
                worksheet.Cells[row, 2].Style.Numberformat.Format = "R #,##0.00";

                AutofitColumns(worksheet, 2);
            }

            return await Task.FromResult(package.GetAsByteArray());
        }

        public async Task<byte[]> ExportStatementOfFinancialPositionAsync(BalanceSheetViewModel balanceSheet, DateTime asOfDate, Hx7Tz3Data company)
        {
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Statement of Financial Position");

            // Header
            SetupCompanyHeader(worksheet, company, "STATEMENT OF FINANCIAL POSITION", $"As of {asOfDate:MMMM dd, yyyy}");

            return await ExportBalanceSheetAsync(balanceSheet, asOfDate, company);
        }

        private void SetupCompanyHeader(ExcelWorksheet worksheet, Hx7Tz3Data company, string reportTitle, string reportPeriod)
        {
            // Company name
            worksheet.Cells[1, 1].Value = company.CompanyName;
            worksheet.Cells[1, 1].Style.Font.Bold = true;
            worksheet.Cells[1, 1].Style.Font.Size = 16;
            worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            // Report title
            worksheet.Cells[2, 1].Value = reportTitle;
            worksheet.Cells[2, 1].Style.Font.Bold = true;
            worksheet.Cells[2, 1].Style.Font.Size = 14;
            worksheet.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            // Report period
            worksheet.Cells[3, 1].Value = reportPeriod;
            worksheet.Cells[3, 1].Style.Font.Size = 12;
            worksheet.Cells[3, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            // Generated date
            worksheet.Cells[4, 1].Value = $"Generated on: {DateTime.Now:MMMM dd, yyyy 'at' HH:mm}";
            worksheet.Cells[4, 1].Style.Font.Size = 10;
            worksheet.Cells[4, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells[4, 1].Style.Font.Italic = true;
        }

        private void FormatHeaderRow(ExcelWorksheet worksheet, int row, int startColumn, int endColumn)
        {
            var headerRange = worksheet.Cells[row, startColumn, row, endColumn];
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
            headerRange.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
            headerRange.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            headerRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            headerRange.Style.Border.Left.Style = ExcelBorderStyle.Thin;
            headerRange.Style.Border.Right.Style = ExcelBorderStyle.Thin;
        }

        private void FormatTotalRow(ExcelWorksheet worksheet, int row, int startColumn, int endColumn, bool isGrandTotal = false)
        {
            var totalRange = worksheet.Cells[row, startColumn, row, endColumn];
            totalRange.Style.Font.Bold = true;
            
            if (isGrandTotal)
            {
                totalRange.Style.Border.Top.Style = ExcelBorderStyle.Double;
                totalRange.Style.Border.Bottom.Style = ExcelBorderStyle.Double;
            }
            else
            {
                totalRange.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            }
        }

        private void AutofitColumns(ExcelWorksheet worksheet, int columnCount)
        {
            for (int i = 1; i <= columnCount; i++)
            {
                worksheet.Column(i).AutoFit();
            }
        }
    }
}