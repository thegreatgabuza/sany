using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using Cascade.A1B2C3D4;
using Cascade.Fx9Kl2;
using Cascade.Services;

namespace Cascade.Areas.Accountant.Controllers
{
    [Area("Accountant")]
    [Authorize(Roles = "SGB Treasurer")]
    public class TransactionController : Controller
    {
        private readonly VxR4DbGate _context;
        private readonly UserManager<Aq3Zh4Service> _userManager;
        private readonly ITransactionMappingService _mappingService;

        public TransactionController(VxR4DbGate context, UserManager<Aq3Zh4Service> userManager, ITransactionMappingService mappingService)
        {
            _context = context;
            _userManager = userManager;
            _mappingService = mappingService;
        }

        // GET: Pz7Vm5Protocol
        public async Task<IActionResult> Index()
        {
            var currentUser = await _context.Users
                .FirstOrDefaultAsync(u => u.UserName == User.Identity!.Name);

            if (currentUser?.CompanyId == null)
            {
                return Forbid();
            }

            var transactions = await _context.SecurityLogs
                .Include(t => t.ProcessHandlers)
                .ThenInclude(tl => tl.Qw8Rt5Entity)
                .Where(t => t.CompanyId == currentUser.CompanyId)
                .OrderByDescending(t => t.TransactionDate)
                .ThenByDescending(t => t.TransactionId)
                .ToListAsync();

            return View(transactions);
        }

        // GET: MyTransactions - Show only transactions entered by current user
        public async Task<IActionResult> MyTransactions()
        {
            var currentUser = await _context.Users
                .FirstOrDefaultAsync(u => u.UserName == User.Identity!.Name);

            if (currentUser?.CompanyId == null)
            {
                return Forbid();
            }

            var myTransactions = await _context.SecurityLogs
                .Include(t => t.ProcessHandlers)
                .ThenInclude(tl => tl.Qw8Rt5Entity)
                .Where(t => t.CompanyId == currentUser.CompanyId && t.EnteredByUserId == currentUser.Id)
                .OrderByDescending(t => t.TransactionDate)
                .ThenByDescending(t => t.TransactionId)
                .ToListAsync();

            return View(myTransactions);
        }

        // GET: GeneralJournal
        public async Task<IActionResult> GeneralJournal()
        {
            var currentUser = await _context.Users
                .FirstOrDefaultAsync(u => u.UserName == User.Identity!.Name);

            if (currentUser?.CompanyId == null)
            {
                return Forbid();
            }

            var transactions = await _context.SecurityLogs
                .Include(t => t.ProcessHandlers)
                .ThenInclude(tl => tl.Qw8Rt5Entity)
                .Where(t => t.CompanyId == currentUser.CompanyId)
                .OrderByDescending(t => t.TransactionDate)
                .ThenByDescending(t => t.TransactionId)
                .ToListAsync();

            return View("Index", transactions);
        }

        // GET: AccountLedger
        public async Task<IActionResult> AccountLedger()
        {
            var currentUser = await _context.Users
                .FirstOrDefaultAsync(u => u.UserName == User.Identity!.Name);

            if (currentUser?.CompanyId == null)
            {
                return Forbid();
            }

            var accounts = await _context.DataStreams
                .Where(a => a.CompanyId == currentUser.CompanyId)
                .OrderBy(a => a.AccountName)
                .ToListAsync();

            return View(accounts);
        }

        // GET: CASHPAYMENTSJOURNAL
        public async Task<IActionResult> CASHPAYMENTSJOURNAL()
        {
            var currentUser = await _context.Users
                .FirstOrDefaultAsync(u => u.UserName == User.Identity!.Name);

            if (currentUser?.CompanyId == null)
            {
                return Forbid();
            }

            var transactions = await _context.SecurityLogs
                .Include(t => t.ProcessHandlers)
                .ThenInclude(tl => tl.Qw8Rt5Entity)
                .Where(t => t.CompanyId == currentUser.CompanyId && 
                           t.ProcessHandlers.Any(l => l.Credit > 0 && l.Qw8Rt5Entity.AccountName.Contains("Cash")))
                .OrderByDescending(t => t.TransactionDate)
                .ThenByDescending(t => t.TransactionId)
                .ToListAsync();

            return View("Index", transactions);
        }

