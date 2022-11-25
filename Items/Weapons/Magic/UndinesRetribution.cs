using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class UndinesRetribution : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Undine's Retribution");
            Tooltip.SetDefault("Casts a swarm of homing spears");
            Item.staff[Item.type] = true;
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 38;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 15;
            Item.width = 64;
            Item.height = 64;
            Item.useTime = 18;
            Item.useAnimation = 18;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 3.5f;
            Item.value = CalamityGlobalItem.Rarity8BuyPrice;
            Item.rare = ItemRarityID.Lime;
            Item.UseSound = SoundID.Item66;
            Item.autoReuse = true;
            Item.shootSpeed = 12f;
            Item.shoot = ModContent.ProjectileType<UndinesRetributionSpear>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float num72 = Item.shootSpeed;
            Vector2 vector2 = player.RotatedRelativePoint(player.MountedCenter, true);
            float num78 = (float)Main.mouseX + Main.screenPosition.X - vector2.X;
            float num79 = (float)Main.mouseY + Main.screenPosition.Y - vector2.Y;
            if (player.gravDir == -1f)
            {
                num79 = Main.screenPosition.Y + (float)Main.screenHeight - (float)Main.mouseY - vector2.Y;
            }
            float num80 = (float)Math.Sqrt((double)(num78 * num78 + num79 * num79));
            if ((float.IsNaN(num78) && float.IsNaN(num79)) || (num78 == 0f && num79 == 0f))
            {
                num78 = (float)player.direction;
            }
            else
            {
                num80 = num72 / num80;
            }
            for (int num108 = 0; num108 < 3; num108++)
            {
                vector2 = new Vector2(player.position.X + (float)player.width * 0.5f + (float)(Main.rand.Next(51) * -(float)player.direction) + ((float)Main.mouseX + Main.screenPosition.X - /* - */ player.position.X), player.MountedCenter.Y + 600f); //-
                vector2.X = (vector2.X + player.Center.X) / 2f + (float)Main.rand.Next(-50, 51); //200
                vector2.Y += (float)(100 * num108); //-=
                num78 = (float)Main.mouseX + Main.screenPosition.X - vector2.X; //+ -
                num79 = (float)Main.mouseY + Main.screenPosition.Y - vector2.Y; //+ -
                if (num79 < 0f)
                {
                    num79 *= -1f;
                }
                if (num79 < 20f)
                {
                    num79 = 20f;
                }
                num80 = (float)Math.Sqrt((double)(num78 * num78 + num79 * num79));
                num80 = num72 / num80;
                num78 *= num80;
                num79 *= num80;
                float speedX6 = num78 + (float)Main.rand.Next(-60, 61) * 0.02f;
                float speedY7 = num79 + (float)Main.rand.Next(-60, 61) * 0.02f;
                float ai1 = Main.rand.NextFloat() + 0.5f;
                int bullet2 = Projectile.NewProjectile(source, vector2.X, vector2.Y, speedX6, -speedY7, type, damage, knockback, player.whoAmI, 0.0f, ai1);
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<Voidstone>(30).
                AddIngredient<DepthCells>(30).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
