using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Legs)]
    public class AstralLeggings : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Astral Leggings");
            Tooltip.SetDefault("10% increased movement speed\n" +
                               "Treasure and ore detection");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = 450000;
            item.rare = 7;
            item.defense = 17;
        }

        public override void UpdateEquip(Player player)
        {
            player.moveSpeed += 0.1f;
            player.findTreasure = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "AstralBar", 10);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}