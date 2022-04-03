using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Enemy
{
    public class CragmawBubble : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Enemy/SulphuricAcidBubble";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Acid Bubble");
            Main.projFrames[Projectile.type] = 7;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.scale = 0.01f;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
            Projectile.timeLeft = 240;
            Projectile.penetrate = 1;
        }

        public override void AI()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 6)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame > 6)
            {
                Projectile.frame = 0;
            }
            if (Projectile.localAI[1] < 1f)
            {
                Projectile.localAI[1] += 0.01f;
                Projectile.scale += 0.01f;
                Projectile.width = (int)(30f * Projectile.scale);
                Projectile.height = (int)(30f * Projectile.scale);
            }
            else
            {
                Projectile.damage = 20;
                Projectile.width = 30;
                Projectile.height = 30;
                Projectile.tileCollide = true;
            }
            if (Projectile.localAI[0] > 2f)
            {
                Projectile.alpha -= 20;
                if (Projectile.alpha < 100)
                {
                    Projectile.alpha = 100;
                }
            }
            else
            {
                Projectile.localAI[0] += 1f;
            }
            if (Projectile.ai[1] > 30f)
            {
                if (Projectile.velocity.Y > -8f)
                {
                    Projectile.velocity.Y -= 0.125f;
                }
            }
            else
            {
                Projectile.ai[1] += 1f;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = ModContent.Request<Texture2D>(Texture).Value;
            int num214 = ModContent.Request<Texture2D>(Texture).Value.Height / Main.projFrames[Projectile.type];
            int y6 = num214 * Projectile.frame;
            Main.spriteBatch.Draw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y6, texture2D13.Width, num214)), Projectile.GetAlpha(lightColor), Projectile.rotation, new Vector2((float)texture2D13.Width / 2f, (float)num214 / 2f), Projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item54, Projectile.position);
            for (int i = 0; i < 36; i++)
            {
                float angle = MathHelper.TwoPi * i / 36f;
                float y = (float)Math.Sin(angle) * (float)Math.Log(Math.Abs(Math.Cos(angle)));
                if (!float.IsNaN(y))
                {
                    Vector2 velocity = new Vector2((float)Math.Cos(angle), y) * 3f;
                    Dust dust = Dust.NewDustPerfect(Projectile.Center, (int)CalamityDusts.SulfurousSeaAcid);
                    dust.velocity = velocity;
                    dust.noGravity = true;
                    dust.scale = 2f;

                    velocity = new Vector2(y, (float)Math.Cos(angle)) * 3f;
                    dust = Dust.NewDustPerfect(Projectile.Center, (int)CalamityDusts.SulfurousSeaAcid);
                    dust.velocity = velocity;
                    dust.noGravity = true;
                    dust.scale = 2f;
                }
            }
            for (int i = 0; i < 4; i++)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    float angle = MathHelper.TwoPi / 4f * i + MathHelper.PiOver2;
                    Projectile.NewProjectileDirect(Projectile.Center, angle.ToRotationVector2().RotatedByRandom(0.1f) * 8f, ModContent.ProjectileType<GammaAcid>(),
                        Projectile.damage, 3f).tileCollide = true;
                }
            }
        }
    }
}
