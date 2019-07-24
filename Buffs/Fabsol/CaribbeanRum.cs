﻿using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod;
using CalamityMod.NPCs;

namespace CalamityMod.Buffs.Fabsol
{
	public class CaribbeanRum : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Caribbean Rum");
			Description.SetDefault("Life regen, movement speed, and wing flight time boosted, you are floaty and defense is reduced");
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = false;
			longerExpertDebuff = false;
            canBeCleared = false;
        }
		
		public override void Update(Player player, ref int buffIndex)
		{
			player.GetModPlayer<CalamityPlayer>(mod).caribbeanRum = true;
		}
	}
}