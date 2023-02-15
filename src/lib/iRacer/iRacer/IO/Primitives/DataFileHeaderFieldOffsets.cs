namespace iRacer.IO.Primitives;

public static class DataFileHeaderFieldOffsets
{
    public const int HeaderVersion = 0;
    public const int Status = 4;
    public const int TickRate = 8;
    public const int SessionInfoVersion = 12;
    public const int SessionInfoLength = 16;
    public const int SessionInfoOffset = 20;
    public const int VariableCount = 24;
    public const int VariableHeaderOffset = 28;
    public const int BufferCount = 32;
    public const int BufferLength = 36;
    public const int Padding = 40;
}