using CalamityMod.Buffs.StatDebuffs;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
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
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 100;
            Projectile.timeLeft = 600;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0f, 0.5f, 0.5f);

            int dustTypeOnTimer = dust1;

            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] > 15f)
            {
                for (int num447 = 0; num447 < 2; num447++)
                {
                    Vector2 vector33 = Projectile.position;
                    vector33 -= Projectile.velocity * ((float)num447 * 0.25f);
                    Projectile.alpha = 255;
                    int num448 = Dust.NewDust(vector33, 1, 1, dustTypeOnTimer, 0f, 0f, 0, default, 1f);
                    Main.dust[num448].noGravity = true;
                    Main.dust[num448].position = vector33;
                    Main.dust[num448].scale = (float)Main.rand.Next(70, 110) * 0.013f;
                    Main.dust[num448].velocity *= 0.2f;
                }
                Projectile.ai[0] += 1f;
                if (Projectile.ai[0] == 48f)
                {
                    Projectile.ai[0] = 0f;

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
                        value8 = -Vector2.UnitY.RotatedBy((double)(Projectile.ai[0] * 0.1308997f + (float)num41 * 3.14159274f), default) * value7 * 1.5f;
                        int num42 = Dust.NewDust(Projectile.Center, 0, 0, dustType, 0f, 0f, 160, default, 1f);
                        Main.dust[num42].scale = 0.75f;
                        Main.dust[num42].noGravity = true;
                        Main.dust[num42].position = Projectile.Center + value8;
                        Main.dust[num42].velocity = Projectile.velocity;
                    }
                }
            }

            if (Projectile.localAI[0] == 15f)
            {
                float angleRandom = 0.1f;

                for (int num53 = 0; num53 < 40; num53++)
                {
                    float dustSpeed = Main.rand.NextFloat(6.0f, 12.0f);
                    Vector2 dustVel = new Vector2(dustSpeed, 0.0f).RotatedBy(Projectile.velocity.ToRotation());
                    dustVel = dustVel.RotatedBy(-angleRandom);
                    dustVel = dustVel.RotatedByRandom(2.0f * angleRandom);
                    int randomDustType = Main.rand.NextBool(2) ? dust1 : dust2;

                    int num54 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, randomDustType, dustVel.X, dustVel.Y, 200, default, 1.7f);
                    Dust dust = Main.dust[num54];
                    dust.position = Projectile.Center + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * (float)Projectile.width / 2f;
                    dust.noGravity = true;

                    dust.velocity *= randomDustType == dust2 ? 2f : 4f;

                    num54 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, randomDustType, dustVel.X, dustVel.Y, 100, default, 0.8f);
                    dust.position = Projectile.Center + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * (float)Projectile.width / 2f;

                    dust.velocity *= randomDustType == dust2 ? 1.33f : 2.66f;

                    dust.noGravity = true;
                    dust.fadeIn = 1f;
                    dust.color = Color.Blue * 0.5f;
                }
                for (int num55 = 0; num55 < 20; num55++)
                {
                    float dustSpeed = Main.rand.NextFloat(6.0f, 12.0f);
                    Vector2 dustVel = new Vector2(dustSpeed, 0.0f).RotatedBy(Projectile.velocity.ToRotation());
                    dustVel = dustVel.RotatedBy(-angleRandom);
                    dustVel = dustVel.RotatedByRandom(2.0f * angleRandom);
                    int randomDustType = Main.rand.NextBool(2) ? dust1 : dust2;

                    int num56 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, randomDustType, dustVel.X, dustVel.Y, 0, default, 2f);
                    Dust dust = Main.dust[num56];
                    dust.position = Projectile.Center + Vector2.UnitX.RotatedByRandom(3.1415927410125732).RotatedBy((double)Projectile.velocity.ToRotation(), default) * (float)Projectile.width / 3f;
                    dust.noGravity = true;

                    dust.velocity *= randomDustType == dust2 ? 0.33f : 0.66f;
                }
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 180);
            target.AddBuff(BuffID.Confused, 300);
            target.AddBuff(ModContent.BuffType<Irradiated>(), 180);
        }

        public override void Kill(int timeLeft)
        {
            int height = 150;

            SoundEngine.PlaySound(SoundID.NPCDeath43, Projectile.Center);

            Projectile.position = Projectile.Center;
            Projectile.width = Projectile.height = height;
            Projectile.Center = Projectile.position;
            Projectile.maxPenetrate = -1;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.Damage();

            int num226 = 36;
            for (int num227 = 0; num227 < num226; num227++) // 108 dusts
            {
                Vector2 vector6 = Vector2.Normalize(Projectile.velocity) * new Vector2((float)Projectile.width / 2f, (float)Projectile.height) * 0.3f;
                vector6 = vector6.RotatedBy((double)((float)(num227 - (num226 / 2 - 1)) * 6.28318548f / (float)num226), default) + Projectile.Center;
                Vector2 vector7 = vector6 - Projectile.Center;

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
                Dust d = Dust.NewDustPerfect(Projectile.Center, random ? dust1 : dust2, velocity);
                d.noGravity = true;
                d.customData = 0.025f;
                d.scale = 2f;
            }
        }
    }
}
