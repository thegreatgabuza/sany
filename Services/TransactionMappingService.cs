using Cascade.A1B2C3D4;
using Cascade.Fx9Kl2;
using Microsoft.EntityFrameworkCore;

namespace Cascade.Services
{
    /// <summary>
    /// Service for automatically mapping single account selections to proper debit/credit entries
    /// Implements Issue #001 - Simplify Account Selection for non-accounting users
    /// </summary>
    public interface ITransactionMappingService
    {
        Task<TransactionMappingResult> DetermineAccountMappingAsync(
            int selectedAccountId,
            TransactionDirection direction,
            decimal amount,
            int companyId);
            
        Task<List<AvailableContraAccount>> GetAvailableContraAccountsAsync(
            int primaryAccountId,
            TransactionDirection direction,
            int companyId);
            
        string GetTransactionExplanation(
            Qw8Rt5Entity primaryAccount,
            Qw8Rt5Entity contraAccount,
            TransactionDirection direction,
            decimal amount);
    }

    public class TransactionMappingService : ITransactionMappingService
    {
        private readonly VxR4DbGate _context;

        public TransactionMappingService(VxR4DbGate context)
        {
            _context = context;
        }

        public async Task<TransactionMappingResult> DetermineAccountMappingAsync(
            int selectedAccountId,
            TransactionDirection direction,
            decimal amount,
            int companyId)
        {
            try
            {
                if (selectedAccountId <= 0)
                {
                    return new TransactionMappingResult
                    {
                        IsSuccess = false,
                        ErrorMessage = "Invalid account ID provided."
                    };
                }

                if (companyId <= 0)
                {
                    return new TransactionMappingResult
                    {
                        IsSuccess = false,
                        ErrorMessage = "Invalid company ID provided."
                    };
                }

                var selectedAccount = await _context.DataStreams
                    .FirstOrDefaultAsync(a => a.AccountId == selectedAccountId && a.CompanyId == companyId);

                if (selectedAccount == null)
                {
                    return new TransactionMappingResult
                    {
                        IsSuccess = false,
                        ErrorMessage = "Selected account not found or does not belong to your company."
                    };
                }

                // Get the best contra account based on the transaction direction and account type
                var contraAccount = await GetBestContraAccountAsync(selectedAccount, direction, companyId);

                if (contraAccount == null)
                {
                    return new TransactionMappingResult
                    {
                        IsSuccess = false,
                        ErrorMessage = "No suitable contra account found. Please ensure you have Cash or Bank accounts set up."
                    };
                }

                // Determine which account should be debited and which should be credited
                var (debitAccountId, creditAccountId) = DetermineDebitCreditAccounts(
                    selectedAccount, contraAccount, direction);

                return new TransactionMappingResult
                {
                    IsSuccess = true,
                    DebitAccountId = debitAccountId,
                    CreditAccountId = creditAccountId,
                    PrimaryAccount = selectedAccount,
                    ContraAccount = contraAccount,
                    Direction = direction,
                    Amount = amount,
                    Explanation = GetTransactionExplanation(selectedAccount, contraAccount, direction, amount)
                };
            }
            catch (NullReferenceException ex)
            {
                return new TransactionMappingResult
                {
                    IsSuccess = false,
                    ErrorMessage = $"A null reference error occurred. Please ensure all accounts are properly configured. Details: {ex.Message}"
                };
            }
            catch (Exception ex)
            {
                return new TransactionMappingResult
                {
                    IsSuccess = false,
                    ErrorMessage = $"Error creating transaction mapping: {ex.Message}"
                };
            }
        }

        public async Task<List<AvailableContraAccount>> GetAvailableContraAccountsAsync(
            int primaryAccountId,
            TransactionDirection direction,
            int companyId)
        {
            var primaryAccount = await _context.DataStreams
                .FirstOrDefaultAsync(a => a.AccountId == primaryAccountId && a.CompanyId == companyId);

            if (primaryAccount == null)
                return new List<AvailableContraAccount>();

            var contraAccounts = await GetPossibleContraAccountsAsync(primaryAccount, direction, companyId);

            if (contraAccounts == null || !contraAccounts.Any())
            {
                return new List<AvailableContraAccount>();
            }

            return contraAccounts.Where(account => account != null).Select(account => new AvailableContraAccount
            {
                AccountId = account.AccountId,
                AccountName = account.AccountName ?? "Unknown Account",
                AccountType = account.Mx9Qw7Type,
                Priority = GetContraAccountPriority(account, direction),
                IsRecommended = IsRecommendedContraAccount(account, direction)
            }).OrderByDescending(a => a.Priority).ToList();
        }

