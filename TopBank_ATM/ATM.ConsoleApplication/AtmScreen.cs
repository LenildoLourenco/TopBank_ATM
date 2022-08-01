using ATM.ConsoleApplication.ViewModels;
using ATM.ConsoleApplicationLib;
using ATM.Domain.Entities;
using System;

namespace ATM.ConsoleApplication
{
    internal class AtmScreen
    {

        internal const string cur = "RS ";

        internal static void WelcomeATM()
        {
            Console.Clear();
            Console.Title = "Sistema TopBank ATM.";
            Console.WriteLine("Bem vindo(a) ao TopBank ATM.\n");
            Console.WriteLine("Por favor, insira o seu cartão ATM.");
            Utility.PrintEnterMessage();
        }

        internal static void WelcomeCustomer(string fullName)
        {
            Utility.PrintConsoleWriteLine("Bem vindo(a), " + fullName);
        }


        internal static void PrintLockAccount()
        {
            Console.Clear();
            Utility.PrintMessage("Sua conta está bloqueada. Por favor vá a " +
                " agência mais próxima para desbloquear sua conta. Obrigada.", true);

            Utility.PrintEnterMessage();
            Environment.Exit(1);
        }

        internal UserBankAccount LoginForm()
        {
            var vmUserBankAccount = new UserBankAccount();

            vmUserBankAccount.CardNumber = Validator.Convert<long>("o número do cartão");

            vmUserBankAccount.CardPin = Convert.ToInt32(Utility.GetHiddenConsoleInput("Digite a senha do cartão: "));

            return vmUserBankAccount;
        }

        internal static void LoginProgress()
        {
            Console.Write("\nVerificando o número e a senha do cartão.");
            Utility.printDotAnimation();
            Console.Clear();
        }

        internal static void LogoutProgress()
        {
            Console.WriteLine("Obrigado por usar o sistema TopBank ATM.");
            Utility.printDotAnimation();
            Console.Clear();
        }


        internal static void DisplaySecureMenu()
        {
            Console.Clear();
            Console.WriteLine(" ---------------------------");
            Console.WriteLine("| TopBank ATM Menu Seguro   |");
            Console.WriteLine("|                           |");
            Console.WriteLine("| 1. Consulta de Saldo      |");
            Console.WriteLine("| 2. Depósito em Dinheiro   |");
            Console.WriteLine("| 3. Saque                  |");
            Console.WriteLine("| 4. Transferência          |");
            Console.WriteLine("| 5. Transações             |");
            Console.WriteLine("| 6. Sair                   |");
            Console.WriteLine("|                           |");
            Console.WriteLine(" ---------------------------");

        }

        internal static void PrintCheckBalanceScreen()
        {
            Console.Write("Saldo: ");
        }

        internal VMThirdPartyTransfer ThirdPartyTransferForm()
        {
            var vMThirdPartyTransfer = new VMThirdPartyTransfer();

            vMThirdPartyTransfer.RecipientBankAccountNumber = Validator.Convert<long>("número da conta do destinatário");
            
            vMThirdPartyTransfer.TransferAmount = Validator.Convert<decimal>($"valor {cur}");

            vMThirdPartyTransfer.RecipientBankAccountName = Utility.GetRawInput("nome do destinatário");

            return vMThirdPartyTransfer;
        }
    }
}



