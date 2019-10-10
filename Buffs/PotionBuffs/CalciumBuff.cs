using Terraria;
using Terraria.ModLoader;
using CalamityMod.CalPlayer;

namespace CalamityMod.Buffs.PotionBuffs
{
    public class CalciumBuff : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Calcium");
			Description.SetDefault("You are immune to fall damage");
			Main.debuff[Type] = false;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = false;
			longerExpertDebuff = false;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.Calamity().calcium = true;
		}
	}
}
