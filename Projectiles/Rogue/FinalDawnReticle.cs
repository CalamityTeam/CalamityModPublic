using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class FinalDawnReticle : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Final Dawn");
        }
        public override void SetDefaults()
        {
            projectile.scale = 1.5f;
            projectile.width = 120;
            projectile.height = 120;
            projectile.penetrate = -1;
            projectile.hostile = false;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.Calamity().rogue = true;
            projectile.alpha = 0;

            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = -1;
        }
        public override void AI()
        {
            if (projectile.ai[1] == 0)
                projectile.ai[1] = 1;
            if (projectile.ai[0] == 0)
            {
                int dustCount = 36;
                for (int i = 0; i < dustCount; i++)
                {
                    Vector2 startingPosition = projectile.Center + 10f * Vector2.UnitX;
                    Vector2 offset = Vector2.UnitX * projectile.width * 0.1875f;
                    offset = offset.RotatedBy((i - (dustCount / 2 - 1)) * MathHelper.TwoPi / 20f);
                    int dustIdx = Dust.NewDust(startingPosition + offset, 0, 0, ModContent.DustType<FinalFlame>(), offset.X * 2f, offset.Y * 2f, 100, default, 3.4f);
                    Main.dust[dustIdx].noGravity = true;
                    Main.dust[dustIdx].noLight = true;
                    Main.dust[dustIdx].velocity = Vector2.Normalize(offset) * 5f;
                }
                projectile.ai[0] = 1;
            }
            projectile.alpha += 8;
            projectile.scale *= 0.98f;
            projectile.ai[1] *= 1.01f;
            if(projectile.alpha >= 255)
                projectile.Kill();
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D ring = Main.projectileTexture[projectile.type];
            Texture2D symbol = ModContent.GetTexture("CalamityMod/Projectiles/Rogue/FinalDawnReticleSymbol");
            spriteBatch.Draw(symbol,
                             projectile.Center - Main.screenPosition,
                             null,
                             projectile.GetAlpha(Color.White),
                             projectile.rotation,
                             new Vector2(symbol.Width / 2, symbol.Height / 2),
                             projectile.scale,
                             projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                             0f);
            spriteBatch.Draw(ring,
                             projectile.Center - Main.screenPosition,
                             null,
                             projectile.GetAlpha(Color.White),
                             projectile.rotation,
                             new Vector2(ring.Width / 2, ring.Height / 2),
                             projectile.ai[1],
                             projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                             0f);
            return false;
        }
    }
}
