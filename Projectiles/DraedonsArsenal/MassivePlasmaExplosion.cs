using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.DraedonsArsenal
{
    public class MassivePlasmaExplosion : ModProjectile
    {
        public float Time
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
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
            Projectile.width = Projectile.height = 383;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.Calamity().rogue = true;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = frameLength * horizontalFrames * verticalFrames / 2;
        }

        public override void AI()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter % frameLength == frameLength - 1)
            {
                frameY++;
                if (frameY >= verticalFrames)
                {
                    frameX++;
                    frameY = 0;
                }
                if (frameX >= horizontalFrames)
                {
                    Projectile.Kill();
                }
            }

            Lighting.AddLight(Projectile.Center, Color.White.ToVector3() * 4f * lightAmt);
            if (Projectile.localAI[0] == 0f)
            {
                SoundEngine.PlaySound(Mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/FlareSound"), Projectile.Center);
                Projectile.localAI[0] = 1f;
            }
            lightAmt = (float)Math.Sin(Time / 37 * MathHelper.Pi) * 2f;
            if (lightAmt > 1f)
                lightAmt = 1f;
            Time++;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(Projectile.Center, radius, targetHitbox);

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = Main.projectileTexture[Projectile.type];
            int length = texture.Width / horizontalFrames;
            int height = texture.Height / verticalFrames;
            Vector2 drawPos = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
            Rectangle frame = new Rectangle(frameX * length, frameY * height, length, height);
            Vector2 origin = new Vector2(length / 2f, height / 2f);
            spriteBatch.Draw(texture, drawPos, frame, Color.White, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}
