namespace ATM.ConsoleApplication.ViewModels
{
    public class VMThirdPartyTransfer
    {
        public decimal TransferAmount { get; set; }
        public long RecipientBankAccountNumber { get; set; }
        public string RecipientBankAccountName { get; set; }
    }
}

