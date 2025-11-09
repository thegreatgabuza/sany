using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Cascade.Fx9Kl2;
using Cascade.A1B2C3D4;
using Cascade.Services;
using Cascade.ViewModels;

namespace Cascade.Zr9Kq6
{
    /// <summary>
    /// Financial Reports Controller - Handles all financial statement views and exports
    /// Issue #004: Excel Export Functionality
    /// </summary>
    public class Bx4Yq1Controller : Controller
    {
        private readonly VxR4DbGate _context;
        private readonly IExcelExportService _excelExportService;
        private readonly ILogger<Bx4Yq1Controller> _logger;

        public Bx4Yq1Controller(VxR4DbGate context, IExcelExportService excelExportService, ILogger<Bx4Yq1Controller> logger)
        {
            _context = context;
            _excelExportService = excelExportService;
            _logger = logger;
        }

        // GET: General Ledger (View 3)
        public async Task<IActionResult> GeneralLedger(DateTime? startDate, DateTime? endDate)
        {
            var userName = User.Identity?.Name;
            var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName);
            if (currentUser?.CompanyId == null)
                return Forbid();

            startDate ??= DateTime.Today.AddMonths(-1);
            endDate ??= DateTime.Today;

            var accounts = await _context.DataStreams
                .Include(a => a.ProcessHandlers)
                .ThenInclude(ph => ph.Pz7Vm5Protocol)
                .Where(a => a.CompanyId == currentUser.CompanyId)
                .OrderBy(a => a.AccountName)
                .ToListAsync();

            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;
            ViewBag.Hx7Tz3Data = await _context.SystemEntries.FirstOrDefaultAsync(c => c.CompanyId == currentUser.CompanyId);

            return View("3", accounts);
        }

        // GET: General Journal (View 2)
        public async Task<IActionResult> GeneralJournal(DateTime? startDate, DateTime? endDate)
        {
            var userName = User.Identity?.Name;
            var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName);
            if (currentUser?.CompanyId == null)
                return Forbid();

            startDate ??= DateTime.Today.AddMonths(-1);
            endDate ??= DateTime.Today;

            var transactions = await _context.SecurityLogs
                .Include(t => t.ProcessHandlers)
                .ThenInclude(ph => ph.Qw8Rt5Entity)
                .Where(t => t.CompanyId == currentUser.CompanyId &&
                           t.TransactionDate >= startDate &&
                           t.TransactionDate <= endDate)
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();

            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;
            ViewBag.Hx7Tz3Data = await _context.SystemEntries.FirstOrDefaultAsync(c => c.CompanyId == currentUser.CompanyId);

