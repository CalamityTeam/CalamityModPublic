using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Armor;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Legs)]
    public class StatigelGreaves : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Statigel Greaves");
            Tooltip.SetDefault("5% increased damage and movement speed");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = 122500;
            item.rare = 5;
            item.defense = 8;
        }

        public override void UpdateEquip(Player player)
        {
            player.meleeDamage += 0.05f;
            player.magicDamage += 0.05f;
            player.rangedDamage += 0.05f;
            player.thrownDamage += 0.05f;
            player.minionDamage += 0.05f;
            player.moveSpeed += 0.05f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "PurifiedGel", 6);
            recipe.AddIngredient(ItemID.HellstoneBar, 11);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}