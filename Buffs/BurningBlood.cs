using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod;
using CalamityMod.NPCs;

namespace CalamityMod.Buffs
{
	public class BurningBlood : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Burning Blood");
			Description.SetDefault("Your blood is on fire");
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = true;
			longerExpertDebuff = true;
		}
		
		public override void Update(Player player, ref int buffIndex)
		{
			player.GetModPlayer<CalamityPlayer>(mod).bBlood = true;
		}
	}
}