using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.DraedonsArsenal
{
    public class TeslaCannonShot : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        private int[] dustArray = new int[7] { 56, 111, 137, 160, 206, 229, 226 };

        private int arcs = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Tesla Beam");
        }

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
                for (int num447 = 0; num447 < 2; num447++)
                {
                    Vector2 vector33 = Projectile.position;
                    vector33 -= Projectile.velocity * (num447 * 0.25f);
                    int num448 = Dust.NewDust(vector33, 1, 1, dustArray[3], 0f, 0f, 0, default, 1f);
                    Main.dust[num448].noGravity = true;
                    Main.dust[num448].position = vector33;
                    Main.dust[num448].scale = Main.rand.Next(70, 110) * 0.026f;
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
                        Vector2 value7 = new Vector2(5f, 10f);
                        Vector2 value8 = Vector2.UnitX * -12f;
                        float scale = 1f;

                        for (int num41 = 0; num41 < 2; num41++)
                        {
                            value8 = -Vector2.UnitY.RotatedBy(Projectile.ai[0] * 0.1308997f + num41 * MathHelper.Pi) * value7;
                            int num42 = Dust.NewDust(Projectile.Center, 0, 0, dustArray[2], 0f, 0f, 160, default, 1f);
                            Main.dust[num42].scale = scale;
                            Main.dust[num42].noGravity = true;
                            Main.dust[num42].position = Projectile.Center + value8;
                        }

                        for (int num41 = 0; num41 < 2; num41++)
                        {
                            value8 = -Vector2.UnitY.RotatedBy(Projectile.ai[0] * 0.1308997f + num41 * MathHelper.Pi) * value7 * 1.5f;
                            int num42 = Dust.NewDust(Projectile.Center, 0, 0, dustArray[1], 0f, 0f, 160, default, 1f);
                            Main.dust[num42].scale = scale;
                            Main.dust[num42].noGravity = true;
                            Main.dust[num42].position = Projectile.Center + value8;
                        }

                        for (int num41 = 0; num41 < 2; num41++)
                        {
                            value8 = -Vector2.UnitY.RotatedBy(Projectile.ai[0] * 0.1308997f + num41 * MathHelper.Pi) * value7 * 2f;
                            int num42 = Dust.NewDust(Projectile.Center, 0, 0, dustArray[0], 0f, 0f, 160, default, 1f);
                            Main.dust[num42].scale = scale;
                            Main.dust[num42].noGravity = true;
                            Main.dust[num42].position = Projectile.Center + value8;
                        }
                    }

                    if (Projectile.ai[1] == 0f && Main.myPlayer == Projectile.owner)
                    {
                        if (arcs < 10)
                        {
                            Vector2 vector35 = new Vector2(Projectile.position.X + Projectile.width * 0.5f, Projectile.position.Y + Projectile.height * 0.5f);
                            float num474 = 600f;

                            for (int i = 0; i < Main.maxNPCs; i++)
                            {
                                if (!Main.npc[i].CanBeChasedBy(Projectile, false) || !Collision.CanHit(Projectile.Center, 1, 1, Main.npc[i].Center, 1, 1))
                                    continue;

                                if (Projectile.Center.ManhattanDistance(Main.npc[i].Center) < num474)
                                {
                                    Projectile.NewProjectile(Projectile.Center, Projectile.SafeDirectionTo(Main.npc[i].Center) * 2f, Projectile.type, (int)(Projectile.damage * 0.6), Projectile.knockBack * 0.6f, Projectile.owner, 0f, -1f);

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

        public override void Kill(int timeLeft)
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

            for (int num672 = 0; num672 < dustAmt; num672++)
            {
                int dustType = dustArray[4];
                float num674 = dustSpeed;
                if (num672 > fourth)
                {
                    num674 = dustSpeed * 0.6875f;
                    dustType = dustArray[5];
                }
                if (num672 > half)
                {
                    num674 = dustSpeed * 0.5f;
                    dustType = dustArray[4];
                }
                if (num672 > threeFourths)
                {
                    num674 = dustSpeed * 0.3125f;
                    dustType = dustArray[5];
                }
                float scale = (dustType == dustArray[4] ? 3f : 1.5f);

                int num675 = Dust.NewDust(Projectile.Center, 6, 6, dustType, 0f, 0f, 100, default, scale);
                float num676 = Main.dust[num675].velocity.X;
                float num677 = Main.dust[num675].velocity.Y;

                if (num676 == 0f && num677 == 0f)
                    num676 = 1f;

                float num678 = (float)Math.Sqrt(num676 * num676 + num677 * num677);
                num678 = num674 / num678;
                if (num672 > threeFourths)
                {
                    num676 = num676 * num678 * 0.7f;
                    num677 *= num678;
                }
                else if (num672 > half)
                {
                    num676 *= num678;
                    num677 = num677 * num678 * 0.7f;
                }
                else if (num672 > fourth)
                {
                    num676 = num676 * num678 * 0.7f;
                    num677 *= num678;
                }
                else
                {
                    num676 *= num678;
                    num677 = num677 * num678 * 0.7f;
                }

                Dust dust8 = Main.dust[num675];
                dust8.velocity *= 0.5f;
                dust8.velocity.X = dust8.velocity.X + num676;
                dust8.velocity.Y = dust8.velocity.Y + num677;
                dust8.noGravity = true;

                if (num672 > threeFourths)
                {
                    int num650 = Dust.NewDust(Projectile.Center, 6, 6, dustArray[5], 0f, 0f, 100, default, 1.3f);
                    float num651 = Main.dust[num650].velocity.X;
                    float num652 = Main.dust[num650].velocity.Y;

                    if (num651 == 0f && num652 == 0f)
                        num651 = 1f;

                    float num653 = (float)Math.Sqrt(num651 * num651 + num652 * num652);
                    num653 = 16f / num653;
                    num651 = num651 * num653 * 1.25f;
                    num652 = num652 * num653 * 0.75f;

                    Dust dust9 = Main.dust[num650];
                    dust9.velocity *= 0.5f;
                    dust9.velocity.X = dust9.velocity.X + num651;
                    dust9.velocity.Y = dust9.velocity.Y + num652;
                    dust9.noGravity = true;
                }
            }
        }

        private void ElectricalBurst(float speed1, float speed2)
        {
            float angleRandom = 0.05f;

            for (int num53 = 0; num53 < 40; num53++)
            {
                float dustSpeed = Main.rand.NextFloat(speed1, speed2);
                Vector2 dustVel = new Vector2(dustSpeed, 0.0f).RotatedBy(Projectile.velocity.ToRotation());
                dustVel = dustVel.RotatedBy(-angleRandom);
                dustVel = dustVel.RotatedByRandom(2f * angleRandom);
                int randomDustType = Main.rand.Next(2) == 0 ? dustArray[4] : dustArray[5];
                float scale = randomDustType == dustArray[4] ? 1.5f : 1f;

                int num54 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, randomDustType, dustVel.X, dustVel.Y, 200, default, 2.5f * scale);
                Main.dust[num54].position = Projectile.Center + Vector2.UnitY.RotatedByRandom(MathHelper.Pi) * (float)Main.rand.NextDouble() * Projectile.width / 2f;
                Main.dust[num54].noGravity = true;

                Dust dust = Main.dust[num54];
                dust.velocity *= 3f;
                dust = Main.dust[num54];

                num54 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, randomDustType, dustVel.X, dustVel.Y, 100, default, 1.5f * scale);
                Main.dust[num54].position = Projectile.Center + Vector2.UnitY.RotatedByRandom(MathHelper.Pi) * (float)Main.rand.NextDouble() * Projectile.width / 2f;

                dust = Main.dust[num54];
                dust.velocity *= 2f;

                Main.dust[num54].noGravity = true;
                Main.dust[num54].fadeIn = 1f;
                Main.dust[num54].color = Color.Cyan * 0.5f;

                dust = Main.dust[num54];
            }
            for (int num55 = 0; num55 < 20; num55++)
            {
                float dustSpeed = Main.rand.NextFloat(speed1, speed2);
                Vector2 dustVel = new Vector2(dustSpeed, 0f).RotatedBy(Projectile.velocity.ToRotation());
                dustVel = dustVel.RotatedBy(-angleRandom);
                dustVel = dustVel.RotatedByRandom(2f * angleRandom);
                int randomDustType = Main.rand.Next(2) == 0 ? dustArray[4] : dustArray[5];
                float scale = randomDustType == dustArray[4] ? 1.5f : 1f;

                int num56 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, randomDustType, dustVel.X, dustVel.Y, 0, default, 3f * scale);
                Main.dust[num56].position = Projectile.Center + Vector2.UnitX.RotatedByRandom(MathHelper.Pi).RotatedBy(Projectile.velocity.ToRotation()) * Projectile.width / 3f;
                Main.dust[num56].noGravity = true;

                Dust dust = Main.dust[num56];
                dust.velocity *= 0.5f;
                dust = Main.dust[num56];
            }
        }
    }
}
