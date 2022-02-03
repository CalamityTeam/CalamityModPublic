using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.VanillaProjectileOverrides
{
    public static class ChlorophyteCrystalAI
    {
        public const int AmountToHealPerPulse = 10;
        public const int PulseReleaseRate = 180;
        public const int DelayBetweenHeals = 150;
        public static bool DoChlorophyteCrystalAI(Projectile projectile)
        {
            Player owner = Main.player[projectile.owner];
            ref float timer = ref projectile.ai[1];

            timer++;
            projectile.aiStyle = -1;
            if (!owner.crystalLeaf)
            {
                projectile.Kill();
                return false;
            }

            // Stay above the player.
            projectile.Center = owner.Center + Vector2.UnitY * (owner.gfxOffY - 60f);
            if (Main.player[projectile.owner].gravDir == -1f)
            {
                projectile.position.Y += 120f;
                projectile.rotation = MathHelper.Pi;
            }
            else
                projectile.rotation = 0f;
            projectile.Center = projectile.Center.Floor();

            // Have a periodically pulsing scale.
            projectile.scale = MathHelper.Lerp(0.9f, 1.15f, (float)Math.Sin(timer / 27f) * 0.5f + 0.5f);

            // Emit life pulses periodically.
            if (timer % PulseReleaseRate == PulseReleaseRate - 1f)
            {
                Main.PlaySound(SoundID.Item45, projectile.Center);
                if (Main.myPlayer == projectile.owner)
                {
                    int pulseDamage = (int)(owner.AverageDamage() * 300f);
                    Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<ChlorophyteLifePulse>(), pulseDamage, 0f, projectile.owner);
                }
            }

            return false;
        }

        public static bool DoChlorophyteCrystalDrawing(SpriteBatch spriteBatch, Projectile projectile)
        {
            Texture2D texture = Main.projectileTexture[projectile.type];
            Vector2 drawPosition = projectile.Center - Main.screenPosition;
            Vector2 origin = texture.Size() * 0.5f;

            float backglowColorInterpolant = MathHelper.Lerp(0.44f, 0.88f, (float)Math.Sin(Main.GlobalTime * 1.1f) * 0.5f + 0.5f);
            Color backglowColor = Color.Lerp(Color.Cyan, Color.Lime, backglowColorInterpolant) * 0.37f;
            backglowColor.A = 0;
            for (int i = 0; i < 8; i++)
            {
                Vector2 drawOffset = (MathHelper.TwoPi * i / 8f).ToRotationVector2() * 3f;
                spriteBatch.Draw(texture, drawPosition + drawOffset, null, projectile.GetAlpha(backglowColor), projectile.rotation, origin, projectile.scale, 0, 0f);
            }

            spriteBatch.Draw(texture, drawPosition, null, projectile.GetAlpha(Color.White * 0.75f), projectile.rotation, origin, projectile.scale, 0, 0f);
            return false;
        }
    }
}
