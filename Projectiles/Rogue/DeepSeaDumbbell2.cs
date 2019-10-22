using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Rogue
{
    public class DeepSeaDumbbell2 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Deep Sea Dumbbell");
        }

        public override void SetDefaults()
        {
            projectile.width = 26;
            projectile.height = 26;
            projectile.friendly = true;
            projectile.Calamity().rogue = true;
            projectile.ignoreWater = true;
            projectile.penetrate = 1;
        }

        public override void AI()
        {
            if (projectile.ai[0] < 60f)
                projectile.ai[0] += 1f;
            else
            {
                float num472 = projectile.Center.X;
                float num473 = projectile.Center.Y;
                float num474 = 600f;
                bool flag17 = false;

                for (int num475 = 0; num475 < 200; num475++)
                {
                    if (Main.npc[num475].CanBeChasedBy(projectile, false) && Collision.CanHit(projectile.Center, 1, 1, Main.npc[num475].Center, 1, 1))
                    {
                        float num476 = Main.npc[num475].position.X + (float)(Main.npc[num475].width / 2);
                        float num477 = Main.npc[num475].position.Y + (float)(Main.npc[num475].height / 2);
                        float num478 = Math.Abs(projectile.position.X + (float)(projectile.width / 2) - num476) + Math.Abs(projectile.position.Y + (float)(projectile.height / 2) - num477);
                        if (num478 < num474)
                        {
                            num474 = num478;
                            num472 = num476;
                            num473 = num477;
                            flag17 = true;
                        }
                    }
                }

                if (flag17)
                {
                    float num483 = 20f;
                    Vector2 vector35 = new Vector2(projectile.position.X + (float)projectile.width * 0.5f, projectile.position.Y + (float)projectile.height * 0.5f);
                    float num484 = num472 - vector35.X;
                    float num485 = num473 - vector35.Y;
                    float num486 = (float)Math.Sqrt((double)(num484 * num484 + num485 * num485));
                    num486 = num483 / num486;
                    num484 *= num486;
                    num485 *= num486;
                    projectile.velocity.X = (projectile.velocity.X * 20f + num484) / 21f;
                    projectile.velocity.Y = (projectile.velocity.Y * 20f + num485) / 21f;
                }
            }

            projectile.rotation += Math.Abs(projectile.velocity.X) * 0.01f * (float)projectile.direction;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Main.PlaySound(4, (int)projectile.position.X, (int)projectile.position.Y, 43, 0.65f, 0f);

            if (projectile.velocity.X != oldVelocity.X)
                projectile.velocity.X = -oldVelocity.X;
            if (projectile.velocity.Y != oldVelocity.Y)
                projectile.velocity.Y = -oldVelocity.Y;

            if (projectile.owner == Main.myPlayer)
            {
                Projectile.NewProjectile(projectile.position.X, projectile.position.Y, projectile.velocity.X, projectile.velocity.Y, ModContent.ProjectileType<DeepSeaDumbbell3>(),
                        (int)((double)projectile.damage * 0.75), projectile.knockBack * 0.75f, Main.myPlayer, 0f, 0f);

                float num628 = (float)Main.rand.Next(-35, 36) * 0.01f;
                float num629 = (float)Main.rand.Next(-35, 36) * 0.01f;
                int num3;
                for (int num627 = 0; num627 < 2; num627 = num3 + 1)
                {
                    if (num627 == 1)
                    {
                        num628 *= 10f;
                        num629 *= 10f;
                    }
                    else
                    {
                        num628 *= -10f;
                        num629 *= -10f;
                    }

                    Projectile.NewProjectile(projectile.position.X, projectile.position.Y, num628, num629, ModContent.ProjectileType<DeepSeaDumbbellWeight>(),
                        (int)((double)projectile.damage * 0.25), projectile.knockBack * 0.25f, Main.myPlayer, 0f, 0f);

                    num3 = num627;
                }
            }

            projectile.Kill();

            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (target.defense > 0)
                target.defense -= 15;

            target.AddBuff(ModContent.BuffType<CrushDepth>(), 600);

            Main.PlaySound(4, (int)projectile.position.X, (int)projectile.position.Y, 43, 0.65f, 0f);

            projectile.velocity.X = -projectile.velocity.X;
            projectile.velocity.Y = -projectile.velocity.Y;

            if (projectile.owner == Main.myPlayer)
            {
                Projectile.NewProjectile(projectile.position.X, projectile.position.Y, projectile.velocity.X, projectile.velocity.Y, ModContent.ProjectileType<DeepSeaDumbbell3>(),
                        (int)((double)projectile.damage * 0.75), projectile.knockBack * 0.75f, Main.myPlayer, 0f, 0f);

                float num628 = (float)Main.rand.Next(-35, 36) * 0.01f;
                float num629 = (float)Main.rand.Next(-35, 36) * 0.01f;
                int num3;
                for (int num627 = 0; num627 < 2; num627 = num3 + 1)
                {
                    if (num627 == 1)
                    {
                        num628 *= 10f;
                        num629 *= 10f;
                    }
                    else
                    {
                        num628 *= -10f;
                        num629 *= -10f;
                    }

                    Projectile.NewProjectile(projectile.position.X, projectile.position.Y, num628, num629, ModContent.ProjectileType<DeepSeaDumbbellWeight>(),
                        (int)((double)projectile.damage * 0.25), projectile.knockBack * 0.25f, Main.myPlayer, 0f, 0f);

                    num3 = num627;
                }
            }

            projectile.Kill();
        }
    }
}
