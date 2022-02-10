using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

// ============================================================================
// (c) Sandy Bultena 2018
// * Released under the GNU General Public License
// ============================================================================

namespace Budget
{

    /// <summary>
    /// Utility class which manages the files in a budget project. Has functionality to verify that a given file is readable and/or writeable. 
    /// </summary>
    /// <seealso cref="Expenses"/>
    /// <seealso cref="Categories"/>
    /// <seealso cref="HomeBudget"/>
    public class BudgetFiles
    {
        private static String DefaultSavePath = @"Budget\";
        private static String DefaultAppData = @"%USERPROFILE%\AppData\Local\";

        // ====================================================================
        // verify that the name of the file, or set the default file, and 
        // is it readable?
        // throws System.IO.FileNotFoundException if file does not exist
        // ====================================================================
        /// <summary>
        /// Verifies that the passed filepath exists and can be read. If no file is specified, the method verifies a passed default file name instead.
        /// </summary>
        /// <param name="FilePath">The file to be verified as readable.</param>
        /// <param name="DefaultFileName">A default file to be checked in case no file is specified.</param>
        /// <returns>The verified filepath.</returns>
        /// <exception cref="FileNotFoundException">Thrown if the passed file does not exist.</exception>
        /// <example>
        /// In this example, the method attempts to verify that the expenses file can be read. A try-catch block is used to catch any exceptions that might be thrown.
        /// <code>
        ///     String expensesFile = "./test_expenses.exps";
        ///     try
        ///     {
        ///         String readableFile = BudgetFiles.VerifyReadFromFileName(expensesFile, "");
        ///         Console.WriteLine(readableFile + " exists and is readable.");
        ///     }
        ///     catch(Exception e)
        ///     {
        ///         Console.WriteLine("ERROR READING FILE: " + e.Message);
        ///     }
        /// </code>
        /// </example>
        public static String VerifyReadFromFileName(String FilePath, String DefaultFileName)
        {

            // ---------------------------------------------------------------
            // if file path is not defined, use the default one in AppData
            // ---------------------------------------------------------------
            if (FilePath == null)
            {
                FilePath = Environment.ExpandEnvironmentVariables(DefaultAppData + DefaultSavePath + DefaultFileName);
            }

            // ---------------------------------------------------------------
            // does FilePath exist?
            // ---------------------------------------------------------------
            if (!File.Exists(FilePath))
            {
                throw new FileNotFoundException("ReadFromFileException: FilePath (" + FilePath + ") does not exist");
            }

            // ----------------------------------------------------------------
            // valid path
            // ----------------------------------------------------------------
            return FilePath;

        }

        // ====================================================================
        // verify that the name of the file, or set the default file, and 
        // is it writable
        // ====================================================================
        /// <summary>
        /// Verifies that a passed filepath exists and is writeable. If no file is specified, the method creates and verifies a default filepath instead.
        /// </summary>
        /// <param name="FilePath">A file to be verified as writeable.</param>
        /// <param name="DefaultFileName">A default file name to be used if no filepath was specified.</param>
        /// <returns>The verified filepath.</returns>
        /// <exception cref="Exception">Thrown when directory of passed filepath does not exist, or when passed file is read-only.</exception>
        /// <example>
        /// In this example, the method attempts to verify that the expenses file is writeable. A try-catch block is used to handle any exceptions which may be thrown.
        /// <code>
        ///     String expensesFile = "./test_expenses.exps";
        ///     try
        ///     {
        ///         String writeableFile = BudgetFiles.VerifyWriteFromFileName(expensesFile, "");
        ///         Console.WriteLine(writeableFile + " exists and is writeable.");
        ///     }
        ///     catch(Exception e)
        ///     {
        ///         Console.WriteLine("ERROR READING FILE: " + e.Message);
        ///     }
        /// </code>
        /// </example>
        public static String VerifyWriteToFileName(String FilePath, String DefaultFileName)
        {
            // ---------------------------------------------------------------
            // if the directory for the path was not specified, then use standard application data
            // directory
            // ---------------------------------------------------------------
            if (FilePath == null)
            {
                // create the default appdata directory if it does not already exist
                String tmp = Environment.ExpandEnvironmentVariables(DefaultAppData);
                if (!Directory.Exists(tmp))
                {
                    Directory.CreateDirectory(tmp);
                }

                // create the default Budget directory in the appdirectory if it does not already exist
                tmp = Environment.ExpandEnvironmentVariables(DefaultAppData + DefaultSavePath);
                if (!Directory.Exists(tmp))
                {
                    Directory.CreateDirectory(tmp);
                }

                FilePath = Environment.ExpandEnvironmentVariables(DefaultAppData + DefaultSavePath + DefaultFileName);
            }

            // ---------------------------------------------------------------
            // does directory where you want to save the file exist?
            // ... this is possible if the user is specifying the file path
            // ---------------------------------------------------------------
            String folder = Path.GetDirectoryName(FilePath);
            String delme = Path.GetFullPath(FilePath);
            if (!Directory.Exists(folder))
            {
                throw new Exception("SaveToFileException: FilePath (" + FilePath + ") does not exist");
            }

            // ---------------------------------------------------------------
            // can we write to it?
            // ---------------------------------------------------------------
            if (File.Exists(FilePath))
            {
                FileAttributes fileAttr = File.GetAttributes(FilePath);
                if ((fileAttr & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                {
                    throw new Exception("SaveToFileException:  FilePath(" + FilePath + ") is read only");
                }
            }

            // ---------------------------------------------------------------
            // valid file path
            // ---------------------------------------------------------------
            return FilePath;

        }



    }
}
