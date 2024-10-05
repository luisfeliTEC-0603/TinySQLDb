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

        // Method to process SQL sentences
        public class SQLQueryProcessor
        {

            public static OperationStatus Execute(string sentence)
            {
                // CREATE DATABASE SQL sentence
                if (sentence.StartsWith("CREATE DATABASE"))
                {
                    directoryName = sentence.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[2];

                    return new CreateDataBase().Execute(directoryName);
                }

                // SET DATABASE SQL sentence
                if (sentence.StartsWith("SET"))
                {
                    directoryName = sentence.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[2];

                    return new Set().Execute(directoryName);
                }

                // DROP DATABASE SQL sentence
                if (sentence.StartsWith("DROP TABLE"))
                {
                    directoryName = sentence.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[2];

                    return new DropTable().Execute(directoryName);
                }

                // CREATE DATABASE SQL sentence
                if (sentence.StartsWith("CREATE TABLE"))
                {
                    directoryName = sentence.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[2];
                    sentence = sentence.Replace($"CREATE TABLE {directoryName}", "").Trim().TrimEnd(';');

                    if (sentence.StartsWith("(") && sentence.EndsWith(")"))
                    {
                        sentence = sentence.Substring(1, sentence.Length - 2).Trim();
                    } 

                    return new CreateTable().Execute(directoryName, sentence);
                }

                // UPDATE SQL sentence
                if (sentence.StartsWith("UPDATE"))
                {
                    directoryName = sentence.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[1];
                    sentence = sentence.Replace($"CREATE TABLE {directoryName} SET", "").Trim().TrimEnd(';');

                    selectedColumns = sentence.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                    whereIndex = sentence.IndexOf("WHERE");

                    if (whereIndex != -1)
                    {
                        string columnAssignments = sentence.Substring(0, index).Trim();

                        string whereClause = sentence.Substring(index + "WHERE".Length).Trim();
                    }

                    return new Update().Execute();
                }

                // SELECT DATABASE SQL sentence
                if (sentence.StartsWith("SELECT"))
                {
                    //...

                    return new Select().Execute();
                }

                else
                {
                    throw new UnknownSQLSentenceException();
                }
            }
        }
    }
