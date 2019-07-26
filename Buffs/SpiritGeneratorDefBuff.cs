using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod;
using CalamityMod.NPCs;

namespace CalamityMod.Buffs
{
	public class SpiritGeneratorDefBuff : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Spirit Defense");
			Description.SetDefault("Defense boosted by 5 and damage reduction boosted by 5%");
			Main.debuff[Type] = false;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = true;
			longerExpertDebuff = false;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.GetModPlayer<CalamityPlayer>(mod).sDefense = true;
		}
	}
}
