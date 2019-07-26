using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod;
using CalamityMod.NPCs;

namespace CalamityMod.Buffs
{
	public class GraxDefense : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Grax Boost");
			Description.SetDefault("Your defenses and muscles are strong");
			Main.debuff[Type] = false;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = true;
			longerExpertDebuff = false;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.GetModPlayer<CalamityPlayer>(mod).graxDefense = true;
		}
	}
}
