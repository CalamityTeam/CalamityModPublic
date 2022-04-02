using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class RedirectingLostSoul : ModProjectile
    {
        public ref float BurstIntensity => ref projectile.ai[0];
        public ref float Time => ref projectile.ai[1];
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Soul");
            Main.projFrames[projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 32;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.tileCollide = false;
            projectile.magic = true;
            projectile.ignoreWater = true;
        }

        public override void AI() => RedirectingVengefulSoul.DoSoulAI(projectile, ref Time, 1f);

        public override Color? GetAlpha(Color lightColor)
        {
            Color baseColor = Color.Lerp(Color.DarkRed, Color.DarkViolet, projectile.identity % 5f / 5f * 0.5f);
            Color color = Color.Lerp(baseColor, Color.White, BurstIntensity * 0.5f + (float)Math.Cos(Main.GlobalTime * 2.7f) * 0.04f);
            color.A = 0;
            return color * projectile.Opacity;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            SpriteEffects direction = projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Texture2D texture = Main.projectileTexture[projectile.type];
            Rectangle frame = texture.Frame(1, Main.projFrames[projectile.type], 0, projectile.frame);

            for (int j = 0; j < projectile.oldPos.Length / 2; j++)
            {
                float fade = (float)Math.Pow(1f - Utils.InverseLerp(0f, projectile.oldPos.Length / 2, j, true), 2D);
                Color drawColor = projectile.GetAlpha(lightColor) * fade;
                Vector2 drawPosition = projectile.oldPos[j] + projectile.Size * 0.5f - Main.screenPosition;
                float rotation = projectile.oldRot[j];

                spriteBatch.Draw(texture, drawPosition, frame, drawColor, rotation, frame.Size() * 0.5f, projectile.scale, direction, 0f);
            }

            return false;
        }

        public override void Kill(int timeLeft)
        {
            // Play a wraith death sound at max intensity and a dungeon spirit hit sound otherwise.
            Main.PlaySound(BurstIntensity >= 1f ? SoundID.NPCDeath52 : SoundID.NPCHit35, projectile.Center);

            // Make a ghost sound and explode into ectoplasmic energy.
            if (Main.dedServ)
                return;

            for (int i = 0; i < 45; i++)
            {
                Dust ectoplasm = Dust.NewDustPerfect(projectile.Center + Main.rand.NextVector2Circular(50f, 50f) * BurstIntensity, 264);
                ectoplasm.velocity = Main.rand.NextVector2Circular(2f, 2f);
                ectoplasm.color = projectile.GetAlpha(Color.White);
                ectoplasm.scale = MathHelper.Lerp(1f, 1.6f, BurstIntensity);
                ectoplasm.noGravity = true;
                ectoplasm.noLight = true;
            }
        }
    }
}
