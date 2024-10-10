using BST;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BinaryTableManager
{
    public class BinaryUpdate
    {
        public static void UpdateRows(string tablePath, BinarySearchTree bst, string whereClause, string[] selectedColumns)
        {
            if (whereClause == null)
            {
                List<Nodo> NodesToModify = BinaryTreeInitializer.CreateNodesForBST(tablePath, bst, null); 
                foreach (Nodo node in NodesToModify)
                {
                    List<object> AtrbiuteList = new List<object>();
                    for (int i = 1; i < node.GetTotalAttributesCount(); i++)
                    {
                        AtrbiuteList.Add(node.GetAttribute(i));
                    }
                    bst.Add((int)node.GetAttribute(0), AtrbiuteList);
                }

                CleanBinaryTable.CleanPath(tablePath);

                List<Nodo> NodesToWirte = bst.GetAllNodes();

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
            else
            {
                int columnCount;
                var columnTypes = new List<ColumnType>();
                var columnPadRight = new List<int>();
                var columnNames = new List<string>();
                int wordLength;
                List<Nodo> NodesToModify = BinaryTreeInitializer.CreateNodesForBST(tablePath, bst, whereClause); 
                foreach (Nodo nodo in NodesToModify)
                {
                    int IndicatorValue = (int)nodo.GetAttribute(0);
                    bst.Delete(IndicatorValue);
                }

                using (FileStream stream = new FileStream(tablePath, FileMode.Open))
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

                for (int i = 0; i < columnCount; i++)
                {   
                    if (columnNames[i].StartsWith(selectedColumns[0])) 
                    {
                        foreach (Nodo node in NodesToModify)
                        {
                            node.SetAttribute(i, selectedColumns[1]);
                        }
                    }
                }

                foreach (Nodo node in NodesToModify)
                {
                    List<object> AtrbiuteList = new List<object>();
                    for (int i = 1; i < node.GetTotalAttributesCount(); i++)
                    {
                        AtrbiuteList.Add(node.GetAttribute(i));
                    }
                    bst.Add((int)node.GetAttribute(0), AtrbiuteList);
                }

                CleanBinaryTable.CleanPath(tablePath);

                List<Nodo> NodesToWirte = bst.GetAllNodes();

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