using System; 
using System.IO;

namespace BinaryTableManager.TableManagement
{
    public class BinaryTableInitializer
    {
        public static void WriteTable(string filePath, TableSchema schema)
        {
            // Create a new binary file at the specified path
            using (FileStream stream = new FileStream(filePath, FileMode.Create))
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                // Write the number of columns
                writer.Write(schema.ColumnNames.Length);

                // Server verification 
                Console.WriteLine($"\nTABLE[{schema.ColumnNames.Length}] INITILIZED IN : {filePath}");
                Console.WriteLine("\nWriter------");

                // Write each column properties
                for (int i = 0; i < schema.ColumnNames.Length; i++)
                {
                    int padRightValue = schema.ColumnPadRight[i];
                    
                    // Write column type, padding, and name
                    writer.Write((int)schema.ColumnTypes[i]);
                    writer.Write(schema.ColumnPadRight[i]);
                    writer.Write(schema.ColumnNames[i].PadRight( padRightValue != -1 ?  padRightValue : 50 ).ToCharArray());

                    // Server verification 
                    Console.WriteLine($"[{i + 1}] {schema.ColumnNames[i]} (Type: {schema.ColumnTypes[i]})");
                }

                Console.WriteLine("----------\n");
            }
        }
    }
}