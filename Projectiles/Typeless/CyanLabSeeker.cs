using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless
{
    public class CyanLabSeeker : ModProjectile
    {
        public ref float Time => ref projectile.ai[0];
        public override string Texture => "CalamityMod/Items/LabFinders/CyanSeekingMechanism";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Seeking Mechanism");
            ProjectileID.Sets.TrailingMode[projectile.type] = 1;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 20;
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 24;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.timeLeft = 300;
        }

        public override void AI() => RedLabSeeker.Behavior(projectile, CalamityWorld.SunkenSeaLabCenter, Color.Cyan, ref Time);

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 8; i++)
            {
                Dust dust = Dust.NewDustPerfect(projectile.Center + Main.rand.NextVector2Circular(16f, 16f), 267);
                dust.color = Color.Cyan;
                dust.scale = Main.rand.NextFloat(0.95f, 1.25f);
                dust.velocity = Main.rand.NextVector2Circular(2.5f, 2.5f);
                dust.velocity.Y -= 1.5f;
                dust.fadeIn = 1.2f;
                dust.noGravity = true;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = Main.projectileTexture[projectile.type];
            if (Time > 80f)
            {
                for (int i = 0; i < 24; i += 2)
                {
                    Vector2 drawPosition = projectile.Center - projectile.velocity.SafeNormalize(Vector2.Zero) * i * Utils.InverseLerp(80f, 125f, Time, true) * 25f - Main.screenPosition;
                    spriteBatch.Draw(texture, drawPosition, null, projectile.GetAlpha(lightColor) * (1f - i / 24f), projectile.rotation, texture.Size() * 0.5f, projectile.scale, SpriteEffects.None, 0f);
                }
            }
            return true;
        }
    }
}
