namespace xtronpro_uf2tool.Models
{
    internal struct UF2Block
    {
        public readonly uint MagicStart0 = 0x0A324655;
        public readonly uint MagicStart1 = 0x9E5D5157;
        public uint Flags = (uint)Enums.Flags.FamilyIdPresent; // always the same for XTron Pro
        public uint TargetAddr; // starts at 0x08010000 and increases by 0x100 per block
        public uint PayloadSize;
        public uint BlockNo;
        public uint NumBlocks;
        public readonly uint FamilyID = 0x57755A57; // always the same for XTron Pro - STM32F4: https://www.st.com/en/microcontrollers-microprocessors/stm32f4-series.html
        public byte[] Data = new byte[476];
        public readonly uint MagicEnd = 0x0AB16F30;
        public UF2Block()
        {
        }

        public UF2Block(BinaryReader reader)
        {
            reader.ReadUInt32(); // magicStart0
            reader.ReadUInt32(); // magicStart1
            Flags = reader.ReadUInt32();
            TargetAddr = reader.ReadUInt32();
            PayloadSize = reader.ReadUInt32();
            BlockNo = reader.ReadUInt32();
            NumBlocks = reader.ReadUInt32();
            reader.ReadUInt32(); // familyID
            Data = reader.ReadBytes(476);
            reader.ReadUInt32(); // magicEnd
        }

        public override bool Equals(object? obj)
        {
            if (obj is UF2Block other)
            {
                return MagicStart0 == other.MagicStart0 &&
                       MagicStart1 == other.MagicStart1 &&
                       Flags == other.Flags &&
                       TargetAddr == other.TargetAddr &&
                       PayloadSize == other.PayloadSize &&
                       BlockNo == other.BlockNo &&
                       NumBlocks == other.NumBlocks &&
                       FamilyID == other.FamilyID &&
                       Data.SequenceEqual(other.Data) &&
                       MagicEnd == other.MagicEnd;
            }
            return false;
        }
    }
}
