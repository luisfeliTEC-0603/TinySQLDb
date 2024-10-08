namespace BinaryTableManager
{
    public class BinaryTableEditor
    {
        public static void BinaryInsertRow( string[] columnEntries, string filePath)
        {            
            int columnCount;
            var columnTypes = new List<ColumnType>();
            var columnPadRight = new List<int>();
            var columnNames = new List<string>();
            int wordLength;
                                    
            using (FileStream stream = new FileStream(filePath, FileMode.Open))
            using (BinaryReader reader = new BinaryReader(stream))
            {
                columnCount = reader.ReadInt32();

                for (int i = 0; i < columnCount; i++)
                {
                    columnTypes.Add((ColumnType)reader.ReadInt32());
                    Console.WriteLine($"(Type) {columnTypes[i]}");

                    wordLength = reader.ReadInt32();
                    columnPadRight.Add(wordLength);
                    Console.WriteLine($"(PadRight) {columnPadRight[i]}");

                    int charCount = (wordLength == -1) ? 50 : wordLength;
                    columnNames.Add(new string(reader.ReadChars(charCount)));
                    Console.WriteLine($"{columnNames[i]}");
                }
            }

            using (FileStream stream = new FileStream(filePath, FileMode.Append))
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                for (int i = 0; i < columnEntries.Length; i++)
                {
                        Console.WriteLine("Adding data");
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
                                    Console.WriteLine($"Error al convertir a Integer: {columnEntries[i]}");
                                    throw new FormatException($"Invalid integer format for entry: {columnEntries[i]}");
                                }
                                break;

                            case ColumnType.String:
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
                                    Console.WriteLine($"Error al convertir a DateTime: {columnEntries[i]}");
                                    throw new FormatException($"Invalid DateTime format for entry: {columnEntries[i]}");
                                }
                                break;

                            default:
                                Console.WriteLine("UnsupportedColumnType");
                                throw new InvalidOperationException("Unsupported column type.");
                        }
                }
            }
            
        }
    }
}