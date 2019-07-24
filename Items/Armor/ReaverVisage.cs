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
    public class ReaverVisage : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Reaver Visage");
            Tooltip.SetDefault("15% increased ranged damage, 20% decreased ammo usage, and 5% increased ranged critical strike chance\n" +
                "10% increased movement speed and can move freely through liquids");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
			item.value = Item.buyPrice(0, 30, 0, 0);
			item.rare = 7;
            item.defense = 13; //46
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == mod.ItemType("ReaverScaleMail") && legs.type == mod.ItemType("ReaverCuisses");
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadowSubtle = true;
            player.armorEffectDrawOutlines = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
            modPlayer.reaverDoubleTap = true;
            player.setBonus = "5% increased ranged damage\n" +
                "While using a ranged weapon you have a 10% chance to fire a powerful rocket";
            player.rangedDamage += 0.05f;
        }

        public override void UpdateEquip(Player player)
        {
            player.ignoreWater = true;
            player.rangedDamage += 0.15f;
            player.rangedCrit += 5;
            player.ammoCost80 = true;
            player.moveSpeed += 0.1f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "DraedonBar", 8);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}