using BinaryTableManager.TableManagement;

namespace BinaryTableManager.TreeStructure
{
    public class BinaryDelet
    {
        public static void DeleteNodes(string tablePath, BinarySearchTree bst, string whereClause)
        {
            if (whereClause == null)
            {
                CleanBinaryTable.CleanPath(tablePath);
            }
            else
            {
                // Create nodes for the Binary Search Tree based on the whereClause
                List<Nodo> NodesToWirte = BinaryTreeInitializer.CreateNodesForBST(tablePath, bst, whereClause); 
                
                // Clean the binary table after selecting nodes
                CleanBinaryTable.CleanPath(tablePath);

                if (NodesToWirte != null)
                {
                    // Iterate through the nodes to write them back to the table
                    foreach (Nodo node in NodesToWirte)
                    {
                        int Lenght = NodesToWirte[0].GetTotalAttributesCount();
                        string[] Data = new string[Lenght]; // Array to hold the data

                        for (int i = 0; i < Lenght; i++)
                        {
                            Data[i] = node.GetAttribute(i).ToString();

                        }

                        // Insert the row into the binary table
                        BinaryTableEditor.BinaryInsertRow(Data, tablePath);
                    }
                }
            }
        }
    }
}