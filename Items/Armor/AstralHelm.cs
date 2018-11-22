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
    public class AstralHelm : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Astral Helm");
            Tooltip.SetDefault("Danger detection");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = 500000;
            item.rare = 7;
            item.defense = 13; //53
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == mod.ItemType("AstralBreastplate") && legs.type == mod.ItemType("AstralLeggings");
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadow = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "25% increased movement speed\n" +
                "22% increased damage and critical strike chance\n" +
                "Whenever you crit an enemy fallen, hallowed, and astral stars will rain down\n" +
                "This effect has a 2 second cooldown before it can trigger again";
            CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
            modPlayer.astralStarRain = true;
            player.moveSpeed += 0.25f;
            player.meleeDamage += 0.22f;
            player.meleeCrit += 22;
            player.rangedDamage += 0.22f;
            player.rangedCrit += 22;
            player.magicDamage += 0.22f;
            player.magicCrit += 22;
            player.thrownDamage += 0.22f;
            player.thrownCrit += 22;
            player.minionDamage += 0.22f;
        }

        public override void UpdateEquip(Player player)
        {
            player.dangerSense = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "AstralBar", 8);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}