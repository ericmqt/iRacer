namespace iRacer.Telemetry;

public class TelemetryVariableDescriptorProviderBuilder
{
    private int _nextId = -1;
    private readonly List<ITelemetryVariableDescriptor> _variableDescriptors;

    public TelemetryVariableDescriptorProviderBuilder()
    {
        _variableDescriptors = new List<ITelemetryVariableDescriptor>();
    }

    /// <summary>
    /// Creates a <see cref="ITelemetryVariableDescriptorProvider"/> from the registered telemetry variable descriptors.
    /// </summary>
    /// <returns></returns>
    public ITelemetryVariableDescriptorProvider Build()
    {
        return new TelemetryVariableDescriptorProvider(_variableDescriptors.ToList());
    }

    /// <summary>
    /// Gets a value indicating whether a telemetry variable descriptor with the name <paramref name="variableName"/> has been registered.
    /// </summary>
    /// <param name="variableName"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"><paramref name="variableName"/> is null or an empty string.</exception>
    public bool IsRegistered(string variableName)
    {
        if (string.IsNullOrEmpty(variableName))
        {
            throw new ArgumentException($"'{nameof(variableName)}' cannot be null or empty.", nameof(variableName));
        }

        return _variableDescriptors.Any(x => string.Equals(x.Name, variableName, StringComparison.Ordinal));
    }

    /// <summary>
    /// Creates a new telemetry variable descriptor from the specified variable name and registers it for inclusion in the <see cref="ITelemetryVariableDescriptorProvider"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name">The telemetry variable name.</param>
    /// <returns>The <see cref="ITelemetryVariableDescriptor{T}"/> that was created.</returns>
    /// <exception cref="ArgumentException"><paramref name="name"/> is <see langword="null"/> or an empty string.</exception>
    /// <exception cref="ArgumentException">A telemetry variable with the name specified by <paramref name="name"/> is already registered.</exception>
    public ITelemetryVariableDescriptor<T> Register<T>(string name)
        where T : unmanaged
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentException($"'{nameof(name)}' cannot be null or empty.", nameof(name));
        }

        if (IsRegistered(name))
        {
            throw new ArgumentException($"Telemetry variable '{name}' is already registered.", nameof(name));
        }

        var descriptor = new TelemetryVariableDescriptor<T>(GetNextDescriptorIndex(), name);

        _variableDescriptors.Add(descriptor);

        return descriptor;
    }

    /// <summary>
    /// Creates a new telemetry variable array descriptor from the specified variable name and registers it for inclusion in the <see cref="ITelemetryVariableDescriptorProvider"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name">The telemetry variable name.</param>
    /// <param name="arrayLength">The length of the array.</param>
    /// <returns>The <see cref="ITelemetryVariableArrayDescriptor{T}"/> that was created.</returns>
    /// <exception cref="ArgumentException"><paramref name="name"/> is <see langword="null"/> or an empty string.</exception>
    /// <exception cref="ArgumentException">A telemetry variable with the name specified by <paramref name="name"/> is already registered.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="arrayLength"/> is less than zero.</exception>
    public ITelemetryVariableArrayDescriptor<T> RegisterArray<T>(string name, int arrayLength)
        where T : unmanaged
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentException($"'{nameof(name)}' cannot be null or empty.", nameof(name));
        }

        if (arrayLength < 0)
        {
            // TODO: Consider changing condition to < 1

            throw new ArgumentOutOfRangeException(nameof(arrayLength), $"Value '{nameof(arrayLength)}' cannot be less than zero.");
        }

        if (IsRegistered(name))
        {
            throw new ArgumentException($"Telemetry variable '{name}' is already registered.", nameof(name));
        }

        var descriptor = new TelemetryVariableArrayDescriptor<T>(GetNextDescriptorIndex(), name, arrayLength);

        _variableDescriptors.Add(descriptor);

        return descriptor;
    }

    private TelemetryVariableDescriptorIndex GetNextDescriptorIndex()
    {
        return new TelemetryVariableDescriptorIndex(Interlocked.Increment(ref _nextId));
    }
}
