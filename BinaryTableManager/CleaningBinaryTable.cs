using BinaryTableManager;
public class CleanBinaryTable
{
    public static void CleanPath(string tablePath)
    {
        int columnCount;
        var columnTypes = new List<ColumnType>();
        var columnPadRight = new List<int>();
        var columnNames = new List<string>();
        int wordLength;

        // Usar un bloque 'using' para asegurar el cierre adecuado
        using (var memoryStream = new MemoryStream())
        using (FileStream stream = new FileStream(tablePath, FileMode.Open, FileAccess.Read))
        using (BinaryReader reader = new BinaryReader(stream))
        {
            columnCount = reader.ReadInt32();

            for (int i = 0; i < columnCount; i++)
            {
                columnTypes.Add((ColumnType)reader.ReadInt32());

                wordLength = reader.ReadInt32();
                columnPadRight.Add(wordLength);

                // Validar wordLength antes de usarlo
                if (wordLength < 0)
                {
                    wordLength = 50; // Valor predeterminado
                }

                int charCount = Math.Max(wordLength, 50); // Asegura un valor positivo
                columnNames.Add(new string(reader.ReadChars(charCount)).TrimEnd('\0')); // Remover caracteres nulos
            }
        }
        // Reemplazar el archivo binario original
        using (var memoryStream = new MemoryStream())
        using (var outputStream = new FileStream(tablePath, FileMode.Create, FileAccess.Write))
        {
            memoryStream.Seek(0, SeekOrigin.Begin);
            memoryStream.CopyTo(outputStream);
        }
        
        using (FileStream stream = new FileStream(tablePath, FileMode.Create))
        using (BinaryWriter writer = new BinaryWriter(stream))
        {
            writer.Write(columnNames.Count());
            
            for (int i = 0; i < columnNames.Count(); i++)
            {
                int padRightValue = columnPadRight[i];
                
                writer.Write((int)columnTypes[i]);
                writer.Write(columnPadRight[i]);
                writer.Write(columnNames[i].PadRight( padRightValue != -1 ?  padRightValue : 50 ).ToCharArray());
            }
        }
    }
    
}
