using BST;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BinaryTableManager
{
    public class BinarySelect
    {
        public static void SelectNodes(string filePath, BinarySearchTree bst, string[] columnEntries, string whereClause, string orderClause)
        {
            Console.WriteLine("La where clause es: " + whereClause);
            List<Nodo> Nodes = new List<Nodo>();
            List<string> DataToPrint = new List<string>();

            try
            {
                if (whereClause == null)
                {
                    if (orderClause == null)
                    {
                        Nodes = BinaryTreeInitializer.CreateNodesForBST(filePath, bst, null);
                    }
                    else
                    {
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
                        List<Nodo> DisorderedNodes = BinaryTreeInitializer.CreateNodesForBST(filePath, bst, whereClause);
                        Nodes = BinarySearchTree.ArrangeNodes(DisorderedNodes, orderClause);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ocurrió un error al procesar la cláusula WHERE: {ex.Message}");
            }

            foreach (Nodo node in Nodes)
            {
                for (int i = 0; i < node.GetTotalAttributesCount(); i++)
                {
                    var attribute = node.GetAttribute(i);
                    if (attribute != null)
                    {
                        DataToPrint.Add(attribute.ToString());
                    }
                }
            }

            // Debugging Output
            Console.WriteLine("Datos a imprimir:");
            foreach (var dataItem in DataToPrint)
            {
                Console.WriteLine(dataItem); // Verifica que haya datos
            }

            int columnCount;
            var columnTypes = new List<ColumnType>();
            var columnPadRight = new List<int>();
            var columnNames = new List<string>();
            int wordLength;

            using (FileStream stream = new FileStream(filePath, FileMode.Open))
            using (BinaryReader reader = new BinaryReader(stream))
            {
                columnCount = reader.ReadInt32();

                for (int i = 0; i < columnCount; i++)
                {
                    columnTypes.Add((ColumnType)reader.ReadInt32());
                    wordLength = reader.ReadInt32();
                    columnPadRight.Add(wordLength);

                    int charCount = (wordLength == -1) ? 50 : wordLength;
                    columnNames.Add(new string(reader.ReadChars(charCount)));
                }
            }

            string DirectoryNameForTXT = GetLastSectionOfPath(filePath);

            // Filtrar columnas a imprimir
            var filteredColumnNames = columnNames.Where(c => columnEntries.Contains(c)).ToList();

            // Imprimir para depuración
            Console.WriteLine("Columnas filtradas:");
            foreach (var column in filteredColumnNames)
            {
                Console.WriteLine(column);
            }

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

                // Escribir los guiones alineados con los encabezados y el contenido
                writer.WriteLine(string.Join(" ", maxLengths.Select(length => new string('-', length))));

                // Escribir los datos en líneas
                for (int i = 0; i < data.Count; i += columnNames.Count)
                {
                    // Obtener una fila de datos
                    var rowData = data.Skip(i).Take(columnNames.Count);
                    writer.WriteLine(string.Join(" ", rowData));
                }
            }

            Console.WriteLine($"Los datos se han guardado en {fullPath}");
        }
      
    }
}
