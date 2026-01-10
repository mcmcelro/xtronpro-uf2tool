namespace xtronpro_uf2tool.Models;

internal class UF2FileVerifier
{
    private string _targetFile;
    private List<UF2Block> _dataBlocks;
    public UF2FileVerifier(List<UF2Block> DataBlocks, string TargetFile)
    {
        _dataBlocks = DataBlocks;
        _targetFile = TargetFile;
    }

    public bool VerifyConversion()
    {
        // TODO: make UF2FileReader to read original data blocks
        using (BinaryReader uf2reader = new BinaryReader(File.OpenRead(_targetFile)))
        {
            for (int i = 0; i < _dataBlocks.Count; i++)
            {
                // validate written UF2 blocks against original data
                UF2Block block = new(uf2reader);
                if (!block.Equals(_dataBlocks[i]))
                {

                    return false;
                }
            }
        }
        return true;
    }
}
