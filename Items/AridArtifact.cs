using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Events;

namespace CalamityMod.Items
{
	public class AridArtifact : ModItem
	{
		public override void SetStaticDefaults()
	 	{
	 		DisplayName.SetDefault("Arid Artifact");
	 		Tooltip.SetDefault("Summons a sandstorm\n" +
	 		                   "The sandstorm will happen shortly after the item is used");
	 	}
	
		public override void SetDefaults()
		{
			item.width = 20;
			item.height = 20;
			item.maxStack = 20;
			item.rare = 5;
			item.useAnimation = 20;
			item.useTime = 20;
			item.useStyle = 4;
			item.UseSound = SoundID.Item66;
			item.consumable = true;
		}
		
		public override bool CanUseItem(Player player)
		{
			return !Sandstorm.Happening;
		}
		
		public override bool UseItem(Player player)
		{
			Sandstorm.Happening = true;
			Sandstorm.TimeLeft = (int)(3600f * (8f + Main.rand.NextFloat() * 16f));
			if (Main.netMode == 2)
			{
				NetMessage.SendData(7, -1, -1, null, 0, 0f, 0f, 0f, 0, 0, 0);
			}
			return true;
		}
		
		public override void AddRecipes()
	    {
	        ModRecipe recipe = new ModRecipe(mod);
	        recipe.AddIngredient(ItemID.AncientBattleArmorMaterial);
			recipe.AddIngredient(ItemID.AdamantiteBar, 5);
	        recipe.AddTile(TileID.Anvils);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
	        recipe = new ModRecipe(mod);
	        recipe.AddIngredient(ItemID.AncientBattleArmorMaterial);
			recipe.AddIngredient(ItemID.TitaniumBar, 5);
	        recipe.AddTile(TileID.Anvils);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
	    }
	}
}