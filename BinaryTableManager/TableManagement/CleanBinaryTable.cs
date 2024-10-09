namespace BinaryTableManager.TableManagement
{
    public class CleanBinaryTable
    {
        public static void CleanPath(string tablePath)
        {
            int columnCount; // Number of columns
            var columnTypes = new List<ColumnType>(); // List of ColumnTypes
            var columnPadRight = new List<int>(); // List of padding lengths
            var columnNames = new List<string>(); // List of column titles
            int wordLength; // Varible that holds the lenght of the words

            // Reading the binary table structure
            using (var memoryStream = new MemoryStream())
            using (FileStream stream = new FileStream(tablePath, FileMode.Open, FileAccess.Read))
            using (BinaryReader reader = new BinaryReader(stream))
            {
                columnCount = reader.ReadInt32(); // Read the number of columns

                for (int i = 0; i < columnCount; i++)
                {
                    columnTypes.Add((ColumnType)reader.ReadInt32());

                    // Read & store the padding value
                    wordLength = reader.ReadInt32(); 
                    columnPadRight.Add(wordLength);

                    // Validar wordLength antes de usarlo
                    if (wordLength < 0) wordLength = 50; 

                    // Calculate character count
                    int charCount = Math.Max(wordLength, 50);
                    // Read the column name
                    columnNames.Add(new string(reader.ReadChars(charCount)).TrimEnd('\0')); // Remover caracteres nulos
                }
            }

            // Replace the original binary file with cleaned-up data
            using (var memoryStream = new MemoryStream())
            using (var outputStream = new FileStream(tablePath, FileMode.Create, FileAccess.Write))
            {
                memoryStream.Seek(0, SeekOrigin.Begin); // Move to the start of the memory stream
                memoryStream.CopyTo(outputStream); // Copy the contents to the file
            }
            
            using (FileStream stream = new FileStream(tablePath, FileMode.Create))
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                writer.Write(columnNames.Count());
                
                for (int i = 0; i < columnNames.Count(); i++)
                {
                    int padRightValue = columnPadRight[i];
                    
                    writer.Write((int)columnTypes[i]); // Write the column type
                    writer.Write(columnPadRight[i]); // Write the padding value
                    writer.Write(columnNames[i].PadRight(padRightValue != -1 ? padRightValue : 50).ToCharArray()); // Write the column name
                }
            }
        }
    }
}
