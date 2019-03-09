using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;
using CalamityMod.Items.CalamityCustomThrowingDamage;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Legs)]
    public class XerocCuisses : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Xeroc Cuisses");
            Tooltip.SetDefault("5% increased rogue damage and critical strike chance\n" +
                       "20% increased movement speed\n" +
                       "Speed of the cosmos");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
			item.value = Item.buyPrice(0, 24, 0, 0);
			item.rare = 9;
            item.defense = 24;
        }

        public override void UpdateEquip(Player player)
        {
            CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingCrit += 5;
            CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingDamage += 0.05f;
            player.moveSpeed += 0.2f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "MeldiateBar", 18);
            recipe.AddIngredient(ItemID.LunarBar, 12);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}