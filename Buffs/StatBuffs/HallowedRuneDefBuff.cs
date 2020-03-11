using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.StatBuffs
{
    public class HallowedRuneDefBuff : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Hallowed Defense");
            Description.SetDefault("Defense boosted by 7 and damage reduction boosted by 7%");
            Main.debuff[Type] = false;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
			Main.buffNoTimeDisplay[Type] = true;
            longerExpertDebuff = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().hallowedDefense = true;
        }
    }
}
