using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.NPCs.AstrumDeus;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Boss
{
    public class DeusMine : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Boss";
        private const int MaxTimeLeft = 600;
        private const int FadeTime = 85;

        public override void SetDefaults()
        {
            Projectile.Calamity().DealsDefenseDamage = true;
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.hostile = true;
            Projectile.alpha = 100;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = MaxTimeLeft;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.localAI[0]);
            writer.Write(Projectile.localAI[1]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.localAI[0] = reader.ReadSingle();
            Projectile.localAI[1] = reader.ReadSingle();
        }

        public override void AI()
        {
            // Sound on spawn
            if (Projectile.ai[1] == 0f)
            {
                Projectile.ai[1] = 1f;
                SoundEngine.PlaySound(AstrumDeusHead.MineSound, Projectile.Center);
            }

            // Deal no damage if fading out and not set to explode
            if (Projectile.timeLeft < FadeTime)
            {
                if (Projectile.ai[0] == 0f)
                    Projectile.damage = 0;
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(Projectile.Center, 12f, targetHitbox);

        public override bool CanHitPlayer(Player target) => Projectile.timeLeft <= MaxTimeLeft - FadeTime && (Projectile.timeLeft >= FadeTime || Projectile.ai[0] == 1f);

        public override Color? GetAlpha(Color lightColor)
        {
            // Fade in
            if (Projectile.timeLeft > MaxTimeLeft - FadeTime)
            {
                Projectile.localAI[1] += 1f;
                byte b2 = (byte)(((int)Projectile.localAI[1]) * 3);
                byte a2 = (byte)(Projectile.alpha * (b2 / 255f));
                return new Color(b2, b2, b2, a2);
            }

            // Fade out if not set to explode
            // Glow more red over time if set to explode
            if (Projectile.timeLeft < FadeTime)
            {
                byte b2 = (byte)(Projectile.timeLeft * 3);
                if (Projectile.ai[0] == 0f)
                {
                    byte a2 = (byte)(Projectile.alpha * (b2 / 255f));
                    return new Color(b2, b2, b2, a2);
                }
                else
                    return new Color(255, b2, b2, Projectile.alpha);
            }

            // Normal
            return new Color(255, 255, 255, Projectile.alpha);
        }

        public override void OnKill(int timeLeft)
        {
            // Explode and split into accelerating lasers
            if (Projectile.ai[0] == 1f)
            {
                SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
                Projectile.position = Projectile.Center;
                Projectile.width = Projectile.height = 96;
                Projectile.position.X = Projectile.position.X - (Projectile.width / 2);
                Projectile.position.Y = Projectile.position.Y - (Projectile.height / 2);
                for (int i = 0; i < 5; i++)
                {
                    int purpleDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 173, 0f, 0f, 100, default, 1.2f);
                    Main.dust[purpleDust].velocity *= 3f;
                    if (Main.rand.NextBool())
                    {
                        Main.dust[purpleDust].scale = 0.5f;
                        Main.dust[purpleDust].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int j = 0; j < 10; j++)
                {
                    int astralDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, ModContent.DustType<AstralOrange>(), 0f, 0f, 100, default, 1.7f);
                    Main.dust[astralDust].noGravity = true;
                    Main.dust[astralDust].velocity *= 1.5f;
                    Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, ModContent.DustType<AstralOrange>(), 0f, 0f, 100, default, 1f);
                }

                // Spawn diagonal lasers
                if (Main.myPlayer == Projectile.owner)
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
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity2, type, (int)Math.Round(Projectile.damage * 0.75), 0f, Main.myPlayer, 1f, 0f);
                    }
                }
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (info.Damage <= 0 || Projectile.timeLeft > MaxTimeLeft - FadeTime || (Projectile.timeLeft < FadeTime && Projectile.ai[0] == 0f))
                return;

            target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 75);
        }
    }
}
