using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class SupremeCatastropheSlash : ModProjectile
    {
        public ref float Time => ref projectile.ai[0];

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Resonance Slash");
            Main.projFrames[projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            projectile.Calamity().canBreakPlayerDefense = true;

            // These never naturally use rotations, so this shouldn't be an issue.
            projectile.width = 100;
            projectile.height = 60;
            projectile.hostile = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.extraUpdates = 1;
            projectile.penetrate = -1;
            projectile.timeLeft = 1500;
            projectile.Opacity = 0f;
            cooldownSlot = 1;
        }

        public override void AI()
        {
            // Decide frames.
            projectile.frameCounter++;
            projectile.frame = projectile.frameCounter / 7 % Main.projFrames[projectile.type];

            // Fade in and handle visuals.
            projectile.Opacity = Utils.InverseLerp(0f, 8f, projectile.timeLeft, true) * Utils.InverseLerp(1500f, 1492f, projectile.timeLeft, true);
            projectile.spriteDirection = (projectile.velocity.X > 0f).ToDirectionInt();
            Time++;

            // Emit light.
            Lighting.AddLight(projectile.Center, 0.5f * projectile.Opacity, 0f, 0f);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            SpriteEffects direction = projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Texture2D texture = Main.projectileTexture[projectile.type];
            if (projectile.ai[1] == 0f)
                texture = ModContent.GetTexture("CalamityMod/Projectiles/Boss/SupremeCatastropheSlashAlt");

            Vector2 drawPosition = projectile.Center - Main.screenPosition + Vector2.UnitY * projectile.gfxOffY;
            drawPosition -= projectile.velocity.SafeNormalize(Vector2.UnitX) * 38f;
            Rectangle frame = texture.Frame(1, Main.projFrames[projectile.type], 0, projectile.frame);

            for (int i = 0; i < 3; i++)
            {
                Color afterimageColor = projectile.GetAlpha(lightColor) * (1f - i / 3f) * 0.5f;
                Vector2 afterimageOffset = projectile.velocity * -i * 4f;
                spriteBatch.Draw(texture, drawPosition + afterimageOffset, frame, afterimageColor, projectile.rotation, frame.Size() * 0.5f, projectile.scale, direction, 0f);
            }

            spriteBatch.Draw(texture, drawPosition, frame, projectile.GetAlpha(lightColor), projectile.rotation, frame.Size() * 0.5f, projectile.scale, direction, 0f);
            return false;
        }

        public override bool CanHitPlayer(Player target) => projectile.Opacity >= 1f;

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (projectile.Opacity != 1f)
                return;

            target.AddBuff(ModContent.BuffType<AbyssalFlames>(), 180);
            target.AddBuff(ModContent.BuffType<VulnerabilityHex>(), 120, true);
        }

        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)    
        {
            target.Calamity().lastProjectileHit = projectile;
        }
    }
}
