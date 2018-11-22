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
    public class DaedalusHeadgear : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Daedalus Circlet");
            Tooltip.SetDefault("10% increased minion damage and +2 max minions\n" +
                "Immune to Cursed and gives control over gravity");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = 300000;
            item.rare = 5;
            item.defense = 3; //33
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
            player.setBonus = "5% increased summon damage\n" +
                "A daedalus crystal floats above you to protect you";
            CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
            modPlayer.daedalusCrystal = true;
            if (player.whoAmI == Main.myPlayer)
            {
                if (player.FindBuffIndex(mod.BuffType("DaedalusCrystal")) == -1)
                {
                    player.AddBuff(mod.BuffType("DaedalusCrystal"), 3600, true);
                }
                if (player.ownedProjectileCounts[mod.ProjectileType("DaedalusCrystal")] < 1)
                {
                    Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, -1f, mod.ProjectileType("DaedalusCrystal"), 0, 0f, Main.myPlayer, 0f, 0f);
                }
            }
            player.minionDamage += 0.05f;
        }

        public override void UpdateEquip(Player player)
        {
            player.minionDamage += 0.1f;
            player.maxMinions += 2;
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