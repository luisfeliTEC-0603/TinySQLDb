using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using BinaryTableManager.TableManagement;
using NCalc;

namespace BinaryTableManager.TreeStructure
{
    public class Nodo
    {
        public int Data;  // Main integer values for the node
        public Nodo Left;  // Left child node
        public Nodo Right; // Right child node
        public List<object> Attributes;  // List of attributes

        public Nodo(int data, List<object> attributes) // Node Constructor 
        {
            Data = data;
            Attributes = new List<object>();

            // Validate that only string, int, or DateTime are added as attributes
            foreach (var attr in attributes)
            {
                if (attr is string || attr is int || attr is DateTime)
                {
                    Attributes.Add(attr);
                }
                else
                {
                    throw new ArgumentException("!Error : Only string, int, or DateTime types are allowed as attributes");
                }
            }

            // Children initialize as null
            Left = null; 
            Right = null;
        }

        // Retrieves an attribute by index from the attributes list
        public object GetAttribute(int index)
        {
            if (index == 0)
            {
                return Data;
            }
            if (index > 0 && index <= Attributes.Count) 
            {
                return Attributes[index - 1]; 
            }
            throw new IndexOutOfRangeException("\n!Error : Index Out of Range");
        }

        // Sets an attribute at the specified index
        public void SetAttribute(int index, object NewValue)
        {
            if (index == 0)
            {  
                int NewNum = (int)NewValue;
                Data = NewNum;
            }
            if (index < Attributes.Count)
            {
                Attributes[index - 1] = NewValue;
            }
            throw new IndexOutOfRangeException("\n!Error : Index Out of Range");
        }

        public override string ToString()
        {
            return $"Data: {Data}, Attributes: [{string.Join(", ", Attributes)}]";
        }

        // Returns the amount of attributes hold by tne node
        public int GetTotalAttributesCount()
        {
            return Attributes.Count + 1;
        }
    }
    
    public class BinarySearchTree
    {
        public Nodo Root; 

        public BinarySearchTree()
        {
            Root = null; 
        }

        // Adds a new node to the BST
        public void Add(int data, List<object> attributes)
        {
            Root = AddRecursive(Root, data, attributes); 
        }

        // Recursive method to add a node to the BST
        private Nodo AddRecursive(Nodo node, int data, List<object> attributes)
        {
            if (node == null)
            {
                return new Nodo(data, attributes);
            }

            if (data < node.Data)
            {
                node.Left = AddRecursive(node.Left, data, attributes);
            }

            else if (data > node.Data)
            {
                node.Right = AddRecursive(node.Right, data, attributes);
            }

            return node; 
        }

        // Deletes a node from the BST.
        public void Delete(int data)
        {
            Root = DeleteRecursive(Root, data);
        }

        // Recursive method to delete a node from the BST
        private Nodo DeleteRecursive(Nodo node, int data)
        {
            if (node == null)
            {
                return null;
            }

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
                if (node.Left == null && node.Right == null)
                {
                    return null;
                }

                if (node.Left == null)
                {
                    return node.Right;
                }

                else if (node.Right == null)
                {
                    return node.Left; 
                }

                node.Data = MinValue(node.Right);

                node.Right = DeleteRecursive(node.Right, node.Data);
            }

            return node; 
        }

        // Finds the minimum value in a subtree
        private int MinValue(Nodo node)
        {
            int minValue = node.Data; 

            while (node.Left != null)
            {
                minValue = node.Left.Data;
                node = node.Left;
            }
            return minValue;
        }

        // Performs in-order traversal of the BST
        public void InOrderTraversal()
        {
            InOrderRecursive(Root); 
            Console.WriteLine(); 
        }

        // Recursive method for in-order traversal
        private void InOrderRecursive(Nodo node)
        {
            if (node != null)
            {
                InOrderRecursive(node.Left);  
                Console.WriteLine(node.ToString());  
                InOrderRecursive(node.Right);  
            }
        }

        // Returns nodes that match a specified condition
        public List<Nodo> GetNodesThat(string whereClause, List<ColumnType> columnTypes, List<string> columnNames)
        {
            var result = new List<Nodo>();
            GetNodesThatRec(Root, whereClause, result, columnTypes, columnNames);
            return result;
        }

        // Recursive method to find nodes that match a condition
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
        
        //Return all nodes of tree
        public List<Nodo> GetAllNodes()
        {
            List<Nodo> nodos = new List<Nodo>();
            RecusriveGetAllNodes(Root, nodos);
            return nodos;
        }

        // Recursive method to get all nodes
        private void RecusriveGetAllNodes(Nodo nodo, List<Nodo> nodos)
        {
            if (nodo != null)
            {
                RecusriveGetAllNodes(nodo.Left, nodos); 
                nodos.Add(nodo); 
                RecusriveGetAllNodes(nodo.Right, nodos); 
            }
        }

        // Sorts nodes using QuickSort
        public static List<Nodo> ArrangeNodes(List<Nodo> listaNodos, string orden)
        {
            return QuickSort(listaNodos, 0, listaNodos.Count - 1, orden);
        }

        // Recursive QuickSort method
        private static List<Nodo> QuickSort(List<Nodo> lista, int low, int high, string orden)
        {
            if (low < high)
            {
                int pi = Partition(lista, low, high, orden);
                QuickSort(lista, low, pi - 1, orden);
                QuickSort(lista, pi + 1, high, orden);
            }

            return lista;
        }

        // Partition method for QuickSort
        private static int Partition(List<Nodo> lista, int low, int high, string orden)
        {
            int pivot = Convert.ToInt32(lista[high].GetAttribute(0));
            int i = (low - 1);

            for (int j = low; j < high; j++)
            {
                bool condition = orden == "ASC"
                    ? Convert.ToInt32(lista[j].GetAttribute(0)) < pivot
                    : Convert.ToInt32(lista[j].GetAttribute(0)) > pivot;

                if (condition)
                {
                    i++;
                    Swap(lista, i, j);
                }
            }

            Swap(lista, i + 1, high);
            return i + 1;
        }

        // Swaps two nodes in the list
        private static void Swap(List<Nodo> lista, int i, int j)
        {
            Nodo temp = lista[i];
            lista[i] = lista[j];
            lista[j] = temp;
        }
    }
}