using System.Runtime.InteropServices;
using System.Text;

namespace iRacer.IO.Primitives;

[StructLayout(LayoutKind.Explicit)]
public unsafe struct TelemetryVariableHeader
{
    [FieldOffset(TelemetryVariableHeaderOffsets.TypeOffset)]
    public int Type;

    [FieldOffset(TelemetryVariableHeaderOffsets.OffsetOffset)]
    public int Offset;

    [FieldOffset(TelemetryVariableHeaderOffsets.CountOffset)]
    public int Count;

    [FieldOffset(TelemetryVariableHeaderOffsets.CountAsTimeOffset)]
    public bool CountAsTime;

    [FieldOffset(TelemetryVariableHeaderOffsets.NameOffset)]
    public fixed byte Name[DataFileConstants.MaxStringLength];

    [FieldOffset(TelemetryVariableHeaderOffsets.DescriptionOffset)]
    public fixed byte Description[DataFileConstants.MaxDescriptionLength];

    [FieldOffset(TelemetryVariableHeaderOffsets.UnitOffset)]
    public fixed byte Unit[DataFileConstants.MaxStringLength];

    public static string GetDescriptionString(in TelemetryVariableHeader header)
    {
        fixed (byte* pDescription = header.Description)
        {
            var descSpan = MemoryMarshal.CreateReadOnlySpanFromNullTerminated(pDescription);
            return Encoding.ASCII.GetString(descSpan);
        }
    }

    public static string GetNameString(in TelemetryVariableHeader header)
    {
        fixed (byte* pName = header.Name)
        {
            var nameSpan = MemoryMarshal.CreateReadOnlySpanFromNullTerminated(pName);
            return Encoding.ASCII.GetString(nameSpan);
        }
    }

    public static string? GetUnitString(in TelemetryVariableHeader header)
    {
        fixed (byte* pUnit = header.Unit)
        {
            var unitSpan = MemoryMarshal.CreateReadOnlySpanFromNullTerminated(pUnit);

            if (unitSpan.Length == 0)
            {
                return null;
            }

            return Encoding.ASCII.GetString(unitSpan);
        }
    }
}