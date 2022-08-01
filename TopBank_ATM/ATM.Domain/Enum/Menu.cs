using System.ComponentModel;

namespace ATM.Domain.Enum
{
    public enum SecureMenu
    {

        [Description("Consultar Saldo")]
        CheckBalance = 1,

        [Description("Fazer Depósito")]
        PlaceDeposit = 2,

        [Description("Fazer Saque")]
        MakeWithdrawal = 3,

        [Description("Fazer Transferência")]
        ThirdPartyTransfer = 4,

        [Description("Transação")]
        ViewTransaction = 5,

        [Description("Sair")]
        Logout = 6
    }
}



