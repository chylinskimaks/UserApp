using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using UserApp;

namespace UserManagerTests
{
    [TestFixture]
    public class UserManagerTests
    {
        private UserManager userManager;
        private static string xmlPath = "C:\\Users\\Trimlogic\\source\\repos\\UserApp\\UserAppTests\\Data\\testCases.xml";

        [SetUp]
        public void Setup()
        {
            userManager = new UserManager();
        }

        private static IEnumerable<TestCaseData> ReadXmlData()
        {
            XDocument doc = XDocument.Load(xmlPath);
            foreach (var el in doc.Descendants("User"))
            {
                string username = el.Element("Username")?.Value;
                string password = el.Element("Password")?.Value;
                bool isValid = bool.Parse(el.Element("IsValid")?.Value);
                var testCaseData = new TestCaseData(username, password, isValid)
                    .SetName($"IsUserValid_{username}")
                    .SetCategory("XMLUsers"); 
                yield return testCaseData;
            }
        }

        [Test, TestCaseSource(nameof(ReadXmlData))]
        public void IsUserValid_FromXmlData_VerifiesUserValidity(string username, string password, bool expected)
        {
            
            if (expected)
            {
                userManager.RegisterUser(username, password);
            }

            
            Assert.That(userManager.IsUserValid(username, password), Is.EqualTo(expected), $"Expected user validation for '{username}' to be {expected}");
        }

        [Test, Category("AddFunds")]
        public void AddFunds_ValidUser_IncreasesBalance()
        {
            userManager.RegisterUser("user3", "password123", 100);
            userManager.AddFunds("user3", 50);
            Assert.That(userManager.GetUserBalance("user3"), Is.EqualTo(150));
        }

        [Test, Category("AddFunds")]
        public void AddFunds_NegativeAmount_ThrowsArgumentException()
        {
            userManager.RegisterUser("user4", "password123", 100);
            Assert.Throws<ArgumentException>(() => userManager.AddFunds("user4", -50));
        }

        [Test, Category("WithdrawFunds")]
        public void WithdrawFunds_ValidUserWithSufficientBalance_DecreasesBalance()
        {
            userManager.RegisterUser("user5", "password123", 200);
            userManager.WithdrawFunds("user5", 150);
            Assert.That(userManager.GetUserBalance("user5"), Is.EqualTo(50));
        }

        [Test, Category("WithdrawFunds")]
        public void WithdrawFunds_ValidUserWithInsufficientBalance_ThrowsInvalidOperationException()
        {
            userManager.RegisterUser("user6", "password123", 50);
            Assert.Throws<InvalidOperationException>(() => userManager.WithdrawFunds("user6", 100));
        }

        [Test, Category("DeleteUser")]
        public void DeleteUser_ExistingUser_UserIsRemoved()
        {
            userManager.RegisterUser("user7", "password123");
            userManager.DeleteUser("user7");
            Assert.That(() => userManager.GetUserBalance("user7"), Throws.TypeOf<ArgumentException>());
        }

        [Test, Category("DeleteUser")]
        public void DeleteUser_NonExistingUser_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => userManager.DeleteUser("nonexistentUser"));
        }

        [Test, Category("ChangePassword")]
        public void ChangePassword_ExistingUser_UpdatesPassword()
        {
            userManager.RegisterUser("user8", "oldPassword");
            userManager.ChangePassword("user8", "newPassword");
            Assert.That(userManager.IsUserValid("user8", "newPassword"), Is.True);
        }

        [Test, Category("ChangePassword")]
        public void ChangePassword_NonExistingUser_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => userManager.ChangePassword("nonexistentUser", "newPassword"));
        }

        [Test, Category("GetTotalBalance")]
        public void GetTotalBalance_MultipleUsers_ReturnsCorrectSum()
        {
            userManager.RegisterUser("user9", "password123", 300);
            userManager.RegisterUser("user10", "password456", 500);
            Assert.That(userManager.GetTotalBalance(), Is.EqualTo(800));
        }

        [Test, Category("TransferFunds")]
        public void TransferFunds_ValidTransfer_UpdatesBothBalances()
        {
            userManager.RegisterUser("user11", "password123", 500);
            userManager.RegisterUser("user12", "password456", 100);
            userManager.TransferFunds("user11", "user12", 200);
            Assert.That(userManager.GetUserBalance("user11"), Is.EqualTo(300));
            Assert.That(userManager.GetUserBalance("user12"), Is.EqualTo(300));
        }

        [Test, Category("TransferFunds")]
        public void TransferFunds_InsufficientFunds_ThrowsInvalidOperationException()
        {
            userManager.RegisterUser("user13", "password789", 100);
            userManager.RegisterUser("user14", "password101112", 50);
            Assert.Throws<InvalidOperationException>(() => userManager.TransferFunds("user13", "user14", 150));
        }
    }
}