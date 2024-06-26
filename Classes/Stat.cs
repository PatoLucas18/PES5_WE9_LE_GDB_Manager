namespace PES5_WE9_LE_GDB_Manager
{
    public class Stat
    {
        private Player player;
        private uint Offset;
        private int Shift;
        private int Mask;
        public Stat(Player player, uint offset, int shift, int mask)
        {
            this.player = player;
            Offset = offset;
            Shift = shift;
            Mask = mask;
        }

        public int getValue()
        {
            uint i = player.GetOffset() + Player.NameSize + Player.ShirtNameSize + Offset;
            int j = player.optionFile.Data[i] << 8 | player.optionFile.Data[(i - 1)];
            j >>= Shift;
            j &= Mask;
            return j;
        }
    }
}
