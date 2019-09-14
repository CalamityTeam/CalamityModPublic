using Terraria;
using Terraria.ModLoader;
using CalamityMod.CalPlayer;

namespace CalamityMod.Buffs.PotionBuffs
{
    public class Invincible : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Invincible");
			Description.SetDefault("Immune to damage and most debuffs");
			Main.debuff[Type] = false;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = false;
			longerExpertDebuff = false;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.GetCalamityPlayer().invincible = true;
		}
	}
}
