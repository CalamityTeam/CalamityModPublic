using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;

namespace CalamityMod.Items.Providence
{
	public class ProfanedCore : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Profaned Core");
			Tooltip.SetDefault("The core of the unholy flame\nCan only be used during daytime");
		}
		
		public override void SetDefaults()
		{
			item.width = 20;
			item.height = 20;
			item.maxStack = 20;
			item.useAnimation = 45;
			item.useTime = 45;
			item.useStyle = 4;
			item.consumable = true;
			item.rare = 9;
		}
		
		public override bool CanUseItem(Player player)
		{
			return !NPC.AnyNPCs(mod.NPCType("Providence")) && Main.dayTime && (player.ZoneHoly || player.ZoneUnderworldHeight);
		}
		
		public override bool UseItem(Player player)
		{
			NPC.SpawnOnPlayer(player.whoAmI, mod.NPCType("Providence"));
			Main.PlaySound(SoundID.Roar, player.position, 0);
			return true;
		}
		
		public override void ModifyTooltips(List<TooltipLine> list)
	    {
	        foreach (TooltipLine line2 in list)
	        {
	            if (line2.mod == "Terraria" && line2.Name == "ItemName")
	            {
	                line2.overrideColor = new Color(0, 255, 200);
	            }
	        }
	    }
	}
}