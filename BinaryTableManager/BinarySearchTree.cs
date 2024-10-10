using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using BinaryTableManager;
using NCalc;


namespace BST
{
    // Node class represents each node in the binary search tree (BST).
    public class Nodo
    {
        public int Data;  // Integer for forming the BST structure.
        public Nodo Left;  // Left child node.
        public Nodo Right; // Right child node.
        public List<object> Attributes;  // List of attributes (string, int, or DateTime).

        // Constructor that initializes the node with data and a list of attributes.
        // Only string, int, or DateTime attributes are allowed.
        public Nodo(int data, List<object> attributes) //[ID, Color, Marca]
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
                return Data;  // Return the data when index is 0.
            }
            if (index > 0 && index <= Attributes.Count)  // Corrected to handle the case where index is in bounds.
            {
                return Attributes[index - 1];  // Return the attribute at the given index.
            }
            throw new IndexOutOfRangeException("Index out of range.");  // Handle invalid index.
        }


        public void SetAttribute(int index, object NewValue)
        {
            if (index == 0)
            {  
                int NewNum = (int)NewValue;
                Data = NewNum; //Eception for value not to be int needed
            }
            if (index <= GetTotalAttributesCount()) //Plus one means 
            {
                Attributes[index - 1] = NewValue;  // Return the attribute at the given index.
            }
        }

        // Override the ToString() method to display node data and its attributes.
        public override string ToString()
        {
            return $"Data: {Data}, Attributes: [{string.Join(", ", Attributes)}]";
        }

        public int GetTotalAttributesCount()
        {
            return Attributes.Count + 1;
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
        public List<Nodo> GetNodesThat(string whereClause, List<ColumnType> columnTypes, List<string> columnNames)
        {
            if (whereClause != null)
            {
                var result = new List<Nodo>();
                GetNodesThatRec(Root, whereClause, result, columnTypes, columnNames);
                return result;
            }
            else
            {
                return GetAllNodes();
            }

        }

        private void GetNodesThatRec(Nodo node, string whereClause, List<Nodo> result, List<ColumnType> columnTypes, List<string> columnNames)
        {
            if (node == null) return;

            var datos = new Dictionary<string, object>();


            for (int i = 0; i < columnTypes.Count; i++)
            {
                string columnName = columnNames[i].Trim();  
                switch (columnTypes[i])
                {
                    case ColumnType.Integer:
                        int intValueAttr = (int)node.GetAttribute(i);
                        datos[columnName] = intValueAttr;
                        Console.WriteLine($"{columnName}: {intValueAttr}");
                        break;

                    case ColumnType.String:
                        string stringValueAttr = ((string)node.GetAttribute(i)).Trim();  
                        datos[columnName] = stringValueAttr;
                        Console.WriteLine($"{columnName}: {stringValueAttr}");
                        break;

                    case ColumnType.DateTime:
                        DateTime dateValueAttr = (DateTime)node.GetAttribute(i);
                        datos[columnName] = dateValueAttr;
                        Console.WriteLine($"{columnName}: {dateValueAttr}");
                        break;
                }
            }

            var expression = new NCalc.Expression(whereClause);

            foreach (var columnName in datos.Keys)
            {
                expression.Parameters[columnName] = datos[columnName];
            }

            Console.WriteLine("Parámetros en la expresión:");
            foreach (var param in expression.Parameters)
            {
                Console.WriteLine($"Clave: '{param.Key}', Valor: {param.Value}, Tipo: {param.Value.GetType()}");
            }

            try
            {
                bool conditionMet = (bool)expression.Evaluate();

                Console.WriteLine($"Evaluación de expresión resultó en: {conditionMet}");

                if (conditionMet)
                {
                    result.Add(node);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al evaluar la expresión: {ex.Message}");
            }

            // Recursivamente procesar los nodos hijos
            GetNodesThatRec(node.Left, whereClause, result, columnTypes, columnNames);
            GetNodesThatRec(node.Right, whereClause, result, columnTypes, columnNames);
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

        public static List<Nodo> ArrangeNodes(List<Nodo> listaNodos, string orden)
        {
            // Llama al método recursivo QuickSort
            return QuickSort(listaNodos, 0, listaNodos.Count - 1, orden);
        }

        // Método QuickSort recursivo
        private static List<Nodo> QuickSort(List<Nodo> lista, int low, int high, string orden)
        {
            if (low < high)
            {
                // Encuentra el índice de partición
                int pi = Partition(lista, low, high, orden);

                // Llama recursivamente a QuickSort en las dos particiones
                QuickSort(lista, low, pi - 1, orden);
                QuickSort(lista, pi + 1, high, orden);
            }

            return lista;
        }

        // Método de partición
        private static int Partition(List<Nodo> lista, int low, int high, string orden)
        {
            // Usamos el último elemento como pivote
            int pivot = Convert.ToInt32(lista[high].GetAttribute(0)); // Convertimos a int
            int i = (low - 1); // Índice de elementos más pequeños

            for (int j = low; j < high; j++)
            {
                // Compara según el orden especificado (ASC o DESC)
                bool condition = orden == "ASC"
                    ? Convert.ToInt32(lista[j].GetAttribute(0)) < pivot // Convertimos a int
                    : Convert.ToInt32(lista[j].GetAttribute(0)) > pivot; // Convertimos a int

                if (condition)
                {
                    i++;
                    // Intercambia lista[i] y lista[j]
                    Swap(lista, i, j);
                }
            }

            // Intercambia el pivote con el elemento que sigue a los más pequeños
            Swap(lista, i + 1, high);
            return i + 1;
        }

        // Método para intercambiar dos nodos en la lista
        private static void Swap(List<Nodo> lista, int i, int j)
        {
            Nodo temp = lista[i];
            lista[i] = lista[j];
            lista[j] = temp;
        }
    }
}