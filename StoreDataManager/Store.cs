using Entities;
using System.IO.Compression;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System;
using System.Data;
using System.Diagnostics;
using System.Text.RegularExpressions;
using NCalc;
using BinaryTableManager;
using BST;

namespace StoreDataManager
{
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

        private readonly string BaseDirectory;
        private readonly string DataPath;
        private readonly string DatabaseBasePath;
        private readonly string SystemCatalogPath;
        private readonly string SystemDatabasesFile;
        private readonly string SystemTablesFile;
        private readonly string SystemColumnsFile;
        private readonly string SystemIndexesFile;
        private string CurrentPath = null;
        
        public Store()
        {
            // Base directory of the project
            BaseDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).FullName;

            // Path where all databases and the system catalog are stored
            DatabaseBasePath = Path.Combine(BaseDirectory, "TinySQL");

            // Path to the shared SystemCatalog folder
            SystemCatalogPath = Path.Combine(DatabaseBasePath, "SystemCatalog");

            // Paths for each system catalog file
            SystemDatabasesFile = Path.Combine(SystemCatalogPath, "SystemDatabases.Table");
            SystemTablesFile = Path.Combine(SystemCatalogPath, "SystemTables.Table");
            SystemColumnsFile = Path.Combine(SystemCatalogPath, "SystemColumns.Table");
            SystemIndexesFile = Path.Combine(SystemCatalogPath, "SystemIndexes.Table");

