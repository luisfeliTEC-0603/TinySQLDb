﻿using Entities;
using System.IO.Compression;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

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

        private readonly string baseDirectory;
        private readonly string DatabaseBasePath;
        private readonly string DataPath;
        private readonly string SystemCatalogPath;
        private readonly string SystemDatabasesFile;
        private readonly string SystemTablesFile;     
        private string CurrentPath = null;   
        
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

        public OperationStatus DeleteFromTable(string directory, string whereClause)
        {

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
                int IndicatorLenght = IndicatorsVarchar.Count;
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

        public OperationStatus Select(string column, string whereClause, string OrderClause)
        {
            if (OrderClause == null)
            {
                // Creates a default Table called ESTUDIANTES
                var tablePath = $@"{DataPath}\TESTDB\ESTUDIANTES.Table";
                using (FileStream stream = File.Open(tablePath, FileMode.OpenOrCreate))
                using (BinaryReader reader = new (stream))
                {
                    // Print the values as a I know exactly the types, but this needs to be done right
                    Console.WriteLine(reader.ReadInt32());
                    Console.WriteLine(reader.ReadString());
                    Console.WriteLine(reader.ReadString());
                    return OperationStatus.Success;
                }
            }
            else
            {
                return OperationStatus.Success;
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
    }
}
