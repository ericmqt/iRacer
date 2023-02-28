using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace iRacer.Tools.Telemetry;
public class VariableInfoCollection : IReadOnlyCollection<VariableInfo>
{
    private readonly List<VariableInfo> _variables;

    public VariableInfoCollection()
    {
        _variables = new List<VariableInfo>();
    }

    public VariableInfoCollection(IEnumerable<VariableInfo> variables)
    {
        if (variables is null)
        {
            throw new ArgumentNullException(nameof(variables));
        }

        _variables = new List<VariableInfo>();

        foreach (var tv in variables)
        {
            if (ContainsByName(tv))
            {
                throw new ArgumentException($"Variable '{tv.Name}' is included more than once.", nameof(variables));
            }

            _variables.Add(tv);
        }
    }

    public int Count => _variables.Count;

    public bool ContainsByName(VariableInfo variable)
    {
        return ContainsByName(variable.Name);
    }

    public bool ContainsByName(string variableName)
    {
        if (string.IsNullOrEmpty(variableName))
        {
            throw new ArgumentException($"'{nameof(variableName)}' cannot be null or empty.", nameof(variableName));
        }

        return _variables.Any(x => string.Equals(x.Name, variableName, StringComparison.Ordinal));
    }

    public bool TryAdd(VariableInfo variable)
    {
        if (variable is null)
        {
            throw new ArgumentNullException(nameof(variable));
        }

        if (ContainsByName(variable))
        {
            return false;
        }

        _variables.Add(variable);

        return true;
    }

    public IEnumerator<VariableInfo> GetEnumerator()
    {
        return _variables.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _variables.GetEnumerator();
    }
}
