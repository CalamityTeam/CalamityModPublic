using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Boss
{
    public class IceBlast : ModProjectile
    {
        private const int TimeLeft = 600;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ice Blast");
        }

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.scale = 1.2f;
            Projectile.penetrate = -1;
            Projectile.hostile = true;
            Projectile.coldDamage = true;
            Projectile.timeLeft = TimeLeft;
        }

        public override void AI()
        {
            if (Projectile.ai[1] == 1f)
            {
                float spreadOutCutoffTime = TimeLeft - 90;
                float homeInCutoffTime = TimeLeft - 165;
                float minAcceleration = 0.05f;
                float maxAcceleration = 0.1f;
                float homingVelocity = 20f;

                if (Projectile.timeLeft > homeInCutoffTime && Projectile.timeLeft <= spreadOutCutoffTime)
                {
                    int playerIndex = (int)Projectile.ai[0];
                    Vector2 velocity = Projectile.velocity;
                    if (Main.player.IndexInRange(playerIndex))
                    {
                        Player player = Main.player[playerIndex];
                        velocity = Projectile.DirectionTo(player.Center) * homingVelocity;
                    }

                    float amount = MathHelper.Lerp(minAcceleration, maxAcceleration, Utils.GetLerpValue(spreadOutCutoffTime, 30f, Projectile.timeLeft, clamped: true));
                    Projectile.velocity = Vector2.SmoothStep(Projectile.velocity, velocity, amount);
                }
            }

            Lighting.AddLight((int)((Projectile.position.X + (Projectile.width / 2)) / 16f), (int)((Projectile.position.Y + (Projectile.height / 2)) / 16f), 0f, 0.25f, 0.25f);

            Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X) + MathHelper.PiOver2;

            for (int num322 = 0; num322 < 2; num322++)
            {
                int num323 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 92, Projectile.velocity.X, Projectile.velocity.Y, 50, default, 0.6f);
                Main.dust[num323].noGravity = true;
                Dust dust = Main.dust[num323];
                dust.velocity *= 0.3f;
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, Projectile.alpha);
        }

        public override void Kill(int timeLeft)
        {
            int num497 = 5;
            SoundEngine.PlaySound(SoundID.Item27, Projectile.position);
            for (int num498 = 0; num498 < num497; num498++)
            {
                int num499 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 92, 0f, 0f, 0, default, 1f);
                if (Main.rand.Next(3) != 0)
                {
                    Dust dust = Main.dust[num499];
                    dust.velocity *= 2f;
                    Main.dust[num499].noGravity = true;
                    dust = Main.dust[num499];
                    dust.scale *= 1.75f;
                }
                else
                {
                    Dust dust = Main.dust[num499];
                    dust.scale *= 0.5f;
                }
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Frostburn, 120, true);
            target.AddBuff(BuffID.Chilled, 60, true);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, tex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}
