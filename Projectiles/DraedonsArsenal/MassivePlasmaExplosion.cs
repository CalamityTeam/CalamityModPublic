using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.DraedonsArsenal
{
    public class MassivePlasmaExplosion : ModProjectile
    {
        public float Time
        {
            get => projectile.ai[0];
            set => projectile.ai[0] = value;
        }
        private float lightAmt = 1f;

        public int frameX = 0;
        public int frameY = 0;
        private const int horizontalFrames = 4;
        private const int verticalFrames = 5;
        private const int frameLength = 5;
        private const float radius = 191.5f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Plasma Explosion");
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 383;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.Calamity().rogue = true;
            projectile.usesIDStaticNPCImmunity = true;
            projectile.idStaticNPCHitCooldown = frameLength * horizontalFrames * verticalFrames / 2;
        }

        public override void AI()
        {
            projectile.frameCounter++;
            if (projectile.frameCounter % frameLength == frameLength - 1)
            {
                frameY++;
                if (frameY >= verticalFrames)
                {
                    frameX++;
                    frameY = 0;
                }
                if (frameX >= horizontalFrames)
                {
                    projectile.Kill();
                }
            }

            Lighting.AddLight(projectile.Center, Color.White.ToVector3() * 4f * lightAmt);
            if (projectile.localAI[0] == 0f)
            {
                Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/FlareSound"), projectile.Center);
                projectile.localAI[0] = 1f;
            }
            lightAmt = (float)Math.Sin(Time / 37 * MathHelper.Pi) * 2f;
            if (lightAmt > 1f)
                lightAmt = 1f;
            Time++;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(projectile.Center, radius, targetHitbox);

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = Main.projectileTexture[projectile.type];
            int length = texture.Width / horizontalFrames;
            int height = texture.Height / verticalFrames;
            Vector2 drawPos = projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY);
            Rectangle frame = new Rectangle(frameX * length, frameY * height, length, height);
            Vector2 origin = new Vector2(length / 2f, height / 2f);
            spriteBatch.Draw(texture, drawPos, frame, Color.White, projectile.rotation, origin, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}
