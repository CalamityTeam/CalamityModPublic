using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;
using CalamityMod.Items.CalamityCustomThrowingDamage;

namespace CalamityMod.Items.Accessories
{
    public class Nanotech : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Nanotech");
            Tooltip.SetDefault("Rogue projectiles leave behind nanoblades as they travel\n" +
                "Rogue weapons have a chance to instantly kill normal enemies\n" +
                "15% increased rogue damage, 10% increased rogue crit chance, and 15% increased rogue velocity");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 32;
            item.value = Item.buyPrice(0, 90, 0, 0);
            item.accessory = true;
			item.GetGlobalItem<CalamityGlobalItem>(mod).postMoonLordRarity = 20;
		}

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
            modPlayer.nanotech = true;
            CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingDamage += 0.15f;
            CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingCrit += 10;
            CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingVelocity += 0.15f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.MartianConduitPlating, 250);
            recipe.AddIngredient(ItemID.Nanites, 500);
            recipe.AddIngredient(null, "Phantoplasm", 20);
            recipe.AddIngredient(null, "NightmareFuel", 20);
            recipe.AddIngredient(null, "EndothermicEnergy", 20);
            recipe.AddTile(null, "DraedonsForge");
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}