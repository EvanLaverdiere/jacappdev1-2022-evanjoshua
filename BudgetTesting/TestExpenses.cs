using System;
using Xunit;
using System.IO;
using System.Collections.Generic;
using Budget;
using System.Data.SQLite;

namespace BudgetCodeTests
{
    [Collection("Sequential")]
    public class TestExpenses
    {
        int numberOfExpensesInFile = TestConstants.numberOfExpensesInFile;
        String testInputFile = TestConstants.testExpensesInputFile;
        int maxIDInExpenseFile = TestConstants.maxIDInExpenseFile;
        Expense firstExpenseInFile = new Expense(1, new DateTime(2021, 1, 10), 10, 12, "hat (on credit)");


        // ========================================================================

        [Fact]
        public void ExpensesObject_New()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String newDB = $"{folder}\\newDB.db";
            Database.newDatabase(newDB);
            SQLiteConnection connection = Database.dbConnection;

            // Act
            Expenses expenses = new Expenses(connection, true);

            // Assert 
            Assert.IsType<Expenses>(expenses);
        }

        // ========================================================================

        [Fact]
        public void ExpensesMethod_EmptyDatabase()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String newDB = $"{folder}\\newDB.db";
            Database.newDatabase(newDB);
            SQLiteConnection connection = Database.dbConnection;

            // Act
            Expenses expenses = new Expenses(connection, true);

