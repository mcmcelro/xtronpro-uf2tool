namespace xtronpro_uf2tool.Exceptions;

internal class InvalidNesRomException : Exception
{
    public InvalidNesRomException() : base("The source file is not a valid NES ROM.")
    { }
}