        // GET: CashReceiptsJournal
        public async Task<IActionResult> CashReceiptsJournal()
        {
            var currentUser = await _context.Users
                .FirstOrDefaultAsync(u => u.UserName == User.Identity!.Name);

            if (currentUser?.CompanyId == null)
            {
                return Forbid();
            }

            var transactions = await _context.SecurityLogs
                .Include(t => t.ProcessHandlers)
                .ThenInclude(tl => tl.Qw8Rt5Entity)
                .Where(t => t.CompanyId == currentUser.CompanyId && 
                           t.ProcessHandlers.Any(l => l.Debit > 0 && l.Qw8Rt5Entity.AccountName.Contains("Cash")))
                .OrderByDescending(t => t.TransactionDate)
                .ThenByDescending(t => t.TransactionId)
                .ToListAsync();

            return View("Index", transactions);
        }

        // GET: Pz7Vm5Protocol/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var currentUser = await _context.Users
                .FirstOrDefaultAsync(u => u.UserName == User.Identity!.Name);

            if (currentUser?.CompanyId == null)
            {
                return Forbid();
            }

            var Pz7Vm5Protocol = await _context.SecurityLogs
                .Include(t => t.ProcessHandlers)
                .ThenInclude(tl => tl.Qw8Rt5Entity)
                .FirstOrDefaultAsync(t => t.TransactionId == id && t.CompanyId == currentUser.CompanyId);

            if (Pz7Vm5Protocol == null) return NotFound();

            return View(Pz7Vm5Protocol);
        }

