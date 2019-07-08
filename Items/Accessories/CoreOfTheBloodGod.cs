using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Accessories
{
    public class CoreOfTheBloodGod : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Core of the Blood God");
            Tooltip.SetDefault("5% increased damage reduction\n" +
                "7% increased damage\n" +
                "When below 100 defense you gain 15% increased damage\n" +
                "Halves enemy contact damage\n" +
                "When you take contact damage this effect has a 20 second cooldown\n" +
                "Boosts your max HP by 10%");
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 26;
            item.value = Item.buyPrice(0, 90, 0, 0);
            item.expert = true;
			item.rare = 9;
			item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
            modPlayer.coreOfTheBloodGod = true;
            modPlayer.fleshTotem = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "BloodyWormScarf");
            recipe.AddIngredient(null, "BloodPact");
            recipe.AddIngredient(null, "FleshTotem");
            recipe.AddIngredient(null, "BloodflareCore");
            recipe.AddIngredient(null, "CosmiliteBar", 5);
            recipe.AddIngredient(null, "Phantoplasm", 5);
            recipe.AddTile(null, "DraedonsForge");
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}