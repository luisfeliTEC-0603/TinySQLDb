using System;
using System.Collections.Generic;
using BinaryTableManager.TableManagement;

namespace BinaryTableManager.TreeStructure
{
    public class BinaryTreeInitializer
    {
        public static List<Nodo> CreateNodesForBST(string filePath, BinarySearchTree bst, string whereClause)
        {
            int columnCount; // Number of columns
            var columnTypes = new List<ColumnType>(); // List of ColumnTypes
            var columnPadRight = new List<int>(); // List of padding lengths
            var columnNames = new List<string>(); // List of column titles
            int wordLength; // Varible that holds the lenght of the words

            // List to store attributes of a node -row of a table-
            List<object> attributesForNode = new List<object>();
                                    
            using (FileStream stream = new FileStream(filePath, FileMode.Open))
            using (BinaryReader reader = new BinaryReader(stream))
            {
                columnCount = reader.ReadInt32(); // Read and holds the number of columns in the table

                // Read and store the metadata for each column
                Console.WriteLine("\nWriterTree---");
                for (int i = 0; i < columnCount; i++)
                {
                    columnTypes.Add((ColumnType)reader.ReadInt32());
                    Console.WriteLine($"(Type) {columnTypes[i]}");

                    wordLength = reader.ReadInt32();
                    columnPadRight.Add(wordLength);
                    Console.WriteLine($"(PadRight) {columnPadRight[i]}");

                    int charCount = (wordLength == -1) ? 50 : wordLength;
                    columnNames.Add(new string(reader.ReadChars(charCount)));
                    Console.WriteLine($"(Column Title) {columnNames[i]}");
                }
                Console.WriteLine("--------------");

                // Loop to read each row of data from the file
                while (reader.BaseStream.Position != reader.BaseStream.Length)
                {
                    int intValueIndicator = reader.ReadInt32();

                    for (int i = 1; i < columnCount; i++)
                    {
                        switch(columnTypes[i])
                        {
                            case ColumnType.Integer:
                                int colInt = reader.ReadInt32();
                                attributesForNode.Add(colInt);
                                break;

                            case ColumnType.String:
                                string colStr = reader.ReadString().PadRight(columnPadRight[i]);
                                attributesForNode.Add(colStr);
                                break;

                            case ColumnType.DateTime:
                                long ticks = reader.ReadInt64(); 
                                attributesForNode.Add(new DateTime(ticks));
                                break;

                            default:
                                // Handle error when unsupported column type is encountered
                                Console.WriteLine("!Error : UnsupportedColumnType in Tree");
                                throw new InvalidOperationException($"Unsupported column type find in Tree");
                        }
                    }

                    // Add the node to the BST with its indicator and attributes
                    bst.Add(intValueIndicator, attributesForNode);

                    // Server verification
                    Console.WriteLine("\n--------------");
                    Console.Write($"NODE : {intValueIndicator}");
                    Console.WriteLine("--------------");
                    foreach (object objeto in attributesForNode)
                    {
                        Console.Write($"-> {objeto}");
                    }
                    Console.WriteLine("--------------");

                    // Clear the attributes list to prepare for the next row
                    attributesForNode.Clear();

                }
            }

            // Get the nodes that match the whereClause condition and delete them from the BST
            List<Nodo> ForDeletion = bst.GetNodesThat(whereClause, columnTypes, columnNames );

            return ForDeletion;
        }
    }
}