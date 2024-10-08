namespace BinaryTableManager
{
    public class BinaryTableInitializer
    {
        public static void WriteTable(string filePath, TableSchema schema)
        {
            using (FileStream stream = new FileStream(filePath, FileMode.Create))
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                writer.Write(schema.ColumnNames.Length);

                // Server verification 
                Console.WriteLine($"\nTable in {filePath} with {schema.ColumnNames.Length} columns was initilized");
                
                for (int i = 0; i < schema.ColumnNames.Length; i++)
                {
                    int padRightValue = schema.ColumnPadRight[i];
                    
                    writer.Write((int)schema.ColumnTypes[i]);
                    writer.Write(schema.ColumnPadRight[i]);
                    writer.Write(schema.ColumnNames[i].PadRight( padRightValue != -1 ?  padRightValue : 50 ).ToCharArray());

                    // Server verification 
                    Console.WriteLine($"Column {i + 1}: {schema.ColumnNames[i]} (Type: {schema.ColumnTypes[i]})");
                }
            }
        }
    }
}