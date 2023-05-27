using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Placeables
{
    public class CrimsonEffigyBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
            Main.debuff[Type] = false;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().crimEffigy = true;
        }
    }
}
