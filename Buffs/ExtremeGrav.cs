using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod;
using CalamityMod.NPCs;

namespace CalamityMod.Buffs
{
	public class ExtremeGrav : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Extreme Gravity");
			Description.SetDefault("Your wing time is reduced by 66%, infinite flight is disabled");
			Main.buffNoTimeDisplay[Type] = true;
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = true;
			longerExpertDebuff = true;
			canBeCleared = false;
		}
		
		public override void Update(Player player, ref int buffIndex)
		{
			player.GetModPlayer<CalamityPlayer>(mod).eGrav = true;
		}
	}
}