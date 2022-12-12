using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class NebulaNova : ModProjectile
    {
        private const int TotalXFrames = 2;
        private const int TotalYFrames = 7;
        private const int FrameTimer = 4;

        public int frameX = 0;
        public int frameY = 0;

        public int CurrentFrame
        {
            get => frameX * TotalYFrames + frameY;
            set
            {
                frameX = value / TotalYFrames;
                frameY = value % TotalYFrames;
            }
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Nebula Nova");
        }

        public override void SetDefaults()
        {
            Projectile.width = 190;
            Projectile.height = 168;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = TotalXFrames * TotalYFrames * FrameTimer;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter % FrameTimer == 0)
            {
                CurrentFrame++;
                if (frameX >= TotalXFrames)
                    CurrentFrame = 0;
            }

            Projectile.velocity *= 0.95f;
        }

        public override Color? GetAlpha(Color lightColor) => new Color(255 - Projectile.alpha, 255 - Projectile.alpha, 255 - Projectile.alpha, 255 - Projectile.alpha);

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 position = Projectile.Center - Main.screenPosition;
            Vector2 origin = texture.Size() / new Vector2(TotalXFrames, TotalYFrames) * 0.5f;
            Rectangle frame = texture.Frame(TotalXFrames, TotalYFrames, frameX, frameY);
            SpriteEffects spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Main.EntitySpriteDraw(texture, position, frame, Color.White, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);

            return false;
        }
    }
}
