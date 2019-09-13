using Terraria;
using Terraria.ModLoader;
using CalamityMod.CalPlayer;

namespace CalamityMod.Buffs.PotionBuffs
{
    public class AnechoicCoating : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Anechoic Coating");
			Description.SetDefault("Abyssal creature's detection radius reduced");
			Main.debuff[Type] = false;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = false;
			longerExpertDebuff = false;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.GetModPlayer<CalamityPlayer>(mod).anechoicCoating = true;
		}
	}
}