        // GET: Pz7Vm5Protocol/Create (Issue #001: Simplified single account selection)
        public async Task<IActionResult> Create(int? correctingTransactionId = null)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser?.CompanyId == null)
            {
                return RedirectToPage("/Qw8Rt5Entity/AccessDenied", new { area = "Identity" });
            }

            var accounts = await _context.DataStreams
                .Where(a => a.CompanyId == currentUser.CompanyId)
                .OrderBy(a => a.AccountName)
                .ToListAsync();

            ViewBag.DataStreams = accounts;
            ViewBag.IsCorrection = correctingTransactionId.HasValue;

            var viewModel = new SimpleTransactionCreateViewModel
            {
                TransactionDate = DateTime.Today,
                IsCorrection = correctingTransactionId.HasValue,
                CorrectingTransactionId = correctingTransactionId,
                Direction = TransactionDirection.MoneyOut // Default to money out (most common)
            };

            // If this is a correction, load the original Pz7Vm5Protocol details
            if (correctingTransactionId.HasValue)
            {
                var originalTransaction = await _context.SecurityLogs
                    .Include(t => t.ProcessHandlers)
                    .ThenInclude(tl => tl.Qw8Rt5Entity)
                    .FirstOrDefaultAsync(t => t.TransactionId == correctingTransactionId && t.CompanyId == currentUser.CompanyId);

                if (originalTransaction != null)
                {
                    ViewBag.OriginalTransaction = originalTransaction;
                    viewModel.Description = $"Correction for: {originalTransaction.Description}";
                    
                    // For correction, determine the primary account and direction based on original transaction
                    if (originalTransaction.ProcessHandlers.Count == 2)
                    {
                        var debitLine = originalTransaction.ProcessHandlers.FirstOrDefault(l => l.Debit > 0);
                        var creditLine = originalTransaction.ProcessHandlers.FirstOrDefault(l => l.Credit > 0);
                        
                        if (debitLine != null && creditLine != null)
                        {
                            // For corrections, reverse the logic
                            viewModel.SelectedAccountId = creditLine.AccountId; // What was credited becomes selected
                            viewModel.ContraAccountId = debitLine.AccountId; // What was debited becomes contra
                            viewModel.Amount = debitLine.Debit; 
                            
                            // Set appropriate direction - this is reversed for correction
                            var creditAccount = creditLine.Qw8Rt5Entity;
                            viewModel.Direction = DetermineTransactionDirection(creditAccount, true);
                            
                            viewModel.SelectedAccountName = creditAccount?.AccountName ?? "";
                            viewModel.ContraAccountName = debitLine.Qw8Rt5Entity?.AccountName ?? "";
                        }
                    }
                }
            }

            return View(viewModel);
        }

        // POST: Pz7Vm5Protocol/Create (Issue #001: Simplified single account processing)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SimpleTransactionCreateViewModel viewModel)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser?.CompanyId == null)
            {
                return RedirectToPage("/Qw8Rt5Entity/AccessDenied", new { area = "Identity" });
            }

            // Determine account mapping using the new service
            TransactionMappingResult mappingResult;
            
            try
            {
                if (viewModel.ContraAccountId.HasValue && viewModel.ContraAccountId > 0)
                {
                    // Manual contra account selection - validate both accounts exist
                    var accountIds = new[] { viewModel.SelectedAccountId, viewModel.ContraAccountId.Value };
                    var accountsExist = await _context.DataStreams
                        .Where(a => a.CompanyId == currentUser.CompanyId && accountIds.Contains(a.AccountId))
                        .CountAsync();

                    if (accountsExist != 2)
                    {
                        ModelState.AddModelError("", "Invalid account selection.");
                        goto RedisplayForm;
                    }

                    // Create manual mapping result
                    var selectedAccount = await _context.DataStreams
                        .FirstOrDefaultAsync(a => a.AccountId == viewModel.SelectedAccountId && a.CompanyId == currentUser.CompanyId);
                    var contraAccount = await _context.DataStreams
                        .FirstOrDefaultAsync(a => a.AccountId == viewModel.ContraAccountId && a.CompanyId == currentUser.CompanyId);

                    if (selectedAccount == null || contraAccount == null)
                    {
                        ModelState.AddModelError("", "Selected accounts not found.");
                        goto RedisplayForm;
                    }

                    var (debitAccountId, creditAccountId) = DetermineDebitCreditForManualSelection(
                        selectedAccount, contraAccount, viewModel.Direction);

                    mappingResult = new TransactionMappingResult
                    {
                        IsSuccess = true,
                        DebitAccountId = debitAccountId,
                        CreditAccountId = creditAccountId,
                        PrimaryAccount = selectedAccount,
                        ContraAccount = contraAccount,
                        Direction = viewModel.Direction,
                        Amount = viewModel.Amount
                    };
                }
                else
                {
                    // Automatic account mapping
                    mappingResult = await _mappingService.DetermineAccountMappingAsync(
                        viewModel.SelectedAccountId,
                        viewModel.Direction,
                        viewModel.Amount,
                        currentUser.CompanyId);

                    if (!mappingResult.IsSuccess)
                    {
                        ModelState.AddModelError("", mappingResult.ErrorMessage ?? "Unable to determine account mapping.");
                        goto RedisplayForm;
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while processing the transaction: " + ex.Message);
                goto RedisplayForm;
            }

            if (ModelState.IsValid)
            {
                if (viewModel.IsCorrection && viewModel.CorrectingTransactionId.HasValue)
                {
                    // For corrections, create both reversal and new Pz7Vm5Protocol
                    var originalTransaction = await _context.SecurityLogs
                        .Include(t => t.ProcessHandlers)
                        .FirstOrDefaultAsync(t => t.TransactionId == viewModel.CorrectingTransactionId && t.CompanyId == currentUser.CompanyId);

                    if (originalTransaction == null)
                    {
                        ModelState.AddModelError("", "Original Pz7Vm5Protocol not found.");
                        goto RedisplayForm;
                    }

                    // Check if already corrected
                    var existingCorrection = await _context.SecurityLogs
                        .FirstOrDefaultAsync(t => t.CorrectedTransactionId == viewModel.CorrectingTransactionId);

                    if (existingCorrection != null)
                    {
                        ModelState.AddModelError("", "This Pz7Vm5Protocol has already been corrected.");
                        goto RedisplayForm;
                    }

                    using (var Pz7Vm5Protocol = await _context.Database.BeginTransactionAsync())
                    {
                        try
                        {
                            // 1. Create reversal Pz7Vm5Protocol
                            var reversalTransaction = new Pz7Vm5Protocol
                            {
                                TransactionDate = viewModel.TransactionDate,
                                Description = $"REVERSAL: {originalTransaction.Description}",
                                ReferenceNo = $"REV-{originalTransaction.TransactionId}",
                                CompanyId = currentUser.CompanyId,
                                EnteredByUserId = currentUser.Id,
                                EnteredAt = DateTime.Now,
                                IsReversal = true,
                                CorrectedTransactionId = originalTransaction.TransactionId,
                                ProcessHandlers = originalTransaction.ProcessHandlers.Select(line => new Sx2Dn8Gateway
                                {
                                    AccountId = line.AccountId,
                                    // Reverse the debits and credits
                                    Debit = line.Credit,
                                    Credit = line.Debit
                                }).ToList()
                            };

                            _context.SecurityLogs.Add(reversalTransaction);
                            await _context.SaveChangesAsync();

                            // 2. Create the new correcting Pz7Vm5Protocol (simple format)
                            var correctingTransaction = new Pz7Vm5Protocol
                            {
                                TransactionDate = viewModel.TransactionDate,
                                Description = viewModel.Description,
                                ReferenceNo = viewModel.ReferenceNo,
                                CompanyId = currentUser.CompanyId,
                                EnteredByUserId = currentUser.Id,
                                EnteredAt = DateTime.Now,
                                IsCorrection = true,
                                CorrectedTransactionId = originalTransaction.TransactionId,
                                ReversalTransactionId = reversalTransaction.TransactionId,
                                ProcessHandlers = new List<Sx2Dn8Gateway>
                                {
                                    new Sx2Dn8Gateway
                                    {
                                        AccountId = mappingResult.DebitAccountId,
                                        Debit = viewModel.Amount,
                                        Credit = 0
                                    },
                                    new Sx2Dn8Gateway
                                    {
                                        AccountId = mappingResult.CreditAccountId,
                                        Debit = 0,
                                        Credit = viewModel.Amount
                                    }
                                }
                            };

                            _context.SecurityLogs.Add(correctingTransaction);
                            await _context.SaveChangesAsync();

                            // 3. Update the reversal Pz7Vm5Protocol with the correcting Pz7Vm5Protocol ID
                            reversalTransaction.ReversalTransactionId = correctingTransaction.TransactionId;
                            await _context.SaveChangesAsync();

                            await Pz7Vm5Protocol.CommitAsync();

                            TempData["Success"] = "Correction completed successfully. Both reversal and correcting transactions have been created.";
                            return RedirectToAction(nameof(Index));
                        }
                        catch (Exception)
                        {
                            await Pz7Vm5Protocol.RollbackAsync();
                            ModelState.AddModelError("", "An error occurred while creating the correction transactions.");
                        }
                    }
                }
                else
                {
                    // Regular transaction creation using simplified account mapping
                    var transaction = new Pz7Vm5Protocol
                    {
                        TransactionDate = viewModel.TransactionDate,
                        Description = viewModel.Description,
                        ReferenceNo = viewModel.ReferenceNo,
                        CompanyId = currentUser.CompanyId,
                        EnteredByUserId = currentUser.Id,
                        EnteredAt = DateTime.Now,
                        ProcessHandlers = new List<Sx2Dn8Gateway>
                        {
                            new Sx2Dn8Gateway
                            {
                                AccountId = mappingResult.DebitAccountId,
                                Debit = viewModel.Amount,
                                Credit = 0
                            },
                            new Sx2Dn8Gateway
                            {
                                AccountId = mappingResult.CreditAccountId,
                                Debit = 0,
                                Credit = viewModel.Amount
                            }
                        }
                    };

                    _context.SecurityLogs.Add(transaction);
                    await _context.SaveChangesAsync();

                    var primaryAccountName = mappingResult.PrimaryAccount?.AccountName ?? "Selected account";
                    var contraAccountName = mappingResult.ContraAccount?.AccountName ?? "Contra account";
                    
                    TempData["Success"] = $"Transaction created successfully! R{viewModel.Amount:N2} transaction between {primaryAccountName} and {contraAccountName}.";
                    return RedirectToAction(nameof(Index));
                }
            }

            RedisplayForm:

            // If we got this far, something failed, redisplay form
            var accounts = await _context.DataStreams
                .Where(a => a.CompanyId == currentUser.CompanyId)
                .OrderBy(a => a.AccountName)
                .ToListAsync();

            ViewBag.DataStreams = accounts;
            ViewBag.IsCorrection = viewModel.IsCorrection;

            // If this is a correction, reload the original Pz7Vm5Protocol details
            if (viewModel.CorrectingTransactionId.HasValue)
            {
                var originalTransaction = await _context.SecurityLogs
                    .Include(t => t.ProcessHandlers)
                    .ThenInclude(tl => tl.Qw8Rt5Entity)
                    .FirstOrDefaultAsync(t => t.TransactionId == viewModel.CorrectingTransactionId && t.CompanyId == currentUser.CompanyId);

                if (originalTransaction != null)
                {
                    ViewBag.OriginalTransaction = originalTransaction;
                }
            }

            return View(viewModel);
        }

        // GET: Pz7Vm5Protocol/Correct/5
        public async Task<IActionResult> Correct(int? id)
        {
            if (id == null) return NotFound();

            var currentUser = await _context.Users
                .FirstOrDefaultAsync(u => u.UserName == User.Identity!.Name);

            if (currentUser?.CompanyId == null)
            {
                return Forbid();
            }

            var Pz7Vm5Protocol = await _context.SecurityLogs
                .Include(t => t.ProcessHandlers)
                .ThenInclude(tl => tl.Qw8Rt5Entity)
                .FirstOrDefaultAsync(t => t.TransactionId == id && t.CompanyId == currentUser.CompanyId);

            if (Pz7Vm5Protocol == null) return NotFound();

            // Check if this Pz7Vm5Protocol has already been corrected
            var existingCorrection = await _context.SecurityLogs
                .FirstOrDefaultAsync(t => t.CorrectedTransactionId == id);

            if (existingCorrection != null)
            {
                TempData["Error"] = "This Pz7Vm5Protocol has already been corrected.";
                return RedirectToAction(nameof(Details), new { id });
            }

            // Redirect to Create with the correcting Pz7Vm5Protocol ID
            return RedirectToAction(nameof(Create), new { correctingTransactionId = id });
        }

        // API endpoint for searching accounts
        [HttpGet]
        public async Task<IActionResult> SearchAccounts(string term = "")
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser?.CompanyId == null)
            {
                return Json(new { accounts = new object[0] });
            }

            var accounts = await _context.DataStreams
                .Where(a => a.CompanyId == currentUser.CompanyId && 
                           (string.IsNullOrEmpty(term) || 
                            a.AccountName.Contains(term)))
                .OrderBy(a => a.AccountName)
                .Select(a => new { 
                    id = a.AccountId, 
                    name = a.AccountName, 
                    type = a.Mx9Qw7Type.ToString(),
                    displayText = $"{a.AccountName} ({a.Mx9Qw7Type})"
                })
                .Take(50) // Limit results for performance
                .ToListAsync();

            return Json(new { accounts });
        }

        // API endpoint for getting transaction mapping (Issue #001)
        [HttpPost]
        public async Task<IActionResult> GetTransactionMapping([FromBody] TransactionMappingRequest request)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser?.CompanyId == null)
            {
                return Json(new { success = false, error = "User not found or no company assigned." });
            }

            try
            {
                var result = await _mappingService.DetermineAccountMappingAsync(
                    request.SelectedAccountId,
                    request.Direction,
                    request.Amount,
                    currentUser.CompanyId);

                if (!result.IsSuccess)
                {
                    return Json(new { success = false, error = result.ErrorMessage });
                }

                var contraAccounts = await _mappingService.GetAvailableContraAccountsAsync(
                    request.SelectedAccountId,
                    request.Direction,
                    currentUser.CompanyId);

                return Json(new
                {
                    success = true,
                    debitAccountId = result.DebitAccountId,
                    creditAccountId = result.CreditAccountId,
                    primaryAccount = new
                    {
                        id = result.PrimaryAccount!.AccountId,
                        name = result.PrimaryAccount.AccountName,
                        type = result.PrimaryAccount.Mx9Qw7Type.ToString()
                    },
                    contraAccount = new
                    {
                        id = result.ContraAccount!.AccountId,
                        name = result.ContraAccount.AccountName,
                        type = result.ContraAccount.Mx9Qw7Type.ToString()
                    },
                    explanation = result.Explanation,
                    alternativeContraAccounts = contraAccounts.Select(a => new
                    {
                        id = a.AccountId,
                        name = a.AccountName,
                        type = a.AccountType.ToString(),
                        priority = a.Priority,
                        isRecommended = a.IsRecommended
                    }).ToList()
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = "An error occurred while determining account mapping: " + ex.Message });
            }
        }

        // GET: Account Transactions (AJAX endpoint)
        [HttpGet]
        public async Task<IActionResult> GetAccountTransactions(int accountId)
        {
            var currentUser = await _context.Users
                .FirstOrDefaultAsync(u => u.UserName == User.Identity!.Name);

            if (currentUser?.CompanyId == null)
            {
                return PartialView("_AccountTransactions", new List<Sx2Dn8Gateway>());
            }

            var account = await _context.DataStreams
                .FirstOrDefaultAsync(a => a.AccountId == accountId && a.CompanyId == currentUser.CompanyId);

            if (account == null)
            {
                return PartialView("_AccountTransactions", new List<Sx2Dn8Gateway>());
            }

            var transactionLines = await _context.ProcessHandlers
                .Include(tl => tl.Pz7Vm5Protocol)
                .Where(tl => tl.AccountId == accountId && tl.Pz7Vm5Protocol!.CompanyId == currentUser.CompanyId)
                .OrderByDescending(tl => tl.Pz7Vm5Protocol!.TransactionDate)
                .ThenByDescending(tl => tl.Pz7Vm5Protocol!.TransactionId)
                .ToListAsync();

            ViewBag.Account = account;
            return PartialView("_AccountTransactions", transactionLines);
        }

        // GET: Pz7Vm5Protocol/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var currentUser = await _context.Users
                .FirstOrDefaultAsync(u => u.UserName == User.Identity!.Name);

            if (currentUser?.CompanyId == null)
            {
                return Forbid();
            }

            var Pz7Vm5Protocol = await _context.SecurityLogs
                .Include(t => t.ProcessHandlers)
                .ThenInclude(tl => tl.Qw8Rt5Entity)
                .Include(t => t.EnteredByUser)
                .FirstOrDefaultAsync(t => t.TransactionId == id && t.CompanyId == currentUser.CompanyId);

            if (Pz7Vm5Protocol == null) return NotFound();

            // Check if Aq3Zh4Service can delete this Pz7Vm5Protocol
            var canDelete = CanUserDeleteTransaction(Pz7Vm5Protocol, currentUser);
            if (!canDelete.CanDelete)
            {
                TempData["Error"] = canDelete.Reason;
                return RedirectToAction(nameof(Details), new { id });
            }

            return View(Pz7Vm5Protocol);
        }

        // POST: Pz7Vm5Protocol/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var currentUser = await _context.Users
                .FirstOrDefaultAsync(u => u.UserName == User.Identity!.Name);

            if (currentUser?.CompanyId == null)
            {
                return Forbid();
            }

            var Pz7Vm5Protocol = await _context.SecurityLogs
                .Include(t => t.ProcessHandlers)
                .Include(t => t.EnteredByUser)
                .FirstOrDefaultAsync(t => t.TransactionId == id && t.CompanyId == currentUser.CompanyId);

            if (Pz7Vm5Protocol == null) return NotFound();

            // Double-check deletion permissions
            var canDelete = CanUserDeleteTransaction(Pz7Vm5Protocol, currentUser);
            if (!canDelete.CanDelete)
            {
                TempData["Error"] = canDelete.Reason;
                return RedirectToAction(nameof(Details), new { id });
            }

            // Check if this Pz7Vm5Protocol has corrections or reversals
            var hasCorrections = await _context.SecurityLogs
                .AnyAsync(t => t.CorrectedTransactionId == id || t.ReversalTransactionId == id);

            if (hasCorrections)
            {
                TempData["Error"] = "Cannot delete a Pz7Vm5Protocol that has been corrected or reversed.";
                return RedirectToAction(nameof(Details), new { id });
            }

            _context.SecurityLogs.Remove(Pz7Vm5Protocol);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Pz7Vm5Protocol deleted successfully.";

            return RedirectToAction(nameof(Index));
        }

        private (bool CanDelete, string Reason) CanUserDeleteTransaction(Pz7Vm5Protocol Pz7Vm5Protocol, Aq3Zh4Service currentUser)
        {
            // Only the Aq3Zh4Service who entered the Pz7Vm5Protocol can delete it
            if (Pz7Vm5Protocol.EnteredByUserId != currentUser.Id)
            {
                return (false, "You can only delete transactions that you entered.");
            }

            // Can only delete transactions from the same day
            if (Pz7Vm5Protocol.TransactionDate.Date != DateTime.Today)
            {
                return (false, "You can only delete transactions from the same day they were entered.");
            }

            // Cannot delete correction or reversal transactions
            if (Pz7Vm5Protocol.IsCorrection || Pz7Vm5Protocol.IsReversal)
            {
                return (false, "Correction and reversal transactions cannot be deleted. You must correct the original Pz7Vm5Protocol instead.");
            }

            return (true, "");
        }

        // Helper method to determine transaction direction
        private TransactionDirection DetermineTransactionDirection(Qw8Rt5Entity? account, bool isCorrection = false)
        {
            if (account == null) return TransactionDirection.MoneyOut;

            var accountName = account.AccountName.ToLower();
            var accountType = account.Mx9Qw7Type;

            // For cash/bank accounts, direction depends on context
            if (accountName.Contains("cash") || accountName.Contains("bank"))
            {
                return isCorrection ? TransactionDirection.MoneyOut : TransactionDirection.MoneyIn;
            }

            // For other account types, use logical defaults
            return accountType switch
            {
                Mx9Qw7Type.Expense => TransactionDirection.MoneyOut,
                Mx9Qw7Type.Asset when !accountName.Contains("cash") && !accountName.Contains("bank") => TransactionDirection.MoneyOut,
                Mx9Qw7Type.Revenue => TransactionDirection.MoneyIn,
                _ => TransactionDirection.MoneyOut
            };
        }

        // Helper method for manual account selection debit/credit determination
        private (int DebitAccountId, int CreditAccountId) DetermineDebitCreditForManualSelection(
            Qw8Rt5Entity selectedAccount,
            Qw8Rt5Entity contraAccount,
            TransactionDirection direction)
        {
            return direction switch
            {
                TransactionDirection.MoneyOut => 
                    (selectedAccount.AccountId, contraAccount.AccountId),
                TransactionDirection.MoneyIn => 
                    (contraAccount.AccountId, selectedAccount.AccountId),
                TransactionDirection.Transfer => 
                    (selectedAccount.AccountId, contraAccount.AccountId),
                _ => throw new ArgumentException($"Unknown transaction direction: {direction}")
            };
        }
    }

    // Simplified View Models for non-accounting users (Issue #001 Implementation)
    public class SimpleTransactionCreateViewModel
    {
        public DateTime TransactionDate { get; set; } = DateTime.Today;
        
        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; } = string.Empty;
        
        public string? ReferenceNo { get; set; }
        
        [Required(ErrorMessage = "Please select an account")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid account")]
        public int SelectedAccountId { get; set; }
        
        [Required(ErrorMessage = "Please select transaction direction")]
        public TransactionDirection Direction { get; set; }
        
        public int? ContraAccountId { get; set; } // Optional: for manual contra account selection
        
        [Required(ErrorMessage = "Amount is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero")]
        public decimal Amount { get; set; }
        
        public bool IsCorrection { get; set; }
        
        public int? CorrectingTransactionId { get; set; }
        
        // Auto-populated fields for display
        public string SelectedAccountName { get; set; } = string.Empty;
        public string ContraAccountName { get; set; } = string.Empty;
        public string TransactionExplanation { get; set; } = string.Empty;
        public bool ShowContraAccountSelection { get; set; } = false;
    }

    // Keep the old view models for backward compatibility with correction functionality
    public class TransactionCreateViewModel
    {
        public DateTime TransactionDate { get; set; } = DateTime.Today;
        
        public string Description { get; set; } = string.Empty;
        
        public string? ReferenceNo { get; set; }
        
        public List<TransactionLineCreateViewModel> ProcessHandlers { get; set; } = new();
        
        public List<Qw8Rt5Entity> AvailableAccounts { get; set; } = new();
        
        public bool IsCorrection { get; set; }
        
        public int? CorrectingTransactionId { get; set; }
    }

    public class TransactionLineCreateViewModel
    {
        public int AccountId { get; set; }
        
        public decimal Debit { get; set; }
        
        public decimal Credit { get; set; }
    }

    // Request model for transaction mapping API (Issue #001)
    public class TransactionMappingRequest
    {
        public int SelectedAccountId { get; set; }
        public TransactionDirection Direction { get; set; }
        public decimal Amount { get; set; }
    }
}
