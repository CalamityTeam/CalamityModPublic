using Terraria;
using Terraria.ModLoader;
using CalamityMod.CalPlayer;

namespace CalamityMod.Buffs.PotionBuffs
{
    public class TitanScale : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Titan Scale");
			Description.SetDefault("You feel tanky");
			Main.debuff[Type] = false;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = false;
			longerExpertDebuff = false;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.GetModPlayer<CalamityPlayer>(mod).tScale = true;
		}
	}
}
