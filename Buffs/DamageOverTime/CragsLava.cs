using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.DamageOverTime
{
    public class CragsLava : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Searing Lava");
            Description.SetDefault("The brimstone lava sears your flesh");
            Main.buffNoTimeDisplay[Type] = true;
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            longerExpertDebuff = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().cragsLava = true;
        }
    }
}
