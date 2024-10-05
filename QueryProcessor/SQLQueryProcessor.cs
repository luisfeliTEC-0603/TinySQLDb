    using Entities;
    using QueryProcessor.Exceptions;
    using QueryProcessor.Operations;
    using StoreDataManager;

    namespace QueryProcessor
    {
        // CREAT DATABSE <dataBase-name>
        // SET <dataBase-name>
        // CREAT TABLE <table-name> ( arg, )
        // DROP TABLE <table-name>
        // SELECT ( *|<columns> ) FROM <table-name> [WHERE ( arg ) ORDER BY ( ASC/DEC )]
        // UPDATE <table-name> SET <column-name> = <new-value> [WHERE ( arg )];
        // DELETE FROM <table-name> [WHERE ( arg )]
        // INSERT INTO <table-name> VALUES ( arg1, agr2, ... )
        // INDEX ???

        // whereClause (string) => bool 
        // columns string[]
        // string => "INTEGER ID, COLOR VARCAHR(30)"

        // ??? 
        // string[] => [INTEGER, ID, COLOR, VARCHAR(30)]

        // Method to process SQL sentences
        public class SQLQueryProcessor
        {
            public static OperationStatus Execute(string sentence)
            {

                string directoryName;
                string[] columnEntries;
                string whereClause;
                int whereIndex;
                bool orderClause = false;

                // CREATE DATABASE SQL sentence
                if (sentence.StartsWith("CREATE DATABASE"))
                {
                    string directoryName = sentence.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[2];

                    return new CreateDataBase().Execute(directoryName);
                }

                // SET DATABASE SQL sentence, change from different DB
                if (sentence.StartsWith("SET"))
                {
                    string directoryName = sentence.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[2];

                    return new SetDataBase().Execute(directoryName);
                }

                // DROP DATABASE SQL sentence
                if (sentence.StartsWith("DROP TABLE"))
                {
                    string directoryName = sentence.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[2];

                    return new DropTable().Execute(directoryName);
                }

                // CREATE DATABASE SQL sentence
                if (sentence.StartsWith("CREATE TABLE"))
                {
                    string directoryName = sentence.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[2];
                    sentence = sentence.Replace($"CREATE TABLE {directoryName}", "").Trim().TrimEnd(';');

                    if (sentence.StartsWith("(") && sentence.EndsWith(")"))
                    {
                        sentence = sentence.Substring(1, sentence.Length - 2).Trim();
                    } 

                    columnEntries = sentence.Split(new[] { ' ' , ','}, StringSplitOptions.RemoveEmptyEntries);

                    return new CreateTable().Execute(directoryName, columnEntries);
                }

                // UPDATE SQL sentence
                if (sentence.StartsWith("UPDATE"))
                {
                    directoryName = sentence.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[1];
                    sentence = sentence.Replace($"UPDATE {directoryName} SET", "").Trim().TrimEnd(';');

                    int whereIndex = sentence.IndexOf("WHERE");

                    if (whereIndex != -1)
                    {
                        string columnAssignments = sentence.Substring(0, whereIndex).Trim();

                        whereClause = sentence.Substring(whereIndex + "WHERE".Length).Trim();

                        columnEntries = columnAssignments.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    }
                    else
                    {
                        columnEntries = sentence.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    }

                    return new Update().Execute(directoryName, columnEntries, whereClause);
                }

                // SELECT DATABASE SQL sentence
                if (sentence.StartsWith("SELECT"))
                {
                    directoryName = sentence.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[1];
                    sentence = sentence.Replace($"SELECT {directoryName} FROM", "").Trim().TrimEnd(';');



                    return new Select().Execute();
                }

                // DELETE FROM SQL sentence
                if (sentence.StartsWith("DELETE FROM"))
                {
                    directoryName = sentence.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[2];
                    sentence = sentence.Replace($"DELETE FROM {directoryName}", "").Trim().TrimEnd(';');

                    whereIndex = sentence.IndexOf("WHERE");

                    if (whereIndex != -1)
                    {
                        whereClause = sentence.Substring(whereIndex + "WHERE".Length).Trim();
                    }

                    return new Delete().Execute(directoryName, whereClause);
                }

                // INSERT INTO SQL sentence
                if (sentence.StartsWith("INSERT INTO"))
                {
                    directoryName = sentence.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[2];
                    sentence = sentence.Replace($"INSERT INTO {directoryName} VALUES", "").Trim().TrimEnd(';');

                    if (sentence.StartsWith("(") && sentence.EndsWith(")"))
                    {
                        sentence = sentence.Substring(1, sentence.Length - 2).Trim();
                    } 

                    columnEntries = sentence.Split(new[] { ' ' , ','}, StringSplitOptions.RemoveEmptyEntries);

                    return new Insert().Execute(directoryName, columnEntries);
                }

                else
                {
                    throw new UnknownSQLSentenceException();
                }
            }
        }
    }
