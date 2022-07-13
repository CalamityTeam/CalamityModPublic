using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class SupremeCatastropheSlash : ModProjectile
    {
        public ref float Time => ref Projectile.ai[0];

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Resonance Slash");
            Main.projFrames[Projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.Calamity().DealsDefenseDamage = true;

            // These never naturally use rotations, so this shouldn't be an issue.
            Projectile.width = 100;
            Projectile.height = 60;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 1;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 1500;
            Projectile.Opacity = 0f;
            CooldownSlot = 1;
        }

        public override void AI()
        {
            // Decide frames.
            Projectile.frameCounter++;
            Projectile.frame = Projectile.frameCounter / 7 % Main.projFrames[Projectile.type];

            // Fade in and handle visuals.
            Projectile.Opacity = Utils.GetLerpValue(0f, 8f, Projectile.timeLeft, true) * Utils.GetLerpValue(1500f, 1492f, Projectile.timeLeft, true);
            Projectile.spriteDirection = (Projectile.velocity.X > 0f).ToDirectionInt();
            Time++;

            // Emit light.
            Lighting.AddLight(Projectile.Center, 0.5f * Projectile.Opacity, 0f, 0f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects direction = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            if (Projectile.ai[1] == 0f)
                texture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Boss/SupremeCatastropheSlashAlt").Value;

            Vector2 drawPosition = Projectile.Center - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY;
            drawPosition -= Projectile.velocity.SafeNormalize(Vector2.UnitX) * 38f;
            Rectangle frame = texture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame);

            for (int i = 0; i < 3; i++)
            {
                Color afterimageColor = Projectile.GetAlpha(lightColor) * (1f - i / 3f) * 0.5f;
                Vector2 afterimageOffset = Projectile.velocity * -i * 4f;
                Main.EntitySpriteDraw(texture, drawPosition + afterimageOffset, frame, afterimageColor, Projectile.rotation, frame.Size() * 0.5f, Projectile.scale, direction, 0);
            }

            Main.EntitySpriteDraw(texture, drawPosition, frame, Projectile.GetAlpha(lightColor), Projectile.rotation, frame.Size() * 0.5f, Projectile.scale, direction, 0);
            return false;
        }

        public override bool CanHitPlayer(Player target) => Projectile.Opacity >= 1f;

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (Projectile.Opacity != 1f)
                return;

            target.AddBuff(ModContent.BuffType<VulnerabilityHex>(), 120, true);
        }

        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)
        {
            target.Calamity().lastProjectileHit = Projectile;
        }
    }
}
