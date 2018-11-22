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
    [AutoloadEquip(EquipType.Body)]
    public class DaedalusBreastplate : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Daedalus Breastplate");
            Tooltip.SetDefault("3% increased damage and critical strike chance");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = 250000;
            item.rare = 5;
            item.defense = 17; //41
        }

        public override void UpdateEquip(Player player)
        {
            player.meleeCrit += 3;
            player.meleeDamage += 0.03f;
            player.magicCrit += 3;
            player.magicDamage += 0.03f;
            player.rangedCrit += 3;
            player.rangedDamage += 0.03f;
            player.thrownCrit += 3;
            player.thrownDamage += 0.03f;
            player.minionDamage += 0.03f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "VerstaltiteBar", 15);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}