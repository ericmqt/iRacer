using System.Runtime.InteropServices;
using System.Text;

namespace iRacer.IO.Primitives;

[StructLayout(LayoutKind.Explicit)]
public unsafe struct VariableHeader
{
    [FieldOffset(VariableHeaderOffsets.TypeOffset)]
    public int Type;

    [FieldOffset(VariableHeaderOffsets.OffsetOffset)]
    public int Offset;

    [FieldOffset(VariableHeaderOffsets.CountOffset)]
    public int Count;

    [FieldOffset(VariableHeaderOffsets.CountAsTimeOffset)]
    public bool CountAsTime;

    [FieldOffset(VariableHeaderOffsets.NameOffset)]
    public fixed byte Name[DataFileConstants.MaxStringLength];

    [FieldOffset(VariableHeaderOffsets.DescriptionOffset)]
    public fixed byte Description[DataFileConstants.MaxDescriptionLength];

    [FieldOffset(VariableHeaderOffsets.UnitOffset)]
    public fixed byte Unit[DataFileConstants.MaxStringLength];

    public static string GetDescriptionString(in VariableHeader header)
    {
        fixed (byte* pDescription = header.Description)
        {
            var descSpan = MemoryMarshal.CreateReadOnlySpanFromNullTerminated(pDescription);
            return Encoding.ASCII.GetString(descSpan);
        }
    }

    public static string GetNameString(in VariableHeader header)
    {
        fixed (byte* pName = header.Name)
        {
            var nameSpan = MemoryMarshal.CreateReadOnlySpanFromNullTerminated(pName);
            return Encoding.ASCII.GetString(nameSpan);
        }
    }

    public static string? GetUnitString(in VariableHeader header)
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