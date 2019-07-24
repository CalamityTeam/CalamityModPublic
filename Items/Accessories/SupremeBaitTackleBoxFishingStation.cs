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
    public class SupremeBaitTackleBoxFishingStation : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Supreme Bait Tackle Box Fishing Station");
            Tooltip.SetDefault("The ultimate fishing accessory\n" +
                "Increases fishing skill by 100\n" +
                "Fishing line will never break and decreases chance of bait consumption\n" +
                "Crate potion effect, does not stack with crate potions\n" +
                "Sonar potion effect");
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 26;
            item.value = Item.buyPrice(0, 45, 0, 0);
            item.rare = 10;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.fishingSkill += 100;
            player.accFishingLine = true;
            player.accTackleBox = true;
            player.cratePotion = true;
            player.sonarPotion = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.AnglerHat); //5
            recipe.AddIngredient(ItemID.AnglerVest); //5
            recipe.AddIngredient(ItemID.AnglerPants); //5
            recipe.AddIngredient(ItemID.AnglerTackleBag); //10
            recipe.AddIngredient(ItemID.FishingPotion, 5); //15
            recipe.AddIngredient(ItemID.CratePotion, 5);
            recipe.AddIngredient(ItemID.SonarPotion, 5);
            recipe.AddIngredient(ItemID.MasterBait, 5); //50
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}