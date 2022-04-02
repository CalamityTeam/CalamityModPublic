using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class DeusMine : ModProjectile
    {
        private const int MaxTimeLeft = 600;
        private const int FadeTime = 85;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Astral Mine");
        }

        public override void SetDefaults()
        {
            projectile.Calamity().canBreakPlayerDefense = true;
            projectile.width = 30;
            projectile.height = 30;
            projectile.hostile = true;
            projectile.alpha = 100;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.timeLeft = MaxTimeLeft;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(projectile.localAI[0]);
            writer.Write(projectile.localAI[1]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            projectile.localAI[0] = reader.ReadSingle();
            projectile.localAI[1] = reader.ReadSingle();
        }

        public override void AI()
        {
            // Sound on spawn
            if (projectile.ai[1] == 0f)
            {
                projectile.ai[1] = 1f;
                Main.PlaySound(SoundID.Item, (int)projectile.position.X, (int)projectile.position.Y, 33);
            }

            // Deal no damage if fading out and not set to explode
            if (projectile.timeLeft < FadeTime)
            {
                if (projectile.ai[0] == 0f)
                    projectile.damage = 0;
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(projectile.Center, 12f, targetHitbox);

        public override bool CanHitPlayer(Player target) => projectile.timeLeft <= MaxTimeLeft - FadeTime && (projectile.timeLeft >= FadeTime || projectile.ai[0] == 1f);

        public override Color? GetAlpha(Color lightColor)
        {
            // Fade in
            if (projectile.timeLeft > MaxTimeLeft - FadeTime)
            {
                projectile.localAI[1] += 1f;
                byte b2 = (byte)(((int)projectile.localAI[1]) * 3);
                byte a2 = (byte)(projectile.alpha * (b2 / 255f));
                return new Color(b2, b2, b2, a2);
            }

            // Fade out if not set to explode
            // Glow more red over time if set to explode
            if (projectile.timeLeft < FadeTime)
            {
                byte b2 = (byte)(projectile.timeLeft * 3);
                if (projectile.ai[0] == 0f)
                {
                    byte a2 = (byte)(projectile.alpha * (b2 / 255f));
                    return new Color(b2, b2, b2, a2);
                }
                else
                    return new Color(255, b2, b2, projectile.alpha);
            }

            // Normal
            return new Color(255, 255, 255, projectile.alpha);
        }

        public override void Kill(int timeLeft)
        {
            // Explode and split into accelerating lasers
            if (projectile.ai[0] == 1f)
            {
                Main.PlaySound(SoundID.Item14, (int)projectile.position.X, (int)projectile.position.Y);
                projectile.position = projectile.Center;
                projectile.width = projectile.height = 96;
                projectile.position.X = projectile.position.X - (projectile.width / 2);
                projectile.position.Y = projectile.position.Y - (projectile.height / 2);
                for (int num621 = 0; num621 < 5; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 173, 0f, 0f, 100, default, 1.2f);
                    Main.dust[num622].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int num623 = 0; num623 < 10; num623++)
                {
                    int num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, ModContent.DustType<AstralOrange>(), 0f, 0f, 100, default, 1.7f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 1.5f;
                    Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, ModContent.DustType<AstralOrange>(), 0f, 0f, 100, default, 1f);
                }

                // Spawn diagonal lasers
                if (Main.myPlayer == projectile.owner)
                {
                    int totalProjectiles = 4;
                    float radians = MathHelper.TwoPi / totalProjectiles;
                    int type = ModContent.ProjectileType<AstralShot2>();
                    float velocity = 1f;
                    double angleA = radians * 0.5;
                    double angleB = MathHelper.ToRadians(90f) - angleA;
                    float velocityX2 = (float)(velocity * Math.Sin(angleA) / Math.Sin(angleB));
                    Vector2 spinningPoint = new Vector2(-velocityX2, -velocity);
                    for (int k = 0; k < totalProjectiles; k++)
                    {
                        Vector2 velocity2 = spinningPoint.RotatedBy(radians * k);
                        Projectile.NewProjectile(projectile.Center, velocity2, type, (int)Math.Round(projectile.damage * 0.75), 0f, Main.myPlayer, 1f, 0f);
                    }
                }
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (projectile.timeLeft > MaxTimeLeft - FadeTime || (projectile.timeLeft < FadeTime && projectile.ai[0] == 0f))
                return;

            target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 180);
        }
    }
}
