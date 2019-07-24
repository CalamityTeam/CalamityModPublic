﻿using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs
{
	public class BrittleStar : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Brittle Star");
			Description.SetDefault("The brittle star will protect you");
			Main.buffNoTimeDisplay[Type] = true;
			Main.buffNoSave[Type] = true;
		}
		
		public override void Update(Player player, ref int buffIndex)
		{
			CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
			if (player.ownedProjectileCounts[mod.ProjectileType("BrittleStar")] > 0)
			{
				modPlayer.bStar = true;
			}
			if (!modPlayer.bStar)
			{
				player.DelBuff(buffIndex);
				buffIndex--;
			}
			else
			{
				player.buffTime[buffIndex] = 18000;
			}
		}
	}
}