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
    public class DaedalusEmblem : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Daedalus Emblem");
            Tooltip.SetDefault("10% increased ranged damage, critical strike chance, and 20% reduced ammo usage\n" +
                "Increases life regen, minion knockback, defense, and pick speed");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 32;
            item.value = 5000000;
            item.rare = 9;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.ammoCost80 = true;
            player.lifeRegen += 2;
            player.statDefense += 5;
            player.rangedDamage += 0.1f;
            player.rangedCrit += 10;
            player.pickSpeed -= 0.15f;
            player.minionKB += 0.5f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.CelestialStone);
            recipe.AddIngredient(null, "CoreofCalamity");
            recipe.AddIngredient(ItemID.SniperScope);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}