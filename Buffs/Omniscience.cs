using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs
{
    public class Omniscience : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Omniscience");
			Description.SetDefault("You can see everything");
			Main.debuff[Type] = false;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = false;
			longerExpertDebuff = false;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.GetModPlayer<CalamityPlayer>(mod).omniscience = true;
		}
	}
}
