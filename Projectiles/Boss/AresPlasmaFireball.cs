using CalamityMod.Events;
using CalamityMod.NPCs;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using CalamityMod.Sounds;

namespace CalamityMod.Projectiles.Boss
{
    public class AresPlasmaFireball : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Boss";
        private const int timeLeft = 120;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 6;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.Calamity().DealsDefenseDamage = true;
            Projectile.width = 36;
            Projectile.height = 36;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.Opacity = 0f;
            CooldownSlot = ImmunityCooldownID.Bosses;
            Projectile.timeLeft = BossRushEvent.BossRushActive ? 96 : timeLeft;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.localAI[0]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.localAI[0] = reader.ReadSingle();
        }

        public override void AI()
        {
            if (Projectile.ai[0] != -1f)
            {
                Vector2 targetLocation = new Vector2(Projectile.ai[0], Projectile.ai[1]);
                if (Vector2.Distance(targetLocation, Projectile.Center) < 80f)
                    Projectile.tileCollide = true;
            }

            int fadeInTime = 3;
            Projectile.Opacity = MathHelper.Clamp(1f - ((Projectile.timeLeft - ((BossRushEvent.BossRushActive ? 96 : timeLeft) - fadeInTime)) / (float)fadeInTime), 0f, 1f);

            Lighting.AddLight(Projectile.Center, 0f, 0.6f * Projectile.Opacity, 0f);

            Projectile.frameCounter++;
            if (Projectile.frameCounter > 4)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame > 5)
                Projectile.frame = 0;

            Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X) - MathHelper.PiOver2;

            if (Projectile.localAI[0] == 0f)
            {
                Projectile.localAI[0] = 1f;

                float randDustSpeed1 = 1.8f;
                float randDustSpeed2 = 2.8f;
                float angleRandom = 0.35f;

                for (int i = 0; i < 40; i++)
                {
                    float dustSpeed = Main.rand.NextFloat(randDustSpeed1, randDustSpeed2);
                    Vector2 dustVel = new Vector2(dustSpeed, 0.0f).RotatedBy(Projectile.velocity.ToRotation());
                    dustVel = dustVel.RotatedBy(-angleRandom);
                    dustVel = dustVel.RotatedByRandom(2.0f * angleRandom);
                    int randomDustType = Main.rand.NextBool() ? 107 : 110;

                    int plasmaDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, randomDustType, dustVel.X, dustVel.Y, 200, default, 1.7f);
                    Main.dust[plasmaDust].position = Projectile.Center + Vector2.UnitY.RotatedByRandom(MathHelper.Pi) * (float)Main.rand.NextDouble() * Projectile.width / 2f;
                    Main.dust[plasmaDust].noGravity = true;

                    Dust dust = Main.dust[plasmaDust];
                    dust.velocity *= 3f;

                    plasmaDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, randomDustType, dustVel.X, dustVel.Y, 100, default, 0.8f);
                    Main.dust[plasmaDust].position = Projectile.Center + Vector2.UnitY.RotatedByRandom(MathHelper.Pi) * (float)Main.rand.NextDouble() * Projectile.width / 2f;

                    dust = Main.dust[plasmaDust];
                    dust.velocity *= 2f;

                    Main.dust[plasmaDust].noGravity = true;
                    Main.dust[plasmaDust].fadeIn = 1f;
                    Main.dust[plasmaDust].color = Color.Green * 0.5f;
                }
                for (int j = 0; j < 20; j++)
                {
                    float dustSpeed = Main.rand.NextFloat(randDustSpeed1, randDustSpeed2);
                    Vector2 dustVel = new Vector2(dustSpeed, 0.0f).RotatedBy(Projectile.velocity.ToRotation());
                    dustVel = dustVel.RotatedBy(-angleRandom);
                    dustVel = dustVel.RotatedByRandom(2.0f * angleRandom);
                    int randomDustType = Main.rand.NextBool() ? 107 : 110;

                    int plasmaDust2 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, randomDustType, dustVel.X, dustVel.Y, 0, default, 2f);
                    Main.dust[plasmaDust2].position = Projectile.Center + Vector2.UnitX.RotatedByRandom(MathHelper.Pi).RotatedBy(Projectile.velocity.ToRotation()) * Projectile.width / 3f;
                    Main.dust[plasmaDust2].noGravity = true;

                    Dust dust = Main.dust[plasmaDust2];
                    dust.velocity *= 0.5f;
                }
            }
        }

        public override bool CanHitPlayer(Player target) => Projectile.Opacity == 1f;

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (info.Damage <= 0 || Projectile.Opacity != 1f)
                return;

            target.AddBuff(BuffID.OnFire, 360);
            target.AddBuff(BuffID.CursedInferno, 180);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            lightColor.R = (byte)(255 * Projectile.Opacity);
            lightColor.G = (byte)(255 * Projectile.Opacity);
            lightColor.B = (byte)(255 * Projectile.Opacity);
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            int height = 90;
            Projectile.position = Projectile.Center;
            Projectile.width = Projectile.height = height;
            Projectile.Center = Projectile.position;
            Projectile.Damage();

            SoundEngine.PlaySound(CommonCalamitySounds.ExoPlasmaExplosionSound, Projectile.Center);

            if (Main.myPlayer == Projectile.owner && Projectile.ai[1] != -1f)
            {
                bool splitNormal = true;
                if (CalamityGlobalNPC.draedonExoMechPrimePlasmaCannon != -1)
                {
                    if (Main.npc[CalamityGlobalNPC.draedonExoMechPrimePlasmaCannon].active)
                        splitNormal = Main.npc[CalamityGlobalNPC.draedonExoMechPrimePlasmaCannon].ai[3] % 2f == 0f;
                }

                // Plasma bolts
                int totalProjectiles = BossRushEvent.BossRushActive ? 12 : 8;

                // Reduce the total amount of projectiles by half if Ares Plasma Arm is shooting them and in deathray phase and not the last mech
                if (Projectile.ai[0] == -1f)
                    totalProjectiles /= 2;

                float radians = MathHelper.TwoPi / totalProjectiles;
                int type = ModContent.ProjectileType<AresPlasmaBolt>();
                float velocity = 0.5f;
                double angleA = radians * 0.5;
                double angleB = MathHelper.ToRadians(90f) - angleA;
                float velocityX2 = (float)(velocity * Math.Sin(angleA) / Math.Sin(angleB));
                Vector2 spinningPoint = splitNormal ? new Vector2(0f, -velocity) : new Vector2(-velocityX2, -velocity);
                for (int k = 0; k < totalProjectiles; k++)
                {
                    Vector2 velocity2 = spinningPoint.RotatedBy(radians * k);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity2, type, (int)Math.Round(Projectile.damage * 0.8), 0f, Main.myPlayer);
                }
            }

            for (int i = 0; i < 200; i++)
            {
                float dustScale = 16f;
                if (i < 150)
                    dustScale = 12f;
                if (i < 100)
                    dustScale = 8f;
                if (i < 50)
                    dustScale = 4f;

                int deathPlasmaDust = Dust.NewDust(Projectile.Center, 6, 6, Main.rand.NextBool() ? 107 : 110, 0f, 0f, 100, default, 1f);
                float deathPlasmaDustX = Main.dust[deathPlasmaDust].velocity.X;
                float deathPlasmaDustY = Main.dust[deathPlasmaDust].velocity.Y;

                if (deathPlasmaDustX == 0f && deathPlasmaDustY == 0f)
                    deathPlasmaDustX = 1f;

                float deathPlasmaDustVel = (float)Math.Sqrt(deathPlasmaDustX * deathPlasmaDustX + deathPlasmaDustY * deathPlasmaDustY);
                deathPlasmaDustVel = dustScale / deathPlasmaDustVel;
                deathPlasmaDustX *= deathPlasmaDustVel;
                deathPlasmaDustY *= deathPlasmaDustVel;

                float scale = 1f;
                switch ((int)dustScale)
                {
                    case 4:
                        scale = 1.2f;
                        break;
                    case 8:
                        scale = 1.1f;
                        break;
                    case 12:
                        scale = 1f;
                        break;
                    case 16:
                        scale = 0.9f;
                        break;
                    default:
                        break;
                }

                Dust dust = Main.dust[deathPlasmaDust];
                dust.velocity *= 0.5f;
                dust.velocity.X = dust.velocity.X + deathPlasmaDustX;
                dust.velocity.Y = dust.velocity.Y + deathPlasmaDustY;
                dust.scale = scale;
                dust.noGravity = true;
            }
        }
    }
}
