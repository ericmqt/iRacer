using System;
using System.Collections.Generic;
using System.Text;

namespace iRacer.Tools.Telemetry;
public class VariableInfo
{
    public VariableInfo()
    {

    }

    public VariableInfo(
        string name,
        VariableValueType valueType,
        int valueCount,
        string? description,
        string? unit,
        bool isTimeSliceArray)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentException($"'{nameof(name)}' cannot be null or empty.", nameof(name));
        }

        if (valueCount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(valueCount), $"'{nameof(valueCount)}' must be greater than or equal to zero.");
        }

        Name = name;
        ValueType = valueType;
        ValueCount = valueCount;
        Description = description;
        Unit = unit;
        IsTimeSliceArray = isTimeSliceArray;
    }

    public string? Description { get; set; }
    public bool IsTimeSliceArray { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Unit { get; set; }
    public int ValueCount { get; set; }
    public VariableValueType ValueType { get; set; }
}
