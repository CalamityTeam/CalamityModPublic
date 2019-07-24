using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod;
using CalamityMod.NPCs;

namespace CalamityMod.Buffs
{
	public class TyrantsFury : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Tyrant's Fury");
			Description.SetDefault("30% increased melee damage and 10% increased melee crit chance");
			Main.debuff[Type] = false;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = true;
			longerExpertDebuff = false;
		}
		
		public override void Update(Player player, ref int buffIndex)
		{
			player.GetModPlayer<CalamityPlayer>(mod).tFury = true;
		}
	}
}