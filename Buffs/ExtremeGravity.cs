using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod;
using CalamityMod.NPCs;

namespace CalamityMod.Buffs
{
	public class ExtremeGravity : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Icarus' Folly");
			Description.SetDefault("Your wing time is reduced by 75%, infinite flight is disabled");
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = true;
			longerExpertDebuff = false;
		}
		
		public override void Update(Player player, ref int buffIndex)
		{
			player.GetModPlayer<CalamityPlayer>(mod).eGravity = true;
		}
	}
}