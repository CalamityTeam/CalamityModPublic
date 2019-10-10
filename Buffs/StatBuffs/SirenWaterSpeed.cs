using Terraria;
using Terraria.ModLoader;
using CalamityMod.CalPlayer;

namespace CalamityMod.Buffs.StatBuffs
{
    public class SirenWaterSpeed : ModBuff
	{
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Siren Speed");
            Description.SetDefault("15% increased max speed and acceleration");
            Main.debuff[Type] = false;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            longerExpertDebuff = false;
        }

        public override void Update(Player player, ref int buffIndex)
		{
            player.Calamity().sirenWaterBuff = true;
        }
	}
}
