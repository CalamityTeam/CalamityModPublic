using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.DraedonsArsenal
{
    public class PulseRifleShot : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Misc";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        private int dust1 = 27; //purple
        private int dust2 = 173; //shortlived purple
        private int dust3 = 234; //cyan and pink
        private bool hasHit = false;

        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 100;
            Projectile.timeLeft = 500;
        }

        public override void AI()
        {
            bool notSplitProjectile = Projectile.ai[1] == 0f;

            Lighting.AddLight(Projectile.Center, 0.3f, 0f, 0.5f);

            float createDustVar = 10f;
            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] > createDustVar)
            {
                for (int i = 0; i < 2; i++)
                {
                    Vector2 projPos = Projectile.position;
                    projPos -= Projectile.velocity * (i * 0.25f);
                    int purpleDust = Dust.NewDust(projPos, 1, 1, dust1, 0f, 0f, 0, default, 1f);
                    Main.dust[purpleDust].noGravity = true;
                    Main.dust[purpleDust].position = projPos;
                    Main.dust[purpleDust].scale = Main.rand.Next(70, 110) * 0.013f;

                    int pinkDust = Dust.NewDust(projPos, 1, 1, dust3, 0f, 0f, 0, default, 1f);
                    Main.dust[pinkDust].noGravity = true;
                    Main.dust[pinkDust].position = projPos;
                    Main.dust[pinkDust].scale = Main.rand.Next(70, 110) * 0.013f;
                }

                if (notSplitProjectile)
                {
                    Vector2 dustRotateVector = new Vector2(5f, 10f);
                    Vector2 dustRotate = Vector2.UnitX * -12f;

                    for (int k = 0; k < 2; k++)
                    {
                        dustRotate = -Vector2.UnitY.RotatedBy(24f * 0.1308997f + 0f * MathHelper.Pi) * dustRotateVector;
                        int darkPurpDust = Dust.NewDust(Projectile.Center, 0, 0, dust2, 0f, 0f, 160, default, 1f);
                        Main.dust[darkPurpDust].scale = 1.5f;
                        Main.dust[darkPurpDust].noGravity = true;
                        Main.dust[darkPurpDust].position = Projectile.Center + dustRotate;
                    }

                    for (int k = 0; k < 2; k++)
                    {
                        dustRotate = -Vector2.UnitY.RotatedBy(24f * 0.1308997f + 1f * MathHelper.Pi) * dustRotateVector;
                        int darkPurpDust = Dust.NewDust(Projectile.Center, 0, 0, dust2, 0f, 0f, 160, default, 1f);
                        Main.dust[darkPurpDust].scale = 1.5f;
                        Main.dust[darkPurpDust].noGravity = true;
                        Main.dust[darkPurpDust].position = Projectile.Center + dustRotate;
                    }
                }
            }

            if (Projectile.localAI[0] == createDustVar && notSplitProjectile)
                PulseBurst(4f, 5f);
        }

        public override bool? CanHitNPC(NPC target) => Projectile.timeLeft < 480 && target.CanBeChasedBy(Projectile);

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Projectile.ai[1] < 3f && !hasHit && Main.myPlayer == Projectile.owner)
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
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.SafeDirectionTo(Main.npc[i].Center) * 5f, Projectile.type, (int)(Projectile.damage * 0.4f), 0f, Projectile.owner, Main.npc[i].whoAmI, Projectile.ai[1] + 1f);
                        break;
                    }
                }
            }
        }

        public override void OnKill(int timeLeft)
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

            for (int i = 0; i < totalDust; i++)
            {
                float dustSpeed = 2f * (i / (float)fourth);
                int dustType = dust1;
                if (i > fourth)
                {
                    dustSpeed = speed1;
                }
                if (i > x)
                {
                    dustSpeed = speed1 * 1.3f;
                    dustType = dust3;
                }

                int killDust = Dust.NewDust(Projectile.Center, 6, 6, dustType, 0f, 0f, 100, default, 1f);
                float killDustX = Main.dust[killDust].velocity.X;
                float killDustY = Main.dust[killDust].velocity.Y;
                if (killDustX == 0f && killDustY == 0f)
                {
                    killDustX = 1f;
                }

                float killDustYVel = (float)Math.Sqrt(killDustX * killDustX + killDustY * killDustY);
                killDustYVel = dustSpeed / killDustYVel;
                if (i <= half)
                {
                    killDustX *= killDustYVel;
                    killDustY *= killDustYVel;
                }
                else
                {
                    killDustX = killDustX * killDustYVel * 1.25f;
                    killDustY = killDustY * killDustYVel * 0.75f;
                }

                Dust dust2 = Main.dust[killDust];
                dust2.velocity *= 0.5f;
                dust2.velocity.X = dust2.velocity.X + killDustX;
                dust2.velocity.Y = dust2.velocity.Y + killDustY;

                if (i > fourth)
                {
                    dust2.scale = 1.3f;
                }

                dust2.noGravity = true;
            }
        }

        private void PulseBurst(float speed1, float speed2)
        {
            float angleRandom = 0.05f;

            for (int i = 0; i < 50; i++)
            {
                float dustSpeed = Main.rand.NextFloat(speed1, speed2);
                Vector2 dustVel = new Vector2(dustSpeed, 0.0f).RotatedBy(Projectile.velocity.ToRotation());
                dustVel = dustVel.RotatedBy(-angleRandom);
                dustVel = dustVel.RotatedByRandom(2.0f * angleRandom);

                int pulseDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, dust3, dustVel.X, dustVel.Y, 200, default, 1.7f);
                Main.dust[pulseDust].position = Projectile.Center + Vector2.UnitY.RotatedByRandom(MathHelper.Pi) * (float)Main.rand.NextDouble() * Projectile.width / 2f;
                Main.dust[pulseDust].noGravity = true;

                Dust dust = Main.dust[pulseDust];
                dust.velocity *= 3f;
                dust = Main.dust[pulseDust];

                pulseDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, dust1, dustVel.X, dustVel.Y, 100, default, 1f);
                Main.dust[pulseDust].position = Projectile.Center + Vector2.UnitY.RotatedByRandom(MathHelper.Pi) * (float)Main.rand.NextDouble() * Projectile.width / 2f;

                dust = Main.dust[pulseDust];
                dust.velocity *= 2f;

                Main.dust[pulseDust].noGravity = true;
                Main.dust[pulseDust].fadeIn = 1f;
                Main.dust[pulseDust].color = Color.Green * 0.5f;

                dust = Main.dust[pulseDust];
            }
            for (int j = 0; j < 25; j++)
            {
                float dustSpeed = Main.rand.NextFloat(speed1, speed2);
                Vector2 dustVel = new Vector2(dustSpeed, 0.0f).RotatedBy(Projectile.velocity.ToRotation());
                dustVel = dustVel.RotatedBy(-angleRandom);
                dustVel = dustVel.RotatedByRandom(2.0f * angleRandom);

                int pulseDust2 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, dust2, dustVel.X, dustVel.Y, 0, default, 3f);
                Main.dust[pulseDust2].position = Projectile.Center + Vector2.UnitX.RotatedByRandom(3.1415927410125732).RotatedBy(Projectile.velocity.ToRotation()) * Projectile.width / 3f;
                Main.dust[pulseDust2].noGravity = true;

                Dust dust = Main.dust[pulseDust2];
                dust.velocity *= 0.5f;
                dust = Main.dust[pulseDust2];
            }
        }
    }
}
