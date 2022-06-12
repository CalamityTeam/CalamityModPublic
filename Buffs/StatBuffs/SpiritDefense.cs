using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.StatBuffs
{
    public class SpiritDefense : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spirit Defense");
            Description.SetDefault("Defense boosted by 6 and damage reduction boosted by 3%");
            Main.debuff[Type] = false;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.statDefense += 6;
            player.endurance += 0.03f;
        }
    }
}
