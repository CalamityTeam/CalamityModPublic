using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CalamityMod.Projectiles.Typeless
{
    public class BossRushTierAnimation : ModProjectile
    {
        public int Tier => (int)Projectile.ai[0];

        public Player Owner => Main.player[Projectile.owner];
        
        public const int FrameChangeRate = 4;

        public const int TotalFrames = 56;

        public override string Texture => "CalamityMod/Projectiles/Typeless/BossRushTier2Animation";
        
        public override void SetDefaults()
        {
            Projectile.width = 76;
            Projectile.height = 180;
            Projectile.aiStyle = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = FrameChangeRate * TotalFrames;
            Projectile.penetrate = -1;
        }

        public override void AI()
        {
            Projectile.Bottom = Owner.Top - Vector2.UnitY * Projectile.scale * 36f;
            Projectile.frameCounter++;
            Projectile.frame = Projectile.frameCounter / FrameChangeRate;
            if (Projectile.frame >= TotalFrames)
                Projectile.frame = TotalFrames;
        }

        public override Color? GetAlpha(Color lightColor) => Color.White * Projectile.Opacity;

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>($"CalamityMod/Projectiles/Typeless/BossRushTier{Tier}Animation").Value;
            Rectangle frame = texture.Frame(14, 4, Projectile.frame % 14, Projectile.frame / 14);
            Vector2 origin = frame.Size() * 0.5f;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, frame, Projectile.GetAlpha(lightColor), 0f, origin, Projectile.scale, 0, 0f);
            return false;
        }
    }
}
