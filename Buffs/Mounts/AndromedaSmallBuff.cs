using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Mounts
{
    public class AndromedaSmallBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }
    }
}
