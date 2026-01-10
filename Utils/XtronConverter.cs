using xtronpro_uf2tool.Exceptions;
using xtronpro_uf2tool.Models;
namespace xtronpro_uf2tool.Utils;

internal class XtronConverter
{
    public XtronConverter()
    {
    }
    public void ConvertFile(string sourceFile, string targetFile)
    {
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

        try
        {
            List<UF2Block> DataBlocks = new UF2FilePrepper(sourceFile).GetDataBlocks();
            try
            {
                using (UF2FileWriter uf2writer = new UF2FileWriter(File.Create(targetFile)))
                {
                    uf2writer.WriteConvertedFile(DataBlocks);
                }
                Console.WriteLine("UF2 file created successfully.");
            }
            catch (WrongBlockSizeException ex)
            {
                Console.Error.WriteLine(ex.Message);
                return;
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
                Console.WriteLine("Verifying the created UF2 file...");
                UF2FileVerifier verifier = new UF2FileVerifier(DataBlocks, targetFile);
                if (!verifier.VerifyConversion())
                {
                    Console.Error.WriteLine("Verification failed: Mismatch in block data.");
                    return;
                }
                Console.WriteLine("UF2 file verified succesfully.");
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
        }
        catch (InvalidNesRomException)
        {
            Console.Error.WriteLine("The source file is not a valid NES ROM.");
            return;
        }
        catch (RomTooBigException)
        {
            Console.Error.WriteLine("Sorry, the source file size exceeds the 256KB limit for UF2 conversion.");
            return;
        }
        catch (UnauthorizedAccessException uaex)
        {
            Console.Error.WriteLine(uaex.Message);
            return;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex.Message);
            return;
        }
    }
}
