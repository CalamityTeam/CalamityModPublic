using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.NPCs;

namespace CalamityMod.Items
{
	public class GodlySoulArtifact : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Godly Soul Artifact");
            Tooltip.SetDefault("Loyalty\n" +
                "Boosts max minions by 8");
        }

		public override void SetDefaults()
		{
			item.width = 28;
			item.height = 28;
			item.value = Item.buyPrice(1, 50, 0, 0);
			item.accessory = true;
			item.rare = 10;
			item.GetGlobalItem<CalamityGlobalItem>(mod).postMoonLordRarity = 16;
		}

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
            modPlayer.gArtifact = true;
        }

        public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(null, "Cinderplate", 5);
            recipe.AddIngredient(null, "EssenceofCinder", 10);
            recipe.AddIngredient(null, "HellcasterFragment", 10);
            recipe.AddIngredient(null, "ExodiumClusterOre", 15);
			recipe.AddTile(TileID.DemonAltar);
			recipe.SetResult(this);
			recipe.AddRecipe();
        }
	}
}
