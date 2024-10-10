using BinaryTableManager.TreeStructure;
namespace BinaryTableManager.TableManagement
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
                List<Nodo> ToBeDeleted = BinaryTreeInitializer.CreateNodesForBST(tablePath, bst, whereClause); //This is a list without nodes to be deleted.
                foreach (Nodo nodo in ToBeDeleted)
                {
                    int IndicatorValue = (int)nodo.GetAttribute(0);
                    bst.Delete(IndicatorValue);
                }
                List<Nodo> NodesToWirte = bst.GetAllNodes();

                CleanBinaryTable.CleanPath(tablePath);
                
                if (NodesToWirte != null)
                {
                    foreach (Nodo node in NodesToWirte)
                    {
                        int Lenght = NodesToWirte[0].GetTotalAttributesCount();
                        string[] Data = new string[Lenght];
                        for (int i = 0; i < Lenght; i++)
                        {
                            Data[i] = node.GetAttribute(i).ToString();

                        }
                        BinaryTableEditor.BinaryInsertRow(Data, tablePath);
                    }

                }

            }


        }
    }
}