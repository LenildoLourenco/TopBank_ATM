using ATM.ConsoleApplication.ViewModels;
using ATM.ConsoleApplicationLib;
using ATM.Domain.Entities;
using ATM.Domain.Enum;
using ATM.Domain.Interface;
using ConsoleTables;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ATM.ConsoleApplication
{
    public class AtmApp : IATMApp, ITransaction, IUserBankAccount
    {
        private List<UserBankAccount> _accountList;
        private UserBankAccount selectedAccount;
        private const decimal minimum_kept_amt = 20;
        private List<Transaction> _listOfTransactions;
        private readonly AtmScreen screen;

        public AtmApp()
        {
            screen = new AtmScreen();
        }

        public void Initialization()
        {
            _accountList = new List<UserBankAccount>
            {
                new UserBankAccount() { Id=1, FullName = "José Teste", AccountNumber=333111, CardNumber = 123123, CardPin = 111111, AccountBalance = 20000.00m, IsLocked = false },
                new UserBankAccount() { Id=2, FullName = "Maria Teste", AccountNumber=111222, CardNumber = 456456, CardPin = 222222, AccountBalance = 7500.30m, IsLocked = false },
                new UserBankAccount() { Id=3, FullName = "João Teste", AccountNumber=888555, CardNumber = 789789, CardPin = 333333, AccountBalance = 2900.12m, IsLocked = false },
                new UserBankAccount() { Id=2, FullName = "Teste Teste", AccountNumber=222444, CardNumber = 343434, CardPin = 444444, AccountBalance = 1500.30m, IsLocked = false }
            };

            _listOfTransactions = new List<Transaction>();
        }

        public void Execute()
        {
            AtmScreen.WelcomeATM();

            CheckCardNoPassword();
            AtmScreen.WelcomeCustomer(selectedAccount.FullName);

            while (true)
            {
                AtmScreen.DisplaySecureMenu();
                ProcessMenuOption();
            }
        }

        public void CheckCardNoPassword()
        {
            bool isLoginPassed = false;

            while (isLoginPassed == false)
            {
                var inputAccount = screen.LoginForm();

                AtmScreen.LoginProgress();

                foreach (UserBankAccount account in _accountList)
                {
                    selectedAccount = account;
                    if (inputAccount.CardNumber.Equals(account.CardNumber))
                    {
                        selectedAccount.TotalLogin++;

                        if (inputAccount.CardPin.Equals(account.CardPin))
                        {
                            selectedAccount = account;
                            if (selectedAccount.IsLocked)
                            {
                                AtmScreen.PrintLockAccount();
                            }
                            else
                            {
                                selectedAccount.TotalLogin = 0;
                                isLoginPassed = true;
                                break;
                            }
                        }
                    }
                }

                if (isLoginPassed == false)
                {
                    Utility.PrintMessage("Número do cartão ou senha inválidos.", false);

                    selectedAccount.IsLocked = selectedAccount.TotalLogin == 3;
                    if (selectedAccount.IsLocked)
                        AtmScreen.PrintLockAccount();
                }

                Console.Clear();
            }
        }

        private void ProcessMenuOption()
        {
            switch (Validator.Convert<int>("a sua opção"))
            {
                case (int)SecureMenu.CheckBalance:
                    CheckBalance();
                    break;
                case (int)SecureMenu.PlaceDeposit:
                    PlaceDeposit();
                    break;
                case (int)SecureMenu.MakeWithdrawal:
                    MakeWithdrawal();
                    break;
                case (int)SecureMenu.ThirdPartyTransfer:

                    var vMThirdPartyTransfer = screen.ThirdPartyTransferForm();
                    PerformThirdPartyTransfer(vMThirdPartyTransfer);
                    break;
                case (int)SecureMenu.ViewTransaction:
                    ViewTransaction();
                    break;

                case (int)SecureMenu.Logout:
                    AtmScreen.LogoutProgress();
                    Utility.PrintConsoleWriteLine("Você saiu com sucesso. Por favor, retire o seu cartão ATM.");
                    ClearSession();
                    Execute();
                    break;
                default:
                    Utility.PrintMessage("Opção inválida.", false);

                    break;
            }
        }

        public void CheckBalance()
        {
            AtmScreen.PrintCheckBalanceScreen();
            Utility.PrintConsoleWriteLine(Utility.FormatAmount(selectedAccount.AccountBalance), false);
        }

        public void PlaceDeposit()
        {

            Utility.PrintConsoleWriteLine("\nNota: O sistema ATM real apenas\ndeixa você " +
            "colocar notas na máquina ATM física. \n");

            var transaction_amt = Validator.Convert<int>($"valor {AtmScreen.cur}");

            Utility.PrintUserInputLabel("\nVerifique e conte as notas.");
            Utility.printDotAnimation();
            Console.SetCursorPosition(0, Console.CursorTop - 3);
            Console.WriteLine("");

            if (transaction_amt <= 0)
            {
                Utility.PrintMessage("O valor precisa ser maior que zero. Tente novamente.", false);
                return;
            }

            if (transaction_amt % 10 != 0)
            {
                Utility.PrintMessage($"Digite o valor do depósito apenas multiplicando por 10. Tente novamente.", false);
                return;
            }

            if (PreviewBankNotesCount(transaction_amt) == false)
            {
                Utility.PrintMessage($"Você cancelou sua ação.", false);
                return;
            }
          
            InsertTransaction(selectedAccount.Id, TransactionType.Deposito, +
                transaction_amt, "");
            
            selectedAccount.AccountBalance = selectedAccount.AccountBalance + transaction_amt;

            Utility.PrintMessage($"Você depositou com sucesso {Utility.FormatAmount(transaction_amt)}. " +
                "Favor retirar o comprovante bancário. ", true);
        }

        public void MakeWithdrawal()
        {
            Console.WriteLine("\nNota: Para o sistema ATM real, o usuário pode ");
            Console.Write("escolher algum valor de retirada padrão ou valor personalizado. \n\n");

            var transaction_amt = Validator.Convert<int>($"valor {AtmScreen.cur}");

            if (transaction_amt <= 0)
            {
                Utility.PrintMessage("O valor precisa ser maior que zero. Tente novamente.", false);
                return;
            }

            if (transaction_amt % 10 != 0)
            {
                Utility.PrintMessage($"Digite o valor do depósito apenas multiplicando por 10. Tente novamente.", false);
                return;
            }
            
            if (transaction_amt > selectedAccount.AccountBalance)
            {
                Utility.PrintMessage($"A retirada falhou. Você não tem fundos suficientes para sacar {Utility.FormatAmount(transaction_amt)}", false);
                return;
            }

            if ((selectedAccount.AccountBalance - transaction_amt) < minimum_kept_amt)
            {
                Utility.PrintMessage($"A retirada falhou. Sua conta precisa ter o mínimo {Utility.FormatAmount(minimum_kept_amt)}", false);
                return;
            }
            
            InsertTransaction(selectedAccount.Id, TransactionType.Saque, +
                -transaction_amt, "");
            
            selectedAccount.AccountBalance = selectedAccount.AccountBalance - transaction_amt;

            Utility.PrintMessage("Por favor, retire o seu dinheiro. Você sacou com sucesso " +
                $"{Utility.FormatAmount(transaction_amt)}. Favor retirar seu comprovante bancário.", true);

        }

        public void PerformThirdPartyTransfer(VMThirdPartyTransfer vMThirdPartyTransfer)
        {
            if (vMThirdPartyTransfer.TransferAmount <= 0)
            {
                Utility.PrintMessage("O valor precisa ser maior que zero. Tente novamente.", false);
                return;
            }

            if (vMThirdPartyTransfer.TransferAmount > selectedAccount.AccountBalance)
            {
                Utility.PrintMessage("A retirada falhou. Você não tem fundo " +
                    $"suficiente para sacar {Utility.FormatAmount(vMThirdPartyTransfer.TransferAmount)}", false);
                return;
            }

            if (selectedAccount.AccountBalance - vMThirdPartyTransfer.TransferAmount < minimum_kept_amt)
            {
                Utility.PrintMessage($"A retirada falhou. Sua conta precisa ter " +
                    $"o minimo {Utility.FormatAmount(minimum_kept_amt)}", false);
                return;
            }
            
            var selectedBankAccountReceiver = (from b in _accountList
                                               where b.AccountNumber == vMThirdPartyTransfer.RecipientBankAccountNumber
                                               select b).FirstOrDefault();

            if (selectedBankAccountReceiver == null)
            {
                Utility.PrintMessage($"Falha na transferência. O número da conta bancária do destinatário é inválido.", false);
                return;
            }

            if (selectedBankAccountReceiver.FullName != vMThirdPartyTransfer.RecipientBankAccountName)
            {
                Utility.PrintMessage($"Falha na transferência. O nome do destinatário não corresponde.", false);
                return;
            }
         
            InsertTransaction(selectedAccount.Id, TransactionType.Transferencia, +
                -vMThirdPartyTransfer.TransferAmount, "Transferido " +
                $"para {selectedBankAccountReceiver.AccountNumber} ({selectedBankAccountReceiver.FullName})");
            
            selectedAccount.AccountBalance = selectedAccount.AccountBalance - vMThirdPartyTransfer.TransferAmount;

            InsertTransaction(selectedBankAccountReceiver.Id, TransactionType.Transferencia, +
                vMThirdPartyTransfer.TransferAmount, "Transferido " +
                $"de {selectedAccount.AccountNumber} ({selectedAccount.FullName})");
            
            selectedBankAccountReceiver.AccountBalance = selectedBankAccountReceiver.AccountBalance + vMThirdPartyTransfer.TransferAmount;

            Utility.PrintMessage("Transferido com sucesso " +
                $" {Utility.FormatAmount(vMThirdPartyTransfer.TransferAmount)} para {vMThirdPartyTransfer.RecipientBankAccountName}", true);
        }

        private bool PreviewBankNotesCount(decimal amount)
        {
            int hundredNotesCount = (int)amount / 100;
            int fiftyNotesCount = ((int)amount % 100) / 50;
            int tenNotesCount = ((int)amount % 50) / 10;

            Utility.PrintUserInputLabel("\nResumo                                                  ", true);
            Utility.PrintUserInputLabel("-------", true);
            Utility.PrintUserInputLabel($"{AtmScreen.cur} 100 x {hundredNotesCount} = {100 * hundredNotesCount}", true);
            Utility.PrintUserInputLabel($"{AtmScreen.cur} 50 x {fiftyNotesCount} = {50 * fiftyNotesCount}", true);
            Utility.PrintUserInputLabel($"{AtmScreen.cur} 10 x {tenNotesCount} = {10 * tenNotesCount}", true);
            Utility.PrintUserInputLabel($"Valor Total: {Utility.FormatAmount(amount)}\n\n", true);

            char opt = Validator.Convert<char>("1 para confirmar");
            return opt.Equals('1');
        }

        public void ViewTransaction()
        {
            var filteredTransactionList = _listOfTransactions.Where(t => t.UserBankAccountId == selectedAccount.Id).ToList();

            if (filteredTransactionList.Count <= 0)
                Utility.PrintMessage($"Ainda não há transação.", true);
            else
            {
                var table = new ConsoleTable("Cod", "Data da transação", "Tipo", "Descrição", "Valor " + AtmScreen.cur);

                foreach (var tran in filteredTransactionList)
                {
                    table.AddRow(tran.TransactionId, tran.TransactionDate, tran.TransactionType, tran.Description, tran.TransactionAmount);
                }
                table.Options.EnableCount = false;
                table.Write();
                Utility.PrintMessage($"Você tem {filteredTransactionList.Count} transação(s).", true);
            }
        }

        public void InsertTransaction(long _UserBankAccountId, TransactionType _tranType, decimal _tranAmount, string _desc)
        {
            var transaction = new Transaction()
            {
                TransactionId = Utility.GetTransactionId(),
                UserBankAccountId = _UserBankAccountId,
                TransactionDate = DateTime.Now,
                TransactionType = _tranType,
                TransactionAmount = _tranAmount,
                Description = _desc
            };

            _listOfTransactions.Add(transaction);
        }

        private void ClearSession()
        {
            
        }

    }
}


