using System;
using System.Collections.Generic;
using System.Linq;

namespace TechShare.Utility.Tools.TableDefinitions
{
    public class FilterItem
    {
        public FilterItem(FilterTypeEnum filterType, FilterValueOperatorEnum valueOperator, string displayText = null)
        {
            FilterType = filterType;
            ValueOperator = valueOperator;
            DisplayText = displayText;
        }
        public FilterTypeEnum FilterType { get; private set; }
        public FilterValueOperatorEnum ValueOperator { get; private set; }
        public string DisplayText { get; private set; }
        private List<string> _columnNames = null;
        public IEnumerable<string> ColumnNames { get { return _columnNames != null ? _columnNames.Where(x => !string.IsNullOrEmpty(x)) : null; } }
        private bool HasColumnNames { get { return ColumnNames != null && ColumnNames.Any(); } }
        public void AddColumn(string columnName)
        {
            if (!string.IsNullOrEmpty(columnName))
            {
                if (_columnNames == null)
                    _columnNames = new List<string>();
                if (!_columnNames.Contains(columnName))
                    _columnNames.Add(columnName);
            }
        }
        public void AddColumns(IEnumerable<string> columnNames)
        {
            if (columnNames != null && columnNames.Any())
            {
                if (_columnNames == null)
                    _columnNames = new List<string>();
                _columnNames.AddRange(columnNames.Except(_columnNames));
            }
        }


        private List<FilterValue> _values = null;
        public IEnumerable<FilterValue> Values { get { return _values != null ? _values.Where(x => x != null) : null; } }
        private bool HasValues { get { return Values != null && Values.Any(); } }
        public void AddValue(FilterComparisonEnum comparisonType, object value)
        {
            if (value != null && !string.IsNullOrEmpty(value.ToString()))
            {
                if (_values == null)
                    _values = new List<FilterValue>();
                if (!_values.Any(x => x.ComparisonType == comparisonType && x.Value == value))
                {
                    _values.Add(new FilterValue() { ComparisonType = comparisonType, Value = value });
                }
            }
        }
        public void AddValues(IEnumerable<FilterValue> values)
        {
            if (values != null && values.Any())
            {
                if (_values == null)
                    _values = new List<FilterValue>();
                _values.AddRange(values.Except(_values));
            }
        }

        public bool HasData { get { return HasColumnNames && HasValues; } }

        public string GetComparisonText(FilterValue filterValue)
        {
            string retVal = null;
            if (filterValue != null && filterValue.Value != null && !string.IsNullOrEmpty(filterValue.Value.ToString()))
            {
                retVal = " = '" + filterValue.Value.ToString() + "'";
                switch (filterValue.ComparisonType)
                {
                    case FilterComparisonEnum.GREATERTHAN:
                        if (filterValue.Value is DateTime)
                            retVal = " > #" + ((DateTime)filterValue.Value).ToShortDateString() + "#";
                        else if (filterValue.Value is string)
                            retVal = " > '" + filterValue.Value.ToString() + "'";
                        else
                            retVal = " > " + filterValue.Value.ToString();
                        break;
                    case FilterComparisonEnum.GREATERTHANOREQUAL:
                        if (filterValue.Value is DateTime)
                            retVal = " >= #" + ((DateTime)filterValue.Value).ToShortDateString() + "#";
                        else if (filterValue.Value is string)
                            retVal = " >= '" + filterValue.Value.ToString() + "'";
                        else
                            retVal = " >= " + filterValue.Value.ToString();
                        break;
                    case FilterComparisonEnum.LESSTHAN:
                        if (filterValue.Value is DateTime)
                            retVal = " < #" + ((DateTime)filterValue.Value).ToShortDateString() + "#";
                        else if (filterValue.Value is string)
                            retVal = " < '" + filterValue.Value.ToString() + "'";
                        else
                            retVal = " < " + filterValue.Value.ToString();
                        break;
                    case FilterComparisonEnum.LESSTHANOREQUAL:
                        if (filterValue.Value is DateTime)
                            retVal = " <= #" + ((DateTime)filterValue.Value).ToShortDateString() + "#";
                        else if (filterValue.Value is string)
                            retVal = " <= '" + filterValue.Value.ToString() + "'";
                        else
                            retVal = " <= " + filterValue.Value.ToString();
                        break;
                    case FilterComparisonEnum.EQUAL:
                        if (filterValue.Value is DateTime)
                            retVal = " = #" + ((DateTime)filterValue.Value).ToShortDateString() + "#";
                        else if (filterValue.Value is string)
                            retVal = " = '" + filterValue.Value.ToString() + "'";
                        else
                            retVal = " = " + filterValue.Value.ToString();
                        break;
                    case FilterComparisonEnum.LIKE:
                        if (filterValue.Value is string)
                            retVal = " LIKE '%" + filterValue.Value.ToString() + "%'";
                        else
                            throw new ArgumentException("LIKE invalid type");
                        break;
                }
            }
            return retVal;
        }
    }
}
