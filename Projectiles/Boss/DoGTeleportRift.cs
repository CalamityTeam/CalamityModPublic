using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class DoGTeleportRift : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Boss";
        public override string Texture => "CalamityMod/Projectiles/StarProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 30;
            Projectile.scale = 2.5f;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.Opacity = 0f;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 200;
        }

        public override void AI()
        {
            if (Projectile.ai[0] == -1f)
                return;

            if (Projectile.timeLeft < 160)
                Projectile.timeLeft = 160;

            if (!Main.npc[(int)Projectile.ai[0]].active)
                Projectile.Kill();

            Projectile.ai[1] += MathHelper.Pi / 30f;
            Projectile.Opacity = MathHelper.Clamp(Projectile.ai[1], 0f, 1f);

            if (Projectile.Opacity == 1f && Main.rand.NextBool(15))
            {
                Dust dust = Main.dust[Dust.NewDust(Projectile.Top, 0, 0, 267, 0f, 0f, 100, new Color(150, 100, 255, 255), 1f)];
                dust.velocity.X = 0f;
                dust.noGravity = true;
                dust.fadeIn = 1f;
                dust.position = Projectile.Center + Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi) * (4f * Main.rand.NextFloat() + 26f);
                dust.scale = 0.5f;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            float lerpMult = Utils.GetLerpValue(15f, 30f, Projectile.timeLeft, clamped: true) * Utils.GetLerpValue(240f, 200f, Projectile.timeLeft, clamped: true) * (1f + 0.2f * (float)Math.Cos(Main.GlobalTimeWrappedHourly % 30f / 0.5f * (MathHelper.Pi * 2f) * 3f)) * 0.8f;

            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
            Color baseColor = new Color(150, 100, 255, 255) * Projectile.Opacity;
            baseColor *= 0.5f;
            baseColor.A = 0;
            Color colorA = baseColor;
            Color colorB = baseColor * 0.5f;
            colorA *= lerpMult;
            colorB *= lerpMult;
            Vector2 origin = texture.Size() / 2f;
            Vector2 scale = new Vector2(3f, 9f) * Projectile.Opacity * lerpMult;

            SpriteEffects spriteEffects = SpriteEffects.None;
            if (Projectile.spriteDirection == -1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(texture, drawPos, null, colorA, MathHelper.PiOver2, origin, scale, spriteEffects, 0);
            Main.EntitySpriteDraw(texture, drawPos, null, colorA, 0f, origin, scale, spriteEffects, 0);
            Main.EntitySpriteDraw(texture, drawPos, null, colorB, MathHelper.PiOver2, origin, scale * 0.8f, spriteEffects, 0);
            Main.EntitySpriteDraw(texture, drawPos, null, colorB, 0f, origin, scale * 0.8f, spriteEffects, 0);

            Main.EntitySpriteDraw(texture, drawPos, null, colorA, MathHelper.PiOver2 + Projectile.ai[1] * 0.25f, origin, scale, spriteEffects, 0);
            Main.EntitySpriteDraw(texture, drawPos, null, colorA, Projectile.ai[1] * 0.25f, origin, scale, spriteEffects, 0);
            Main.EntitySpriteDraw(texture, drawPos, null, colorB, MathHelper.PiOver2 + Projectile.ai[1] * 0.5f, origin, scale * 0.8f, spriteEffects, 0);
            Main.EntitySpriteDraw(texture, drawPos, null, colorB, Projectile.ai[1] * 0.5f, origin, scale * 0.8f, spriteEffects, 0);

            Main.EntitySpriteDraw(texture, drawPos, null, colorA, MathHelper.PiOver4, origin, scale * 0.6f, spriteEffects, 0);
            Main.EntitySpriteDraw(texture, drawPos, null, colorA, MathHelper.PiOver4 * 3f, origin, scale * 0.6f, spriteEffects, 0);
            Main.EntitySpriteDraw(texture, drawPos, null, colorB, MathHelper.PiOver4, origin, scale * 0.4f, spriteEffects, 0);
            Main.EntitySpriteDraw(texture, drawPos, null, colorB, MathHelper.PiOver4 * 3f, origin, scale * 0.4f, spriteEffects, 0);

            Main.EntitySpriteDraw(texture, drawPos, null, colorA, MathHelper.PiOver4 + Projectile.ai[1] * 0.75f, origin, scale * 0.6f, spriteEffects, 0);
            Main.EntitySpriteDraw(texture, drawPos, null, colorA, MathHelper.PiOver4 * 3f + Projectile.ai[1] * 0.75f, origin, scale * 0.6f, spriteEffects, 0);
            Main.EntitySpriteDraw(texture, drawPos, null, colorB, MathHelper.PiOver4 + Projectile.ai[1], origin, scale * 0.4f, spriteEffects, 0);
            Main.EntitySpriteDraw(texture, drawPos, null, colorB, MathHelper.PiOver4 * 3f + Projectile.ai[1], origin, scale * 0.4f, spriteEffects, 0);

            return false;
        }

        public override void Kill(int timeLeft)
        {
            if (Projectile.ai[0] == -1f || Projectile.ai[2] == 1f)
                return;

            int dustAmt = 50;
            int random = 5;

            for (int j = 0; j < 10; j++)
            {
                random += j * 2;
                int dustAmtSpawned = 0;
                int scale = random * 13;
                float dustPositionX = Projectile.Center.X - (scale / 2);
                float dustPositionY = Projectile.Center.Y - (scale / 2);
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
                    Main.dust[dust].position.X = Projectile.Center.X;
                    Main.dust[dust].position.Y = Projectile.Center.Y;
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
