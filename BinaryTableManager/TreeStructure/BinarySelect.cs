using System.Collections.Generic;
using System.IO;
using System.Linq;
using BinaryTableManager.TableManagement;

namespace BinaryTableManager.TreeStructure
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

            SaveDataToTxt("DataToPrint.txt", columnNames, DataToPrint, columnEntries, columnTypes);
        }

        static void SaveDataToTxt(string fileName, List<string> columnNames, List<string> data, string[] columnEntries, List<ColumnType> columnTypes)
        {
            string path = @"C:\Users\ejcan\Desktop\U\FSC\Proyecto 2\TinySQLDb\SavedTables";
            Directory.CreateDirectory(path);
            string fullPath = Path.Combine(path, fileName);

            // Verificar si el primer elemento de columnEntries es "*"
            List<int> selectedColumnIndices;
            if (columnEntries.Length > 0 && columnEntries[0] == "*")
            {
                // Si es "*", seleccionar todas las columnas
                Console.WriteLine("Se seleccionaron todas las columnas.");
                selectedColumnIndices = Enumerable.Range(0, columnNames.Count).ToList();
            }
            else
            {
                // Seleccionar índices de columnas según los encabezados
                selectedColumnIndices = columnNames
                    .Select((col, index) => new { col, index })
                    .Where(x => columnEntries.Any(entry => x.col.StartsWith(entry)))
                    .Select(x => x.index)
                    .ToList();
            }

            using (StreamWriter writer = new StreamWriter(fullPath))
            {
                // Escribir encabezados
                writer.WriteLine(string.Join(" ", selectedColumnIndices.Select(index => columnNames[index])));

                // Inicializar longitudes máximas de columnas según encabezados
                List<int> maxLengths = selectedColumnIndices.Select(index => columnNames[index].Length).ToList();

                // Calcular longitudes máximas para los datos de las columnas seleccionadas
                for (int rowIndex = 0; rowIndex < data.Count; rowIndex += columnNames.Count)
                {
                    for (int colIndex = 0; colIndex < selectedColumnIndices.Count; colIndex++)
                    {
                        int columnIndex = selectedColumnIndices[colIndex];

                        // Asegurarse de no exceder el tamaño de la lista de datos
                        if (rowIndex + columnIndex < data.Count)
                        {
                            // Verificar el tipo de columna
                            maxLengths[colIndex] = Math.Max(maxLengths[colIndex], data[rowIndex + columnIndex].Length);
                        }
                    }
                }

                // Crear el separador usando las longitudes de `selectedColumnIndices`
                List<string> separator = maxLengths.Select(length => new string('-', length)).ToList();
                writer.WriteLine(string.Join(" ", separator));

                // Escribir los datos alineados
                for (int rowIndex = 0; rowIndex < data.Count; rowIndex += columnNames.Count)
                {
                    List<string> alignedRowData = new List<string>();

                    for (int colIndex = 0; colIndex < selectedColumnIndices.Count; colIndex++)
                    {
                        int columnIndex = selectedColumnIndices[colIndex];
                        string value = (rowIndex + columnIndex < data.Count) ? data[rowIndex + columnIndex] : string.Empty;
                        alignedRowData.Add(value.PadRight(maxLengths[colIndex])); // Alineación correcta
                    }
                    writer.WriteLine(string.Join(" ", alignedRowData));
                }
            }
            FormatTable(fullPath);
            Console.WriteLine($"Los datos se han guardado en {fullPath}");
        }

        public static void FormatTable(string filePath)
        {
            string[] lines = File.ReadAllLines(filePath);

            // Leer encabezados
            string[] headers = lines[0].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var tableData = new string[lines.Length - 2][];

            // Ignorar la segunda línea de guiones y procesar las filas restantes
            for (int i = 2; i < lines.Length; i++)
            {
                tableData[i - 2] = lines[i].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            }

            // Calcular el ancho máximo de cada columna
            int[] columnWidths = headers
                .Select((header, index) => Math.Max(header.Length, tableData.Max(row => row[index].Length)))
                .ToArray();

            // Escribir la tabla formateada
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                // Escribir encabezados
                for (int i = 0; i < headers.Length; i++)
                {
                    writer.Write(headers[i].PadRight(columnWidths[i] + 1));
                }
                writer.WriteLine();

                // Escribir separadores de guiones
                for (int i = 0; i < columnWidths.Length; i++)
                {
                    writer.Write(new string('-', columnWidths[i]).PadRight(columnWidths[i] + 1));
                }
                writer.WriteLine();

                // Escribir filas de la tabla
                foreach (var row in tableData)
                {
                    for (int i = 0; i < row.Length; i++)
                    {
                        writer.Write(row[i].PadRight(columnWidths[i] + 1));
                    }
                    writer.WriteLine();
                }
            }
        }

    }
}   