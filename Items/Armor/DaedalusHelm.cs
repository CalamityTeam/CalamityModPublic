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
    public class DaedalusHelm : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Daedalus Helm");
            Tooltip.SetDefault("10% increased melee damage and critical strike chance\n" +
                "Immune to Cursed and gives control over gravity");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = 300000;
            item.rare = 5;
            item.defense = 21; //51
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == mod.ItemType("DaedalusBreastplate") && legs.type == mod.ItemType("DaedalusLeggings");
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadowSubtle = true;
            player.armorEffectDrawOutlines = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "5% increased melee damage and critical strike chance\n" +
                "You have a 33% chance to reflect projectiles back at enemies\n" +
                "If you reflect a projectile you are also healed for 1/5 of that projectile's damage";
            CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
            modPlayer.daedalusReflect = true;
            player.meleeDamage += 0.05f;
            player.meleeCrit += 5;
        }

        public override void UpdateEquip(Player player)
        {
            player.meleeDamage += 0.1f;
            player.meleeCrit += 10;
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