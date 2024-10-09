using System;
using System.Collections.Generic;
using System.IO;

namespace BinaryTableManager.TableManagement
{
    public class BinaryTableReader
    {
        public static (TableSchema schema, List<object[]> rows) ReadTable(string filePath)
        {
            var rows = new List<object[]>(); // List to hold rows of data
            TableSchema schema;

            using (FileStream stream = new FileStream(filePath, FileMode.Open))
            using (BinaryReader reader = new BinaryReader(stream))
            {
                int columnCount = reader.ReadInt32(); // Read the number of columns

                // Lists to store schema information
                var columnTypes = new List<ColumnType>(columnCount);
                var columnPadRight = new List<int>(columnCount);
                var columnNames = new List<string>(columnCount);
                int wordLength;

                // Read each column type, padding length, and name
                Console.WriteLine("\nReader------");
                for (int i = 0; i < columnCount; i++)
                {
                    columnTypes.Add((ColumnType)reader.ReadInt32()); // Column type associeted value (int)
                    Console.WriteLine($"(Type) {columnTypes[i]}");

                    wordLength = reader.ReadInt32(); // Padding lenght
                    columnPadRight.Add(wordLength);
                    Console.WriteLine($"(PadRight) {columnPadRight[i]}");

                    int charCount = (wordLength == -1) ? 50 : wordLength; // Determine character count
                    columnNames.Add(new string(reader.ReadChars(charCount))); // Read and store column name
                    Console.WriteLine($"(Column Title) {columnNames[i]}");
                }
                Console.WriteLine("----------");

                // Create a schema object from the read information
                schema = new TableSchema(columnNames.ToArray(), columnTypes.ToArray(), columnPadRight.ToArray());

                // Read rows of data until the end of the stream
                while (reader.BaseStream.Position != reader.BaseStream.Length)
                {
                    var row = new object[columnCount];
                    for (int i = 0; i < columnCount; i++)
                    {
                        switch (columnTypes[i])
                        {
                            case ColumnType.Integer:
                                row[i] = reader.ReadInt32(); // Read an integer
                                break;

                            case ColumnType.String:
                                row[i] = new string(reader.ReadString().PadRight(columnPadRight[i])).Trim();
                                break;

                            case ColumnType.DateTime:
                                long ticks = reader.ReadInt64(); 
                                row[i] = new DateTime(ticks); // Convert ticks to DateTime
                                break;

                            default:
                                // Handle unsupported column types
                                Console.WriteLine("\n!Error : UnsupportedColumnType");
                                throw new InvalidOperationException($"Unsupported column type find in file");
                        }

                        Console.WriteLine($"({columnTypes[i]}) {row[i]}");
                    }
                    rows.Add(row);
                }
            }
            Console.WriteLine("----------\n");

            return (schema, rows); // Return the schema and rows
        }
    }
}
