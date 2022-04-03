using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Boss
{
    public class RedLightningFeather : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lightning Feather");
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
            cooldownSlot = 1;
            Projectile.Calamity().affectedByMaliceModeVelocityMultiplier = true;
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
                int num103 = Player.FindClosest(Projectile.Center, 1, 1);

                if (Projectile.ai[0] <= timeGateValue2)
                {
                    if (Projectile.ai[0] == timeGateValue2)
                    {
                        Vector2 aimVector = Projectile.ai[1] == 0f ? (Main.player[num103].velocity * 20f) : Vector2.Zero;
                        Vector2 v4 = Main.player[num103].Center + aimVector - Projectile.Center;

                        if (float.IsNaN(v4.X) || float.IsNaN(v4.Y))
                            v4 = -Vector2.UnitY;

                        EmitDust();

                        Projectile.velocity = Vector2.Normalize(v4) * 18f;
                    }
                    else if (Projectile.ai[0] > timeGateValue2 - 30f)
                    {
                        if (Projectile.velocity.Length() > 2f)
                            Projectile.velocity *= 0.9f;
                    }
                    else
                    {
                        if (Projectile.velocity.Length() < 12f)
                            Projectile.velocity *= 1.02f;

                        float scaleFactor2 = Projectile.velocity.Length();
                        Vector2 vector11 = Main.player[num103].Center - Projectile.Center;
                        vector11.Normalize();
                        vector11 *= scaleFactor2;

                        Projectile.velocity = (Projectile.velocity * 24f + vector11) / 25f;
                        Projectile.velocity.Normalize();
                        Projectile.velocity *= scaleFactor2;
                    }
                }
                else
                    Projectile.tileCollide = true;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1, ModContent.Request<Texture2D>(Texture).Value, false);
            return false;
        }

        public override void Kill(int timeLeft)
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
            SoundEngine.PlaySound(SoundID.Item109, Projectile.position);
            for (int num193 = 0; num193 < 6; num193++)
            {
                Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 60, 0f, 0f, 100, default, 1.5f);
            }
            for (int num194 = 0; num194 < 10; num194++)
            {
                int num195 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 60, 0f, 0f, 0, default, 2.5f);
                Main.dust[num195].noGravity = true;
                Main.dust[num195].velocity *= 3f;
                num195 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 60, 0f, 0f, 100, default, 1.5f);
                Main.dust[num195].velocity *= 2f;
                Main.dust[num195].noGravity = true;
            }
        }

        public override bool CanHitPlayer(Player target) => Projectile.Opacity == 1f;

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (Projectile.Opacity != 1f)
                return;

            target.AddBuff(BuffID.Electrified, 60);
        }

        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)
        {
            target.Calamity().lastProjectileHit = projectile;
        }
    }
}
