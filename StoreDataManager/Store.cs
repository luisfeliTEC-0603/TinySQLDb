﻿using Entities;
using System.IO.Compression;
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
        
        public Store()
        {
            baseDirectory = Directory.GetCurrentDirectory();
            DatabaseBasePath = Path.Combine(baseDirectory, "TinySQL");
            SystemCatalogPath = Path.Combine(DatabaseBasePath, "SystemCatalog");
            SystemDatabasesFile = Path.Combine(SystemCatalogPath, "SystemDatabases.table");
            SystemTablesFile = Path.Combine(SystemCatalogPath, "SystemTables.table");

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

        public OperationStatus CreateTable(string DirectoryName, string Sentence) 
        {
            // Creates a default DB called TESTDB
            Directory.CreateDirectory($@"{DataPath}\TESTDB");

            // Creates a default Table called ESTUDIANTES
            var tablePath = $@"{DataPath}\TESTDB\ESTUDIANTES.Table";

            using (FileStream stream = File.Open(tablePath, FileMode.OpenOrCreate))
            using (BinaryWriter writer = new (stream))
            {
                // Create an object with a hardcoded.
                // First field is an int, second field is a string of size 30,
                // third is a string of 50
                int id = 1;
                string nombre = "Isaac".PadRight(30); // Pad to make the size of the string fixed
                string apellido = "Ramirez".PadRight(50);

                writer.Write(id);
                writer.Write(nombre);   
                writer.Write(apellido);        
            }
            return OperationStatus.Success;
        }

        public OperationStatus Insert(string directory, string[] data)
        {
            // Creates a default DB called TESTDB
            Directory.CreateDirectory($@"{DataPath}\TESTDB");

            // Creates a default Table called ESTUDIANTES
            var tablePath = $@"{DataPath}\TESTDB\ESTUDIANTES.Table";

            using (FileStream stream = File.Open(tablePath, FileMode.OpenOrCreate))
            using (BinaryWriter writer = new (stream))
            {
                // Create an object with a hardcoded.
                // First field is an int, second field is a string of size 30,
                // third is a string of 50
                int id = 1;
                string nombre = "Isaac".PadRight(30); // Pad to make the size of the string fixed
                string apellido = "Ramirez".PadRight(50);

                writer.Write(id);
                writer.Write(nombre);   
                writer.Write(apellido);        
            }
            return OperationStatus.Success;
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
    }
}
