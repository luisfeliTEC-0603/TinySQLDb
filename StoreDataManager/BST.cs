namespace BST
{
    using System;
    using System.Collections.Generic;
    using System.Security.Cryptography;
    using NCalc;

    // Node class represents each node in the binary search tree (BST).
    public class Nodo
    {
        public int Data;  // Integer for forming the BST structure.
        public Nodo Left;  // Left child node.
        public Nodo Right; // Right child node.
        public List<object> Attributes;  // List of attributes (string, int, or DateTime).

        // Constructor that initializes the node with data and a list of attributes.
        // Only string, int, or DateTime attributes are allowed.
        public Nodo(int data, List<object> attributes)
        {
            Data = data;
            Attributes = new List<object>();

            // Validate that only string, int, or DateTime are added as attributes.
            foreach (var attr in attributes)
            {
                if (attr is string || attr is int || attr is DateTime)
                {
                    Attributes.Add(attr);
                }
                else
                {
                    throw new ArgumentException("Only string, int, or DateTime types are allowed as attributes.");
                }
            }

            Left = null;  // Initialize left child as null.
            Right = null; // Initialize right child as null.
        }

        // Method to retrieve an attribute by index from the attributes list.
        // Throws an exception if the index is out of bounds.
        public object GetAttribute(int index)
        {
            if (index == 0)
            {
                return Data;
            }
            if (index > 0 && index < Attributes.Count)
            {
                return Attributes[index];  // Return the attribute at the given index.
            }
            throw new IndexOutOfRangeException("Index out of range.");  // Handle invalid index.
        }

        // Override the ToString() method to display node data and its attributes.
        public override string ToString()
        {
            return $"Data: {Data}, Attributes: [{string.Join(", ", Attributes)}]";
        }
    }
    
    // Class representing the Binary Search Tree (BST).
    public class BinarySearchTree
    {
        public Nodo Root;  // Root node of the tree.

        public BinarySearchTree()
        {
            Root = null;  // Initialize the root as null when the tree is empty.
        }

        // Method to add a new node to the BST.
        // Takes integer data and a list of attributes.
        public void Add(int data, List<object> attributes)
        {
            Root = AddRecursive(Root, data, attributes);  // Call recursive method to add the node.
        }

        // Recursive method to add a node to the BST in the correct position.
        private Nodo AddRecursive(Nodo node, int data, List<object> attributes)
        {
            // If the node is null, create a new node.
            if (node == null)
            {
                return new Nodo(data, attributes);
            }

            // If the data is smaller, insert in the left subtree.
            if (data < node.Data)
            {
                node.Left = AddRecursive(node.Left, data, attributes);
            }
            // If the data is larger, insert in the right subtree.
            else if (data > node.Data)
            {
                node.Right = AddRecursive(node.Right, data, attributes);
            }

            // If the data is equal, we can decide if we want to update (optional).
            return node;  // Return the unchanged node pointer.
        }

        // Method to delete a node from the BST.
        public void Delete(int data)
        {
            Root = DeleteRecursive(Root, data);  // Call recursive method to delete the node.
        }

        // Recursive method to delete a node from the BST.
        private Nodo DeleteRecursive(Nodo node, int data)
        {
            if (node == null)
            {
                return null;  // Node not found.
            }

            // Traverse the tree to find the node to delete.
            if (data < node.Data)
            {
                node.Left = DeleteRecursive(node.Left, data);
            }
            else if (data > node.Data)
            {
                node.Right = DeleteRecursive(node.Right, data);
            }
            else
            {
                // Node found. Perform deletion.

                // Case 1: Node has no children (leaf node).
                if (node.Left == null && node.Right == null)
                {
                    return null;  // Simply remove the node.
                }

                // Case 2: Node has one child.
                if (node.Left == null)
                {
                    return node.Right;  // Replace the node with its right child.
                }
                else if (node.Right == null)
                {
                    return node.Left;  // Replace the node with its left child.
                }

                // Case 3: Node has two children.
                // Find the smallest node in the right subtree (in-order successor).
                node.Data = MinValue(node.Right);

                // Delete the successor.
                node.Right = DeleteRecursive(node.Right, node.Data);
            }

            return node;  // Return the updated node.
        }

        // Helper method to find the minimum value in a subtree.
        private int MinValue(Nodo node)
        {
            int minValue = node.Data;  // Initialize the minimum value with the node's data.
            while (node.Left != null)
            {
                minValue = node.Left.Data;  // Move to the left to find the smallest value.
                node = node.Left;
            }
            return minValue;
        }

        // Method to perform in-order traversal of the BST.
        // This prints the nodes in ascending order.
        public void InOrderTraversal()
        {
            InOrderRecursive(Root);  // Call recursive method for in-order traversal.
            Console.WriteLine();  // Print a new line after traversal.
        }

        // Recursive method for in-order traversal.
        private void InOrderRecursive(Nodo node)
        {
            if (node != null)
            {
                InOrderRecursive(node.Left);  // Visit left subtree.
                Console.WriteLine(node.ToString());  // Print the current node's data and attributes.
                InOrderRecursive(node.Right);  // Visit right subtree.
            }
        }


        // Return nodes that agree with bool
        public List<Nodo> GetNodesThat(bool expresion, List<string> columns)
        {
            var result = new List<Nodo>();
            GetNodesThatRec(Root, expresion, result, columns);
            return result;
        }

        private void GetNodesThatRec(Nodo node, bool expresion, List<Nodo> result, List<string> columns) //This function must tell which nodes
        //make the expresion true.
        {
            if (node == null) return;

            var datos = new Dictionary<string, object>();

            for (int i = 0; i < columns.Count; i++)
            {
                try
                {
                    int number = (int)node.GetAttribute(i);
                    datos[columns[i]] = new List<object> { number };
                }
                catch
                {
                    try
                    {
                        string Data = (string)node.GetAttribute(i);
                        datos[columns[i]] = new List<object> { Data };
                    }
                    catch
                    {
                        try
                        {
                            DateTime date = (DateTime)node.GetAttribute(i);
                            datos[columns[i]] = new List<object> { date };
                        }
                        catch
                        {
                            continue;
                        }
                    }
                }
            }

            if (expresion)
            {
                result.Add(node);
            }

            GetNodesThatRec(node.Left, expresion, result, columns);
            GetNodesThatRec(node.Right, expresion, result, columns);
        }
        public List<Nodo> GetAllNodes() //Return all nodes of tree
        {
            List<Nodo> nodos = new List<Nodo>();
            RecusriveGetAllNodes(Root, nodos);
            return nodos;
        }

        private void RecusriveGetAllNodes(Nodo nodo, List<Nodo> nodos)
        {
            if (nodo != null)
            {
                RecusriveGetAllNodes(nodo.Left, nodos); 
                nodos.Add(nodo); 
                RecusriveGetAllNodes(nodo.Right, nodos); 
            }
        }
    }
}
