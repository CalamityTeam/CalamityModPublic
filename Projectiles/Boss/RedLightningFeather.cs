using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Boss
{
    public class RedLightningFeather : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Boss";
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 1200;
        }

        public override void AI()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 4)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame > 3)
            {
                Projectile.frame = 0;
            }

            if (Projectile.velocity.X < 0f)
            {
                Projectile.spriteDirection = -1;
                Projectile.rotation = (float)Math.Atan2(-Projectile.velocity.Y, -Projectile.velocity.X);
            }
            else
            {
                Projectile.spriteDirection = 1;
                Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X);
            }

            Projectile.Opacity = MathHelper.Clamp(1f - ((Projectile.timeLeft - 1170) / 30f), 0f, 1f);

            Lighting.AddLight(Projectile.Center, 0.7f * Projectile.Opacity, 0f, 0f);

            Projectile.ai[0] += 1f;
            float timeGateValue = 150f;
            float timeGateValue2 = 300f;
            if (Projectile.ai[0] > timeGateValue)
            {
                if (Projectile.ai[0] <= timeGateValue2)
                {
                    if (Projectile.velocity.Length() < 10f)
                        Projectile.velocity *= 1.015f;

                    int playerTracker = Player.FindClosest(Projectile.Center, 1, 1);
                    float projVelocity = Projectile.velocity.Length();
                    Vector2 playerDirection = Main.player[playerTracker].Center - Projectile.Center;
                    playerDirection.Normalize();
                    playerDirection *= projVelocity;

                    Projectile.velocity = (Projectile.velocity * 19f + playerDirection) / 20f;
                    Projectile.velocity.Normalize();
                    Projectile.velocity *= projVelocity;
                }
                else
                {
                    Projectile.tileCollide = true;

                    if (Projectile.velocity.Length() < 20f)
                        Projectile.velocity *= 1.015f;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1, ModContent.Request<Texture2D>(Texture).Value, false);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            Projectile.position = Projectile.Center;
            Projectile.width = Projectile.height = 64;
            Projectile.position.X = Projectile.position.X - (Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y - (Projectile.height / 2);
            EmitDust();
            Projectile.Damage();
        }

        private void EmitDust()
        {
            SoundEngine.PlaySound(SoundID.Item109, Projectile.Center);
            for (int j = 0; j < 6; j++)
            {
                Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 60, 0f, 0f, 100, default, 1.5f);
            }
            for (int k = 0; k < 10; k++)
            {
                int redDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 60, 0f, 0f, 0, default, 2.5f);
                Main.dust[redDust].noGravity = true;
                Main.dust[redDust].velocity *= 3f;
                redDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 60, 0f, 0f, 100, default, 1.5f);
                Main.dust[redDust].velocity *= 2f;
                Main.dust[redDust].noGravity = true;
            }
        }

        public override bool CanHitPlayer(Player target) => Projectile.Opacity == 1f;

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (info.Damage <= 0 || Projectile.Opacity != 1f)
                return;

            target.AddBuff(BuffID.Electrified, 60);
        }
    }
}
