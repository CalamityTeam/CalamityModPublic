using CalamityMod.Buffs.StatDebuffs;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Ranged
{
    public class KarasawaShot : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        private int dust1 = 187; //160
        private int dust2 = 229; //187

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Karasawa Shot");
        }

        public override void SetDefaults()
        {
            projectile.width = 4;
            projectile.height = 4;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.ranged = true;
            projectile.penetrate = -1;
            projectile.extraUpdates = 100;
            projectile.timeLeft = 600;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, 0f, 0.5f, 0.5f);

            int dustTypeOnTimer = dust1;

            projectile.localAI[0] += 1f;
            if (projectile.localAI[0] > 15f)
            {
                for (int num447 = 0; num447 < 2; num447++)
                {
                    Vector2 vector33 = projectile.position;
                    vector33 -= projectile.velocity * ((float)num447 * 0.25f);
                    projectile.alpha = 255;
                    int num448 = Dust.NewDust(vector33, 1, 1, dustTypeOnTimer, 0f, 0f, 0, default, 1f);
                    Main.dust[num448].noGravity = true;
                    Main.dust[num448].position = vector33;
                    Main.dust[num448].scale = (float)Main.rand.Next(70, 110) * 0.013f;
                    Main.dust[num448].velocity *= 0.2f;
                }
                projectile.ai[0] += 1f;
                if (projectile.ai[0] == 48f)
                {
                    projectile.ai[0] = 0f;

                    if (dustTypeOnTimer == dust1)
                        dustTypeOnTimer = dust2;
                    else
                        dustTypeOnTimer = dust1;
                }
                else
                {
                    Vector2 value7 = new Vector2(5f, 10f);

                    for (int num41 = 0; num41 < 2; num41++)
                    {
                        int dustType = num41 == 0 ? dust1 : dust2;
                        Vector2 value8 = Vector2.UnitX * -12f;
                        value8 = -Vector2.UnitY.RotatedBy((double)(projectile.ai[0] * 0.1308997f + (float)num41 * 3.14159274f), default) * value7 * 1.5f;
                        int num42 = Dust.NewDust(projectile.Center, 0, 0, dustType, 0f, 0f, 160, default, 1f);
                        Main.dust[num42].scale = 0.75f;
                        Main.dust[num42].noGravity = true;
                        Main.dust[num42].position = projectile.Center + value8;
                        Main.dust[num42].velocity = projectile.velocity;
                    }
                }
            }

            if (projectile.localAI[0] == 15f)
            {
                float angleRandom = 0.1f;

                for (int num53 = 0; num53 < 40; num53++)
                {
                    float dustSpeed = Main.rand.NextFloat(6.0f, 12.0f);
                    Vector2 dustVel = new Vector2(dustSpeed, 0.0f).RotatedBy(projectile.velocity.ToRotation());
                    dustVel = dustVel.RotatedBy(-angleRandom);
                    dustVel = dustVel.RotatedByRandom(2.0f * angleRandom);
                    int randomDustType = Main.rand.NextBool(2) ? dust1 : dust2;

                    int num54 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, randomDustType, dustVel.X, dustVel.Y, 200, default, 1.7f);
                    Dust dust = Main.dust[num54];
                    dust.position = projectile.Center + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * (float)projectile.width / 2f;
                    dust.noGravity = true;

                    dust.velocity *= randomDustType == dust2 ? 2f : 4f;

                    num54 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, randomDustType, dustVel.X, dustVel.Y, 100, default, 0.8f);
                    dust.position = projectile.Center + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * (float)projectile.width / 2f;

                    dust.velocity *= randomDustType == dust2 ? 1.33f : 2.66f;

                    dust.noGravity = true;
                    dust.fadeIn = 1f;
                    dust.color = Color.Blue * 0.5f;
                }
                for (int num55 = 0; num55 < 20; num55++)
                {
                    float dustSpeed = Main.rand.NextFloat(6.0f, 12.0f);
                    Vector2 dustVel = new Vector2(dustSpeed, 0.0f).RotatedBy(projectile.velocity.ToRotation());
                    dustVel = dustVel.RotatedBy(-angleRandom);
                    dustVel = dustVel.RotatedByRandom(2.0f * angleRandom);
                    int randomDustType = Main.rand.NextBool(2) ? dust1 : dust2;

                    int num56 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, randomDustType, dustVel.X, dustVel.Y, 0, default, 2f);
                    Dust dust = Main.dust[num56];
                    dust.position = projectile.Center + Vector2.UnitX.RotatedByRandom(3.1415927410125732).RotatedBy((double)projectile.velocity.ToRotation(), default) * (float)projectile.width / 3f;
                    dust.noGravity = true;

                    dust.velocity *= randomDustType == dust2 ? 0.33f : 0.66f;
                }
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 600);
            target.AddBuff(BuffID.Confused, 300);
            target.AddBuff(ModContent.BuffType<Irradiated>(), 300);
        }

        public override void Kill(int timeLeft)
        {
            int height = 150;

            Main.PlaySound(SoundID.NPCDeath43, projectile.Center);

            projectile.position = projectile.Center;
            projectile.width = projectile.height = height;
            projectile.Center = projectile.position;
            projectile.maxPenetrate = -1;
            projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
            projectile.Damage();

            int num226 = 36;
            for (int num227 = 0; num227 < num226; num227++) // 108 dusts
            {
                Vector2 vector6 = Vector2.Normalize(projectile.velocity) * new Vector2((float)projectile.width / 2f, (float)projectile.height) * 0.3f;
                vector6 = vector6.RotatedBy((double)((float)(num227 - (num226 / 2 - 1)) * 6.28318548f / (float)num226), default) + projectile.Center;
                Vector2 vector7 = vector6 - projectile.Center;

                int num228 = Dust.NewDust(vector6 + vector7, 0, 0, Main.rand.NextBool(2) ? dust1 : dust2, vector7.X * 0.3f, vector7.Y * 0.3f, 100, default, 2f);
                Main.dust[num228].noGravity = true;

                int num229 = Dust.NewDust(vector6 + vector7, 0, 0, Main.rand.NextBool(2) ? dust1 : dust2, vector7.X * 0.2f, vector7.Y * 0.2f, 100, default, 2f);
                Main.dust[num229].noGravity = true;

                int num230 = Dust.NewDust(vector6 + vector7, 0, 0, Main.rand.NextBool(2) ? dust1 : dust2, vector7.X * 0.1f, vector7.Y * 0.1f, 100, default, 2f);
                Main.dust[num230].noGravity = true;
            }

            bool random = Main.rand.NextBool();
            float angleStart = Main.rand.NextFloat(0f, MathHelper.TwoPi);
            for (float angle = 0f; angle < MathHelper.TwoPi; angle += 0.05f) // 125 dusts
            {
                random = !random;
                Vector2 velocity = angle.ToRotationVector2() * (2f + (float)(Math.Sin(angleStart + angle * 3f) + 1) * 2.5f) * Main.rand.NextFloat(0.95f, 1.05f);
                Dust d = Dust.NewDustPerfect(projectile.Center, random ? dust1 : dust2, velocity);
                d.noGravity = true;
                d.customData = 0.025f;
                d.scale = 2f;
            }
        }
    }
}
