using xtronpro_uf2tool.Exceptions;
namespace xtronpro_uf2tool.Models;

internal class UF2FileWriter : BinaryWriter
{
    public UF2FileWriter(Stream baseStream) : base(baseStream)
    {
    }

    public void WriteConvertedFile(List<UF2Block> DataBlocks)
    {
        foreach (UF2Block block in DataBlocks)
        {
            long startPos = BaseStream.Position;
            Write(block.MagicStart0);
            Write(block.MagicStart1);
            Write(block.Flags);
            Write(block.TargetAddr);
            Write(block.PayloadSize);
            Write(block.BlockNo);
            Write(block.NumBlocks);
            Write(block.FamilyID);
            Write(block.Data);
            Write(block.MagicEnd);
            long endPos = BaseStream.Position;
            if (endPos - startPos != 0x200)
            {
                throw new WrongBlockSizeException(0x200, endPos - startPos);
            }
        }
    }
}
