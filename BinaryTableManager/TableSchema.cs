namespace BinaryTableManager
{
    public class TableSchema
    {
        public string[] ColumnNames { get; }
        public ColumnType[] ColumnTypes { get; }
        public int[] ColumnPadRight { get; }

        public TableSchema(string[] columnNames, ColumnType[] columnTypes, int[] columnPadRight)
        {
            ColumnNames = columnNames;
            ColumnTypes = columnTypes;
            ColumnPadRight = columnPadRight;
        }
    }
}

/*
string[] ColumnNames = [ID, NAME, AGE ]
ColumnType[] ColumnTypes = [INTEGER, STRING, DATETIME ]
string[] PadrightDef = [ null, PadRight, null ]
*/