            return View("2", transactions);
        }

        // GET: Income & Expenditure Statement (View 7)
        public async Task<IActionResult> IncomeExpenditure(int? financialYear = null)
        {
            var userName = User.Identity?.Name;
            var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName);
            if (currentUser?.CompanyId == null)
                return Forbid();

            financialYear ??= DateTime.Now.Year;

            // Get date range for current calendar year (Jan 1 to Dec 31)
            var currentStartDate = new DateTime(financialYear.Value, 1, 1);
            var currentEndDate = new DateTime(financialYear.Value, 12, 31);

            // Get date range for previous calendar year
            var previousStartDate = new DateTime(financialYear.Value - 1, 1, 1);
            var previousEndDate = new DateTime(financialYear.Value - 1, 12, 31);

            // Get all transaction years for dropdown (from transactions table)
            var allTransactionYears = await _context.SecurityLogs
                .Where(t => t.CompanyId == currentUser.CompanyId)
                .Select(t => t.TransactionDate.Year)
                .Distinct()
                .OrderByDescending(y => y)
                .ToListAsync();

            // Query transaction lines directly from database for current year income
            var currentIncomeData = await _context.ProcessHandlers
                .Include(ph => ph.Qw8Rt5Entity)
                .Include(ph => ph.Pz7Vm5Protocol)
                .Where(ph => ph.Qw8Rt5Entity.CompanyId == currentUser.CompanyId &&
                            ph.Qw8Rt5Entity.Mx9Qw7Type == Mx9Qw7Type.Revenue &&
                            ph.Pz7Vm5Protocol.TransactionDate >= currentStartDate &&
                            ph.Pz7Vm5Protocol.TransactionDate <= currentEndDate)
                .GroupBy(ph => ph.Qw8Rt5Entity.AccountName)
                .Select(g => new { AccountName = g.Key, Amount = g.Sum(ph => ph.Credit - ph.Debit) })
                .ToListAsync();

            // Query transaction lines directly from database for previous year income
            var previousIncomeData = await _context.ProcessHandlers
                .Include(ph => ph.Qw8Rt5Entity)
                .Include(ph => ph.Pz7Vm5Protocol)
                .Where(ph => ph.Qw8Rt5Entity.CompanyId == currentUser.CompanyId &&
                            ph.Qw8Rt5Entity.Mx9Qw7Type == Mx9Qw7Type.Revenue &&
                            ph.Pz7Vm5Protocol.TransactionDate >= previousStartDate &&
                            ph.Pz7Vm5Protocol.TransactionDate <= previousEndDate)
                .GroupBy(ph => ph.Qw8Rt5Entity.AccountName)
                .Select(g => new { AccountName = g.Key, Amount = g.Sum(ph => ph.Credit - ph.Debit) })
                .ToListAsync();

            // Query transaction lines directly from database for current year expenses
            var currentExpenseData = await _context.ProcessHandlers
                .Include(ph => ph.Qw8Rt5Entity)
                .Include(ph => ph.Pz7Vm5Protocol)
                .Where(ph => ph.Qw8Rt5Entity.CompanyId == currentUser.CompanyId &&
                            ph.Qw8Rt5Entity.Mx9Qw7Type == Mx9Qw7Type.Expense &&
                            ph.Pz7Vm5Protocol.TransactionDate >= currentStartDate &&
                            ph.Pz7Vm5Protocol.TransactionDate <= currentEndDate)
                .GroupBy(ph => ph.Qw8Rt5Entity.AccountName)
                .Select(g => new { AccountName = g.Key, Amount = g.Sum(ph => ph.Debit - ph.Credit) })
                .ToListAsync();

            // Query transaction lines directly from database for previous year expenses
            var previousExpenseData = await _context.ProcessHandlers
                .Include(ph => ph.Qw8Rt5Entity)
                .Include(ph => ph.Pz7Vm5Protocol)
                .Where(ph => ph.Qw8Rt5Entity.CompanyId == currentUser.CompanyId &&
                            ph.Qw8Rt5Entity.Mx9Qw7Type == Mx9Qw7Type.Expense &&
                            ph.Pz7Vm5Protocol.TransactionDate >= previousStartDate &&
                            ph.Pz7Vm5Protocol.TransactionDate <= previousEndDate)
                .GroupBy(ph => ph.Qw8Rt5Entity.AccountName)
                .Select(g => new { AccountName = g.Key, Amount = g.Sum(ph => ph.Debit - ph.Credit) })
                .ToListAsync();

            // Build view model from database results
            var incomeItems = currentIncomeData
                .Select(c => new IncomeExpenditureItem
                {
                    AccountName = c.AccountName,
                    Amount = c.Amount,
                    BudgetedAmount = previousIncomeData.FirstOrDefault(p => p.AccountName == c.AccountName)?.Amount ?? 0
                })
                .Union(previousIncomeData
                    .Where(p => !currentIncomeData.Any(c => c.AccountName == p.AccountName))
                    .Select(p => new IncomeExpenditureItem
                    {
                        AccountName = p.AccountName,
                        Amount = 0,
                        BudgetedAmount = p.Amount
                    }))
                .ToList();

            var expenseItems = currentExpenseData
                .Select(c => new IncomeExpenditureItem
                {
                    AccountName = c.AccountName,
                    Amount = c.Amount,
                    BudgetedAmount = previousExpenseData.FirstOrDefault(p => p.AccountName == c.AccountName)?.Amount ?? 0
                })
                .Union(previousExpenseData
                    .Where(p => !currentExpenseData.Any(c => c.AccountName == p.AccountName))
                    .Select(p => new IncomeExpenditureItem
                    {
                        AccountName = p.AccountName,
                        Amount = 0,
                        BudgetedAmount = p.Amount
                    }))
                .ToList();

            var viewModel = new IncomeExpenditureViewModel
            {
                IncomeItems = incomeItems ?? new List<IncomeExpenditureItem>(),
                ExpenditureItems = expenseItems ?? new List<IncomeExpenditureItem>()
            };

            // Calculate totals and variances with safe null checking
            viewModel.TotalIncome = viewModel.IncomeItems?.Sum(i => i.Amount) ?? 0;
            viewModel.TotalExpenditure = viewModel.ExpenditureItems?.Sum(e => e.Amount) ?? 0;
            viewModel.NetPosition = viewModel.TotalIncome - viewModel.TotalExpenditure;

            viewModel.TotalBudgetedIncome = viewModel.IncomeItems?.Sum(i => i.BudgetedAmount) ?? 0;
            viewModel.TotalBudgetedExpenditure = viewModel.ExpenditureItems?.Sum(e => e.BudgetedAmount) ?? 0;
            viewModel.NetBudgeted = viewModel.TotalBudgetedIncome - viewModel.TotalBudgetedExpenditure;

            // Calculate variances
            viewModel.IncomeVariance = viewModel.TotalIncome - viewModel.TotalBudgetedIncome;
            viewModel.ExpenditureVariance = viewModel.TotalExpenditure - viewModel.TotalBudgetedExpenditure;
            viewModel.NetVariance = viewModel.NetPosition - viewModel.NetBudgeted;

            var company = await _context.SystemEntries.FirstOrDefaultAsync(c => c.CompanyId == currentUser.CompanyId);
            ViewBag.Hx7Tz3Data = company;
            ViewBag.SelectedFinancialYear = financialYear;
            ViewBag.AvailableFinancialYears = allTransactionYears;
            ViewBag.PreviousYearIncome = viewModel.TotalBudgetedIncome;
            ViewBag.PreviousYearExpenditure = viewModel.TotalBudgetedExpenditure;
            ViewBag.PreviousYearNet = viewModel.NetBudgeted;
            ViewBag.AccountNotes = new Dictionary<string, object>(); // For future use
            ViewBag.StartDate = currentStartDate;
            ViewBag.EndDate = currentEndDate;

            return View("7", viewModel);
        }

        // Export Methods
        [HttpGet]
        public async Task<IActionResult> ExportGeneralLedgerExcel(DateTime? startDate, DateTime? endDate)
        {
            try
            {
                var userName = User.Identity?.Name;
                var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName);
                if (currentUser?.CompanyId == null)
                    return Forbid();

                startDate ??= DateTime.Today.AddMonths(-1);
                endDate ??= DateTime.Today;

                var accounts = await _context.DataStreams
                    .Include(a => a.ProcessHandlers)
                    .ThenInclude(ph => ph.Pz7Vm5Protocol)
                    .Where(a => a.CompanyId == currentUser.CompanyId)
                    .ToListAsync();

                var company = await _context.SystemEntries.FirstOrDefaultAsync(c => c.CompanyId == currentUser.CompanyId);
                var excelBytes = await _excelExportService.ExportGeneralLedgerAsync(accounts, startDate.Value, endDate.Value, company);

                return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
                    $"GeneralLedger_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting general ledger");
                return BadRequest("Failed to export general ledger");
            }
        }

        [HttpGet]
        public async Task<IActionResult> ExportGeneralJournalExcel(DateTime? startDate, DateTime? endDate)
        {
            try
            {
                var userName = User.Identity?.Name;
                var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName);
                if (currentUser?.CompanyId == null)
                    return Forbid();

                startDate ??= DateTime.Today.AddMonths(-1);
                endDate ??= DateTime.Today;

                var transactions = await _context.SecurityLogs
                    .Include(t => t.ProcessHandlers)
                    .Where(t => t.CompanyId == currentUser.CompanyId &&
                               t.TransactionDate >= startDate &&
                               t.TransactionDate <= endDate)
                    .ToListAsync();

                var company = await _context.SystemEntries.FirstOrDefaultAsync(c => c.CompanyId == currentUser.CompanyId);
                var excelBytes = await _excelExportService.ExportGeneralJournalAsync(transactions, startDate.Value, endDate.Value, company);

                return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
                    $"GeneralJournal_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting general journal");
                return BadRequest("Failed to export general journal");
            }
        }

        // POST: Save Account Note
        [HttpPost]
        public async Task<IActionResult> SaveNote(string reportType, string accountName, string content, string noteId)
        {
            try
            {
                var userName = User.Identity?.Name;
                var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName);
                if (currentUser?.CompanyId == null || !User.IsInRole("SGB Treasurer"))
                    return Json(new { success = false, message = "Unauthorized" });

                // For now, just return success (notes storage not implemented)
                // In the future, this would save to a notes table
                var newNoteId = string.IsNullOrEmpty(noteId) ? DateTime.Now.Ticks.ToString() : noteId;
                
                return Json(new { success = true, noteId = newNoteId, message = "Note saved successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving note");
                return Json(new { success = false, message = "Error saving note" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ExportIncomeExpenditureExcel(int financialYear = 0, bool includeVariance = false)
        {
            try
            {
                var userName = User.Identity?.Name;
                var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName);
                if (currentUser?.CompanyId == null)
                    return Forbid();

                if (financialYear == 0)
                    financialYear = DateTime.Now.Year;

                // Get date range for current calendar year (Jan 1 to Dec 31)
                var currentStartDate = new DateTime(financialYear, 1, 1);
                var currentEndDate = new DateTime(financialYear, 12, 31);

                // Get date range for previous calendar year
                var previousStartDate = new DateTime(financialYear - 1, 1, 1);
                var previousEndDate = new DateTime(financialYear - 1, 12, 31);

                // Query transaction lines directly from database for current year income
                var currentIncomeData = await _context.ProcessHandlers
                    .Include(ph => ph.Qw8Rt5Entity)
                    .Include(ph => ph.Pz7Vm5Protocol)
                    .Where(ph => ph.Qw8Rt5Entity.CompanyId == currentUser.CompanyId &&
                                ph.Qw8Rt5Entity.Mx9Qw7Type == Mx9Qw7Type.Revenue &&
                                ph.Pz7Vm5Protocol.TransactionDate >= currentStartDate &&
                                ph.Pz7Vm5Protocol.TransactionDate <= currentEndDate)
                    .GroupBy(ph => ph.Qw8Rt5Entity.AccountName)
                    .Select(g => new { AccountName = g.Key, Amount = g.Sum(ph => ph.Credit - ph.Debit) })
                    .ToListAsync();

                // Query transaction lines directly from database for previous year income
                var previousIncomeData = await _context.ProcessHandlers
                    .Include(ph => ph.Qw8Rt5Entity)
                    .Include(ph => ph.Pz7Vm5Protocol)
                    .Where(ph => ph.Qw8Rt5Entity.CompanyId == currentUser.CompanyId &&
                                ph.Qw8Rt5Entity.Mx9Qw7Type == Mx9Qw7Type.Revenue &&
                                ph.Pz7Vm5Protocol.TransactionDate >= previousStartDate &&
                                ph.Pz7Vm5Protocol.TransactionDate <= previousEndDate)
                    .GroupBy(ph => ph.Qw8Rt5Entity.AccountName)
                    .Select(g => new { AccountName = g.Key, Amount = g.Sum(ph => ph.Credit - ph.Debit) })
                    .ToListAsync();

                // Query transaction lines directly from database for current year expenses
                var currentExpenseData = await _context.ProcessHandlers
                    .Include(ph => ph.Qw8Rt5Entity)
                    .Include(ph => ph.Pz7Vm5Protocol)
                    .Where(ph => ph.Qw8Rt5Entity.CompanyId == currentUser.CompanyId &&
                                ph.Qw8Rt5Entity.Mx9Qw7Type == Mx9Qw7Type.Expense &&
                                ph.Pz7Vm5Protocol.TransactionDate >= currentStartDate &&
                                ph.Pz7Vm5Protocol.TransactionDate <= currentEndDate)
                    .GroupBy(ph => ph.Qw8Rt5Entity.AccountName)
                    .Select(g => new { AccountName = g.Key, Amount = g.Sum(ph => ph.Debit - ph.Credit) })
                    .ToListAsync();

                // Query transaction lines directly from database for previous year expenses
                var previousExpenseData = await _context.ProcessHandlers
                    .Include(ph => ph.Qw8Rt5Entity)
                    .Include(ph => ph.Pz7Vm5Protocol)
                    .Where(ph => ph.Qw8Rt5Entity.CompanyId == currentUser.CompanyId &&
                                ph.Qw8Rt5Entity.Mx9Qw7Type == Mx9Qw7Type.Expense &&
                                ph.Pz7Vm5Protocol.TransactionDate >= previousStartDate &&
                                ph.Pz7Vm5Protocol.TransactionDate <= previousEndDate)
                    .GroupBy(ph => ph.Qw8Rt5Entity.AccountName)
                    .Select(g => new { AccountName = g.Key, Amount = g.Sum(ph => ph.Debit - ph.Credit) })
                    .ToListAsync();

                // Build view model from database results
                var incomeItems = currentIncomeData
                    .Select(c => new IncomeExpenditureItem
                    {
                        AccountName = c.AccountName,
                        Amount = c.Amount,
                        BudgetedAmount = previousIncomeData.FirstOrDefault(p => p.AccountName == c.AccountName)?.Amount ?? 0
                    })
                    .Union(previousIncomeData
                        .Where(p => !currentIncomeData.Any(c => c.AccountName == p.AccountName))
                        .Select(p => new IncomeExpenditureItem
                        {
                            AccountName = p.AccountName,
                            Amount = 0,
                            BudgetedAmount = p.Amount
                        }))
                    .ToList();

                var expenseItems = currentExpenseData
                    .Select(c => new IncomeExpenditureItem
                    {
                        AccountName = c.AccountName,
                        Amount = c.Amount,
                        BudgetedAmount = previousExpenseData.FirstOrDefault(p => p.AccountName == c.AccountName)?.Amount ?? 0
                    })
                    .Union(previousExpenseData
                        .Where(p => !currentExpenseData.Any(c => c.AccountName == p.AccountName))
                        .Select(p => new IncomeExpenditureItem
                        {
                            AccountName = p.AccountName,
                            Amount = 0,
                            BudgetedAmount = p.Amount
                        }))
                    .ToList();

                var viewModel = new IncomeExpenditureViewModel
                {
                    IncomeItems = incomeItems ?? new List<IncomeExpenditureItem>(),
                    ExpenditureItems = expenseItems ?? new List<IncomeExpenditureItem>()
                };

                viewModel.TotalIncome = viewModel.IncomeItems?.Sum(i => i.Amount) ?? 0;
                viewModel.TotalExpenditure = viewModel.ExpenditureItems?.Sum(e => e.Amount) ?? 0;
                viewModel.NetPosition = viewModel.TotalIncome - viewModel.TotalExpenditure;

                viewModel.TotalBudgetedIncome = viewModel.IncomeItems?.Sum(i => i.BudgetedAmount) ?? 0;
                viewModel.TotalBudgetedExpenditure = viewModel.ExpenditureItems?.Sum(e => e.BudgetedAmount) ?? 0;
                viewModel.NetBudgeted = viewModel.TotalBudgetedIncome - viewModel.TotalBudgetedExpenditure;

                viewModel.IncomeVariance = viewModel.TotalIncome - viewModel.TotalBudgetedIncome;
                viewModel.ExpenditureVariance = viewModel.TotalExpenditure - viewModel.TotalBudgetedExpenditure;
                viewModel.NetVariance = viewModel.NetPosition - viewModel.NetBudgeted;

                var company = await _context.SystemEntries.FirstOrDefaultAsync(c => c.CompanyId == currentUser.CompanyId);
                var excelBytes = await _excelExportService.ExportIncomeExpenditureAsync(viewModel, currentStartDate, currentEndDate, company, includeVariance);

                return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
                    $"IncomeExpenditure_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting income expenditure");
                return BadRequest("Failed to export income expenditure");
            }
        }

        // GET: Income & Expenditure Report with Variance (for NonProfit organizations)
        public async Task<IActionResult> IncomeExpenditureReport(int? financialYear = null)
        {
            return await IncomeExpenditure(financialYear);
        }

        // GET: Income Statement (for Business organizations)
        public async Task<IActionResult> IncomeStatement(int? financialYear = null)
        {
            return await IncomeExpenditure(financialYear);
        }

        // GET: Statement of Financial Position / Balance Sheet
        public async Task<IActionResult> StatementOfFinancialPosition(int? financialYear = null)
        {
            var userName = User.Identity?.Name;
            var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName);
            if (currentUser?.CompanyId == null)
                return Forbid();

            financialYear ??= DateTime.Now.Year;

            // Get date range for calendar year
            var startDate = new DateTime(financialYear.Value, 1, 1);
            var endDate = new DateTime(financialYear.Value, 12, 31);

            // Get all transaction years for dropdown
            var allTransactionYears = await _context.SecurityLogs
                .Where(t => t.CompanyId == currentUser.CompanyId)
                .Select(t => t.TransactionDate.Year)
                .Distinct()
                .OrderByDescending(y => y)
                .ToListAsync();

            var accounts = await _context.DataStreams
                .Include(a => a.ProcessHandlers)
                .ThenInclude(ph => ph.Pz7Vm5Protocol)
                .Where(a => a.CompanyId == currentUser.CompanyId)
                .ToListAsync();

            // Build balance sheet view model (using placeholder for now)
            var viewModel = new BalanceSheetViewModel
            {
                AssetItems = new List<BalanceSheetItem>(),
                LiabilityItems = new List<BalanceSheetItem>(),
                EquityItems = new List<BalanceSheetItem>(),
                TotalAssets = 0,
                TotalLiabilities = 0,
                TotalEquity = 0
            };

            var company = await _context.SystemEntries.FirstOrDefaultAsync(c => c.CompanyId == currentUser.CompanyId);
            ViewBag.Hx7Tz3Data = company;
            ViewBag.SelectedFinancialYear = financialYear;
            ViewBag.AvailableFinancialYears = allTransactionYears;
            ViewBag.AsOfDate = endDate;
            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;

            return View("10", viewModel);
        }

        // GET: Balance Sheet (for Business organizations)
        public async Task<IActionResult> BalanceSheet(int? financialYear = null)
        {
            return await StatementOfFinancialPosition(financialYear);
        }

        // GET: Trial Balance
        public async Task<IActionResult> TrialBalance(DateTime? asDate = null)
        {
            var userName = User.Identity?.Name;
            var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName);
            if (currentUser?.CompanyId == null)
                return Forbid();

            asDate ??= DateTime.Today;

            // Get all transaction lines and calculate balances
            var transactionLines = await _context.ProcessHandlers
                .Include(tl => tl.Qw8Rt5Entity)
                .Include(tl => tl.Pz7Vm5Protocol)
                .Where(tl => tl.Qw8Rt5Entity.CompanyId == currentUser.CompanyId &&
                            tl.Pz7Vm5Protocol.TransactionDate <= asDate)
                .ToListAsync();

            // Build Trial Balance items
            var trialBalanceItems = transactionLines
                .GroupBy(tl => tl.Qw8Rt5Entity)
                .Select(g => new Lv6Cx9Item
                {
                    Qw8Rt5Entity = g.Key,
                    DebitBalance = g.Where(tl => tl.Debit > 0).Sum(tl => tl.Debit),
                    CreditBalance = g.Where(tl => tl.Credit > 0).Sum(tl => tl.Credit)
                })
                .OrderBy(tb => tb.Qw8Rt5Entity.AccountName)
                .ToList();

            var totalDebits = trialBalanceItems.Sum(tb => tb.DebitBalance);
            var totalCredits = trialBalanceItems.Sum(tb => tb.CreditBalance);

            var company = await _context.SystemEntries.FirstOrDefaultAsync(c => c.CompanyId == currentUser.CompanyId);
            ViewBag.Hx7Tz3Data = company;
            ViewBag.AsOfDate = asDate;
            ViewBag.TotalDebits = totalDebits;
            ViewBag.TotalCredits = totalCredits;

            return View("5", trialBalanceItems);
        }

        // GET: Account Ledger
        public async Task<IActionResult> AccountLedger(int? accountId = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            var userName = User.Identity?.Name;
            var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName);
            if (currentUser?.CompanyId == null)
                return Forbid();

            startDate ??= DateTime.Today.AddMonths(-1);
            endDate ??= DateTime.Today;

            // Get all accounts for this company
            var accounts = await _context.DataStreams
                .Include(a => a.ProcessHandlers)
                .ThenInclude(ph => ph.Pz7Vm5Protocol)
                .Where(a => a.CompanyId == currentUser.CompanyId)
                .ToListAsync();

            var company = await _context.SystemEntries.FirstOrDefaultAsync(c => c.CompanyId == currentUser.CompanyId);
            ViewBag.Hx7Tz3Data = company;
            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;
            ViewBag.AccountId = accountId;

            return View("4", accounts);
        }

        // GET: Manage Notes to Financial Statements
        public async Task<IActionResult> ManageNotes(string reportType = "", string searchTerm = "", int page = 1)
        {
            var userName = User.Identity?.Name;
            var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName);
            if (currentUser?.CompanyId == null)
                return Forbid();

            const int pageSize = 10;

            // Build query for notes
            var query = _context.Set<Jy9Xs1Buffer>().AsQueryable();

            // Filter by report type if provided
            if (!string.IsNullOrWhiteSpace(reportType))
            {
                query = query.Where(n => n.ReportType == reportType);
            }

            // Filter by search term
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(n => n.Content.Contains(searchTerm) || 
                                        (n.AccountName != null && n.AccountName.Contains(searchTerm)));
            }

            // Get total count
            var totalNotes = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalNotes / (double)pageSize);

            // Get paginated notes
            var notes = await query
                .OrderByDescending(n => n.CreatedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var company = await _context.SystemEntries.FirstOrDefaultAsync(c => c.CompanyId == currentUser.CompanyId);
            
            // Get distinct report types for filter
            var reportTypes = await _context.Set<Jy9Xs1Buffer>()
                .Select(n => n.ReportType)
                .Distinct()
                .OrderBy(t => t)
                .ToListAsync();

            ViewBag.Hx7Tz3Data = company;
            ViewBag.ReportTypes = reportTypes;
            ViewBag.SelectedReportType = reportType;
            ViewBag.SearchTerm = searchTerm;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalNotes = totalNotes;

            return View("14", notes);
        }

        // POST: Delete a single note
        [HttpPost]
        public async Task<IActionResult> DeleteNote(int noteId)
        {
            try
            {
                var userName = User.Identity?.Name;
                var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName);
                if (currentUser?.CompanyId == null || !User.IsInRole("SGB Treasurer"))
                    return Json(new { success = false, message = "Unauthorized" });

                var note = await _context.Set<Jy9Xs1Buffer>().FirstOrDefaultAsync(n => n.NoteId == noteId);
                if (note == null)
                    return Json(new { success = false, message = "Note not found" });

                _context.Set<Jy9Xs1Buffer>().Remove(note);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Note deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting note");
                return Json(new { success = false, message = "Error deleting note" });
            }
        }

        // POST: Bulk delete notes
        [HttpPost]
        public async Task<IActionResult> BulkDeleteNotes([FromBody] List<int> noteIds)
        {
            try
            {
                var userName = User.Identity?.Name;
                var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName);
                if (currentUser?.CompanyId == null || !User.IsInRole("SGB Treasurer"))
                    return Json(new { success = false, message = "Unauthorized" });

                var notes = await _context.Set<Jy9Xs1Buffer>()
                    .Where(n => noteIds.Contains(n.NoteId))
                    .ToListAsync();

                if (notes.Count == 0)
                    return Json(new { success = false, message = "No notes found" });

                _context.Set<Jy9Xs1Buffer>().RemoveRange(notes);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = $"{notes.Count} note(s) deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error bulk deleting notes");
                return Json(new { success = false, message = "Error deleting notes" });
            }
        }

        // GET: Cash Payments Journal (CPJ)
        public async Task<IActionResult> CashPaymentsJournal(DateTime? startDate = null, DateTime? endDate = null)
        {
            var userName = User.Identity?.Name;
            var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName);
            if (currentUser?.CompanyId == null)
                return Forbid();

            startDate ??= DateTime.Today.AddMonths(-1);
            endDate ??= DateTime.Today;

            var transactions = await _context.SecurityLogs
                .Include(t => t.ProcessHandlers)
                .Where(t => t.CompanyId == currentUser.CompanyId &&
                           t.TransactionDate >= startDate &&
                           t.TransactionDate <= endDate)
                .ToListAsync();

            var company = await _context.SystemEntries.FirstOrDefaultAsync(c => c.CompanyId == currentUser.CompanyId);
            ViewBag.Hx7Tz3Data = company;
            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;

            return View("12", transactions);
        }

        // GET: Cash Receipts Journal (CRJ)
        public async Task<IActionResult> CashReceiptsJournal(DateTime? startDate = null, DateTime? endDate = null)
        {
            var userName = User.Identity?.Name;
            var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName);
            if (currentUser?.CompanyId == null)
                return Forbid();

            startDate ??= DateTime.Today.AddMonths(-1);
            endDate ??= DateTime.Today;

            var transactions = await _context.SecurityLogs
                .Include(t => t.ProcessHandlers)
                .Where(t => t.CompanyId == currentUser.CompanyId &&
                           t.TransactionDate >= startDate &&
                           t.TransactionDate <= endDate)
                .ToListAsync();

            var company = await _context.SystemEntries.FirstOrDefaultAsync(c => c.CompanyId == currentUser.CompanyId);
            ViewBag.Hx7Tz3Data = company;
            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;

            return View("13", transactions);
        }

        // GET: My Transactions (for SGB Treasurer)
        public async Task<IActionResult> MyTransactions(DateTime? startDate = null, DateTime? endDate = null)
        {
            var userName = User.Identity?.Name;
            var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName);
            if (currentUser?.CompanyId == null)
                return Forbid();

            startDate ??= DateTime.Today.AddMonths(-1);
            endDate ??= DateTime.Today;

            var transactions = await _context.SecurityLogs
                .Include(t => t.ProcessHandlers)
                .Where(t => t.CompanyId == currentUser.CompanyId &&
                           t.EnteredByUserId == currentUser.Id &&
                           t.TransactionDate >= startDate &&
                           t.TransactionDate <= endDate)
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();

            var company = await _context.SystemEntries.FirstOrDefaultAsync(c => c.CompanyId == currentUser.CompanyId);
            ViewBag.Hx7Tz3Data = company;
            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;

            return View("1", transactions);
        }
    }
}

