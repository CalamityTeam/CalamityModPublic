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
    public class DaedalusVisor : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Daedalus Facemask");
            Tooltip.SetDefault("10% increased throwing damage and critcal strike chance, increases throwing velocity by 15%\n" +
                "Immune to Cursed and gives control over gravity");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = 300000;
            item.rare = 5;
            item.defense = 7; //37
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == mod.ItemType("DaedalusBreastplate") && legs.type == mod.ItemType("DaedalusLeggings");
        }

        public override void DrawHair(ref bool drawHair, ref bool drawAltHair)
        {
            drawHair = true;
            drawAltHair = true;
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadowSubtle = true;
            player.armorEffectDrawOutlines = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "5% increased throwing damage and critical strike chance\n" +
                "Throwing projectiles throw out crystal shards as they travel";
            CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
            modPlayer.daedalusSplit = true;
            player.thrownDamage += 0.05f;
            player.thrownCrit += 5;
        }

        public override void UpdateEquip(Player player)
        {
            player.thrownVelocity += 0.15f;
            player.thrownDamage += 0.1f;
            player.thrownCrit += 10;
            player.AddBuff(BuffID.Gravitation, 2);
            player.buffImmune[BuffID.Cursed] = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "VerstaltiteBar", 8);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}