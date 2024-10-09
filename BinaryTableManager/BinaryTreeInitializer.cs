using BST;
namespace BinaryTableManager
{
    public class BinaryTreeInitializer
    {
        public static List<Nodo> CreateNodesForBST(string filePath, BinarySearchTree bst, string whereClause)
        {
            int columnCount;
            var columnTypes = new List<ColumnType>();
            var columnPadRight = new List<int>();
            var columnNames = new List<string>();
            int wordLength;

            List<object> AttributesForNode = new List<object>();
                                    
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
             //At this point all table format has been printed.

                while (reader.BaseStream.Position != reader.BaseStream.Length)
                {
                    int intValueIndicator = reader.ReadInt32();
                    for (int i = 1; i < columnCount; i++)
                    {
                        switch(columnTypes[i])
                        {
                            case ColumnType.Integer:
                                int colInt = reader.ReadInt32();
                                AttributesForNode.Add(colInt);
                                break;
                            case ColumnType.String:
                                string colStr = reader.ReadString().PadRight(columnPadRight[i]);
                                AttributesForNode.Add(colStr);
                                break;
                            case ColumnType.DateTime:
                                long ticks = reader.ReadInt64(); 
                                AttributesForNode.Add(new DateTime(ticks));    
                                break;
                            default:
                                Console.WriteLine("ErrorWhileReadinTableData");
                                throw new InvalidOperationException("Unsupported column type.");
                        }
                    }
                    bst.Add(intValueIndicator, AttributesForNode);
                    Console.Write($"Indicator for node: {intValueIndicator}" +" ");
                    Console.Write($"Con atributos:");  
                    foreach (object objeto in AttributesForNode)
                    {
                        Console.Write(objeto+ " ");
                    }
                    Console.WriteLine(" ");
                    AttributesForNode.Clear();

                }
            }
            List<Nodo> ForDeletion = bst.GetNodesThat(whereClause, columnTypes, columnNames );

            return ForDeletion;
        }
    }
}