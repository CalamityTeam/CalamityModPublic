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
                "Boosts melee speed by 10%, shoot speed by 25%, throwing damage by 15%, max minions by 2, and reduces mana cost by 15%");
		}
		
		public override void SetDefaults()
		{
			item.width = 28;
			item.height = 28;
            item.value = 5000000;
            item.accessory = true;
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine line2 in list)
            {
                if (line2.mod == "Terraria" && line2.Name == "ItemName")
                {
                    line2.overrideColor = new Color(255, 0, 255);
                }
            }
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