using xtronpro_uf2tool.Exceptions;
namespace xtronpro_uf2tool.Models;

internal class UF2FilePrepper
{
    private string _sourceFile;
    public UF2FilePrepper(String SourceFile)
    {
        _sourceFile = SourceFile;
    }

    public List<UF2Block> GetDataBlocks()
    {
        double sourceSize = new FileInfo(_sourceFile).Length;
        uint NumBlocks = (uint)Math.Ceiling(sourceSize / 0x100);
        List<UF2Block> DataBlocks = [];
        uint readBlockCount = 0;
        try
        {
            using (BinaryReader binaryReader = new BinaryReader(File.OpenRead(_sourceFile)))
            {
                // read the 4-byte NES header and validate it
                byte[] nesHeader = binaryReader.ReadBytes(4);
                if (!nesHeader.SequenceEqual(new byte[] { 0x4E, 0x45, 0x53, 0x1A }))
                {
                    throw new InvalidNesRomException();
                }

                // check that the source file size does not exceed 256KB
                if (binaryReader.BaseStream.Length > 0x40000)
                {
                    throw new RomTooBigException();
                }
                while (binaryReader.BaseStream.Position < sourceSize)
                {
                    // read source file data in 256-byte chunks and create UF2 blocks
                    uint blockPosition = readBlockCount * 0x100;
                    UF2Block block = new()
                    {
                        BlockNo = readBlockCount,
                        NumBlocks = NumBlocks,
                        PayloadSize = 0x100,
                        TargetAddr = 0x08010000 + blockPosition,
                    };
                    byte[] blockData = binaryReader.ReadBytes(0x100);
                    if (blockData.Length < 0x100)
                    {
                        // if the last chunk is less than 256 bytes, pad it with 0xFF NOP opcodes
                        byte[] paddedData = new byte[0x100];
                        Array.Fill(paddedData, (byte)0xFF);
                        Array.Copy(blockData, paddedData, blockData.Length);
                        blockData = paddedData;
                    }
                    blockData.CopyTo(block.Data, 0);
                    DataBlocks.Add(block);
                    readBlockCount++;
                }
            }

            return DataBlocks;
        }
        catch (UnauthorizedAccessException)
        {
            // handle access denied errors
            throw new UnauthorizedAccessException($"Sorry, you don't have read access to {_sourceFile}.");
        }
        catch (Exception ex)
        {
            throw new UnauthorizedAccessException($"An error occurred while reading the source file: {ex.Message}");
        }

    }
}
