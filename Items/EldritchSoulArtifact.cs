using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.NPCs;

namespace CalamityMod.Items
{
	public class EldritchSoulArtifact : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Eldritch Soul Artifact");
			Tooltip.SetDefault("Knowledge\n" +
                "Boosts melee speed by 10%, shoot speed by 25%, rogue damage by 15%, max minions by 2, and reduces mana cost by 15%");
		}
		
		public override void SetDefaults()
		{
			item.width = 28;
			item.height = 28;
			item.value = Item.buyPrice(1, 50, 0, 0);
			item.accessory = true;
			item.GetGlobalItem<CalamityGlobalItem>(mod).postMoonLordRarity = 16;
		}

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
            modPlayer.eArtifact = true;
        }

        public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(null, "Cinderplate", 5);
            recipe.AddIngredient(null, "EssenceofChaos", 10);
            recipe.AddIngredient(null, "Phantoplasm", 10);
            recipe.AddIngredient(null, "ExodiumClusterOre", 15);
			recipe.AddTile(TileID.DemonAltar);
			recipe.SetResult(this);
			recipe.AddRecipe();
        }
	}
}