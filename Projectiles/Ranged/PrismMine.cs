using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class PrismMine : ModProjectile
    {
        public List<Vector2> MinesToConnectTo = new List<Vector2>();
        public ref float Time => ref projectile.ai[0];
        public const float DamageFactorLowerBound = 0.425f;
        public const float MineConnectDistanceMax = 1200f;
        public override void SetStaticDefaults() => DisplayName.SetDefault("Mine");

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 40;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.ranged = true;
            projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 7;
            projectile.timeLeft = 280;
        }

        public override void AI()
        {
            projectile.velocity *= 0.96f;
            Lighting.AddLight(projectile.Center, Color.Cyan.ToVector3());

            float idealScale = MathHelper.Lerp(0.93f, 1.07f, (float)Math.Sin(MathHelper.TwoPi * projectile.timeLeft / 14f) * 0.5f + 0.5f);
            projectile.scale = MathHelper.Lerp(0.15f, idealScale, Utils.InverseLerp(0f, 15f, Time, true) * Utils.InverseLerp(0f, 15f, projectile.timeLeft, true));
            Time++;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            foreach (Projectile mine in LocateOtherMines())
            {
                if (projectile.timeLeft <= 12)
                    break;

                float _ = 0f;
                if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), projectile.Center, mine.Center, 20f, ref _))
                    return true;
            }
            return base.Colliding(projHitbox, targetHitbox);
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            // Have the damage be reduced based on hit count. This effect becomes stronger the more hits, before hitting a lower bound.
            float damageFactor = 1f - 3f * (float)Math.Pow(projectile.numHits / 14f, 2D) + 2f * (float)Math.Pow(projectile.numHits / 14f, 3D);

            // The above equation is a polynomial that will eventually shoot off to infinity if the hit count somehow gets high enough.
            // To amend this, the limit is reached by default after enough hits have been made.
            if (projectile.numHits > 12 || damageFactor < DamageFactorLowerBound)
                damageFactor = DamageFactorLowerBound;

            damage = (int)(damage * damageFactor);
        }

        public List<Projectile> LocateOtherMines()
        {
            List<Projectile> mines = new List<Projectile>();
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile proj = Main.projectile[i];
                if (proj.type != projectile.type || !proj.active || proj.timeLeft <= 12 || i == projectile.whoAmI)
                    continue;
                if (!projectile.WithinRange(proj.Center, MineConnectDistanceMax))
                    continue;

                mines.Add(proj);
            }

            return mines;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D baseTexture = Main.projectileTexture[projectile.type];
            Texture2D glowTexture = ModContent.GetTexture($"{Texture}Glowmask");
            Texture2D laserTexture = ModContent.GetTexture($"CalamityMod/Projectiles/Ranged/PrismMineArc");

            void drawLineTo(Vector2 destination, Color laserColor)
            {
                float rotation = projectile.AngleTo(destination);
                float remainingDistance = projectile.Distance(destination) - 30f;
                float laserScale = 0.5f;
                Rectangle frame = laserTexture.Frame(1, 7, 0, (int)(Main.GlobalTime * 10f + projectile.identity * 3f) % 7);
                Vector2 laserOrigin = frame.Size() * 0.5f;
                while (remainingDistance > frame.Height * laserScale)
                {
                    Vector2 laserDrawPosition = projectile.Center + projectile.SafeDirectionTo(destination, -Vector2.UnitY) * remainingDistance - Main.screenPosition;
                    spriteBatch.Draw(laserTexture, laserDrawPosition, frame, laserColor, rotation, laserOrigin, laserScale, SpriteEffects.None, 0f);
                    remainingDistance -= frame.Height * laserScale;
                }
            }

            Vector2 origin = baseTexture.Size() * 0.5f;
            Vector2 drawPosition = projectile.Center - Main.screenPosition;

            foreach (Projectile mine in LocateOtherMines())
            {
                float fade = Utils.InverseLerp(8f, 24f, projectile.timeLeft, true) * Utils.InverseLerp(8f, 24f, mine.timeLeft, true);
                drawLineTo(mine.Center, Color.White * fade);
            }

            spriteBatch.Draw(baseTexture, drawPosition, null, projectile.GetAlpha(lightColor), projectile.rotation, origin, projectile.scale, SpriteEffects.None, 0f);

            Rectangle glowFrame = glowTexture.Frame(1, 6, 0, (int)Time / 4 % 6);
            Color glowColor = Color.White * 0.5f;
            glowColor.A = 0;
            spriteBatch.Draw(glowTexture, drawPosition, glowFrame, glowColor, projectile.rotation, origin, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}
