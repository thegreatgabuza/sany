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

            return contraAccounts.Select(account => new AvailableContraAccount
            {
                AccountId = account.AccountId,
                AccountName = account.AccountName,
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
            return direction switch
            {
                TransactionDirection.MoneyOut => 
                    $"R{amount:N2} flows FROM {contraAccount.AccountName} TO {primaryAccount.AccountName}",
                TransactionDirection.MoneyIn => 
                    $"R{amount:N2} flows FROM {primaryAccount.AccountName} TO {contraAccount.AccountName}",
                TransactionDirection.Transfer => 
                    $"R{amount:N2} transfers FROM {contraAccount.AccountName} TO {primaryAccount.AccountName}",
                _ => $"R{amount:N2} transaction between {primaryAccount.AccountName} and {contraAccount.AccountName}"
            };
        }

        private async Task<Qw8Rt5Entity?> GetBestContraAccountAsync(
            Qw8Rt5Entity selectedAccount,
            TransactionDirection direction,
            int companyId)
        {
            var possibleAccounts = await GetPossibleContraAccountsAsync(selectedAccount, direction, companyId);
            
            if (!possibleAccounts.Any())
                return null;

            // Return the highest priority contra account
            return possibleAccounts
                .OrderByDescending(a => GetContraAccountPriority(a, direction))
                .First();
        }

        private async Task<List<Qw8Rt5Entity>> GetPossibleContraAccountsAsync(
            Qw8Rt5Entity selectedAccount,
            TransactionDirection direction,
            int companyId)
        {
            var allAccounts = await _context.DataStreams
                .Where(a => a.CompanyId == companyId && a.AccountId != selectedAccount.AccountId)
                .ToListAsync();

            var possibleAccounts = direction switch
            {
                TransactionDirection.MoneyOut or TransactionDirection.MoneyIn =>
                    // For money transactions, prefer cash/bank accounts
                    allAccounts.Where(a => 
                        a.Mx9Qw7Type == Mx9Qw7Type.Asset && 
                        (a.AccountName.ToLower().Contains("cash") || 
                         a.AccountName.ToLower().Contains("bank") ||
                         a.AccountName.ToLower().Contains("petty cash"))
                    ).ToList(),
                
                TransactionDirection.Transfer =>
                    // For transfers, allow any account except the selected one
                    allAccounts.Where(a => IsValidTransferAccount(a, selectedAccount)).ToList(),
                
                _ => new List<Qw8Rt5Entity>()
            };

            // If no specific accounts found for money transactions, fall back to all asset accounts
            if ((direction == TransactionDirection.MoneyOut || direction == TransactionDirection.MoneyIn) && 
                !possibleAccounts.Any())
            {
                possibleAccounts = allAccounts.Where(a => a.Mx9Qw7Type == Mx9Qw7Type.Asset).ToList();
            }

            return possibleAccounts;
        }

        private bool IsValidTransferAccount(Qw8Rt5Entity candidateAccount, Qw8Rt5Entity selectedAccount)
        {
            // Allow transfers between:
            // - Asset accounts (cash, bank, equipment)
            // - Same type accounts (asset to asset, etc.)
            // - Logical business transfers

            if (candidateAccount.Mx9Qw7Type == Mx9Qw7Type.Asset && selectedAccount.Mx9Qw7Type == Mx9Qw7Type.Asset)
                return true;

            // Allow some cross-type transfers that make business sense
            if (selectedAccount.Mx9Qw7Type == Mx9Qw7Type.Liability && candidateAccount.Mx9Qw7Type == Mx9Qw7Type.Asset)
                return true; // Paying off liability

            return false;
        }

        private int GetContraAccountPriority(Qw8Rt5Entity account, TransactionDirection direction)
        {
            var accountNameLower = account.AccountName.ToLower();
            
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
            var accountNameLower = account.AccountName.ToLower();
            
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