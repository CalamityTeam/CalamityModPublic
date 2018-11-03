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
    [AutoloadEquip(EquipType.Head)]
    public class StatigelMask : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Statigel Mask");
            Tooltip.SetDefault("10% increased throwing damage and 33% chance to not consume thrown items\n" +
                "7% increased throwing critical strike chance");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = 200000;
            item.rare = 5;
            item.defense = 6; //23
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == mod.ItemType("StatigelArmor") && legs.type == mod.ItemType("StatigelGreaves");
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "When you take over 100 damage in one hit you become immune to damage for an extended period of time\n" +
                    "Grants an extra jump and increased jump height";
            CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
            modPlayer.statigelSet = true;
            player.doubleJumpSail = true;
            player.jumpBoost = true;
        }

        public override void UpdateEquip(Player player)
        {
            player.thrownCost33 = true;
            player.thrownDamage += 0.1f;
            player.thrownCrit += 7;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "PurifiedGel", 5);
            recipe.AddIngredient(ItemID.HellstoneBar, 9);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}