using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class SupremeCataclysmFist : ModProjectile
    {
        public ref float Time => ref projectile.ai[0];

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fist of Fury");
            Main.projFrames[projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 44;
            projectile.hostile = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.timeLeft = 1200;
            projectile.Opacity = 0f;
            cooldownSlot = 1;
        }

        public override void AI()
        {
            // Fly in a sinusoidal wave.
            projectile.velocity.Y = (float)Math.Sin(Time / 5D) * 5f;

            // Decide frames.
            projectile.frameCounter++;
            projectile.frame = projectile.frameCounter / 5 % Main.projFrames[projectile.type];

            // Fade in and handle visuals.
            projectile.Opacity = Utils.InverseLerp(0f, 12f, projectile.timeLeft, true) * Utils.InverseLerp(1200f, 1188f, projectile.timeLeft, true);
            projectile.spriteDirection = (projectile.velocity.X > 0f).ToDirectionInt();
            Time++;

            // Emit light.
            Lighting.AddLight(projectile.Center, 0.5f * projectile.Opacity, 0f, 0f);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            lightColor.R = (byte)(255 * projectile.Opacity);

            SpriteEffects direction = projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Texture2D texture = Main.projectileTexture[projectile.type];
            if (projectile.ai[1] == 1f)
                texture = ModContent.GetTexture("CalamityMod/Projectiles/Boss/SupremeCataclysmFistAlt");

            Vector2 drawPosition = projectile.Center - Main.screenPosition + Vector2.UnitY * projectile.gfxOffY;
            drawPosition.X -= Math.Sign(projectile.velocity.X) * 40f;
            Rectangle frame = texture.Frame(1, Main.projFrames[projectile.type], 0, projectile.frame);
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
