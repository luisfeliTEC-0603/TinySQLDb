    using Entities;
    using QueryProcessor.Exceptions;
    using QueryProcessor.Operations;
    using StoreDataManager;

    namespace QueryProcessor
    {
        // Method to process SQL sentences
        public class SQLQueryProcessor
        {
            public static OperationStatus Execute(string sentence)
            {
                string directoryName = null;
                string[] columnEntries = null;
                string whereClause = null;
                int whereIndex = -1;
                string orderClause = null;

                // CREATE DATABASE SQL sentence
                if (sentence.StartsWith("CREATE DATABASE"))
                {
                    directoryName = sentence.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[2];
                    directoryName = directoryName.TrimEnd(';');
                    
                    return new CreateDataBase().Execute(directoryName);
                }

                // SET DATABASE SQL sentence
                if (sentence.StartsWith("SET"))
                {
                    directoryName = sentence.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[1];
                    directoryName = directoryName.TrimEnd(';');
                    
                    return new SetDataBase().Execute(directoryName);
                }

                // DROP TABLE SQL sentence
                if (sentence.StartsWith("DROP TABLE"))
                {
                    directoryName = sentence.TrimEnd(';').Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[2];
                    return new DropTable().Execute(directoryName);
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

                    columnEntries = sentence.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    return new Insert().Execute(directoryName, columnEntries);
                }

                // CREATE TABLE SQL sentence
                if (sentence.StartsWith("CREATE TABLE"))
                {
                    directoryName = sentence.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[2];
                    sentence = sentence.Replace($"CREATE TABLE {directoryName}", "").Trim().TrimEnd(';');

                    if (sentence.StartsWith("(") && sentence.EndsWith(")"))
                    {
                        sentence = sentence.Substring(1, sentence.Length - 2).Trim();
                    }

                    columnEntries = sentence.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(col => col.Trim()).ToArray();                   
                    // (e.g.) string[] columnEntries = [ "ID INTEGER", "ARGUMENT VARCHAR(30)" ]

                    return new CreateTable().Execute(directoryName, columnEntries);
                }

                // UPDATE SQL sentence
                if (sentence.StartsWith("UPDATE"))
                {
                    directoryName = sentence.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[1];
                    sentence = sentence.Replace($"UPDATE {directoryName} SET", "").Trim().TrimEnd(';');

                    whereIndex = sentence.IndexOf("WHERE");

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

                // SELECT SQL sentence
                if (sentence.StartsWith("SELECT"))
                {
                    sentence = sentence.Replace($"SELECT", "").Trim().TrimEnd(';');

                    int fromIndex = sentence.IndexOf("FROM");
                    int orderIndex = sentence.IndexOf("ORDER BY");
                    whereIndex = sentence.IndexOf("WHERE");

 
                    string columnString = sentence.Substring(0, fromIndex).Trim();
                    columnEntries = columnString.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                    sentence = sentence.Substring(fromIndex + "FROM".Length).Trim();

                    directoryName = sentence.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[0];

                    if (whereIndex != -1)
                    {
                        whereClause = sentence.Substring(whereIndex + "WHERE".Length).Trim();
                        sentence = sentence.Substring(0, whereIndex).Trim();
                    }

                    if (orderIndex != -1)
                    {
                        string orderClauseString = sentence.Substring(orderIndex + "ORDER BY".Length).Trim();
                        
                        if (orderClauseString.EndsWith("DESC"))
                        {
                            orderClause = "DESC";
                        }
                        else if (orderClauseString.EndsWith("ASC"))
                        {
                            orderClause = "ASC";
                        }
                        
                        sentence = sentence.Substring(0, orderIndex).Trim();
                    }

                    return new Select().Execute(directoryName, columnEntries, whereClause, orderClause);
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

                // Auxiliary Sentence for binary-files 
                if (sentence.StartsWith("READ BINARY"))
                {
                    directoryName = sentence.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[2];
                    directoryName = directoryName.TrimEnd(';');
                    
                    return new BinaryFileReader().Execute(directoryName);

                }

                else
                {
                    // throw new UnknownSQLSentenceException();
                    Console.WriteLine($"UnknownSQLSentenceException");

                    return new SetDataBase().Execute("VOID");
                }
            }

            private OperationStatus SyntaxVerifier(string sentence)
            {
                if (sentence.EndsWith(";"))
                {
                    Console.WriteLine($"InavlidSQLSentenceSyntaxis");
                    return OperationStatus.Error;
                }

                return OperationStatus.Success;
            }
        }
    }
