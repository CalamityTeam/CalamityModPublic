using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class PrismRay : ModProjectile
    {
        public Vector2 StartingPosition;
        public Color RayColor => CalamityUtils.MulticolorLerp(RayHue, CalamityUtils.ExoPalette);
        public Color HueDownscaledRayColor => RayColor * 0.66f;
        public ref float RayHue => ref projectile.ai[0];
        public ref float Time => ref projectile.localAI[1];
        public const int Lifetime = 30;
        public override string Texture => "CalamityMod/Projectiles/StarProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Prismatic Light");
            ProjectileID.Sets.MinionShot[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 14;
            projectile.alpha = 255;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.timeLeft = 900;
            projectile.friendly = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
            projectile.melee = true;
        }

        public override void AI()
        {
            DelegateMethods.v3_1 = RayColor.ToVector3() * 0.5f;
            Utils.PlotTileLine(StartingPosition, projectile.Center, 8f, DelegateMethods.CastLight);
            if (projectile.localAI[0] == 0f)
            {
                projectile.direction = Main.rand.NextBool(2).ToDirectionInt();
                projectile.localAI[0] = 1f;
            }
            projectile.rotation = Time / 20f * MathHelper.TwoPi * projectile.direction;
            projectile.alpha = Utils.Clamp(projectile.alpha - 18, 0, 255);
            if (projectile.alpha == 0)
                Lighting.AddLight(projectile.Center, RayColor.ToVector3() * 0.5f);

            for (int i = 0; i < 2; i++)
            {
                if (Main.rand.NextBool(10))
                {
                    Dust verticalMagic = Dust.NewDustDirect(projectile.Center, 0, 0, 267, 0f, 0f, 225, RayColor, 1.5f);
                    verticalMagic.noGravity = true;
                    verticalMagic.noLight = true;
                    verticalMagic.scale = projectile.Opacity;
                    verticalMagic.position = projectile.Center;
                    verticalMagic.velocity = Vector2.UnitY.RotatedBy(projectile.rotation + MathHelper.TwoPi * i / 2f) * 2.5f;
                }
            }
            if (Main.rand.NextBool(10))
            {
                Vector2 dustSpawnPosition = projectile.Center + Main.rand.NextVector2Unit() * Main.rand.NextFloat(20f, 120f);
                Point dustTileCoords = dustSpawnPosition.ToTileCoordinates();
                bool canSpawnDust = true;
                if (!WorldGen.InWorld(dustTileCoords.X, dustTileCoords.Y, 0))
                    canSpawnDust = false;

                if (canSpawnDust && WorldGen.SolidTile(dustTileCoords.X, dustTileCoords.Y))
                    canSpawnDust = false;

                if (canSpawnDust)
                {
                    Dust risingMagic = Dust.NewDustDirect(dustSpawnPosition, 0, 0, 267, 0f, 0f, 127, RayColor, 1f);
                    risingMagic.noGravity = true;
                    risingMagic.position = dustSpawnPosition;
                    risingMagic.velocity = -Vector2.UnitY * Main.rand.NextFloat(1.6f, 7.5f);
                    risingMagic.fadeIn = Main.rand.NextFloat(1f, 2f);
                    risingMagic.scale = Main.rand.NextFloat(0.6f, 1.2f);
                    risingMagic.noLight = true;

                    risingMagic = Dust.CloneDust(risingMagic);
                    risingMagic.scale *= 0.65f;
                    risingMagic.fadeIn *= 0.65f;
                    risingMagic.color = new Color(255, 255, 255, 255);
                }
            }
            projectile.scale = projectile.Opacity * 0.5f;
            projectile.velocity = Vector2.Zero;

            Time++;
            if (Time >= Lifetime)
                projectile.Kill();
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float _ = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), StartingPosition, projectile.Center, projectile.scale * 22f, ref _);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Vector2 baseDrawPosition = projectile.Center + Vector2.UnitY * projectile.gfxOffY - Main.screenPosition;
            Texture2D texture = Main.projectileTexture[projectile.type];
            Rectangle frame = texture.Frame(1, Main.projFrames[projectile.type], 0, projectile.frame);
            Vector2 origin = frame.Size() / 2f;
            Color fadedRayColor = projectile.GetAlpha(lightColor);
            Color fullbrightRayColor = HueDownscaledRayColor.MultiplyRGBA(new Color(255, 255, 255, 0));

            // Draw the stars at the end of the laser.
            for (int i = 0; i < 6; i++)
            {
                Vector2 drawPosition = baseDrawPosition + (MathHelper.TwoPi * i / 6f + Main.GlobalTime * 3f).ToRotationVector2() * projectile.scale * 4f;
                spriteBatch.Draw(texture, drawPosition, frame, fullbrightRayColor, projectile.rotation, origin, projectile.scale * 2f, SpriteEffects.None, 0f);
                spriteBatch.Draw(texture, drawPosition, frame, fullbrightRayColor, 0f, origin, projectile.scale * 2f, SpriteEffects.None, 0f);
            }

            // Draw the shimmering ray itself.
            if (projectile.Opacity > 0.3f)
            {
                Vector2 drawOffset = (StartingPosition - projectile.Center) * 0.5f;
                Vector2 scale = new Vector2(1f, drawOffset.Length() * 2f / texture.Height);
                float rotation = drawOffset.ToRotation() + MathHelper.PiOver2;
                
                // This factor causes the opacity to flash rather quickly.
                float drawFade = MathHelper.Clamp(MathHelper.Distance(Lifetime * 0.5f, Time) / (Lifetime * 0.667f), 0f, 1f);

                for (int i = 0; i < 3; i++)
                {
                    Vector2 drawPosition = baseDrawPosition;
                    drawPosition += (MathHelper.TwoPi * i / 3f + Main.GlobalTime * 3f).ToRotationVector2() * projectile.scale * 2.5f;
                    drawPosition += drawOffset;

                    spriteBatch.Draw(texture, drawPosition, frame, fullbrightRayColor * drawFade * 0.8f, rotation, origin, scale, SpriteEffects.None, 0f);
                    spriteBatch.Draw(texture, drawPosition, frame, fadedRayColor * drawFade * 0.8f, rotation, origin, scale * 0.5f, SpriteEffects.None, 0f);
                }
            }
            return false;
        }

        public override void Kill(int timeLeft)
        {
            CreateKillExplosionBurstDust(Main.rand.Next(7, 13));

            // Adjust values and do damage before dying.
            if (Main.myPlayer != projectile.owner)
                return;

            Vector2 oldSize = projectile.Size;
            projectile.position = projectile.Center;
            projectile.width = projectile.height = 60;
            projectile.Center = projectile.position;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
            projectile.Damage();
            projectile.position = projectile.Center;
            projectile.Size = oldSize;
            projectile.Center = projectile.position;
        }

        public void CreateKillExplosionBurstDust(int dustCount)
        {
            if (Main.dedServ)
                return;

            Vector2 baseExplosionDirection = -Vector2.UnitY.RotatedByRandom(MathHelper.Pi) * 3f;
            Vector2 outwardFireSpeedFactor = new Vector2(2.1f, 2f);
            Color brightenedRayColor = RayColor;
            brightenedRayColor.A = 255;

            for (float i = 0f; i < dustCount; i++)
            {
                Dust explosionDust = Dust.NewDustDirect(projectile.Center, 0, 0, 267, 0f, 0f, 0, brightenedRayColor, 1f);
                explosionDust.position = projectile.Center;
                explosionDust.velocity = baseExplosionDirection.RotatedBy(MathHelper.TwoPi * i / dustCount) * outwardFireSpeedFactor * Main.rand.NextFloat(0.8f, 1.2f);
                explosionDust.noGravity = true;
                explosionDust.scale = 1.1f;
                explosionDust.fadeIn = Main.rand.NextFloat(1.4f, 2.4f);

                explosionDust = Dust.CloneDust(explosionDust);
                explosionDust.scale /= 2f;
                explosionDust.fadeIn /= 2f;
                explosionDust.color = new Color(255, 255, 255, 255);
            }
            for (float i = 0f; i < dustCount; i++)
            {
                Dust explosionDust = Dust.NewDustDirect(projectile.Center, 0, 0, 267, 0f, 0f, 0, brightenedRayColor, 1f);
                explosionDust.position = projectile.Center;
                explosionDust.velocity = baseExplosionDirection.RotatedBy(MathHelper.TwoPi * i / dustCount) * outwardFireSpeedFactor * Main.rand.NextFloat(0.8f, 1.2f);
                explosionDust.velocity *= Main.rand.NextFloat() * 0.8f;
                explosionDust.noGravity = true;
                explosionDust.scale = Main.rand.NextFloat();
                explosionDust.fadeIn = Main.rand.NextFloat(1.4f, 2.4f);

                explosionDust = Dust.CloneDust(explosionDust);
                explosionDust.scale /= 2f;
                explosionDust.fadeIn /= 2f;
                explosionDust.color = new Color(255, 255, 255, 255);
            }
        }

        public override Color? GetAlpha(Color lightColor) => new Color(projectile.Opacity, projectile.Opacity, projectile.Opacity, 0);
    }
}
