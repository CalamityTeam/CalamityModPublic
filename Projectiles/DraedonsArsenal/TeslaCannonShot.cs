using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.DraedonsArsenal
{
    public class TeslaCannonShot : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Misc";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        private int[] dustArray = new int[7] { 56, 111, 137, 160, 206, 229, 226 };

        private int arcs = 0;

        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 100;
            Projectile.timeLeft = 600;
        }

        public override void AI()
        {
            bool notArcingProjectile = Projectile.ai[1] >= 0f;

            Lighting.AddLight(Projectile.Center, 0f, 0.3f, 0.4f);

            float createDustVar = 10f;
            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] > createDustVar)
            {
                for (int i = 0; i < 2; i++)
                {
                    Vector2 projPos = Projectile.position;
                    projPos -= Projectile.velocity * (i * 0.25f);
                    int teslaDust = Dust.NewDust(projPos, 1, 1, dustArray[3], 0f, 0f, 0, default, 1f);
                    Main.dust[teslaDust].noGravity = true;
                    Main.dust[teslaDust].position = projPos;
                    Main.dust[teslaDust].scale = Main.rand.Next(70, 110) * 0.026f;
                }

                if (Main.rand.Next(6) == 0)
                    Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, dustArray[6], 0f, 0f);

                if (notArcingProjectile)
                {
                    if (Projectile.ai[1] > 0f)
                        Projectile.ai[1] -= 1f;

                    Projectile.ai[0] += 1f;
                    if (Projectile.ai[0] == 48f)
                        Projectile.ai[0] = 0f;
                    else
                    {
                        Vector2 dustRotateVector = new Vector2(5f, 10f);
                        Vector2 dustRotate = Vector2.UnitX * -12f;
                        float scale = 1f;

                        for (int j = 0; j < 2; j++)
                        {
                            dustRotate = -Vector2.UnitY.RotatedBy(Projectile.ai[0] * 0.1308997f + j * MathHelper.Pi) * dustRotateVector;
                            int lightBlueDust = Dust.NewDust(Projectile.Center, 0, 0, dustArray[2], 0f, 0f, 160, default, 1f);
                            Main.dust[lightBlueDust].scale = scale;
                            Main.dust[lightBlueDust].noGravity = true;
                            Main.dust[lightBlueDust].position = Projectile.Center + dustRotate;
                        }

                        for (int j = 0; j < 2; j++)
                        {
                            dustRotate = -Vector2.UnitY.RotatedBy(Projectile.ai[0] * 0.1308997f + j * MathHelper.Pi) * dustRotateVector * 1.5f;
                            int lightBlueDust = Dust.NewDust(Projectile.Center, 0, 0, dustArray[1], 0f, 0f, 160, default, 1f);
                            Main.dust[lightBlueDust].scale = scale;
                            Main.dust[lightBlueDust].noGravity = true;
                            Main.dust[lightBlueDust].position = Projectile.Center + dustRotate;
                        }

                        for (int j = 0; j < 2; j++)
                        {
                            dustRotate = -Vector2.UnitY.RotatedBy(Projectile.ai[0] * 0.1308997f + j * MathHelper.Pi) * dustRotateVector * 2f;
                            int lightBlueDust = Dust.NewDust(Projectile.Center, 0, 0, dustArray[0], 0f, 0f, 160, default, 1f);
                            Main.dust[lightBlueDust].scale = scale;
                            Main.dust[lightBlueDust].noGravity = true;
                            Main.dust[lightBlueDust].position = Projectile.Center + dustRotate;
                        }
                    }

                    if (Projectile.ai[1] == 0f && Main.myPlayer == Projectile.owner)
                    {
                        if (arcs < 10)
                        {
                            for (int i = 0; i < Main.maxNPCs; i++)
                            {
                                if (!Main.npc[i].CanBeChasedBy(Projectile, false) || !Collision.CanHit(Projectile.Center, 1, 1, Main.npc[i].Center, 1, 1))
                                    continue;

                                if (Projectile.Center.ManhattanDistance(Main.npc[i].Center) < 600f)
                                {
                                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.SafeDirectionTo(Main.npc[i].Center) * 2f, Projectile.type, (int)(Projectile.damage * 0.6), Projectile.knockBack * 0.6f, Projectile.owner, 0f, -1f);

                                    Projectile.ai[1] = 60f;

                                    arcs++;
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            if (Projectile.localAI[0] == createDustVar && notArcingProjectile)
                ElectricalBurst(5f, 9f);
        }

        public override void OnKill(int timeLeft)
        {
            bool notArcingProjectile = Projectile.ai[1] >= 0f;

            int height = notArcingProjectile ? 120 : 60;
            Projectile.position = Projectile.Center;
            Projectile.width = Projectile.height = height;
            Projectile.Center = Projectile.position;
            Projectile.maxPenetrate = -1;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.Damage();

            SoundEngine.PlaySound(SoundID.Item125, Projectile.Center);

            int dustAmt = notArcingProjectile ? 400 : 100;
            int fourth = notArcingProjectile ? 100 : 25;
            int half = notArcingProjectile ? 200 : 50;
            int threeFourths = notArcingProjectile ? 300 : 75;

            float dustSpeed = notArcingProjectile ? 12f : 6f;

            for (int i = 0; i < dustAmt; i++)
            {
                int dustType = dustArray[4];
                float deathDustSpeed = dustSpeed;
                if (i > fourth)
                {
                    deathDustSpeed = dustSpeed * 0.6875f;
                    dustType = dustArray[5];
                }
                if (i > half)
                {
                    deathDustSpeed = dustSpeed * 0.5f;
                    dustType = dustArray[4];
                }
                if (i > threeFourths)
                {
                    deathDustSpeed = dustSpeed * 0.3125f;
                    dustType = dustArray[5];
                }
                float scale = (dustType == dustArray[4] ? 3f : 1.5f);

                int deathDarkBlue = Dust.NewDust(Projectile.Center, 6, 6, dustType, 0f, 0f, 100, default, scale);
                float deathDustX = Main.dust[deathDarkBlue].velocity.X;
                float deathDustY = Main.dust[deathDarkBlue].velocity.Y;

                if (deathDustX == 0f && deathDustY == 0f)
                    deathDustX = 1f;

                float deathDustVel = (float)Math.Sqrt(deathDustX * deathDustX + deathDustY * deathDustY);
                deathDustVel = deathDustSpeed / deathDustVel;
                if (i > threeFourths)
                {
                    deathDustX = deathDustX * deathDustVel * 0.7f;
                    deathDustY *= deathDustVel;
                }
                else if (i > half)
                {
                    deathDustX *= deathDustVel;
                    deathDustY = deathDustY * deathDustVel * 0.7f;
                }
                else if (i > fourth)
                {
                    deathDustX = deathDustX * deathDustVel * 0.7f;
                    deathDustY *= deathDustVel;
                }
                else
                {
                    deathDustX *= deathDustVel;
                    deathDustY = deathDustY * deathDustVel * 0.7f;
                }

                Dust dust8 = Main.dust[deathDarkBlue];
                dust8.velocity *= 0.5f;
                dust8.velocity.X = dust8.velocity.X + deathDustX;
                dust8.velocity.Y = dust8.velocity.Y + deathDustY;
                dust8.noGravity = true;

                if (i > threeFourths)
                {
                    int tealDust = Dust.NewDust(Projectile.Center, 6, 6, dustArray[5], 0f, 0f, 100, default, 1.3f);
                    float tealDustX = Main.dust[tealDust].velocity.X;
                    float tealDustY = Main.dust[tealDust].velocity.Y;

                    if (tealDustX == 0f && tealDustY == 0f)
                        tealDustX = 1f;

                    float tealDustVel = (float)Math.Sqrt(tealDustX * tealDustX + tealDustY * tealDustY);
                    tealDustVel = 16f / tealDustVel;
                    tealDustX = tealDustX * tealDustVel * 1.25f;
                    tealDustY = tealDustY * tealDustVel * 0.75f;

                    Dust dust9 = Main.dust[tealDust];
                    dust9.velocity *= 0.5f;
                    dust9.velocity.X = dust9.velocity.X + tealDustX;
                    dust9.velocity.Y = dust9.velocity.Y + tealDustY;
                    dust9.noGravity = true;
                }
            }
        }

        private void ElectricalBurst(float speed1, float speed2)
        {
            float angleRandom = 0.05f;

            for (int i = 0; i < 40; i++)
            {
                float dustSpeed = Main.rand.NextFloat(speed1, speed2);
                Vector2 dustVel = new Vector2(dustSpeed, 0.0f).RotatedBy(Projectile.velocity.ToRotation());
                dustVel = dustVel.RotatedBy(-angleRandom);
                dustVel = dustVel.RotatedByRandom(2f * angleRandom);
                int randomDustType = Main.rand.Next(2) == 0 ? dustArray[4] : dustArray[5];
                float scale = randomDustType == dustArray[4] ? 1.5f : 1f;

                int electricDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, randomDustType, dustVel.X, dustVel.Y, 200, default, 2.5f * scale);
                Main.dust[electricDust].position = Projectile.Center + Vector2.UnitY.RotatedByRandom(MathHelper.Pi) * (float)Main.rand.NextDouble() * Projectile.width / 2f;
                Main.dust[electricDust].noGravity = true;

                Dust dust = Main.dust[electricDust];
                dust.velocity *= 3f;
                dust = Main.dust[electricDust];

                electricDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, randomDustType, dustVel.X, dustVel.Y, 100, default, 1.5f * scale);
                Main.dust[electricDust].position = Projectile.Center + Vector2.UnitY.RotatedByRandom(MathHelper.Pi) * (float)Main.rand.NextDouble() * Projectile.width / 2f;

                dust = Main.dust[electricDust];
                dust.velocity *= 2f;

                Main.dust[electricDust].noGravity = true;
                Main.dust[electricDust].fadeIn = 1f;
                Main.dust[electricDust].color = Color.Cyan * 0.5f;

                dust = Main.dust[electricDust];
            }
            for (int j = 0; j < 20; j++)
            {
                float dustSpeed = Main.rand.NextFloat(speed1, speed2);
                Vector2 dustVel = new Vector2(dustSpeed, 0f).RotatedBy(Projectile.velocity.ToRotation());
                dustVel = dustVel.RotatedBy(-angleRandom);
                dustVel = dustVel.RotatedByRandom(2f * angleRandom);
                int randomDustType = Main.rand.Next(2) == 0 ? dustArray[4] : dustArray[5];
                float scale = randomDustType == dustArray[4] ? 1.5f : 1f;

                int electricDust2 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, randomDustType, dustVel.X, dustVel.Y, 0, default, 3f * scale);
                Main.dust[electricDust2].position = Projectile.Center + Vector2.UnitX.RotatedByRandom(MathHelper.Pi).RotatedBy(Projectile.velocity.ToRotation()) * Projectile.width / 3f;
                Main.dust[electricDust2].noGravity = true;

                Dust dust = Main.dust[electricDust2];
                dust.velocity *= 0.5f;
                dust = Main.dust[electricDust2];
            }
        }
    }
}
