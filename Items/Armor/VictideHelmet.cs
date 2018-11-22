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
    public class VictideHelmet : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Victide Helmet");
            Tooltip.SetDefault("5% increased summon damage");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = 40000;
            item.rare = 2;
            item.defense = 1; //8
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == mod.ItemType("VictideBreastplate") && legs.type == mod.ItemType("VictideLeggings");
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "Increased life regen and summon damage while submerged in liquid\n" +
                "Summons a sea urchin to protect you\n" +
                "When using any weapon you have a 10% chance to throw a returning seashell projectile\n" +
                "This seashell does true damage and does not benefit from any damage class\n" +
                "Slightly reduces breath loss in the abyss";
            CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
            modPlayer.victideSet = true;
            modPlayer.urchin = true;
            if (player.whoAmI == Main.myPlayer)
            {
                if (player.FindBuffIndex(mod.BuffType("Urchin")) == -1)
                {
                    player.AddBuff(mod.BuffType("Urchin"), 3600, true);
                }
                if (player.ownedProjectileCounts[mod.ProjectileType("Urchin")] < 1)
                {
                    Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, -1f, mod.ProjectileType("Urchin"), 0, 0f, Main.myPlayer, 0f, 0f);
                }
            }
            player.ignoreWater = true;
            if (Collision.DrownCollision(player.position, player.width, player.height, player.gravDir))
            {
                player.minionDamage += 0.1f;
                player.lifeRegen += 3;
            }
        }

        public override void UpdateEquip(Player player)
        {
            player.minionDamage += 0.05f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "VictideBar", 3);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}