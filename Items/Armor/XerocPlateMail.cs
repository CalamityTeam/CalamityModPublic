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
    public class XerocPlateMail : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Xeroc Plate Mail");
            Tooltip.SetDefault("+20 max life and mana\n" +
                "6% increased movement speed\n" +
                "6% increased damage and critical strike chance\n" +
                "Armor of the cosmos");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = 425000;
            item.rare = 9;
            item.defense = 27;
        }

        public override void UpdateEquip(Player player)
        {
            player.statLifeMax2 += 20;
            player.statManaMax2 += 20;
            player.moveSpeed += 0.06f;
            player.meleeCrit += 6;
            player.meleeDamage += 0.06f;
            player.magicCrit += 6;
            player.magicDamage += 0.06f;
            player.rangedCrit += 6;
            player.rangedDamage += 0.06f;
            player.thrownCrit += 6;
            player.thrownDamage += 0.06f;
            player.minionDamage += 0.06f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "MeldiateBar", 22);
            recipe.AddIngredient(ItemID.LunarBar, 16);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}