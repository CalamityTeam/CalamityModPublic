using Terraria;
using Terraria.ModLoader;
using CalamityMod.CalPlayer;

namespace CalamityMod.Buffs.PotionBuffs
{
    public class YharimPower : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Yharim's Power");
			Description.SetDefault("You feel like you can break the world in two...with your bare hands!");
			Main.debuff[Type] = false;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = false;
			longerExpertDebuff = false;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.GetModPlayer<CalamityPlayer>(mod).yPower = true;
		}
	}
}
