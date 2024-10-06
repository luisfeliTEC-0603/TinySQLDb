using Entities;
using System.IO.Compression;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System;
using System.Data;
using System.Diagnostics;
using System.Collections.Generic;
using NCalc;
using BST;

namespace StoreDataManager
{
        // CREAT DATABSE <dataBase-name> READY
        // SET <dataBase-name> READY
        // CREAT TABLE <table-name> ( arg, ) READY
        // DROP TABLE <table-name> READY
        // SELECT ( *|<columns> ) FROM <table-name> [WHERE ( arg ) ORDER BY ( ASC/DEC )] READY
        // UPDATE <table-name> SET <column-name> = <new-value> [WHERE ( arg )];
        // DELETE FROM <table-name> [WHERE ( arg )] READY 
        // INSERT INTO <table-name> VALUES ( arg1, agr2, ... ) READY
        // INDEX ???
    public sealed class Store
    {
        private static Store? instance = null;
        private static readonly object _lock = new object();
               
        public static Store GetInstance()
        {
            lock(_lock)
            {
                if (instance == null) 
                {
                    instance = new Store();
                }
                return instance;
            }
        }

        private readonly string baseDirectory;
        private readonly string DatabaseBasePath;
        private readonly string DataPath;
        private readonly string SystemCatalogPath;
        private readonly string SystemDatabasesFile;
        private readonly string SystemTablesFile;     
        private string CurrentPath;   
        
        public Store()
        {
            baseDirectory = Directory.GetCurrentDirectory();
            DatabaseBasePath = Path.Combine(baseDirectory, "TinySQL");
            SystemCatalogPath = Path.Combine(DatabaseBasePath, "SystemCatalog");
            SystemDatabasesFile = Path.Combine(SystemCatalogPath, "SystemDatabases.table");
            SystemTablesFile = Path.Combine(SystemCatalogPath, "SystemTables.table");
            DataPath = $@"C:\TinySql\Data";

            this.InitializeSystemCatalog();
        }

        private void InitializeSystemCatalog()
        {
            // Always make sure that the system catalog and above folder
            // exist when initializing
            Directory.CreateDirectory(SystemCatalogPath);
        }

        public OperationStatus CreateDataBase(string dataBase)
        {
            // Creates a new directory in the specified path 
            Directory.CreateDirectory($@"{DataPath}\{dataBase}");
            
            return OperationStatus.Success;
        }
        
        public OperationStatus SetDataBase(string NewDirectory)
        {
            try
            {
                Directory.SetCurrentDirectory($@"{DataPath}\{NewDirectory}");   
                CurrentPath = $@"{DataPath}\{NewDirectory}";
                return OperationStatus.Success;
            }
            catch //In case the newdirectory does not exist
            {
                return OperationStatus.Error;
            }
        }

        public OperationStatus DropTable(string TableName)
        {
            try
            {
                File.Delete(TableName + ".Table");
                return OperationStatus.Success;
            }
            catch //The table to be eliminated didnt exist!
            {
                return OperationStatus.Error;
            }
        }
        public OperationStatus UpdateSentence(string tableName, string[] selectedColumns, string whereClause)
        {
            if (whereClause == null)
            {

            }
            else
            {

            }
            return OperationStatus.Success;
        }

