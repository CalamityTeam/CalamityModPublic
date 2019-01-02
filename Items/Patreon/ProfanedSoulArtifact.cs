using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.NPCs;

namespace CalamityMod.Items.Patreon
{
	public class ProfanedSoulArtifact : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Profaned Soul Artifact");
			Tooltip.SetDefault("Profanation\n" + //idk what tooltip haha
                "Summons a healer guardian which heals for a certain amount of health every few seconds\n" +
				"Summons a defensive guardian if you have at least 8 minion slots, which boosts your movement speed and your damage resistance\n" +
				"Summons an offensive guardian if you are wearing the tarragon summon set (or stronger), which boosts your summon damage and your minion slots\n" +
				"If you get hit their effects will disappear for 5 seconds");
			Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(6, 6));
		}
		
		public override void SetDefaults()
		{
			item.width = 32;
			item.height = 40;
            item.value = Item.buyPrice(2, 50, 0, 0);
            item.accessory = true;
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine line2 in list)
            {
                if (line2.mod == "Terraria" && line2.Name == "ItemName")
                {
                    line2.overrideColor = new Color(139, 0, 0);
                }
            }
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
            modPlayer.pArtifact = true;
        }

        public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(null, "Cinderplate", 5);
            recipe.AddIngredient(null, "CoreofCinder");
            recipe.AddIngredient(null, "DivineGeode", 5);
            recipe.AddIngredient(null, "ExodiumClusterOre", 15);
			recipe.AddTile(TileID.DemonAltar);
			recipe.SetResult(this);
			recipe.AddRecipe();
        }
	}
}