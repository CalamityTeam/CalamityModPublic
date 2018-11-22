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
    public class StatigelHood : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Statigel Hood");
            Tooltip.SetDefault("10% increased summon damage and increased minion knockback\n" +
                "+1 max minion");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = 200000;
            item.rare = 5;
            item.defense = 4; //20
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == mod.ItemType("StatigelArmor") && legs.type == mod.ItemType("StatigelGreaves");
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "Summons a mini slime god to fight for you, the type depends on what world evil you have\n" +
                "When you take over 100 damage in one hit you become immune to damage for an extended period of time\n" +
                "Grants an extra jump and increased jump height";
            CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
            modPlayer.statigelSet = true;
            modPlayer.slimeGod = true;
            player.doubleJumpSail = true;
            player.jumpBoost = true;
            if (player.whoAmI == Main.myPlayer)
            {
                if (player.FindBuffIndex(mod.BuffType("SlimeGod")) == -1)
                {
                    player.AddBuff(mod.BuffType("SlimeGod"), 3600, true);
                }
                if (WorldGen.crimson && player.ownedProjectileCounts[mod.ProjectileType("SlimeGodAlt")] < 1)
                {
                    Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, -1f, mod.ProjectileType("SlimeGodAlt"), 33, 0f, Main.myPlayer, 0f, 0f);
                }
                else if (!WorldGen.crimson && player.ownedProjectileCounts[mod.ProjectileType("SlimeGod")] < 1)
                {
                    Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, -1f, mod.ProjectileType("SlimeGod"), 33, 0f, Main.myPlayer, 0f, 0f);
                }
            }
        }

        public override void UpdateEquip(Player player)
        {
            player.minionKB += 1.5f;
            player.minionDamage += 0.1f;
            player.maxMinions++;
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