        public OperationStatus DeleteFromTable(string DirectoryName, string whereClause) //whereCluse example "ID != 0 && Color = Azul"
        {
            var tablePath = $@"{CurrentPath}\{DirectoryName}.Table";

            BinarySearchTree bst = ConvertBinaryToBST(DirectoryName);
            List<int> RightPadsFormat = IndexStringAndPad(DirectoryName);
            List<string> ColumnsFormat = GetColumnsFormar(DirectoryName);

            string[] resultArray = whereClause.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var ClauseExpresion = new Expression(whereClause);

            for (int i = 0; i < resultArray.Length - 1; i = i + 4)
            {
                ClauseExpresion.Parameters[resultArray[i]] = resultArray[i]; //Adds the name of the parameter to be analize
            }

            bool expresion = (bool)ClauseExpresion.Evaluate(); //whereClause transformed into boolean expresion

            List<Nodo> NodosPorEliminar = bst.GetNodesThat(expresion , ColumnsFormat );

            for (int i = 0; i < NodosPorEliminar.Count() - 1; i++)
            {
                try
                {
                    int number = (int)NodosPorEliminar[i].GetAttribute(0);
                    bst.Delete(number);
                }
                catch
                {
                    continue;
                }
               
            } // at these point the nodes that made the experion true are eliminated now we need to erase the information above columns
            //Configration and rewrite the data
            CleanBinaryPath(DirectoryName); //It deletes all table information but the columns arragmentent.
            List<Nodo> NodesForWriting = bst.GetAllNodes();
            using (FileStream stream = File.Open(tablePath, FileMode.Append))
            using (BinaryWriter writer = new (stream))
            {
                for (int i = 0; i < NodesForWriting.Count(); i++)
                {   
                    for (int j = 0; j < RightPadsFormat.Count() - 1; j += 2 )
                    {
                        if (i == RightPadsFormat[j])
                        {
                            string StringToWrite = (string)NodosPorEliminar[i].GetAttribute(i);
                            writer.Write(StringToWrite.PadRight(RightPadsFormat[i + 1]));
                        }
                        else
                        {
                            try
                            {
                                int NumToWrite = (int)NodosPorEliminar[i].GetAttribute(i);
                                writer.Write(NumToWrite);
                            }
                            catch
                            {
                                DateTime Date = (DateTime)NodosPorEliminar[i].GetAttribute(i);
                                writer.Write(Date.Ticks);
                            }
                        }
                    }
                }
            }
 
            return OperationStatus.Success;
        }

        public OperationStatus CreateTable(string DirectoryName, string[] Sentence) 
        {
            if (CurrentPath != null)
            {
                // Creates a default Table with DirectoryName as name
                var tablePath = $@"{CurrentPath}\{DirectoryName}.Table";

                int SentenceLen = Sentence.Length;

                using (FileStream stream = File.Open(tablePath, FileMode.OpenOrCreate))
                using (BinaryWriter writer = new (stream))
                {
                    writer.Write(SentenceLen);
                    for (int i = 1; 1 < SentenceLen - 1 ; i = i + 2 )
                    {
                        if (Sentence[i].StartsWith("VARCHAR"))
                        {
                            int VarcharValue = GetVarcharSize(Sentence[i]);
                            writer.Write(VarcharValue);
                            writer.Write(Sentence[i-1].PadRight(VarcharValue)); //Writes the column name with RightPath that VARCHAR indicates
                        }
                        else if (Sentence[i] == "INTEGER") 
                        {
                            writer.Write(Sentence[i-1].PadRight(50));
                        }
                        else //DateTime column
                        {
                            writer.Write(Sentence[i-1].PadRight(50));
                        }
                    }     
                }
                return OperationStatus.Success;
            }
            else
            {
                //return new Exception ____missing execption for this case
                return OperationStatus.Error;
            }
        }

        public OperationStatus Insert(string directory, string[] data)
        {
            // Creates a default Table called ESTUDIANTES
            var tablePath = $@"{CurrentPath}\{directory}.Table";
            int ColumnLenght;
            int DataLenght = data.Length;
            using (BinaryReader reader = new BinaryReader(File.Open(tablePath, FileMode.Open)))
            {
                ColumnLenght = reader.ReadInt32();
            }
            if (ColumnLenght != DataLenght) //Data has more or less elements than columns on the table.
            {
                return OperationStatus.Error;
            }
            else
            {
                List<int> IndicatorsVarchar = GetRightPathValues(tablePath);
                int IndicatorLenght = IndicatorsVarchar.Count();
                using (FileStream stream = File.Open(tablePath, FileMode.Append))
                using (BinaryWriter writer = new (stream))
                {
                    for (int i = 0; i < DataLenght - 1; i++ )
                    {
                        for (int j = 0; j < IndicatorLenght - 1; j = j +2 )
                        {
                            if (j == IndicatorsVarchar[i])
                            {
                                writer.Write(data[i].PadRight(IndicatorsVarchar[i + 1])); //In the case that the data poisition is alligned with
                                //a position related to a PadRight
                            }
                        }
                        try //Data position is a integer
                        {
                            int ColumnData = int.Parse(data[i]);
                            writer.Write(ColumnData);
                        }
                        catch //Data position is a DateTime
                        {
                            DateTime date = DateTime.Parse(data[i]);
                            writer.Write(date.Ticks);
                        }
                    }

                }
                return OperationStatus.Success;
            }

        }

