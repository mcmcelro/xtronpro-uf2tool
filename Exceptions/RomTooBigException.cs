namespace xtronpro_uf2tool.Exceptions;

internal class RomTooBigException : Exception
{
    public RomTooBigException() : base("Sorry, the source file size exceeds the 256KB limit for UF2 conversion.")
    { }
}