        public string GetTransactionExplanation(
            Qw8Rt5Entity primaryAccount,
            Qw8Rt5Entity contraAccount,
            TransactionDirection direction,
            decimal amount)
        {
            if (primaryAccount == null || contraAccount == null)
            {
                return $"R{amount:N2} transaction - account information not available";
            }

            var primaryName = primaryAccount.AccountName ?? "Unknown Account";
            var contraName = contraAccount.AccountName ?? "Unknown Account";

            return direction switch
            {
                TransactionDirection.MoneyOut => 
                    $"R{amount:N2} flows FROM {contraName} TO {primaryName}",
                TransactionDirection.MoneyIn => 
                    $"R{amount:N2} flows FROM {primaryName} TO {contraName}",
                TransactionDirection.Transfer => 
                    $"R{amount:N2} transfers FROM {contraName} TO {primaryName}",
                _ => $"R{amount:N2} transaction between {primaryName} and {contraName}"
            };
        }

        private async Task<Qw8Rt5Entity?> GetBestContraAccountAsync(
            Qw8Rt5Entity selectedAccount,
            TransactionDirection direction,
            int companyId)
        {
            if (selectedAccount == null)
                return null;

            var possibleAccounts = await GetPossibleContraAccountsAsync(selectedAccount, direction, companyId);
            
            if (possibleAccounts == null || !possibleAccounts.Any())
                return null;

            // Filter out any null accounts and return the highest priority contra account
            var validAccounts = possibleAccounts.Where(a => a != null).ToList();
            if (!validAccounts.Any())
                return null;

            var bestAccount = validAccounts
                .OrderByDescending(a => GetContraAccountPriority(a!, direction))
                .FirstOrDefault();
            
            return bestAccount;
        }

        private async Task<List<Qw8Rt5Entity>> GetPossibleContraAccountsAsync(
            Qw8Rt5Entity selectedAccount,
            TransactionDirection direction,
            int companyId)
        {
            if (selectedAccount == null)
                return new List<Qw8Rt5Entity>();

            try
            {
                var selectedAccountId = selectedAccount.AccountId;
                if (selectedAccountId <= 0)
                    return new List<Qw8Rt5Entity>();

                var allAccounts = await _context.DataStreams
                    .Where(a => a.CompanyId == companyId && a.AccountId != selectedAccountId)
                    .ToListAsync();

                if (allAccounts == null || !allAccounts.Any())
                    return new List<Qw8Rt5Entity>();

                // Filter out any null accounts that might have been returned
                var validAccounts = allAccounts.Where(a => a != null).ToList();
                if (!validAccounts.Any())
                    return new List<Qw8Rt5Entity>();

                var possibleAccounts = direction switch
                {
                    TransactionDirection.MoneyOut or TransactionDirection.MoneyIn =>
                        // For money transactions, prefer cash/bank accounts
                        validAccounts.Where(a => 
                        {
                            try
                            {
                                if (a.Mx9Qw7Type != Mx9Qw7Type.Asset)
                                {
                                    return false;
                                }

                                var accountName = a.AccountName ?? string.Empty;
                                var accountNameLower = accountName.ToLowerInvariant();
                                return accountNameLower.Contains("cash") ||
                                       accountNameLower.Contains("bank") ||
                                       accountNameLower.Contains("petty cash");
                            }
                            catch
                            {
                                return false;
                            }
                        }).ToList(),

                    TransactionDirection.Transfer =>
                        // For transfers, allow any account except the selected one
                        validAccounts.Where(a => 
                        {
                            try
                            {
                                return IsValidTransferAccount(a, selectedAccount);
                            }
                            catch
                            {
                                return false;
                            }
                        }).ToList(),

                    _ => new List<Qw8Rt5Entity>()
                };

                // If no specific accounts found for money transactions, fall back to all asset accounts
                if ((direction == TransactionDirection.MoneyOut || direction == TransactionDirection.MoneyIn) && 
                    (possibleAccounts == null || !possibleAccounts.Any()))
                {
                    possibleAccounts = validAccounts.Where(a => 
                    {
                        try
                        {
                            return a.Mx9Qw7Type == Mx9Qw7Type.Asset;
                        }
                        catch
                        {
                            return false;
                        }
                    }).ToList();
                }

                return possibleAccounts ?? new List<Qw8Rt5Entity>();
            }
            catch (Exception)
            {
                return new List<Qw8Rt5Entity>();
            }
        }

