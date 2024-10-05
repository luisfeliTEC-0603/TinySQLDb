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

                    return new CreateTable().Execute(directoryName, sentence);
                }

                // UPDATE SQL sentence
                if (sentence.StartsWith("UPDATE"))
                {
                    string directoryName = sentence.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[1];
                    sentence = sentence.Replace($"UPDATE {directoryName} SET", "").Trim().TrimEnd(';');

                    string[] selectedColumns = sentence.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                    int whereIndex = sentence.IndexOf("WHERE");

                    if (whereIndex != -1)
                    {
                        string columnAssignments = sentence.Substring(0, whereIndex).Trim();

                        string whereClause = sentence.Substring(whereIndex + "WHERE".Length).Trim();

                        return new UpdateSentence().Execute(directoryName, selectedColumns, whereClause);
                    }
                    return new UpdateSentence().Execute(directoryName, selectedColumns, null);
         
                }

                // SELECT DATABASE SQL sentence
                if (sentence.StartsWith("SELECT")) // SELECT columns/* FROM talbe WHERE identification num ORDERED BY columns asc/desc
                {
                    string columns = sentence.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[1];
                    sentence = sentence.Replace($"SELECT {columns} FROM", "").Trim().TrimEnd(';');

                    string[] command = sentence.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                    string Table = command[0];

                    string whereClause = command[2] + " " + command[3];
                    
                    int OrderIndex = sentence.IndexOf("ORDERED");

                    if (OrderIndex != -1)
                    {
                        return new Select().Execute(columns, whereClause, null);
                    }
                    else
                    {
                        string OrderClause = command[OrderIndex + 2] + " " + command[OrderIndex + 3];
                        return new Select().Execute(columns, whereClause, OrderClause);
                    }
                }

                // DELETE A ROW FROM TAGBLE 
                if (sentence.StartsWith("DELETE")) //DELETE FROM Estudiantes WHERE (ID == 1)
                {
                    string directoryName = sentence.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[2];
                    sentence = sentence.Replace($"DELETE FROM {directoryName} WHERE", "").Trim().TrimEnd(';');
                    
                    return new DeleteFromTable().Execute(directoryName, sentence);
                }

                // INSERT A ROW TO A TABLE
                if (sentence.StartsWith("INSERT INTO"))
                {
                    string directoryName = sentence.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[2];
                    sentence = sentence.Replace($"INSERT INTO {directoryName}", "").Trim().TrimEnd(';');
                    string[] data = sentence.Split(new[] { ' ' });

                    return new Insert().Execute(directoryName, data);
                }
                else
                {
                    throw new UnknownSQLSentenceException();
                }
            }
        }
    }
