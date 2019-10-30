using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.StatBuffs
{
    public class BossZen : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Boss Zen");
            Description.SetDefault("The active boss is reducing spawn rates...a lot");
            Main.debuff[Type] = true;
			Main.buffNoSave[Type] = true;
			Main.buffNoTimeDisplay[Type] = true;
			canBeCleared = false;
		}

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().bossZen = true;
        }
    }
}
