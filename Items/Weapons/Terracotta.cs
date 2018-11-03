using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons
{
    public class Terracotta : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Terra-cotta");
            Tooltip.SetDefault("Causes enemies to erupt into healing projectiles on death\nEnemies explode on death");
        }

        public override void SetDefaults()
        {
            item.width = 48;
            item.damage = 125;
            item.melee = true;
            item.useAnimation = 17;
            item.useTime = 17;
            item.useTurn = true;
            item.useStyle = 1;
            item.knockBack = 5.5f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 60;
            item.maxStack = 1;
            item.value = 800000;
            item.rare = 8;
            item.shootSpeed = 5f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "BurntSienna");
            recipe.AddIngredient(null, "UnholyCore", 3);
            recipe.AddIngredient(null, "CoreofCinder", 3);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            float spread = 180f * 0.0174f;
            double startAngle = Math.Atan2(item.shootSpeed, item.shootSpeed) - spread / 2;
            double deltaAngle = spread / 8f;
            double offsetAngle;
            int i;
            if (target.life <= 0)
            {
                for (i = 0; i < 1; i++)
                {
                    float randomSpeedX = (float)Main.rand.Next(3);
                    float randomSpeedY = (float)Main.rand.Next(3, 5);
                    offsetAngle = (startAngle + deltaAngle * (i + i * i) / 2f) + 32f * i;
                    int projectile1 = Projectile.NewProjectile(target.Center.X, target.Center.Y, (float)(Math.Sin(offsetAngle) * 5f), (float)(Math.Cos(offsetAngle) * 5f), mod.ProjectileType("Terracotta"), damage, knockback, Main.myPlayer);
                    int projectile2 = Projectile.NewProjectile(target.Center.X, target.Center.Y, (float)(-Math.Sin(offsetAngle) * 5f), (float)(-Math.Cos(offsetAngle) * 5f), mod.ProjectileType("Terracotta"), damage, knockback, Main.myPlayer);
                    int projectile3 = Projectile.NewProjectile(target.Center.X, target.Center.Y, (float)(-Math.Sin(offsetAngle) * 5f), (float)(-Math.Cos(offsetAngle) * 5f), mod.ProjectileType("Terracotta"), damage, knockback, Main.myPlayer);
                    Main.projectile[projectile1].velocity.X = -randomSpeedX;
                    Main.projectile[projectile1].velocity.Y = -randomSpeedY;
                    Main.projectile[projectile2].velocity.X = randomSpeedX;
                    Main.projectile[projectile2].velocity.Y = -randomSpeedY;
                    Main.projectile[projectile3].velocity.X = 0f;
                    Main.projectile[projectile3].velocity.Y = -randomSpeedY;
                }
                Projectile.NewProjectile(target.Center.X, target.Center.Y, 0f, 0f, mod.ProjectileType("TerracottaExplosion"), damage, knockback, Main.myPlayer);
            }
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.Next(5) == 0)
            {
                int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 246);
            }
        }
    }
}