            this.InitializeSystemCatalog();
        }

        private void InitializeSystemCatalog()
        {
            Directory.CreateDirectory(SystemCatalogPath);

            using (FileStream stream = File.Open(SystemDatabasesFile, FileMode.OpenOrCreate)){}
            using (FileStream stream = File.Open(SystemTablesFile, FileMode.OpenOrCreate)){}
            using (FileStream stream = File.Open(SystemColumnsFile, FileMode.OpenOrCreate)){}
            using (FileStream stream = File.Open(SystemIndexesFile, FileMode.OpenOrCreate)){}
        }

        public OperationStatus CreateDataBase(string dataBase)
        {
            try 
            {
                // Creates a new directory in the TinySQL path
                Directory.CreateDirectory($@"{DatabaseBasePath}\{dataBase}");

                // Server verificacion
                Console.WriteLine($"\nDATABSE CREATED IN DIRECTION : {DatabaseBasePath}/{dataBase}");

                // Sets newly created database as path
                return SetDataBase(dataBase);
            }
            catch
            {
                return OperationStatus.Error;
            }
        }
        
        public OperationStatus SetDataBase(string newDirectory)
        {
            try
            {
                // Setting Directory
                Directory.SetCurrentDirectory($@"{DatabaseBasePath}\{newDirectory}");   
                CurrentPath = $@"{DatabaseBasePath}\{newDirectory}";

                // Server verification
                Console.WriteLine($"\nDATABSE SET IN : {CurrentPath}");

                return OperationStatus.Success;
            }
            catch 
            {
                return OperationStatus.Error;
            }
        }

        public OperationStatus DropTable(string tableName)
        {
            try
            {
                // Deletes a table in the specified path
                File.Delete(tableName + ".Table");

                // Server verification
                Console.WriteLine($"\nFILE {tableName}.Table in {CurrentPath} WAS DELETED");

                return OperationStatus.Success;
            }
            catch
            {
                return OperationStatus.Error;
            }
        }
        
        public OperationStatus CreateTable(string DirectoryName, string[] columnsAndTypes) 
        {
            string[] columnNames = new string[columnsAndTypes.Length];
            ColumnType[] dataTypes = new ColumnType[columnsAndTypes.Length];
            int[] varcharSizes = new int[columnsAndTypes.Length];

            // Ensure that the database path is set before proceeding
            if (CurrentPath == null)
            {
                // Server verification
                Console.WriteLine("\nError: No database selected.");
                
                return OperationStatus.Error;
            }

            // Path definition of a new table file
            string tablePath = $@"{CurrentPath}\{DirectoryName}.Table";

            for (int i = 0; i < columnsAndTypes.Length; i++)
            {
                string[] parts = columnsAndTypes[i].Split(' ');

                if (parts.Length == 2)
                {
                    columnNames[i] = parts[0];

                    if (parts[1].StartsWith("VARCHAR"))
                    {
                        dataTypes[i] = ColumnType.String;
                        varcharSizes[i] = GetVarcharSize(parts[1]);
                    }
                    else if (parts[1] == "INTEGER")
                    {
                        dataTypes[i] = ColumnType.Integer; 
                        varcharSizes[i] = -1;
                    }
                    else if (parts[1] == "DATETIME")
                    {
                        dataTypes[i] = ColumnType.DateTime; 
                        varcharSizes[i] = -1;
                    }
                }
                else
                {
                    // Server verifiy
                    Console.WriteLine($"\nError: Invalid column definition");

                    return OperationStatus.Error;
                }
            }

            TableSchema schema = new TableSchema(columnNames, dataTypes, varcharSizes);
            
            BinaryTableInitializer.WriteTable(tablePath, schema);

            return OperationStatus.Success;
        }

        public OperationStatus Insert(string directory, string[] data)
        {
            var tablePath = $@"{CurrentPath}\{directory}.Table";
            int columnLenght;
            int dataLength = data.Length;

            // Read column length from the table file
            using (BinaryReader reader = new BinaryReader(File.Open(tablePath, FileMode.Open)))
            {
                columnLenght = reader.ReadInt32();
            }

            // Check if the data length matches the column length
            if (columnLenght != dataLength)
            {
                // Server verification
                Console.WriteLine($"\nError: Column input mismatch, Table lenght: {columnLenght}");

                return OperationStatus.Error;
            }

            List<int> varcharIndicators  = GetRightPathValues(tablePath);
            int indicatorLength = varcharIndicators.Count();

            using (FileStream stream = File.Open(tablePath, FileMode.Append))
            using (BinaryWriter writer = new (stream))
            {
                for (int i = 0; i < dataLength - 1; i++)
                {
                    bool isVarchar = false;
                    int varcharSize = 0;

                    // Check if the current column is varchar
                    for (int j = 0; j < indicatorLength; j += 2)
                    {
                        if (i == varcharIndicators[j])
                        {
                            isVarchar = true;
                            varcharSize = varcharIndicators[j + 1];
                            break;
                        }
                    }

                    if (isVarchar)
                    {
                        // Write varchar data, padded to the varchar size
                        string columnData = data[i].Length > varcharSize ? data[i].Substring(0, varcharSize) : data[i].PadRight(varcharSize);
                        
                        writer.Write(columnData);
                    }

                    else
                    {
                        try
                        {
                            // Try writing as an integer
                            int columnData = int.Parse(data[i]);
                            writer.Write(columnData);
                        }
                        catch (FormatException)
                        {
                            try
                            {
                                // Try writing as a DateTime
                                DateTime date = DateTime.Parse(data[i]);
                                writer.Write(date.Ticks);
                            }
                            catch
                            {
                                // Server validation 
                                Console.WriteLine($"\nError: Invalid data type for column {i + 1}");

                                // Handle invalid data
                                return OperationStatus.Error;
                            }
                        }
                    }
                }
            }

            // Server verification
            Console.WriteLine($"\nDATA INSERTED INTO {tablePath}");

            return OperationStatus.Success;
        }

        public OperationStatus Update(string tableName, string[] selectedColumns, string whereClause)
        {
            if (whereClause == null)
            {

            }
            else
            {

            }
            return OperationStatus.Success;
        }

        public OperationStatus Delete(string DirectoryName, string whereClause) 
        {
            var tablePath = $@"{CurrentPath}\{DirectoryName}.Table";

            BinarySearchTree bst = ConvertBinaryToBST(DirectoryName);
            List<int> RightPadsFormat = IndexStringAndPad(DirectoryName);
            List<string> ColumnsFormat = GetColumnsFormar(DirectoryName);

            List<Nodo> NodosPorEliminar = bst.GetNodesThat(whereClause , ColumnsFormat );

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
                        if (i == j)
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
                    if (orderClause == null) //Returns with no order all nodes that agree with whereClause
                    { 
                        List<Nodo> Arranged = bst.GetNodesThat(whereClause, ColumnsFormat);
                        PrintNodesForSelect(DirectoryName, Arranged, ColumnsFormat.Count());
                        return OperationStatus.Success;
                    }

                    else //Returns in order all nodes that agree with whereClause
                    {
                        List<Nodo> ExpresionNodes = bst.GetNodesThat(whereClause, ColumnsFormat);
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

                    if (orderClause == null)
                    { 
                        List<Nodo> ExpresionNodes = bst.GetNodesThat(whereClause, ColumnsFormat);
                        PrintNodesForSelect(DirectoryName, ExpresionNodes, ColumnsFormat.Count());
                        return OperationStatus.Success;
                    }

                    else
                    {
                        List<Nodo> ExpresionNodes = bst.GetNodesThat(whereClause, ColumnsFormat);
                        List<Nodo> Arranged = BinarySearchTree.ArrangeNodes(ExpresionNodes, orderClause);
                        PrintNodesForSelect(DirectoryName, Arranged, ColumnsFormat.Count());
                        return OperationStatus.Success;
                    }
                }                
            }
        }

        private int GetVarcharSize(string varcharString)
        {
            // Extract the size of VARCHAR
            var match = Regex.Match(varcharString, @"VARCHAR\((\d+)\)");
            if (match.Success && int.TryParse(match.Groups[1].Value, out int size))
            {
                return size;
            }

            // Default VARCHAR size
            return -1;
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
                    }
                    catch { continue; }
                }

            }

            return result;
        }
        
        public List<int> IndexStringAndPad(string DirectoryName)
        {
            List<int> VarcharValues = new List<int>();
            var tablePath = $@"{CurrentPath}\{DirectoryName}.Table";

            using (BinaryReader reader = new BinaryReader(File.Open(tablePath, FileMode.Open)))
            {
                int ColLength = reader.ReadInt32();
                for (int i = 0; i < ColLength -1; i++)
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
        
        public List<string> GetColumnsFormar(string DirectoryName)
        {
            List<string> ColumnsFormat = new List<string>();
            var tablePath = $@"{CurrentPath}\{DirectoryName}.Table";
            
            using (BinaryReader reader = new BinaryReader(File.Open(tablePath, FileMode.Open)))
            {
                int ColLength = reader.ReadInt32();
                for (int i = 0; i < ColLength -1; i++)
                {
                    try
                    {
                        int VarcarNum = reader.ReadInt32();
                        string column = new string(reader.ReadChars(VarcarNum));
                        ColumnsFormat.Add(column);
                    }
                    catch
                    {
                        string column = new string(reader.ReadChars(50));
                        ColumnsFormat.Add(column);
                    }
                }
            }
            return ColumnsFormat;
        }
        
        public BinarySearchTree ConvertBinaryToBST(string DirectoryName)
        {
            // Server verification
            Console.WriteLine("\nINITILIZED TABLE BST...");

            var tablePath = $@"{DirectoryName}";

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

                        // Server verification
                        Console.WriteLine("\nADD COLUMN FORMAT (1)...");
                    }
                    catch
                    {
                        string column = new string(reader.ReadChars(50)); //Default value for column names for Integers and DateTime.
                        ColumnsFormat.Add(column);

                        // Server verification
                        Console.WriteLine("\nADD COLUMN FORMAT (2)...");
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

            // Server verification 
            Console.WriteLine("\nTABLE BST CREATED");

            return bst;
        }

        public OperationStatus BinaryFileReader(string tableDirectory)
        {
            string tablePath = Path.Combine(CurrentPath, $"{tableDirectory}.Table");

            try
            {
                BinaryTableReader.ReadTable(tablePath);
                
                return OperationStatus.Success;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading file {tableDirectory}.Table: {ex.Message}");
                return OperationStatus.Error;
            }
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
                        for (int j = 0; j < RightPadsFormat.Count() - 1; j += 2)
                        {
                            if (i == j)
                            {
                                writer.Write(RightPadsFormat[i]);
                                writer.Write(ColumnsFormat[i].PadRight(RightPadsFormat[i + 1]));
                            }
                            break;
                        }
                        
                        for (int j = 0; j < RightPadsFormat.Count() - 1; j += 2 )
                        {
                            if (i == j)
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
            // Server verification
            Console.WriteLine("Printing BST nodes");

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
                                    Console.WriteLine("WRONG INPUT...");
                                    continue;
                                }
                            }
                        }
                    }
                    Console.WriteLine(""); 
                }        
            }
        }
    
    }
}
