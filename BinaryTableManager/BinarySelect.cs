using BST;
namespace BinaryTableManager
{
    public class BinarySelect
    {
        public static void SelectNodes(string filePath, BinarySearchTree bst, string[] columnEntries, string whereClause, string orderClause)
        {
            Console.WriteLine("La where clause es: " +whereClause);
            if (whereClause == null)
            {
                if (orderClause == null)
                {
                    List<Nodo> Nodes = BinaryTreeInitializer.CreateNodesForBST(filePath, bst, null);
                    PrintNodes(Nodes);
                    //Write Nodes in txt
                }
                else
                {
                    List<Nodo> Nodes = BinaryTreeInitializer.CreateNodesForBST(filePath, bst, null);
                    List<Nodo> InOrder = BinarySearchTree.ArrangeNodes(Nodes, orderClause);
                    PrintNodes(InOrder);
                    //Write Nodes in txt
                }
            }
            else
            {
                if (orderClause == null)
                {
                    List<Nodo> Nodes = BinaryTreeInitializer.CreateNodesForBST(filePath, bst, whereClause);
                    PrintNodes(Nodes);
                    //Write Nodes in txt
                }
                else
                {
                    List<Nodo> Nodes = BinaryTreeInitializer.CreateNodesForBST(filePath, bst, whereClause);
                    List<Nodo> InOrder = BinarySearchTree.ArrangeNodes(Nodes, orderClause);
                    PrintNodes(InOrder);
                    //Write Nodes in txt
                }
            }
        }
        public static void PrintNodes(List<Nodo> list)
        {
            foreach (Nodo node in list)
            {
                Console.WriteLine((int)node.GetAttribute(0));
            }
        }
    }
}