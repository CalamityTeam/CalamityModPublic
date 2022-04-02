using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class FallenStarProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fallen Star");
        }

        public override void SetDefaults()
        {
            projectile.width = 16;
            projectile.height = 16;
            projectile.aiStyle = 0;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.penetrate = -1;
            projectile.alpha = 50;
            projectile.light = 1f;
            projectile.tileCollide = true;
            projectile.ignoreWater = false;
            projectile.usesLocalNPCImmunity = true;
        }

        public override void AI()
        {
            if (projectile.soundDelay == 0)
            {
                projectile.soundDelay = 20 + Main.rand.Next(40);
                Main.PlaySound(SoundID.Item9, projectile.Center);
            }
            if (projectile.localAI[0] == 0f)
            {
                projectile.localAI[0] = 1f;
            }
            projectile.alpha += (int)(25f * projectile.localAI[0]);
            if (projectile.alpha > 200)
            {
                projectile.alpha = 200;
                projectile.localAI[0] = -1f;
            }
            if (projectile.alpha < 50)
            {
                projectile.alpha = 50;
                projectile.localAI[0] = 1f;
            }
            projectile.rotation += (Math.Abs(projectile.velocity.X) + Math.Abs(projectile.velocity.Y)) * 0.01f * projectile.direction;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            float newdamage = projectile.damage * 0.9375f;
            projectile.damage = (int)newdamage;
            if (projectile.damage < 1)
                projectile.damage = 1;
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item10, projectile.Center);
            int dustAmt = 10;
            int goreAmt = 6;
            for (int i = 0; i < dustAmt; i++)
            {
                Dust.NewDust(projectile.position, projectile.width, projectile.height, 58, projectile.velocity.X * 0.1f, projectile.velocity.Y * 0.1f, 150, default, 1.2f);
            }
            for (int i = 0; i < dustAmt; i++)
            {
                Dust.NewDust(projectile.position, projectile.width, projectile.height, 57, projectile.velocity.X * 0.1f, projectile.velocity.Y * 0.1f, 150, default, 1.2f);
            }
            for (int i = 0; i < goreAmt; i++)
            {
                Gore.NewGore(projectile.position, projectile.velocity * 0.05f, Main.rand.Next(16, 18), 1f);
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(200, 200, 200, lightColor.A - projectile.alpha);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = Main.projectileTexture[projectile.type];
            Vector2 offsets = new Vector2(0f, projectile.gfxOffY) - Main.screenPosition;
            Color alpha = projectile.GetAlpha(lightColor);
            Rectangle spriteRec = new Microsoft.Xna.Framework.Rectangle(0, 0, tex.Width, tex.Height);
            Vector2 spriteOrigin = spriteRec.Size() / 2f;
            SpriteEffects spriteEffects = projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            Texture2D aura = ModContent.GetTexture("CalamityMod/Projectiles/Ranged/FallenStarAura");
            Vector2 drawStart = projectile.Center + projectile.velocity;
            Vector2 drawStart2 = projectile.Center - projectile.velocity * 0.5f;
            Vector2 spinPoint = new Vector2(0f, -10f);
            float time = Main.player[projectile.owner].miscCounter % 216000f / 60f;
            Rectangle auraRec = aura.Frame();
            Color blue = Color.Blue * 0.2f;
            Color white = Color.White * 0.5f;
            white.A = 0;
            blue.A = 0;
            Vector2 auraOrigin = new Vector2(auraRec.Width / 2f, 10f);

            //Draw the aura
            spriteBatch.Draw(aura, drawStart + offsets + spinPoint.RotatedBy(MathHelper.TwoPi * time), auraRec, blue, projectile.velocity.ToRotation() + MathHelper.PiOver2, auraOrigin, 1.5f, SpriteEffects.None, 0);
            spriteBatch.Draw(aura, drawStart + offsets + spinPoint.RotatedBy(MathHelper.TwoPi * time + MathHelper.TwoPi / 3f), auraRec, blue, projectile.velocity.ToRotation() + MathHelper.PiOver2, auraOrigin, 1.1f, SpriteEffects.None, 0);
            spriteBatch.Draw(aura, drawStart + offsets + spinPoint.RotatedBy(MathHelper.TwoPi * time + MathHelper.Pi * 4f / 3f), auraRec, blue, projectile.velocity.ToRotation() + MathHelper.PiOver2, auraOrigin, 1.3f, SpriteEffects.None, 0);
            for (float d = 0f; d < 1f; d += 0.5f)
            {
                float scaleMult = time % 0.5f / 0.5f;
                scaleMult = (scaleMult + d) % 1f;
                float colorMult = scaleMult * 2f;
                if (colorMult > 1f)
                {
                    colorMult = 2f - colorMult;
                }
                spriteBatch.Draw(aura, drawStart2 + offsets, auraRec, white * colorMult, projectile.velocity.ToRotation() + MathHelper.PiOver2, auraOrigin, 0.3f + scaleMult * 0.5f, SpriteEffects.None, 0);
            }

            //Draw the actual projectile
            spriteBatch.Draw(tex, projectile.Center + offsets, spriteRec, alpha, projectile.rotation, spriteOrigin, projectile.scale + 0.1f, spriteEffects, 0);
            return false;
        }
    }
}
