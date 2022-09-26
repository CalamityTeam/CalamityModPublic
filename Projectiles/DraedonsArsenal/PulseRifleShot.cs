using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.DraedonsArsenal
{
    public class PulseRifleShot : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        private int dust1 = 27; //purple
        private int dust2 = 173; //shortlived purple
        private int dust3 = 234; //cyan and pink
        private bool hasHit = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Pulse Bolt");
        }

        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 100;
            Projectile.timeLeft = 300;
        }

        public override void AI()
        {
            bool notSplitProjectile = Projectile.ai[1] == 0f;

            Lighting.AddLight(Projectile.Center, 0.3f, 0f, 0.5f);

            float createDustVar = 10f;
            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] > createDustVar)
            {
                for (int num447 = 0; num447 < 2; num447++)
                {
                    Vector2 vector33 = Projectile.position;
                    vector33 -= Projectile.velocity * (num447 * 0.25f);
                    int num448 = Dust.NewDust(vector33, 1, 1, dust1, 0f, 0f, 0, default, 1f);
                    Main.dust[num448].noGravity = true;
                    Main.dust[num448].position = vector33;
                    Main.dust[num448].scale = Main.rand.Next(70, 110) * 0.013f;

                    int num449 = Dust.NewDust(vector33, 1, 1, dust3, 0f, 0f, 0, default, 1f);
                    Main.dust[num449].noGravity = true;
                    Main.dust[num449].position = vector33;
                    Main.dust[num449].scale = Main.rand.Next(70, 110) * 0.013f;
                }

                if (notSplitProjectile)
                {
                    Vector2 value7 = new Vector2(5f, 10f);
                    Vector2 value8 = Vector2.UnitX * -12f;

                    for (int num41 = 0; num41 < 2; num41++)
                    {
                        value8 = -Vector2.UnitY.RotatedBy(24f * 0.1308997f + 0f * MathHelper.Pi) * value7;
                        int num42 = Dust.NewDust(Projectile.Center, 0, 0, dust2, 0f, 0f, 160, default, 1f);
                        Main.dust[num42].scale = 1.5f;
                        Main.dust[num42].noGravity = true;
                        Main.dust[num42].position = Projectile.Center + value8;
                    }

                    for (int num41 = 0; num41 < 2; num41++)
                    {
                        value8 = -Vector2.UnitY.RotatedBy(24f * 0.1308997f + 1f * MathHelper.Pi) * value7;
                        int num42 = Dust.NewDust(Projectile.Center, 0, 0, dust2, 0f, 0f, 160, default, 1f);
                        Main.dust[num42].scale = 1.5f;
                        Main.dust[num42].noGravity = true;
                        Main.dust[num42].position = Projectile.Center + value8;
                    }
                }
            }

            if (Projectile.localAI[0] == createDustVar && notSplitProjectile)
                PulseBurst(4f, 5f);
        }

        public override bool? CanHitNPC(NPC target) => Projectile.timeLeft < 280 && target.CanBeChasedBy(Projectile);

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (Projectile.ai[1] < 5f && !hasHit && Main.myPlayer == Projectile.owner)
            {
                hasHit = true;

                int alreadyTargetedNPCType = 0;
                if (Projectile.ai[1] > 0f)
                    alreadyTargetedNPCType = (int)Projectile.ai[0];
                else
                    alreadyTargetedNPCType = target.whoAmI;

                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (!Main.npc[i].CanBeChasedBy(Projectile, false) || !Collision.CanHit(Projectile.Center, 1, 1, Main.npc[i].Center, 1, 1))
                        continue;

                    if (alreadyTargetedNPCType != Main.npc[i].whoAmI && Projectile.Center.ManhattanDistance(Main.npc[i].Center) < 600f)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.SafeDirectionTo(Main.npc[i].Center) * 5f, Projectile.type, Projectile.damage / 2, 0f, Projectile.owner, Main.npc[i].whoAmI, Projectile.ai[1] + 1f);
                        break;
                    }
                }
            }
        }

        public override void Kill(int timeLeft)
        {
            int timesSplit = (int)Projectile.ai[1];

            int height = 40;
            int totalDust = 400;
            float speed1 = 10f;
            switch (timesSplit)
            {
                case 1:
                    height = 35;
                    totalDust = 350;
                    speed1 = 8.5f;
                    break;
                case 2:
                    height = 30;
                    totalDust = 300;
                    speed1 = 7f;
                    break;
                case 3:
                    height = 25;
                    totalDust = 250;
                    speed1 = 5.5f;
                    break;
                case 4:
                    height = 20;
                    totalDust = 200;
                    speed1 = 4f;
                    break;
                case 5:
                    height = 15;
                    totalDust = 150;
                    speed1 = 2.5f;
                    break;
                default:
                    break;
            }

            Projectile.position = Projectile.Center;
            Projectile.width = Projectile.height = height;
            Projectile.Center = Projectile.position;
            Projectile.maxPenetrate = -1;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.Damage();

            SoundEngine.PlaySound(SoundID.Item93, Projectile.Center);

            int fourth = totalDust / 4;
            int half = totalDust / 2;
            int x = (int)(totalDust * 0.625f);

            for (int num647 = 0; num647 < totalDust; num647++)
            {
                float num648 = 2f * (num647 / (float)fourth);
                int dustType = dust1;
                if (num647 > fourth)
                {
                    num648 = speed1;
                }
                if (num647 > x)
                {
                    num648 = speed1 * 1.3f;
                    dustType = dust3;
                }

                int num650 = Dust.NewDust(Projectile.Center, 6, 6, dustType, 0f, 0f, 100, default, 1f);
                float num651 = Main.dust[num650].velocity.X;
                float num652 = Main.dust[num650].velocity.Y;
                if (num651 == 0f && num652 == 0f)
                {
                    num651 = 1f;
                }

                float num653 = (float)Math.Sqrt(num651 * num651 + num652 * num652);
                num653 = num648 / num653;
                if (num647 <= half)
                {
                    num651 *= num653;
                    num652 *= num653;
                }
                else
                {
                    num651 = num651 * num653 * 1.25f;
                    num652 = num652 * num653 * 0.75f;
                }

                Dust dust2 = Main.dust[num650];
                dust2.velocity *= 0.5f;
                dust2.velocity.X = dust2.velocity.X + num651;
                dust2.velocity.Y = dust2.velocity.Y + num652;

                if (num647 > fourth)
                {
                    dust2.scale = 1.3f;
                }

                dust2.noGravity = true;
            }
        }

        private void PulseBurst(float speed1, float speed2)
        {
            float angleRandom = 0.05f;

            for (int num53 = 0; num53 < 50; num53++)
            {
                float dustSpeed = Main.rand.NextFloat(speed1, speed2);
                Vector2 dustVel = new Vector2(dustSpeed, 0.0f).RotatedBy(Projectile.velocity.ToRotation());
                dustVel = dustVel.RotatedBy(-angleRandom);
                dustVel = dustVel.RotatedByRandom(2.0f * angleRandom);

                int num54 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, dust3, dustVel.X, dustVel.Y, 200, default, 1.7f);
                Main.dust[num54].position = Projectile.Center + Vector2.UnitY.RotatedByRandom(MathHelper.Pi) * (float)Main.rand.NextDouble() * Projectile.width / 2f;
                Main.dust[num54].noGravity = true;

                Dust dust = Main.dust[num54];
                dust.velocity *= 3f;
                dust = Main.dust[num54];

                num54 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, dust1, dustVel.X, dustVel.Y, 100, default, 1f);
                Main.dust[num54].position = Projectile.Center + Vector2.UnitY.RotatedByRandom(MathHelper.Pi) * (float)Main.rand.NextDouble() * Projectile.width / 2f;

                dust = Main.dust[num54];
                dust.velocity *= 2f;

                Main.dust[num54].noGravity = true;
                Main.dust[num54].fadeIn = 1f;
                Main.dust[num54].color = Color.Green * 0.5f;

                dust = Main.dust[num54];
            }
            for (int num55 = 0; num55 < 25; num55++)
            {
                float dustSpeed = Main.rand.NextFloat(speed1, speed2);
                Vector2 dustVel = new Vector2(dustSpeed, 0.0f).RotatedBy(Projectile.velocity.ToRotation());
                dustVel = dustVel.RotatedBy(-angleRandom);
                dustVel = dustVel.RotatedByRandom(2.0f * angleRandom);

                int num56 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, dust2, dustVel.X, dustVel.Y, 0, default, 3f);
                Main.dust[num56].position = Projectile.Center + Vector2.UnitX.RotatedByRandom(3.1415927410125732).RotatedBy(Projectile.velocity.ToRotation()) * Projectile.width / 3f;
                Main.dust[num56].noGravity = true;

                Dust dust = Main.dust[num56];
                dust.velocity *= 0.5f;
                dust = Main.dust[num56];
            }
        }
    }
}
