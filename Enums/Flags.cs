namespace xtronpro_uf2tool.Enums;

internal enum Flags
{
    NotMainFlash = 0x00000001,
    FileContainer = 0x00001000,
    FamilyIdPresent = 0x00002000,
    MD5ChecksumPresent = 0x00004000,
    ExtensionTagsPresent = 0x00008000
}
