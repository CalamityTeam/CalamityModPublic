using Terraria;
using Terraria.ModLoader;
using CalamityMod.CalPlayer;

namespace CalamityMod.Buffs.PotionBuffs
{
    public class Soaring : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Soaring");
			Description.SetDefault("Increased wing flight time and speed");
			Main.debuff[Type] = false;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = false;
			longerExpertDebuff = false;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.GetCalamityPlayer().soaring = true;
		}
	}
}
