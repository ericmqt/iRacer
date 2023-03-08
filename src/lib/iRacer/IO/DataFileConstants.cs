namespace iRacer.IO;

public static class DataFileConstants
{
    public const int HeaderLength = 48;
    public const int HeaderOffset = 0;
    public const int MaxStringLength = 32;
    public const int MaxDescriptionLength = 64;
    public const int MaxVariableBuffers = 4;
    public const int VariableBufferHeaderArrayLength = VariableDataBufferHeaderLength * MaxVariableBuffers;
    public const int VariableDataBufferHeaderArrayOffset = 48;
    public const int VariableDataBufferHeaderLength = 16;
    public const int VariableHeaderLength = 144;
}