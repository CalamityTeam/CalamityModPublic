using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class Vortexpopper : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Vortexpopper");
            Tooltip.SetDefault("Fires bubbles everywhere that pop into bullets");
        }

        public override void SetDefaults()
        {
            item.damage = 22;
            item.ranged = true;
            item.width = 46;
            item.height = 22;
            item.useTime = 16;
            item.useAnimation = 16;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 3.25f;
            item.value = Item.buyPrice(0, 95, 0, 0);
            item.rare = 9;
            item.UseSound = SoundID.Item95;
            item.autoReuse = true;
            item.shootSpeed = 50f;
            item.shoot = ProjectileID.Xenopopper;
            item.useAmmo = AmmoID.Bullet;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-5, 0);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            float num72 = item.shootSpeed;
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
            num78 *= num80;
            num79 *= num80;
            Vector2 value6 = Vector2.Normalize(new Vector2(num78, num79)) * 40f * item.scale;
            if (Collision.CanHit(vector2, 0, 0, vector2 + value6, 0, 0))
            {
            }
            float ai = new Vector2(num78, num79).ToRotation();
            float num96 = 2.09439516f;
            int num97 = Main.rand.Next(6, 10);
            if (Main.rand.NextBool(4))
            {
                num97++;
            }
            for (int num98 = 0; num98 < num97; num98++)
            {
                float scaleFactor2 = (float)Main.rand.NextDouble() * 0.2f + 0.05f;
                Vector2 vector6 = new Vector2(num78, num79).RotatedBy((double)(num96 * (float)Main.rand.NextDouble() - num96 / 2f), default) * scaleFactor2;
                int num99 = Projectile.NewProjectile(position.X, position.Y, vector6.X, vector6.Y, ProjectileID.Xenopopper, damage, knockBack, player.whoAmI, ai, 0f);
                Main.projectile[num99].localAI[0] = (float)type;
                Main.projectile[num99].localAI[1] = 12f;
            }
            int num107 = Main.rand.Next(5, 10);
            for (int num108 = 0; num108 < num107; num108++)
            {
                vector2 = new Vector2(player.position.X + (float)player.width * 0.5f + (float)(Main.rand.Next(201) * -(float)player.direction) + ((float)Main.mouseX + Main.screenPosition.X - player.position.X), player.MountedCenter.Y - 600f);
                vector2.X = (vector2.X + player.Center.X) / 2f + (float)Main.rand.Next(-800, 801);
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
                float speedX4 = num78 + (float)Main.rand.Next(-1000, 1001) * 0.02f;
                float speedY5 = num79 + (float)Main.rand.Next(-1000, 1001) * 0.02f;
                int projectile = Projectile.NewProjectile(vector2.X, vector2.Y, speedX4, speedY5, ProjectileID.Xenopopper, damage, knockBack, player.whoAmI, ai, 0f);
                Main.projectile[projectile].localAI[0] = (float)type;
                Main.projectile[projectile].localAI[1] = 12f;
            }
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Xenopopper);
            recipe.AddIngredient(ItemID.FragmentVortex, 20);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