        public OperationStatus Select(string DirectoryName, string[] columnEntries, string whereClause, string orderClause)
        {
            var tablePath = $@"{CurrentPath}\{DirectoryName}.Table";
            BinarySearchTree bst = ConvertBinaryToBST(tablePath);
            List<int> RightPadsFormat = IndexStringAndPad(DirectoryName);
            List<string> ColumnsFormat = GetColumnsFormar(DirectoryName);

            List<Nodo> SelectedNodes = bst.GetAllNodes();

            try //All columns selected
            {
                columnEntries[0] = "*";
                if (whereClause == null)
                {
                    if (orderClause == null) //Just send all nodes from bst
                    {
                        PrintNodesForSelect(DirectoryName, SelectedNodes, ColumnsFormat.Count());
                        return OperationStatus.Success;
                    }

                    else //Print bst nodes in order
                    {
                        List<Nodo> Arranged = BinarySearchTree.ArrangeNodes(SelectedNodes, orderClause);
                        PrintNodesForSelect(DirectoryName, Arranged, ColumnsFormat.Count());
                        return OperationStatus.Success;
                    }
                }
                else
                {
                    string[] resultArray = whereClause.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    var ClauseExpresion = new Expression(whereClause);

                    for (int i = 0; i < resultArray.Length - 1; i = i + 4)
                    {
                        ClauseExpresion.Parameters[resultArray[i]] = resultArray[i]; //Adds the name of the parameter to be analize
                    }

                    bool expresion = (bool)ClauseExpresion.Evaluate(); //whereClause transformed into boolean expresion

                    if (orderClause == null) //Returns with no order all nodes that agree with whereClause
                    { 
                        List<Nodo> Arranged = bst.GetNodesThat(expresion, ColumnsFormat);
                        PrintNodesForSelect(DirectoryName, Arranged, ColumnsFormat.Count());
                        return OperationStatus.Success;
                    }

                    else //Returns in order all nodes that agree with whereClause
                    {
                        List<Nodo> ExpresionNodes = bst.GetNodesThat(expresion, ColumnsFormat);
                        List<Nodo> Ordered = BinarySearchTree.ArrangeNodes(ExpresionNodes, orderClause);
                        PrintNodesForSelect(DirectoryName, Ordered, ColumnsFormat.Count());
                        return OperationStatus.Success;
                    }
                }
            }
            catch
            {
                if (whereClause == null)
                {
                    if (orderClause == null) //Just send all nodes from bst
                    {
                        PrintNodesForSelect(DirectoryName, SelectedNodes, ColumnsFormat.Count());
                        return OperationStatus.Success;
                    }

                    else //Print bst nodes in order
                    {
                        List<Nodo> Arranged = BinarySearchTree.ArrangeNodes(SelectedNodes, orderClause);
                        PrintNodesForSelect(DirectoryName, Arranged, ColumnsFormat.Count());
                        return OperationStatus.Success;
                    }
                }
                else
                {                    
                    string[] resultArray = whereClause.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    var ClauseExpresion = new Expression(whereClause);

                    for (int i = 0; i < resultArray.Length - 1; i = i + 4)
                    {
                        ClauseExpresion.Parameters[resultArray[i]] = resultArray[i]; //Adds the name of the parameter to be analize
                    }

                    bool expresion = (bool)ClauseExpresion.Evaluate(); //whereClause transformed into boolean expresion

                    if (orderClause == null)
                    { 
                        List<Nodo> ExpresionNodes = bst.GetNodesThat(expresion, ColumnsFormat);
                        PrintNodesForSelect(DirectoryName, ExpresionNodes, ColumnsFormat.Count());
                        return OperationStatus.Success;
                    }

                    else
                    {
                        List<Nodo> ExpresionNodes = bst.GetNodesThat(expresion, ColumnsFormat);
                        List<Nodo> Arranged = BinarySearchTree.ArrangeNodes(ExpresionNodes, orderClause);
                        PrintNodesForSelect(DirectoryName, Arranged, ColumnsFormat.Count());
                        return OperationStatus.Success;
                    }
                }                
            }
        }

