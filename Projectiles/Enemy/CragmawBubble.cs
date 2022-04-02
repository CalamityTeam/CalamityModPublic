using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Enemy
{
    public class CragmawBubble : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Enemy/SulphuricAcidBubble";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Acid Bubble");
            Main.projFrames[projectile.type] = 7;
        }

        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 30;
            projectile.scale = 0.01f;
            projectile.hostile = true;
            projectile.tileCollide = false;
            projectile.alpha = 255;
            projectile.timeLeft = 240;
            projectile.penetrate = 1;
        }

        public override void AI()
        {
            projectile.frameCounter++;
            if (projectile.frameCounter > 6)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame > 6)
            {
                projectile.frame = 0;
            }
            if (projectile.localAI[1] < 1f)
            {
                projectile.localAI[1] += 0.01f;
                projectile.scale += 0.01f;
                projectile.width = (int)(30f * projectile.scale);
                projectile.height = (int)(30f * projectile.scale);
            }
            else
            {
                projectile.damage = 20;
                projectile.width = 30;
                projectile.height = 30;
                projectile.tileCollide = true;
            }
            if (projectile.localAI[0] > 2f)
            {
                projectile.alpha -= 20;
                if (projectile.alpha < 100)
                {
                    projectile.alpha = 100;
                }
            }
            else
            {
                projectile.localAI[0] += 1f;
            }
            if (projectile.ai[1] > 30f)
            {
                if (projectile.velocity.Y > -8f)
                {
                    projectile.velocity.Y -= 0.125f;
                }
            }
            else
            {
                projectile.ai[1] += 1f;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num214 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type];
            int y6 = num214 * projectile.frame;
            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y6, texture2D13.Width, num214)), projectile.GetAlpha(lightColor), projectile.rotation, new Vector2((float)texture2D13.Width / 2f, (float)num214 / 2f), projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item54, projectile.position);
            for (int i = 0; i < 36; i++)
            {
                float angle = MathHelper.TwoPi * i / 36f;
                float y = (float)Math.Sin(angle) * (float)Math.Log(Math.Abs(Math.Cos(angle)));
                if (!float.IsNaN(y))
                {
                    Vector2 velocity = new Vector2((float)Math.Cos(angle), y) * 3f;
                    Dust dust = Dust.NewDustPerfect(projectile.Center, (int)CalamityDusts.SulfurousSeaAcid);
                    dust.velocity = velocity;
                    dust.noGravity = true;
                    dust.scale = 2f;

                    velocity = new Vector2(y, (float)Math.Cos(angle)) * 3f;
                    dust = Dust.NewDustPerfect(projectile.Center, (int)CalamityDusts.SulfurousSeaAcid);
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
                    Projectile.NewProjectileDirect(projectile.Center, angle.ToRotationVector2().RotatedByRandom(0.1f) * 8f, ModContent.ProjectileType<GammaAcid>(),
                        projectile.damage, 3f).tileCollide = true;
                }
            }
        }
    }
}
