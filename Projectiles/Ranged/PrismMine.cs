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
        public ref float Time => ref Projectile.ai[0];
        public const float DamageFactorLowerBound = 0.425f;
        public const float MineConnectDistanceMax = 1200f;
        public override void SetStaticDefaults() => DisplayName.SetDefault("Mine");

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 7;
            Projectile.timeLeft = 280;
        }

        public override void AI()
        {
            Projectile.velocity *= 0.96f;
            Lighting.AddLight(Projectile.Center, Color.Cyan.ToVector3());

            float idealScale = MathHelper.Lerp(0.93f, 1.07f, (float)Math.Sin(MathHelper.TwoPi * Projectile.timeLeft / 14f) * 0.5f + 0.5f);
            Projectile.scale = MathHelper.Lerp(0.15f, idealScale, Utils.GetLerpValue(0f, 15f, Time, true) * Utils.GetLerpValue(0f, 15f, Projectile.timeLeft, true));
            Time++;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            foreach (Projectile mine in LocateOtherMines())
            {
                if (Projectile.timeLeft <= 12)
                    break;

                float _ = 0f;
                if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, mine.Center, 20f, ref _))
                    return true;
            }
            return base.Colliding(projHitbox, targetHitbox);
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            // Have the damage be reduced based on hit count. This effect becomes stronger the more hits, before hitting a lower bound.
            float damageFactor = 1f - 3f * (float)Math.Pow(Projectile.numHits / 14f, 2D) + 2f * (float)Math.Pow(Projectile.numHits / 14f, 3D);

            // The above equation is a polynomial that will eventually shoot off to infinity if the hit count somehow gets high enough.
            // To amend this, the limit is reached by default after enough hits have been made.
            if (Projectile.numHits > 12 || damageFactor < DamageFactorLowerBound)
                damageFactor = DamageFactorLowerBound;

            damage = (int)(damage * damageFactor);
        }

        public List<Projectile> LocateOtherMines()
        {
            List<Projectile> mines = new List<Projectile>();
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile proj = Main.projectile[i];
                if (proj.type != Projectile.type || !proj.active || proj.timeLeft <= 12 || i == Projectile.whoAmI)
                    continue;
                if (!Projectile.WithinRange(proj.Center, MineConnectDistanceMax))
                    continue;

                mines.Add(proj);
            }

            return mines;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D baseTexture = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D glowTexture = ModContent.Request<Texture2D>($"{Texture}Glowmask").Value;
            Texture2D laserTexture = ModContent.Request<Texture2D>($"CalamityMod/Projectiles/Ranged/PrismMineArc").Value;

            void drawLineTo(Vector2 destination, Color laserColor)
            {
                float rotation = Projectile.AngleTo(destination);
                float remainingDistance = Projectile.Distance(destination) - 30f;
                float laserScale = 0.5f;
                Rectangle frame = laserTexture.Frame(1, 7, 0, (int)(Main.GlobalTimeWrappedHourly * 10f + Projectile.identity * 3f) % 7);
                Vector2 laserOrigin = frame.Size() * 0.5f;
                while (remainingDistance > frame.Height * laserScale)
                {
                    Vector2 laserDrawPosition = Projectile.Center + Projectile.SafeDirectionTo(destination, -Vector2.UnitY) * remainingDistance - Main.screenPosition;
                    Main.EntitySpriteDraw(laserTexture, laserDrawPosition, frame, laserColor, rotation, laserOrigin, laserScale, SpriteEffects.None, 0);
                    remainingDistance -= frame.Height * laserScale;
                }
            }

            Vector2 origin = baseTexture.Size() * 0.5f;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;

            foreach (Projectile mine in LocateOtherMines())
            {
                float fade = Utils.GetLerpValue(8f, 24f, Projectile.timeLeft, true) * Utils.GetLerpValue(8f, 24f, mine.timeLeft, true);
                drawLineTo(mine.Center, Color.White * fade);
            }

            Main.EntitySpriteDraw(baseTexture, drawPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);

            Rectangle glowFrame = glowTexture.Frame(1, 6, 0, (int)Time / 4 % 6);
            Color glowColor = Color.White * 0.5f;
            glowColor.A = 0;
            Main.EntitySpriteDraw(glowTexture, drawPosition, glowFrame, glowColor, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}
