using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class AtaxiaHelmet : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ataxia Helmet");
            Tooltip.SetDefault("12% increased summon damage and increased minion knockback\n" +
                "+2 max minions\n" +
                "Immune to lava and fire damage");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = 450000;
            item.rare = 8;
            item.defense = 6; //40
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == mod.ItemType("AtaxiaArmor") && legs.type == mod.ItemType("AtaxiaSubligar");
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawOutlines = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "5% increased summon damage\n" +
                "Inferno effect when below 50% life\n" +
                "Summons a chaos spirit to protect you\n" +
                "You have a 20% chance to emit a blazing explosion when you are hit";
            CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
            modPlayer.ataxiaBlaze = true;
            modPlayer.chaosSpirit = true;
            if (player.whoAmI == Main.myPlayer)
            {
                if (player.FindBuffIndex(mod.BuffType("ChaosSpirit")) == -1)
                {
                    player.AddBuff(mod.BuffType("ChaosSpirit"), 3600, true);
                }
                if (player.ownedProjectileCounts[mod.ProjectileType("ChaosSpirit")] < 1)
                {
                    Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, -1f, mod.ProjectileType("ChaosSpirit"), 0, 0f, Main.myPlayer, 0f, 0f);
                }
            }
            if (player.statLife <= (player.statLifeMax2 * 0.5f))
            {
                player.AddBuff(BuffID.Inferno, 2);
            }
            player.minionDamage += 0.05f;
        }

        public override void UpdateEquip(Player player)
        {
            player.minionDamage += 0.12f;
            player.minionKB += 1.5f;
            player.maxMinions += 2;
            player.lavaImmune = true;
            player.buffImmune[BuffID.OnFire] = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "CruptixBar", 7);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}