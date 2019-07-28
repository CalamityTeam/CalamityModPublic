using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs
{
    public class XerocWrath : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Xeroc Wrath");
			Description.SetDefault("Wrath of the cosmos");
			Main.debuff[Type] = false;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = true;
			longerExpertDebuff = false;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.GetModPlayer<CalamityPlayer>(mod).xWrath = true;
		}
	}
}
