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
    public class AerospecHelm : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Aerospec Helm");
            Tooltip.SetDefault("8% increased melee damage");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
			item.value = Item.buyPrice(0, 5, 0, 0);
			item.rare = 3;
            item.defense = 7; //20
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == mod.ItemType("AerospecBreastplate") && legs.type == mod.ItemType("AerospecLeggings");
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadow = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "5% increased movement speed and melee critical strike chance\n" +
                    "Taking over 25 damage in one hit will cause a spread of homing feathers to fall\n" +
                    "Allows you to fall more quickly and disables fall damage";
            CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
            modPlayer.aeroSet = true;
            player.noFallDmg = true;
            player.moveSpeed += 0.05f;
            player.meleeCrit += 5;
        }

        public override void UpdateEquip(Player player)
        {
            player.meleeDamage += 0.08f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "AerialiteBar", 5);
            recipe.AddIngredient(ItemID.Cloud, 3);
            recipe.AddIngredient(ItemID.RainCloud);
            recipe.AddIngredient(ItemID.Feather);
            recipe.AddTile(TileID.SkyMill);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}