        public int GetVarcharSize(string varcharString)
        {
            int startIndex = varcharString.IndexOf('(');
            int endIndex = varcharString.IndexOf(')');

            if (startIndex != -1 && endIndex != -1 && endIndex > startIndex)
            {
                string numberString = varcharString.Substring(startIndex + 1, endIndex - startIndex - 1);
                if (int.TryParse(numberString, out int size))
                {
                    return size;
                }
            }
            
            throw new ArgumentException("El formato de la cadena no es válido.");
        }

        public List<int> GetRightPathValues(string path)
        {
            List<int> result = new List<int>();

            using (BinaryReader reader = new BinaryReader(File.Open(path, FileMode.Open)))
            {
                int ColLength = reader.ReadInt32();
                for (int i = 0; i < ColLength - 1; i++)
                {
                    try
                    {
                        int num = reader.ReadInt32();
                        result.Add(i);
                        result.Add(num); 
                        //The idea is to save the column number and the PadRight requested [ColumnNum, VarcharValue]
                    }
                    catch
                    {
                        continue;
                    }
                }

            }

            return result;
        }
        public List<int> IndexStringAndPad(string DirectoryName) //if string found on index 2 of a row it writes [2, 45] where 45 is the PadRight
        {
            List<int> VarcharValues = new List<int>();
            var tablePath = $@"{CurrentPath}\{DirectoryName}.Table";

            using (BinaryReader reader = new BinaryReader(File.Open(tablePath, FileMode.Open)))
            {
                int ColLength = reader.ReadInt32();
                for (int i = 0; i < ColLength -1; i++) // Returns a list with the names of the columns, so they can be compared with the rows.
                {
                    try
                    {
                        int VarcarNum = reader.ReadInt32();
                        string column = new string(reader.ReadChars(VarcarNum));
                        VarcharValues.Add(VarcarNum);
                    }
                    catch
                    {
                        string column = reader.ReadString();
                    }
                }
            }
            return VarcharValues;
        }
        public List<string> GetColumnsFormar(string DirectoryName) //Retrun a list with the names of columns
        {
            List<string> ColumnsFormat = new List<string>();
            var tablePath = $@"{CurrentPath}\{DirectoryName}.Table";
            
            using (BinaryReader reader = new BinaryReader(File.Open(tablePath, FileMode.Open)))
            {
                int ColLength = reader.ReadInt32();
                for (int i = 0; i < ColLength -1; i++) // Returns a list with the names of the columns, so they can be compared with the rows.
                {
                    try
                    {
                        int VarcarNum = reader.ReadInt32();
                        string column = new string(reader.ReadChars(VarcarNum));
                        ColumnsFormat.Add(column);
                    }
                    catch
                    {
                        string column = new string(reader.ReadChars(50)); //Default value for column names for Integers and DateTime.
                        ColumnsFormat.Add(column);
                    }
                }
            }
            return ColumnsFormat;
        }
        public BinarySearchTree ConvertBinaryToBST(string DirectoryName)
        {
            var tablePath = $@"{CurrentPath}\{DirectoryName}.Table";

            BinarySearchTree bst = new BinarySearchTree();

            using (BinaryReader reader = new BinaryReader(File.Open(tablePath, FileMode.Open)))
            {
                List<string> ColumnsFormat = new List<string>(); //Names of columns
                List<int> VarcharValues = new List<int>(); //Contains a list with position and length of binary data [2, 49, 3, 67]
                // that means that on index 2, there is a string with PadRight of 49.
                List<object> RowToAdd = new List<object>(); //row to be added to the BST node
                int ColLength = reader.ReadInt32();
                for (int i = 0; i < ColLength -1; i++) // Returns a list with the names of the columns, so they can be compared with the rows.
                {
                    try
                    {
                        int VarcarNum = reader.ReadInt32();
                        VarcharValues.Add(i);
                        VarcharValues.Add(VarcarNum);
                        string column = new string(reader.ReadChars(VarcarNum));
                        ColumnsFormat.Add(column);
                    }
                    catch
                    {
                        string column = new string(reader.ReadChars(50)); //Default value for column names for Integers and DateTime.
                        ColumnsFormat.Add(column);
                    }
                }

                int VarLenght = VarcharValues.Count(); //Large of the auxiliar list to chech if an element is a string so the PadRight can be
                //properly separated.
                while (reader.BaseStream.Position < reader.BaseStream.Length) //While we are not on the end of the binaryread
                {
                    int Value = reader.ReadInt32();
                    for (int i = 1; i < ColLength - 1; i++) // Check each element of a Binary "Cube" (row)
                    {
                        for (int j = 0; j < VarLenght; j += 2 )
                        {
                            if (i == VarcharValues[j])
                            {
                                int FixedLenght = VarcharValues[j + 1];
                                string data = new string(reader.ReadChars(FixedLenght));
                                RowToAdd.Add(data);
                            }
                            else
                            {
                                try 
                                {
                                    int data = reader.ReadInt32();
                                    RowToAdd.Add(data);
                                }
                                catch
                                {
                                    long ticks = reader.ReadInt64(); 
                                    DateTime date = new DateTime(ticks);
                                    RowToAdd.Add(date);
                                }
                            }
                        }
                    }
                    bst.Add(Value, RowToAdd);
                    RowToAdd.Clear();
                }
            }
            return bst;
        }

