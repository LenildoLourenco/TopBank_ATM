namespace ATM.Domain.Interface
{
    public interface IUserBankAccount
    {
        void CheckBalance();
        void PlaceDeposit();
        void MakeWithdrawal();
    }
}
