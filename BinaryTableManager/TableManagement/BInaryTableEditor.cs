using System; 
using System.Collections.Generic; 
using System.IO;

namespace BinaryTableManager.TableManagement
{
    public class BinaryTableEditor
    {
        public static void BinaryInsertRow(string[] columnEntries, string filePath)
        {            
            int columnCount; // Number of columns
            var columnTypes = new List<ColumnType>(); // List of ColumnTypes
            var columnPadRight = new List<int>(); // List of padding lengths
            var columnNames = new List<string>(); // List of column titles
            int wordLength; // Varible that holds the lenght of the words
                                    
            using (FileStream stream = new FileStream(filePath, FileMode.Open))
            using (BinaryReader reader = new BinaryReader(stream))
            {
                columnCount = reader.ReadInt32(); // Read and holds the number of columns in the table

                Console.WriteLine("\nWriter------");
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
            }

            // Append a new row to the binary table
            using (FileStream stream = new FileStream(filePath, FileMode.Append))
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                for (int i = 0; i < columnEntries.Length; i++)
                {
                    // Write data based on column type
                    switch (columnTypes[i])
                    {
                        case ColumnType.Integer:
                        Console.WriteLine(columnEntries[i]);
                            if (int.TryParse(columnEntries[i], out int num))
                            {
                                writer.Write(num);
                            }
                            else
                            {
                                // Handle parsing error for integers
                                Console.WriteLine($"!Error : Integer parsing error of {columnEntries[i]}");
                                throw new FormatException($"Invalid Integer Format for Entry : {columnEntries[i]}");
                            }
                            break;

                        case ColumnType.String:
                            // Write a string value with specified padding
                            int padRight = columnPadRight[i];
                            writer.Write(columnEntries[i].PadRight(padRight));
                            break;

                        case ColumnType.DateTime:
                            if (DateTime.TryParse(columnEntries[i], out DateTime date))
                            {
                                writer.Write(date.Ticks);
                            }
                            else
                            {
                                // Handle parsing error for DateTime
                                Console.WriteLine($"!Error : DateTime parsing error of {columnEntries[i]}");
                                throw new FormatException($"Invalid DateTime Format for Entry: {columnEntries[i]}");
                            }
                            break;

                        default:
                            // Handle unsupported column types
                            Console.WriteLine("!Error : UnsupportedColumnType");
                            throw new InvalidOperationException($"Unsupported column type find in {columnEntries[i]}");
                    }

                    Console.WriteLine($"({columnTypes[i]}) {columnEntries[i]}");
                }

                Console.WriteLine("----------\n");
            }
        }
    }
}   