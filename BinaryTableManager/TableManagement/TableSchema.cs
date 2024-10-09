namespace BinaryTableManager.TableManagement
{
    public class TableSchema
    {
        // Names, types, and padding information of the table
        public string[] ColumnNames { get; }
        public ColumnType[] ColumnTypes { get; }
        public int[] ColumnPadRight { get; }

        // Constructor to initialize the properties
        public TableSchema(string[] columnNames, ColumnType[] columnTypes, int[] columnPadRight)
        {
            ColumnNames = columnNames;
            ColumnTypes = columnTypes;
            ColumnPadRight = columnPadRight;
        }
    }
}
