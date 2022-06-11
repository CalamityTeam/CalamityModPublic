using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class ShadowdropStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shadowdrop Staff");
            Tooltip.SetDefault("Summons dark aura rain from the sky");
            Item.staff[Item.type] = true;
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 25;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 6;
            Item.width = 44;
            Item.height = 44;
            Item.useTime = 9;
            Item.useAnimation = 18;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2.25f;
            Item.value = Item.buyPrice(0, 4, 0, 0);
            Item.rare = ItemRarityID.Orange;
            Item.UseSound = SoundID.Item66;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<AuraRain>();
            Item.shootSpeed = 14f;
        }

        public override Vector2? HoldoutOrigin() => new Vector2(7, 7);

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
                num79 = 0f;
                num80 = num72;
            }
            else
            {
                num80 = num72 / num80;
            }

            int num107 = 3;
            for (int num108 = 0; num108 < num107; num108++)
            {
                vector2 = new Vector2(player.position.X + (float)player.width * 0.5f + (float)(Main.rand.Next(201) * -(float)player.direction) + ((float)Main.mouseX + Main.screenPosition.X - player.position.X), player.MountedCenter.Y - 600f);
                vector2.X = (vector2.X + player.Center.X) / 2f + (float)Main.rand.Next(-200, 201);
                vector2.Y -= (float)(100 * num108);
                num78 = (float)Main.mouseX + Main.screenPosition.X - vector2.X;
                num79 = (float)Main.mouseY + Main.screenPosition.Y - vector2.Y;
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
                float speedX4 = num78 + (float)Main.rand.Next(-30, 31) * 0.02f;
                float speedY5 = num79 + (float)Main.rand.Next(-30, 31) * 0.02f;
                Projectile.NewProjectile(source, vector2.X, vector2.Y, speedX4, speedY5, ModContent.ProjectileType<AuraRain>(), damage, knockback, player.whoAmI, 0f, (float)Main.rand.Next(10));
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.RottenChunk, 5).
                AddIngredient(ItemID.DemoniteBar, 5).
                AddIngredient<RottenMatter>(15).
                AddTile(TileID.DemonAltar).
                Register();
        }
    }
}
