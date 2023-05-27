using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Alcohol
{
    public class Trippy : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = false;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().trippy = true;
        }
    }
}
