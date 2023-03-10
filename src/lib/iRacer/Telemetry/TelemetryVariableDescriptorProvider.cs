using System.Collections;

namespace iRacer.Telemetry;

internal class TelemetryVariableDescriptorProvider : ITelemetryVariableDescriptorProvider
{
    private readonly List<ITelemetryVariableDescriptor> _variableDescriptors;

    internal TelemetryVariableDescriptorProvider(List<ITelemetryVariableDescriptor> variableDescriptors)
    {
        _variableDescriptors = variableDescriptors ?? throw new ArgumentNullException(nameof(variableDescriptors));
    }

    public ITelemetryVariableDescriptor this[int index]
    {
        get
        {
            if (index < 0 || index >= _variableDescriptors.Count)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(index),
                    $"'{nameof(index)}' must be greater than or equal to zero and less than the number of elements in the collection.");
            }

            return _variableDescriptors[index];
        }
    }

    public int Count => _variableDescriptors.Count;

    public IEnumerator<ITelemetryVariableDescriptor> GetEnumerator()
    {
        return _variableDescriptors.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _variableDescriptors.GetEnumerator();
    }
}