        public void CleanBinaryPath(string DirectoryName)
        {
            var tablePath = $@"{CurrentPath}\{DirectoryName}.Table";

            List<int> RightPadsFormat = IndexStringAndPad(DirectoryName);
            List<string> ColumnsFormat = GetColumnsFormar(DirectoryName);

            using (var memoryStream = new MemoryStream())
            using (var fileStream = new FileStream(tablePath, FileMode.Open, FileAccess.Read))
            using (var reader = new BinaryReader(fileStream))
            {
                int columnCount = reader.ReadInt32(); 

                // Rewind the stream to the beginning for writing
                memoryStream.Seek(0, SeekOrigin.Begin);
                using (var writer = new BinaryWriter(memoryStream))
                {
                    writer.Write(columnCount); // Write number of columns

                    // Write the names of the columns with their size
                    for (int i = 0; i < ColumnsFormat.Count() - 1; i++ )
                    {
                        for (int j = 0; j < RightPadsFormat.Count() - 1; j += 2 )
                        {
                            if (i == RightPadsFormat[j])
                            {
                                writer.Write(RightPadsFormat[i]);
                                writer.Write(ColumnsFormat[i].PadRight(RightPadsFormat[i + 1]));
                            }
                            else
                            {
                                writer.Write(ColumnsFormat[i].PadRight(50));
                            }
                        }
                    }
                }

                // Remplace new binary archive
                using (var outputStream = new FileStream(tablePath, FileMode.Create, FileAccess.Write))
                {
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    memoryStream.CopyTo(outputStream);
                }
            }
        }

        public void PrintNodesForSelect(string DirectoryName, List<Nodo> NodesListToPrint, int ColumnsLenght)
        {
            var tablePath = $@"{CurrentPath}\{DirectoryName}.Table";
            int ListLenght = NodesListToPrint.Count();
            using (FileStream stream = File.Open(tablePath, FileMode.OpenOrCreate))
            using (BinaryReader reader = new (stream))
            {
                for (int i = 0; i < ListLenght - 1; i++) //Tries for eahc node
                {
                    for (int j = 0; j < ColumnsLenght - 1; j++) //Writes
                    {
                        try
                        {
                            int num = (int)NodesListToPrint[i].GetAttribute(j);
                            Console.Write(num + " ");
                        }
                        catch
                        {
                            try
                            {
                                string Phrase = (string)NodesListToPrint[i].GetAttribute(j);
                                Console.WriteLine(Phrase + " ");
                            }
                            catch
                            {
                                try
                                {
                                    long ticks = reader.ReadInt64(); 
                                    DateTime date = new DateTime(ticks);
                                    Console.WriteLine(date + " ");
                                }
                                catch
                                {
                                    continue;
                                }
                            }
                        }
                    }
                    Console.WriteLine(""); //Leaves to next line so rows dont get mixed
                }        
            }
        }
    }
}
