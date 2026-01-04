using xtronpro_uf2tool.Models;

// require a path to a source file as an argument
if (args.Length < 1)
{
    Console.Error.WriteLine("Please provide the path to a source file as an argument.");
    return;
}

string sourceFile = args[0];
string targetFile = $"{Path.GetFileNameWithoutExtension(sourceFile)}-conv.nes";
double sourceSize = 0;
try
{
    // check that the provided file exists
    if (!File.Exists(sourceFile))
    {
        Console.Error.WriteLine("The specified file does not exist.");
        return;
    }

    // check that the provided file is not empty
    sourceSize = new FileInfo(sourceFile).Length;
    if (sourceSize == 0)
    {
        Console.Error.WriteLine("The source file is empty.");
        return;
    }

    // if it's less than 4 bytes, it isn't even big enough to have a valid NES ROM header
    if (sourceSize < 4)
    {
        Console.Error.WriteLine("The source file is too small to be a valid NES ROM.");
        return;
    }
}
catch (UnauthorizedAccessException)
{
    // handle access denied errors
    Console.Error.WriteLine($"Sorry, you don't have access to {sourceFile}.");
    return;
}

try
{
    if (File.Exists(targetFile))
    {
        Console.WriteLine("The target file already exists. Do you want to replace it? Y/[N]");
        string? response = Console.ReadLine()?.Trim();
        if (response == null || (response.ToLower() != "y" && response.ToLower() != "yes"))
        {
            Console.WriteLine("Operation cancelled by the user.");
            return;
        }
    }
}
catch (UnauthorizedAccessException)
{
    // handle access denied errors
    Console.Error.WriteLine($"Sorry, you don't have access to {targetFile}.");
    return;
}

uint NumBlocks = (uint)Math.Ceiling(sourceSize / 0x100);
List<UF2Block> DataBlocks = [];
uint readBlockCount = 0;
try
{
    using (BinaryReader binaryReader = new BinaryReader(File.OpenRead(sourceFile)))
    {
        // read the 4-byte NES header and validate it
        byte[] nesHeader = binaryReader.ReadBytes(4);
        if (!nesHeader.SequenceEqual(new byte[] { 0x4E, 0x45, 0x53, 0x1A }))
        {
            Console.Error.WriteLine("The source file is not a valid NES ROM.");
            return;
        }

        // check that the source file size does not exceed 256KB
        if (binaryReader.BaseStream.Length > 0x40000)
        {
            Console.Error.WriteLine("Sorry, the source file size exceeds the 256KB limit for UF2 conversion.");
            return;
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
}
catch (UnauthorizedAccessException)
{
    // handle access denied errors
    Console.Error.WriteLine($"Sorry, you don't have read access to {sourceFile}.");
    return;
}
catch (Exception ex)
{
    Console.Error.WriteLine($"An error occurred while reading the source file: {ex.Message}");
    return;
}
try
{
    using (BinaryWriter binaryWriter = new BinaryWriter(File.Create(targetFile)))
    {
        foreach (UF2Block block in DataBlocks)
        {
            long startPos = binaryWriter.BaseStream.Position;
            // write UF2 block to target file
            binaryWriter.Write(block.MagicStart0);
            binaryWriter.Write(block.MagicStart1);
            binaryWriter.Write(block.Flags);
            binaryWriter.Write(block.TargetAddr);
            binaryWriter.Write(block.PayloadSize);
            binaryWriter.Write(block.BlockNo);
            binaryWriter.Write(block.NumBlocks);
            binaryWriter.Write(block.FamilyID);
            binaryWriter.Write(block.Data);
            binaryWriter.Write(block.MagicEnd);
            long endPos = binaryWriter.BaseStream.Position;
            if (endPos - startPos != 0x200)
            {
                Console.Error.WriteLine("Error writing UF2 block: Incorrect block size.");
                return;
            }
        }
    }
    Console.WriteLine("UF2 file created successfully.");
}
catch (UnauthorizedAccessException)
{
    // handle access denied errors
    Console.Error.WriteLine($"Sorry, you don't have write access to {targetFile}.");
    return;
}
catch (Exception ex)
{
    Console.Error.WriteLine($"An error occurred while saving the target file: {ex.Message}");
    return;
}
try
{
    using (BinaryReader uf2reader = new BinaryReader(File.OpenRead(targetFile)))
    {
        Console.WriteLine("Verifying the created UF2 file...");
        for (int i = 0; i < DataBlocks.Count; i++)
        {
            // validate written UF2 blocks against original data
            UF2Block block = new(uf2reader);
            if (!block.Equals(DataBlocks[i]))
            {
                Console.Error.WriteLine("Verification failed: Mismatch in block data.");
                return;
            }
        }
        Console.WriteLine("UF2 file verified succesfully.");
    }
}
catch (UnauthorizedAccessException)
{
    // handle access denied errors
    Console.Error.WriteLine($"Sorry, you don't have read access to {targetFile}.");
    return;
}
catch (Exception ex)
{
    Console.Error.WriteLine($"An error occurred during verification: {ex.Message}");
    return;
}