        private bool IsValidTransferAccount(Qw8Rt5Entity candidateAccount, Qw8Rt5Entity selectedAccount)
        {
            // Allow transfers between:
            // - Asset accounts (cash, bank, equipment)
            // - Same type accounts (asset to asset, etc.)
            // - Logical business transfers

            if (candidateAccount == null || selectedAccount == null)
                return false;

            if (candidateAccount.Mx9Qw7Type == Mx9Qw7Type.Asset && selectedAccount.Mx9Qw7Type == Mx9Qw7Type.Asset)
                return true;

            // Allow some cross-type transfers that make business sense
            if (selectedAccount.Mx9Qw7Type == Mx9Qw7Type.Liability && candidateAccount.Mx9Qw7Type == Mx9Qw7Type.Asset)
                return true; // Paying off liability

            return false;
        }

        private int GetContraAccountPriority(Qw8Rt5Entity account, TransactionDirection direction)
        {
            if (account == null)
                return 0;

            var accountNameLower = (account.AccountName ?? string.Empty).ToLowerInvariant();
            
            // Priority scoring for contra accounts
            if (accountNameLower == "cash") return 100;
            if (accountNameLower == "bank") return 90;
            if (accountNameLower.Contains("petty cash")) return 80;
            if (accountNameLower.Contains("cash")) return 70;
            if (accountNameLower.Contains("bank")) return 60;
            
            // For transfers, other asset accounts have medium priority
            if (direction == TransactionDirection.Transfer && account.Mx9Qw7Type == Mx9Qw7Type.Asset) return 50;
            
            // Lower priority for other account types
            if (account.Mx9Qw7Type == Mx9Qw7Type.Liability) return 30;
            if (account.Mx9Qw7Type == Mx9Qw7Type.Equity) return 20;
            
            return 10; // Default low priority
        }

        private bool IsRecommendedContraAccount(Qw8Rt5Entity account, TransactionDirection direction)
        {
            if (account == null)
                return false;

            var accountNameLower = (account.AccountName ?? string.Empty).ToLowerInvariant();
            
            // Cash and bank accounts are always recommended for money transactions
            if (direction == TransactionDirection.MoneyOut || direction == TransactionDirection.MoneyIn)
            {
                return accountNameLower.Contains("cash") || accountNameLower.Contains("bank");
            }

            // For transfers, asset accounts are recommended
            if (direction == TransactionDirection.Transfer)
            {
                return account.Mx9Qw7Type == Mx9Qw7Type.Asset;
            }

            return false;
        }

        private (int DebitAccountId, int CreditAccountId) DetermineDebitCreditAccounts(
            Qw8Rt5Entity selectedAccount,
            Qw8Rt5Entity contraAccount,
            TransactionDirection direction)
        {
            if (selectedAccount == null || contraAccount == null)
                throw new ArgumentException("Both selected account and contra account must be provided.");

            return direction switch
            {
                TransactionDirection.MoneyOut => 
                    // Money going out: Increase selected account (debit), decrease cash/bank (credit)
                    (selectedAccount.AccountId, contraAccount.AccountId),
                
                TransactionDirection.MoneyIn => 
                    // Money coming in: Increase cash/bank (debit), increase/decrease selected account (credit)
                    (contraAccount.AccountId, selectedAccount.AccountId),
                
                TransactionDirection.Transfer => 
                    // Transfer: Increase selected account (debit), decrease contra account (credit)
                    (selectedAccount.AccountId, contraAccount.AccountId),
                
                _ => throw new ArgumentException($"Unknown transaction direction: {direction}")
            };
        }
    }

    // Supporting classes and enums
    public enum TransactionDirection
    {
        MoneyOut,   // Expenses, purchases, payments going out
        MoneyIn,    // Income, sales, receipts coming in  
        Transfer    // Moving money between accounts
    }

    public class TransactionMappingResult
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }
        public int DebitAccountId { get; set; }
        public int CreditAccountId { get; set; }
        public Qw8Rt5Entity? PrimaryAccount { get; set; }
        public Qw8Rt5Entity? ContraAccount { get; set; }
        public TransactionDirection Direction { get; set; }
        public decimal Amount { get; set; }
        public string Explanation { get; set; } = string.Empty;
    }

    public class AvailableContraAccount
    {
        public int AccountId { get; set; }
        public string AccountName { get; set; } = string.Empty;
        public Mx9Qw7Type AccountType { get; set; }
        public int Priority { get; set; }
        public bool IsRecommended { get; set; }
    }
}