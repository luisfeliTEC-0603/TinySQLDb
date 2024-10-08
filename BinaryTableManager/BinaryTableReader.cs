using System;
using System.Collections.Generic;
using System.IO;

namespace BinaryTableManager
{
    public class BinaryTableReader
    {
        public static (TableSchema schema, List<object[]> rows) ReadTable(string filePath)
        {
            var rows = new List<object[]>();
            TableSchema schema;

            using (FileStream stream = new FileStream(filePath, FileMode.Open))
            using (BinaryReader reader = new BinaryReader(stream))
            {
                int columnCount = reader.ReadInt32();

                var columnTypes = new List<ColumnType>(columnCount);
                var columnPadRight = new List<int>(columnCount);
                var columnNames = new List<string>(columnCount);
                int wordLength;

                for (int i = 0; i < columnCount; i++)
                {
                    columnTypes.Add((ColumnType)reader.ReadInt32());
                    Console.WriteLine($"(Type) {columnTypes[i]}");

                    wordLength = reader.ReadInt32();
                    columnPadRight.Add(wordLength);
                    Console.WriteLine($"(PadRight) {columnPadRight[i]}");

                    int charCount = (wordLength == -1) ? 50 : wordLength;
                    columnNames.Add(new string(reader.ReadString().PadRight(charCount).Trim()));
                    Console.WriteLine($"{columnNames[i]}");
                }

                schema = new TableSchema(columnNames.ToArray(), columnTypes.ToArray(), columnPadRight.ToArray());

                while (reader.BaseStream.Position != reader.BaseStream.Length)
                {
                    var row = new object[columnCount];
                    for (int i = 0; i < columnCount; i++)
                    {
                        switch (columnTypes[i])
                        {
                            case ColumnType.Integer:
                                row[i] = reader.ReadInt32();
                                break;
                            case ColumnType.String:
                                row[i] = new string(reader.ReadString().PadRight(columnPadRight[i])).Trim();
                                break;
                            case ColumnType.DateTime:
                                long ticks = reader.ReadInt64(); 
                                row[i] = new DateTime(ticks);    
                                break;
                            default:
                                Console.WriteLine("UnsupportedColumnType");
                                throw new InvalidOperationException("Unsupported column type.");
                        }
                    }
                    rows.Add(row);

                    foreach (var value in row){ Console.Write($"{value,-20}"); }
                    Console.WriteLine();
                }
            }

            return (schema, rows);
        }
    }
}
