using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Yharon
{
	public class ChickenEgg : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Dragon Egg");
			Tooltip.SetDefault("Summons the loyal guardian of the tyrant king\n" +
			                   "It yearns for the jungle\n" +
                               "Not consumable");
		}
		
		public override void SetDefaults()
		{
			item.width = 28;
			item.height = 18;
			item.useAnimation = 45;
			item.useTime = 45;
			item.useStyle = 4;
			item.consumable = false;
		}
		
		public override void ModifyTooltips(List<TooltipLine> list)
	    {
	        foreach (TooltipLine line2 in list)
	        {
	            if (line2.mod == "Terraria" && line2.Name == "ItemName")
	            {
	                line2.overrideColor = new Color(43, 96, 222);
	            }
	        }
	    }
		
		public override bool CanUseItem(Player player)
		{
			return player.ZoneJungle && !NPC.AnyNPCs(mod.NPCType("Yharon"));
		}
		
		public override bool UseItem(Player player)
		{
			NPC.SpawnOnPlayer(player.whoAmI, mod.NPCType("Yharon"));
			Main.PlaySound(SoundID.Roar, player.position, 0);
			return true;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(null, "EffulgentFeather", 15);
			recipe.AddIngredient(null, "BarofLife", 15);
			recipe.AddTile(null, "DraedonsForge");
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}