            // Assert 
            int size = expenses.List().Count;
            Assert.Equal(0, size);
        }

        // ========================================================================

        //[Fact]
        //public void ExpensesMethod_ReadFromFile_NotExist_ThrowsException()
        //{
        //    // Arrange
        //    String badFile = "abc.txt";
        //    Expenses expenses = new Expenses();

        //    // Act and Assert
        //    Assert.Throws<System.IO.FileNotFoundException>(() => expenses.ReadFromFile(badFile));

        //}

        // ========================================================================

        [Fact]
        public void ExpensesMethod_ReadFromDatabase_ValidateCorrectDataWasRead()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String existingDB = $"{folder}\\{TestConstants.testDBInputFile}";
            Database.existingDatabase(existingDB);
            SQLiteConnection connection = Database.dbConnection;

            // Act
            Expenses expenses = new Expenses(connection);
            List<Expense> list = expenses.List();
            Expense firstExpense = list[0];

            // Assert
            Assert.Equal(numberOfExpensesInFile, list.Count);
            Assert.Equal(firstExpenseInFile.Id, firstExpense.Id);
            Assert.Equal(firstExpenseInFile.Description, firstExpense.Description);
        }

        // ========================================================================

        [Fact]
        public void ExpensesMethod_List_ReturnsListOfExpenses()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String newDB = $"{folder}\\{TestConstants.testDBInputFile}";
            Database.existingDatabase(newDB);
            SQLiteConnection connection = Database.dbConnection;
            Expenses expenses = new Expenses(connection);

            // Act
            List<Expense> list = expenses.List();

            // Assert
            Assert.Equal(numberOfExpensesInFile, list.Count);
        }

        // ========================================================================

        //[Fact]
        //public void ExpensesMethod_List_ModifyListDoesNotModifyExpensesInstance()
        //{
        //    // Arrange
        //    String dir = GetSolutionDir();
        //    Expenses expenses = new Expenses();
        //    expenses.ReadFromFile(dir + "\\" + testInputFile);
        //    List<Expense> list = expenses.List();

        //    // Act
        //    list[0].Amount = list[0].Amount + 21.03; 

        //    // Assert
        //    Assert.NotEqual(list[0].Amount, expenses.List()[0].Amount);

        //}

        // ========================================================================

        [Fact]
        public void ExpensesMethod_Add()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String goodDB = $"{folder}\\{TestConstants.testDBInputFile}";
            String messyDB = $"{folder}\\messy.db";
            System.IO.File.Copy(goodDB, messyDB, true);
            Database.existingDatabase(messyDB);
            SQLiteConnection connection = Database.dbConnection;
            Expenses expenses = new Expenses(connection);
            DateTime date = DateTime.Now;
            Double amount = 3.55;
            string description = "Starbucks";
            Category.CategoryType type = Category.CategoryType.Expense;

            // Act
            expenses.Add(date, (int)type, amount, description);
            List<Expense> expensesList = expenses.List();
            int sizeOfList = expenses.List().Count;

            // Assert
            Assert.Equal(numberOfExpensesInFile + 1, sizeOfList);
            Assert.Equal(description, expensesList[sizeOfList - 1].Description);
        }

        [Fact]
        public void ExpensesMethod_Update()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String goodDB = $"{folder}\\{TestConstants.testDBInputFile}";
            String messyDB = $"{folder}\\messy.db";
            System.IO.File.Copy(goodDB, messyDB, true);
            Database.existingDatabase(messyDB);
            SQLiteConnection connection = Database.dbConnection;
            Expenses expense = new Expenses(connection);
            DateTime date = DateTime.Now;
            String newDescription = "Tim Hortons";
            Double amount = 11.22;
            int id = 1;
            int category = (int)Category.CategoryType.Expense;

            // Act
            expense.UpdateProperties(id, date, category, amount, newDescription);
            List<Expense> expensesList = expense.List();

            // Assert 
            Assert.Equal(newDescription, expensesList[0].Description);
            Assert.Equal(amount, expensesList[0].Amount);
        }

        // ========================================================================

        //[Fact]
        //public void ExpensesMethod_Delete()
        //{
        //    // Arrange
        //    String dir = GetSolutionDir();
        //    Expenses expenses = new Expenses();
        //    expenses.ReadFromFile(dir + "\\" + testInputFile);
        //    int IdToDelete = 3;

        //    // Act
        //    expenses.Delete(IdToDelete);
        //    List<Expense> expensesList = expenses.List();
        //    int sizeOfList = expensesList.Count;

        //    // Assert
        //    Assert.Equal(numberOfExpensesInFile - 1, sizeOfList);
        //    Assert.False(expensesList.Exists(e => e.Id == IdToDelete), "correct expense item deleted");

        //}

        //// ========================================================================

        //[Fact]
        //public void ExpensesMethod_Delete_InvalidIDDoesntCrash()
        //{
        //    // Arrange
        //    String dir = GetSolutionDir();
        //    Expenses expenses = new Expenses();
        //    expenses.ReadFromFile(dir + "\\" + testInputFile);
        //    int IdToDelete = 1006;
        //    int sizeOfList = expenses.List().Count;

        //    // Act
        //    try
        //    {
        //        expenses.Delete(IdToDelete);
        //        Assert.Equal(sizeOfList, expenses.List().Count);
        //    }

        //    // Assert
        //    catch
        //    {
        //        Assert.True(false, "Invalid ID causes Delete to break");
        //    }
        //}


        //// ========================================================================

        //[Fact]
        //public void ExpenseMethod_WriteToFile()
        //{
        //    // Arrange
        //    String dir = GetSolutionDir();
        //    Expenses expenses = new Expenses();
        //    expenses.ReadFromFile(dir + "\\" + testInputFile);
        //    string fileName = TestConstants.ExpenseOutputTestFile;
        //    String outputFile = dir + "\\" + fileName;
        //    File.Delete(outputFile);

        //    // Act
        //    expenses.SaveToFile(outputFile);

        //    // Assert
        //    Assert.True(File.Exists(outputFile), "output file created");
        //    Assert.True(FileEquals(dir + "\\" + testInputFile, outputFile), "Input /output files are the same");
        //    String fileDir = Path.GetFullPath(Path.Combine(expenses.DirName, ".\\"));
        //    Assert.Equal(dir, fileDir);
        //    Assert.Equal(fileName, expenses.FileName);

        //    // Cleanup
        //    if (FileEquals(dir + "\\" + testInputFile, outputFile))
        //    {
        //        File.Delete(outputFile);
        //    }

        //}

        //// ========================================================================

        //[Fact]
        //public void ExpenseMethod_WriteToFile_VerifyNewExpenseWrittenCorrectly()
        //{
        //    // Arrange
        //    String dir = GetSolutionDir();
        //    Expenses expenses = new Expenses();
        //    expenses.ReadFromFile(dir + "\\" + testInputFile);
        //    string fileName = TestConstants.ExpenseOutputTestFile;
        //    String outputFile = dir + "\\" + fileName;
        //    File.Delete(outputFile);

        //    // Act
        //    expenses.Add(DateTime.Now, 14, 35.27, "McDonalds");
        //    List<Expense> listBeforeSaving = expenses.List();
        //    expenses.SaveToFile(outputFile);
        //    expenses.ReadFromFile(outputFile);
        //    List<Expense> listAfterSaving = expenses.List();

        //    Expense beforeSaving = listBeforeSaving[listBeforeSaving.Count - 1];
        //    Expense afterSaving = listAfterSaving.Find(e => e.Id == beforeSaving.Id);

        //    // Assert
        //    Assert.Equal(beforeSaving.Id, afterSaving.Id);
        //    Assert.Equal(beforeSaving.Category, afterSaving.Category);
        //    Assert.Equal(beforeSaving.Description, afterSaving.Description);
        //    Assert.Equal(beforeSaving.Amount, afterSaving.Amount);

        //}

        //// ========================================================================

        //[Fact]
        //public void ExpenseMethod_WriteToFile_WriteToLastFileWrittenToByDefault()
        //{
        //    // Arrange
        //    String dir = GetSolutionDir();
        //    Expenses expenses = new Expenses();
        //    expenses.ReadFromFile(dir + "\\" + testInputFile);
        //    string fileName = TestConstants.ExpenseOutputTestFile;
        //    String outputFile = dir + "\\" + fileName;
        //    File.Delete(outputFile);
        //    expenses.SaveToFile(outputFile); // output file is now last file that was written to.
        //    File.Delete(outputFile);  // Delete the file

        //    // Act
        //    expenses.SaveToFile(); // should write to same file as before

        //    // Assert
        //    Assert.True(File.Exists(outputFile), "output file created");
        //    String fileDir = Path.GetFullPath(Path.Combine(expenses.DirName, ".\\"));
        //    Assert.Equal(dir, fileDir);
        //    Assert.Equal(fileName, expenses.FileName);

        //    // Cleanup
        //    if (FileEquals(dir + "\\" + testInputFile, outputFile))
        //    {
        //        File.Delete(outputFile);
        //    }

        //}
    }
}

//// -------------------------------------------------------
//// helpful functions, ... they are not tests
//// -------------------------------------------------------

//        //private String GetSolutionDir() {

//            // this is valid for C# .Net Foundation (not for C# .Net Core)
//            return Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\"));
//        }

//        // source taken from: https://www.dotnetperls.com/file-equals

//        private bool FileEquals(string path1, string path2)
//        {
//            byte[] file1 = File.ReadAllBytes(path1);
//            byte[] file2 = File.ReadAllBytes(path2);
//            if (file1.Length == file2.Length)
//            {
//                for (int i = 0; i < file1.Length; i++)
//                {
//                    if (file1[i] != file2[i])
//                    {
//                        return false;
//                    }
//                }
//                return true;
//            }
//            return false;
//        }
//    }
//}