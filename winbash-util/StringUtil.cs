using System.Text;

namespace winbash.util
    // ReSharper disable once ArrangeNamespaceBody
{
    public static class StringUtil
    {
        public static string Adjust(this string str, int len, bool rightBound = false, bool doFill = true,
            char fill = ' ')
        {
            str = str.Trim();
            var n = len - str.Length;
            if (n < 0) // remove n chars
                if (rightBound)
                    str = str.Substring(0, len);
                else str = str.Substring(Math.Abs(n), str.Length + n);
            else if (n > 0 && doFill)
            {
                // pre-/append n chars
                var extra = string.Empty;
                for (int i = 0; i < n; i++)
                    extra += fill;
                if (rightBound)
                    str = extra + str;
                else str += extra;
            }

            return str;
        }
    }

    public class TextTable
    {
        public readonly List<Column> Columns = new();
        public readonly List<Row> Rows = new();
        private readonly bool _header;
        private readonly bool _lines;

        public TextTable(bool header = true, bool lines = false)
        {
            _header = header;
            _lines = lines;
        }

        public Column AddColumn(string name, bool justifyRight = false)
        {
            var col = new Column(name, justifyRight);
            Columns.Add(col);
            return col;
        }

        public Row AddRow()
        {
            var row = new Row();
            foreach (var col in Columns)
                row._data[col] = string.Empty;
            Rows.Add(row);
            return row;
        }

        public override string ToString()
        {
            var c = Columns.Count;
            var lens = new int[c];
            for (var i = 0; i < c; i++)
            {
                // for each column, collect longest data
                var col = Columns[i];
                foreach (var data in new[] { _header ? col.Name : string.Empty }.Concat(Rows.Select(row =>
                             row._data[col])))
                {
                    var len = data.ToString()!.Length;
                    if (lens[i] < len)
                        lens[i] = len;
                }
            }

            // todo: include outlines & inlines
            var sb = new StringBuilder();
            if (_header)
            {
                for (var i = 0; i < c; i++)
                    sb.Append(Columns[i].Name.Adjust(lens[i])).Append(' ');
                sb.AppendLine();
            }

            foreach (var row in Rows)
            {
                for (var i = 0; i < c; i++)
                {
                    var col = Columns[i];
                    sb.Append(row._data[col].ToString()!.Adjust(lens[i], col._justifyRight)).Append(' ');
                }

                sb.AppendLine();
            }

            return sb.ToString();
        }

        public class Row
        {
            internal readonly Dictionary<Column, object> _data = new Dictionary<Column, object>();

            public Row SetData(Column col, object data)
            {
                _data[col] = data;
                return this;
            }
        }

        public class Column
        {
            public readonly string Name;
            internal readonly bool _justifyRight;

            public Column(string name, bool justifyRight)
            {
                Name = name;
                _justifyRight = justifyRight;
            }
        }
    }
}
