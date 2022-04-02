using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class DoGTeleportRift : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/StarProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rift");
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 30;
            projectile.scale = 2.5f;
            projectile.hostile = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.Opacity = 0f;
            projectile.penetrate = -1;
            projectile.timeLeft = 200;
        }

        public override void AI()
        {
            if (projectile.ai[0] == -1f)
                return;

            if (projectile.timeLeft < 160)
                projectile.timeLeft = 160;

            if (!Main.npc[(int)projectile.ai[0]].active)
                projectile.Kill();

            projectile.ai[1] += MathHelper.Pi / 30f;
            projectile.Opacity = MathHelper.Clamp(projectile.ai[1], 0f, 1f);

            if (projectile.Opacity == 1f && Main.rand.NextBool(15))
            {
                Dust dust = Main.dust[Dust.NewDust(projectile.Top, 0, 0, 267, 0f, 0f, 100, new Color(150, 100, 255, 255), 1f)];
                dust.velocity.X = 0f;
                dust.noGravity = true;
                dust.fadeIn = 1f;
                dust.position = projectile.Center + Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi) * (4f * Main.rand.NextFloat() + 26f);
                dust.scale = 0.5f;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            float lerpMult = Utils.InverseLerp(15f, 30f, projectile.timeLeft, clamped: true) * Utils.InverseLerp(240f, 200f, projectile.timeLeft, clamped: true) * (1f + 0.2f * (float)Math.Cos(Main.GlobalTime % 30f / 0.5f * (MathHelper.Pi * 2f) * 3f)) * 0.8f;

            Texture2D texture = Main.projectileTexture[projectile.type];
            Vector2 drawPos = projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY);
            Color baseColor = new Color(150, 100, 255, 255) * projectile.Opacity;
            baseColor *= 0.5f;
            baseColor.A = 0;
            Color colorA = baseColor;
            Color colorB = baseColor * 0.5f;
            colorA *= lerpMult;
            colorB *= lerpMult;
            Vector2 origin = texture.Size() / 2f;
            Vector2 scale = new Vector2(3f, 9f) * projectile.Opacity * lerpMult;

            SpriteEffects spriteEffects = SpriteEffects.None;
            if (projectile.spriteDirection == -1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            spriteBatch.Draw(texture, drawPos, null, colorA, MathHelper.PiOver2, origin, scale, spriteEffects, 0);
            spriteBatch.Draw(texture, drawPos, null, colorA, 0f, origin, scale, spriteEffects, 0);
            spriteBatch.Draw(texture, drawPos, null, colorB, MathHelper.PiOver2, origin, scale * 0.8f, spriteEffects, 0);
            spriteBatch.Draw(texture, drawPos, null, colorB, 0f, origin, scale * 0.8f, spriteEffects, 0);

            spriteBatch.Draw(texture, drawPos, null, colorA, MathHelper.PiOver2 + projectile.ai[1] * 0.25f, origin, scale, spriteEffects, 0);
            spriteBatch.Draw(texture, drawPos, null, colorA, projectile.ai[1] * 0.25f, origin, scale, spriteEffects, 0);
            spriteBatch.Draw(texture, drawPos, null, colorB, MathHelper.PiOver2 + projectile.ai[1] * 0.5f, origin, scale * 0.8f, spriteEffects, 0);
            spriteBatch.Draw(texture, drawPos, null, colorB, projectile.ai[1] * 0.5f, origin, scale * 0.8f, spriteEffects, 0);

            spriteBatch.Draw(texture, drawPos, null, colorA, MathHelper.PiOver4, origin, scale * 0.6f, spriteEffects, 0);
            spriteBatch.Draw(texture, drawPos, null, colorA, MathHelper.PiOver4 * 3f, origin, scale * 0.6f, spriteEffects, 0);
            spriteBatch.Draw(texture, drawPos, null, colorB, MathHelper.PiOver4, origin, scale * 0.4f, spriteEffects, 0);
            spriteBatch.Draw(texture, drawPos, null, colorB, MathHelper.PiOver4 * 3f, origin, scale * 0.4f, spriteEffects, 0);

            spriteBatch.Draw(texture, drawPos, null, colorA, MathHelper.PiOver4 + projectile.ai[1] * 0.75f, origin, scale * 0.6f, spriteEffects, 0);
            spriteBatch.Draw(texture, drawPos, null, colorA, MathHelper.PiOver4 * 3f + projectile.ai[1] * 0.75f, origin, scale * 0.6f, spriteEffects, 0);
            spriteBatch.Draw(texture, drawPos, null, colorB, MathHelper.PiOver4 + projectile.ai[1], origin, scale * 0.4f, spriteEffects, 0);
            spriteBatch.Draw(texture, drawPos, null, colorB, MathHelper.PiOver4 * 3f + projectile.ai[1], origin, scale * 0.4f, spriteEffects, 0);

            return false;
        }

        public override void Kill(int timeLeft)
        {
            if (projectile.ai[0] == -1f)
                return;

            int dustAmt = 50;
            int random = 5;

            for (int j = 0; j < 10; j++)
            {
                random += j * 2;
                int dustAmtSpawned = 0;
                int scale = random * 13;
                float dustPositionX = projectile.Center.X - (scale / 2);
                float dustPositionY = projectile.Center.Y - (scale / 2);
                while (dustAmtSpawned < dustAmt)
                {
                    float dustVelocityX = Main.rand.Next(-random, random);
                    float dustVelocityY = Main.rand.Next(-random, random);
                    float dustVelocityScalar = random * 2f;
                    float dustVelocity = (float)Math.Sqrt(dustVelocityX * dustVelocityX + dustVelocityY * dustVelocityY);
                    dustVelocity = dustVelocityScalar / dustVelocity;
                    dustVelocityX *= dustVelocity;
                    dustVelocityY *= dustVelocity;
                    int dust = Dust.NewDust(new Vector2(dustPositionX, dustPositionY), scale, scale, (int)CalamityDusts.PurpleCosmilite, 0f, 0f, 100, default, 5f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].position.X = projectile.Center.X;
                    Main.dust[dust].position.Y = projectile.Center.Y;
                    Main.dust[dust].position.X += Main.rand.Next(-10, 11);
                    Main.dust[dust].position.Y += Main.rand.Next(-10, 11);
                    Main.dust[dust].velocity.X = dustVelocityX;
                    Main.dust[dust].velocity.Y = dustVelocityY;
                    dustAmtSpawned++;
                }
            }
        }
    }
}
