using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserApp
{
    public class UserManager
    {
        private Dictionary<string, (string password, decimal balance)> _users = new Dictionary<string, (string, decimal)>();

        
        public void RegisterUser(string username, string password, decimal initialBalance = 0)
        {
            _users[username] = (password, initialBalance);
        }

       
        public bool IsUserValid(string username, string password)
        {
            if (_users.ContainsKey(username))
            {
                return _users[username].password == password;
            }
            return false;
        }

        
        public decimal GetUserBalance(string username)
        {
            if (_users.ContainsKey(username))
            {
                return _users[username].balance;
            }
            throw new ArgumentException("User does not exist", nameof(username));
        }

        
        public void AddFunds(string username, decimal amount)
        {
            if (amount < 0)
            {
                throw new ArgumentException("Amount cannot be negative", nameof(amount));
            }

            if (_users.ContainsKey(username))
            {
                _users[username] = (_users[username].password, _users[username].balance + amount);
            }
            else
            {
                throw new ArgumentException("User does not exist", nameof(username));
            }
        }

        
        public void WithdrawFunds(string username, decimal amount)
        {
            if (amount < 0)
            {
                throw new ArgumentException("Amount cannot be negative", nameof(amount));
            }

            if (_users.ContainsKey(username))
            {
                if (_users[username].balance >= amount)
                {
                    _users[username] = (_users[username].password, _users[username].balance - amount);
                }
                else
                {
                    throw new InvalidOperationException("Insufficient funds");
                }
            }
            else
            {
                throw new ArgumentException("User does not exist", nameof(username));
            }
        }

        
        public void DeleteUser(string username)
        {
            if (_users.ContainsKey(username))
            {
                _users.Remove(username);
            }
            else
            {
                throw new ArgumentException("User does not exist", nameof(username));
            }
        }

        public void ChangePassword(string username, string newPassword)
        {
            if (_users.ContainsKey(username))
            {
                _users[username] = (newPassword, _users[username].balance);
            }
            else
            {
                throw new ArgumentException("User does not exist", nameof(username));
            }
        }

        public decimal GetTotalBalance()
        {
            return _users.Sum(u => u.Value.balance);
        }

        public void TransferFunds(string senderUsername, string recipientUsername, decimal amount)
        {
            if (_users.ContainsKey(senderUsername) && _users.ContainsKey(recipientUsername))
            {
                if (_users[senderUsername].balance >= amount)
                {
                    _users[senderUsername] = (_users[senderUsername].password, _users[senderUsername].balance - amount);
                    _users[recipientUsername] = (_users[recipientUsername].password, _users[recipientUsername].balance + amount);
                }
                else
                {
                    throw new InvalidOperationException("Insufficient funds");
                }
            }
            else
            {
                throw new ArgumentException("One or both users do not exist", nameof(senderUsername));
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            
        }
    }
}
