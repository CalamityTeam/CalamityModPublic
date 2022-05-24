using Terraria.DataStructures;
using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class GastricBelcherStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gastric Belcher Staff");
            Tooltip.SetDefault("Summons aquatic aberrations to protect you\n" +
            "Aberrations fire vomit at nearby enemies with every third attack firing bubbles");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 60;
            Item.mana = 10;
            Item.width = 66;
            Item.height = 70;
            Item.useTime = Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.knockBack = 1f;
            Item.value = CalamityGlobalItem.Rarity7BuyPrice;
            Item.rare = ItemRarityID.Lime;
            Item.UseSound = SoundID.Item76;
            Item.shoot = ModContent.ProjectileType<GastricBelcher>();
            Item.shootSpeed = 10f;
            Item.DamageType = DamageClass.Summon;
            Item.autoReuse = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse != 2)
            {
                player.itemTime = Item.useTime;
                Vector2 playerPos = player.RotatedRelativePoint(player.MountedCenter, true);
                float directionX = Main.mouseX + Main.screenPosition.X - playerPos.X;
                float directionY = Main.mouseY + Main.screenPosition.Y - playerPos.Y;
                if (player.gravDir == -1f)
                {
                    directionY = Main.screenPosition.Y + Main.screenHeight - Main.mouseY - playerPos.Y;
                }
                Vector2 spinningpoint = new Vector2(directionX, directionY);
                float dist = spinningpoint.Length();
                if ((float.IsNaN(spinningpoint.X) && float.IsNaN(spinningpoint.Y)) || (spinningpoint.X == 0f && spinningpoint.Y == 0f))
                {
                    spinningpoint.X = player.direction;
                    spinningpoint.Y = 0f;
                    dist = Item.shootSpeed;
                }
                else
                {
                    dist = Item.shootSpeed / dist;
                }
                spinningpoint.X *= dist;
                spinningpoint.Y *= dist;
                playerPos.X = Main.mouseX + Main.screenPosition.X;
                playerPos.Y = Main.mouseY + Main.screenPosition.Y;
                spinningpoint = spinningpoint.RotatedBy(Math.PI / 2D, default);
                int p = Projectile.NewProjectile(source, playerPos + spinningpoint, spinningpoint, type, damage, knockback, player.whoAmI, 0f, 1f);
                if (Main.projectile.IndexInRange(p))
                    Main.projectile[p].originalDamage = Item.damage;
            }
            return false;
        }
    }
}
