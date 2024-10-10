using BinaryTableManager.TableManagement;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BinaryTableManager.TreeStructure
{
    public class BinarySelect
    {
        public static void SelectNodes(string filePath, BinarySearchTree bst, string[] columnEntries, string whereClause, string orderClause)
        {
            // Server verification
            Console.WriteLine("\nSELECTION PROCESS HAS STARTED...");
            if (whereClause != null) Console.WriteLine($"[ WHERE {whereClause} ]");

            List<Nodo> Nodes = new List<Nodo>(); // List to hold the selected nodes
            List<string> DataToPrint = new List<string>(); // List to hold data for output

            try
            {
                if (whereClause == null) // Verifies WHERE clause existence
                {
                    if (orderClause == null) // Verifies ORDER clause existence
                    {
                        // If no order clause, create nodes directly from the binary search tree
                        Nodes = BinaryTreeInitializer.CreateNodesForBST(filePath, bst, null);
                    }
                    else
                    {
                        // Create nodes and then arrange them based on the order clause
                        List<Nodo> DisorderedNodes = BinaryTreeInitializer.CreateNodesForBST(filePath, bst, null);
                        Nodes = BinarySearchTree.ArrangeNodes(DisorderedNodes, orderClause);
                    }
                }
                else
                {
                    if (orderClause == null)
                    {
                        Nodes = BinaryTreeInitializer.CreateNodesForBST(filePath, bst, whereClause);
                    }
                    else
                    {
                        // Create nodes based on the WHERE clause
                        List<Nodo> DisorderedNodes = BinaryTreeInitializer.CreateNodesForBST(filePath, bst, whereClause);
                        Nodes = BinarySearchTree.ArrangeNodes(DisorderedNodes, orderClause);
                    }
                }
            }
            catch (Exception ex)
            {
                // Catch and display any exceptions 
                Console.WriteLine($"\n!Error : {ex.Message}");
            }

            // Extract nodes attributes
            foreach (Nodo node in Nodes)
            {
                for (int i = 0; i < node.GetTotalAttributesCount(); i++)
                {
                    var attribute = node.GetAttribute(i);
                    if (attribute != null)
                    {
                        DataToPrint.Add(attribute.ToString()); // Add attribute to the data list
                    }
                }
            }
            int columnCount;
            var columnTypes = new List<ColumnType>();
            var columnPadRight = new List<int>();
            var columnNames = new List<string>();
            int wordLength;

            using (FileStream stream = new FileStream(filePath, FileMode.Open))
            using (BinaryReader reader = new BinaryReader(stream))
            {
                columnCount = reader.ReadInt32(); // Read the number of columns

                for (int i = 0; i < columnCount; i++)
                {
                    // Column Type
                    columnTypes.Add((ColumnType)reader.ReadInt32());
                    
                    // Word lenght for padding
                    wordLength = reader.ReadInt32();
                    columnPadRight.Add(wordLength);

                    // Store & Read column name
                    int charCount = (wordLength == -1) ? 50 : wordLength;
                    columnNames.Add(new string(reader.ReadChars(charCount)));
                }
            }

            string DirectoryNameForTXT = GetLastSectionOfPath(filePath);

            SaveDataToTxt(DirectoryNameForTXT, columnNames, DataToPrint);
        }

        static string GetLastSectionOfPath(string path)
        {
            int lastBackslashIndex = path.LastIndexOf('\\');

            if (lastBackslashIndex == -1)
            {
                return path;
            }

            return path.Substring(lastBackslashIndex + 1);
        }
        static void SaveDataToTxt(string fileName, List<string> columnNames, List<string> data)
        {
            // Definir la ruta donde se guardará el archivo
            string path = @"C:\Users\ejcan\Desktop\U\FSC\Proyecto 2\TinySQLDb\SavedTables";

            // Asegurarse de que la carpeta existe
            Directory.CreateDirectory(path);

            // Construir la ruta completa del archivo
            string fullPath = Path.Combine(path, fileName);

            using (StreamWriter writer = new StreamWriter(fullPath))
            {
                // Escribir los nombres de las columnas
                writer.WriteLine(string.Join(" ", columnNames));

                // Calcular la longitud máxima de cada columna
                List<int> maxLengths = new List<int>(columnNames.Select(c => c.Length));

                // Calcular la longitud máxima de los datos por columna
                for (int i = 0; i < data.Count; i += columnNames.Count)
                {
                    for (int j = 0; j < columnNames.Count; j++)
                    {
                        if (i + j < data.Count) // Verificar que no exceda el rango
                        {
                            maxLengths[j] = Math.Max(maxLengths[j], data[i + j].Length);
                        }
                    }
                }

                // Ajustar los guiones según la longitud del contenido de cada columna
                List<string> separator = new List<string>();
                for (int i = 0; i < maxLengths.Count; i++)
                {
                    int dashCount = Math.Max(2, maxLengths[i]); // Asegura que al menos haya 2 guiones
                    separator.Add(new string('-', dashCount));
                }

                writer.WriteLine(string.Join(" ", separator));

                // Escribir los datos en líneas
                for (int i = 0; i < data.Count; i += columnNames.Count)
                {
                    // Obtener una fila de datos
                    var rowData = data.Skip(i).Take(columnNames.Count);
                    // Alinear cada dato según la longitud máxima
                    var alignedRowData = rowData.Select((value, index) => value.PadRight(maxLengths[index]));
                    writer.WriteLine(string.Join(" ", alignedRowData));
                }
            }

            Console.WriteLine($"Los datos se han guardado en {fullPath}");
        }

    }
}   