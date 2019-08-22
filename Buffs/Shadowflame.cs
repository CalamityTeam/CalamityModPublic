using Terraria;
using Terraria.ModLoader;
using CalamityMod.NPCs;

namespace CalamityMod.Buffs
{
    public class Shadowflame : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Shadowflame");
			Description.SetDefault("Rapid health loss");
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = true;
			longerExpertDebuff = false;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.GetModPlayer<CalamityPlayer>(mod).shadowflame = true;
		}
	}
}
