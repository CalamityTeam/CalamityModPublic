using CalamityMod.Items.Weapons.Rogue;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class InfernalKrisProjectile : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/InfernalKris";

        public static int spinTime = 280;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Infernal Kris");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.friendly = true;
            projectile.penetrate = 2;
            projectile.timeLeft = 300;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            if (projectile.timeLeft < spinTime)
            {
                projectile.rotation += 0.4f * projectile.direction;

                float minScale = 1.9f;
                float maxScale = 2.5f;
                int dust = Dust.NewDust(projectile.position - new Vector2(10, 10), 30, 30, 6, projectile.velocity.X, projectile.velocity.Y, 0, default, Main.rand.NextFloat(minScale, maxScale));
                Main.dust[dust].noGravity = true;
            }
            else
            {
                projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + 0.785f;
            }

            projectile.velocity.Y += 0.01f;
            if (projectile.velocity.Y > 10f)
            {
                projectile.velocity.Y = 10f;
            }
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (projectile.timeLeft < spinTime)
            {
                damage = (int)(projectile.damage * 1.75f) + Main.rand.Next(0, 6);
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            int debuffTime = 60 * (projectile.Calamity().stealthStrike ? Main.rand.Next(4,8) : Main.rand.Next(3,6));
            target.AddBuff(BuffID.OnFire, debuffTime);

            if (projectile.Calamity().stealthStrike)
            {
                int sparkCount = Main.rand.Next(3, 7);
                for (int i = 0; i < sparkCount; i++)
                {
                    int sparkScatter = 1;
                    Vector2 sparkVelocity = new Vector2(Main.rand.NextFloat(-sparkScatter, sparkScatter), Main.rand.NextFloat(-sparkScatter - 2, sparkScatter + 2));

                    sparkVelocity.Normalize();
                    sparkVelocity *= 3;

                    Projectile.NewProjectile(projectile.Center, sparkVelocity, ModContent.ProjectileType<InfernalKrisCinder>(), (int)(projectile.damage * 0.4f), 0, projectile.owner, 0, 0);
                }
                Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<InfernalKrisExplosion>(), (int)(projectile.damage * 0.4f), 0, projectile.owner, 0, 0);
                Main.PlaySound(SoundID.Item74, projectile.position);
            }
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            int debuffTime = 60 * (projectile.Calamity().stealthStrike ? Main.rand.Next(4,8) : Main.rand.Next(3,6));
            target.AddBuff(BuffID.OnFire, debuffTime);

            if (projectile.Calamity().stealthStrike)
            {
                int sparkCount = Main.rand.Next(3, 7);
                for (int i = 0; i < sparkCount; i++)
                {
                    int sparkScatter = 1;
                    Vector2 sparkVelocity = new Vector2(Main.rand.NextFloat(-sparkScatter, sparkScatter), Main.rand.NextFloat(-sparkScatter - 2, sparkScatter + 2));

                    sparkVelocity.Normalize();
                    sparkVelocity *= 3;

                    Projectile.NewProjectile(projectile.Center, sparkVelocity, ModContent.ProjectileType<InfernalKrisCinder>(), (int)(projectile.damage * 0.4f), 0, projectile.owner, 0, 0);
                }
                Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<InfernalKrisExplosion>(), (int)(projectile.damage * 0.4f), 0, projectile.owner, 0, 0);
                Main.PlaySound(SoundID.Item74, projectile.position);
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (projectile.Calamity().stealthStrike)
            {
                // If this is a stealth strike, make the blade glow orange
                Color glowColour = new Color(255, 215, 100, 100);
                CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], glowColour, 1);

                float minScale = 1.9f;
                float maxScale = 2.5f;
                int dust = Dust.NewDust(projectile.position, 10, 10, 6, projectile.velocity.X, projectile.velocity.Y, 0, default, Main.rand.NextFloat(minScale, maxScale));
                Main.dust[dust].noGravity = true;
            }
            else
            {
                CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            }
            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(projectile.position + projectile.velocity, projectile.velocity, projectile.width, projectile.height);

            projectile.ai[0] = 0;
            projectile.ai[1] = 0;

            if (projectile.velocity.X != oldVelocity.X)
            {
                if (oldVelocity.X < 0)
                {
                    projectile.ai[0] = 1;
                }
                if (oldVelocity.X > 0)
                {
                    projectile.ai[0] = -1;
                }
            }
            if (projectile.velocity.Y != oldVelocity.Y)
            {
                if (oldVelocity.Y < 0)
                {
                    projectile.ai[1] = 1;
                }
                if (oldVelocity.Y > 0)
                {
                    projectile.ai[1] = -1;
                }
            }

            Main.PlaySound(SoundID.Item74, projectile.position);
            projectile.Kill();
            return false;
        }

        public override void Kill(int timeLeft)
        {
            if (Main.rand.Next(4) == 0)
            {
                Item.NewItem((int)projectile.position.X, (int)projectile.position.Y, projectile.width, projectile.height, ModContent.ItemType<InfernalKris>());
            }

            if (projectile.Calamity().stealthStrike)
            {
                int sparkCount = Main.rand.Next(3, 7);
                for (int i = 0; i < sparkCount; i++)
                {
                    int sparkScatter = 1;
                    Vector2 sparkVelocity = new Vector2(Main.rand.NextFloat(-sparkScatter, sparkScatter), Main.rand.NextFloat(-sparkScatter, sparkScatter));

                    if (projectile.ai[0] != 0)
                    {
                        sparkVelocity.X *= -1;
                    }
                    if (projectile.ai[1] != 0)
                    {
                        sparkVelocity.Y *= -1;
                    }

                    sparkVelocity.X += projectile.ai[0];
                    sparkVelocity.Y += projectile.ai[1];
                    sparkVelocity.Normalize();
                    sparkVelocity *= 3;

                    Projectile.NewProjectile(projectile.Center, sparkVelocity, ModContent.ProjectileType<InfernalKrisCinder>(), (int)(projectile.damage * 0.4f), 0, projectile.owner, 0, 0);
                }
                Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<InfernalKrisExplosion>(), (int)(projectile.damage * 0.4f), 0, projectile.owner, 0, 0);
                Main.PlaySound(SoundID.Item74, projectile.position);
            }
        }
    }
}
