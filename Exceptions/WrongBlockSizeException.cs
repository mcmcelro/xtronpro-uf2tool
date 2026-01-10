namespace xtronpro_uf2tool.Exceptions;

internal class WrongBlockSizeException : Exception
{
    public WrongBlockSizeException(long expectedSize, long actualSize) : base($"Wrong block size: expected {expectedSize} bytes, but got {actualSize} bytes.")
    { }
}
