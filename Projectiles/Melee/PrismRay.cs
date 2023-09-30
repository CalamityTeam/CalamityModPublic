using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class PrismRay : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public Vector2 StartingPosition;
        public Color RayColor => CalamityUtils.MulticolorLerp(RayHue, CalamityUtils.ExoPalette);
        public Color HueDownscaledRayColor => RayColor * 0.66f;
        public ref float RayHue => ref Projectile.ai[0];
        public ref float Time => ref Projectile.localAI[1];
        public const int Lifetime = 30;
        public override string Texture => "CalamityMod/Projectiles/StarProj";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 14;
            Projectile.alpha = 255;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 900;
            Projectile.friendly = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
        }

        public override void AI()
        {
            DelegateMethods.v3_1 = RayColor.ToVector3() * 0.5f;
            Utils.PlotTileLine(StartingPosition, Projectile.Center, 8f, DelegateMethods.CastLight);
            if (Projectile.localAI[0] == 0f)
            {
                Projectile.direction = Main.rand.NextBool().ToDirectionInt();
                Projectile.localAI[0] = 1f;
            }
            Projectile.rotation = Time / 20f * MathHelper.TwoPi * Projectile.direction;
            Projectile.alpha = Utils.Clamp(Projectile.alpha - 18, 0, 255);
            if (Projectile.alpha == 0)
                Lighting.AddLight(Projectile.Center, RayColor.ToVector3() * 0.5f);

            for (int i = 0; i < 2; i++)
            {
                if (Main.rand.NextBool(10))
                {
                    Dust verticalMagic = Dust.NewDustDirect(Projectile.Center, 0, 0, 267, 0f, 0f, 225, RayColor, 1.5f);
                    verticalMagic.noGravity = true;
                    verticalMagic.noLight = true;
                    verticalMagic.scale = Projectile.Opacity;
                    verticalMagic.position = Projectile.Center;
                    verticalMagic.velocity = Vector2.UnitY.RotatedBy(Projectile.rotation + MathHelper.TwoPi * i / 2f) * 2.5f;
                }
            }
            if (Main.rand.NextBool(10))
            {
                Vector2 dustSpawnPosition = Projectile.Center + Main.rand.NextVector2Unit() * Main.rand.NextFloat(20f, 120f);
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
            Projectile.scale = Projectile.Opacity * 0.5f;
            Projectile.velocity = Vector2.Zero;

            Time++;
            if (Time >= Lifetime)
                Projectile.Kill();
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float _ = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), StartingPosition, Projectile.Center, Projectile.scale * 22f, ref _);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 baseDrawPosition = Projectile.Center + Vector2.UnitY * Projectile.gfxOffY - Main.screenPosition;
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Rectangle frame = texture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame);
            Vector2 origin = frame.Size() / 2f;
            Color fadedRayColor = Projectile.GetAlpha(lightColor);
            Color fullbrightRayColor = HueDownscaledRayColor.MultiplyRGBA(new Color(255, 255, 255, 0));

            // Draw the stars at the end of the laser.
            for (int i = 0; i < 6; i++)
            {
                Vector2 drawPosition = baseDrawPosition + (MathHelper.TwoPi * i / 6f + Main.GlobalTimeWrappedHourly * 3f).ToRotationVector2() * Projectile.scale * 4f;
                Main.EntitySpriteDraw(texture, drawPosition, frame, fullbrightRayColor, Projectile.rotation, origin, Projectile.scale * 2f, SpriteEffects.None, 0);
                Main.EntitySpriteDraw(texture, drawPosition, frame, fullbrightRayColor, 0f, origin, Projectile.scale * 2f, SpriteEffects.None, 0);
            }

            // Draw the shimmering ray itself.
            if (Projectile.Opacity > 0.3f)
            {
                Vector2 drawOffset = (StartingPosition - Projectile.Center) * 0.5f;
                Vector2 scale = new Vector2(1f, drawOffset.Length() * 2f / texture.Height);
                float rotation = drawOffset.ToRotation() + MathHelper.PiOver2;

                // This factor causes the opacity to flash rather quickly.
                float drawFade = MathHelper.Clamp(MathHelper.Distance(Lifetime * 0.5f, Time) / (Lifetime * 0.667f), 0f, 1f);

                for (int i = 0; i < 3; i++)
                {
                    Vector2 drawPosition = baseDrawPosition;
                    drawPosition += (MathHelper.TwoPi * i / 3f + Main.GlobalTimeWrappedHourly * 3f).ToRotationVector2() * Projectile.scale * 2.5f;
                    drawPosition += drawOffset;

                    Main.EntitySpriteDraw(texture, drawPosition, frame, fullbrightRayColor * drawFade * 0.8f, rotation, origin, scale, SpriteEffects.None, 0);
                    Main.EntitySpriteDraw(texture, drawPosition, frame, fadedRayColor * drawFade * 0.8f, rotation, origin, scale * 0.5f, SpriteEffects.None, 0);
                }
            }
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            CreateKillExplosionBurstDust(Main.rand.Next(7, 13));

            // Adjust values and do damage before dying.
            if (Main.myPlayer != Projectile.owner)
                return;

            Vector2 oldSize = Projectile.Size;
            Projectile.position = Projectile.Center;
            Projectile.width = Projectile.height = 60;
            Projectile.Center = Projectile.position;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.Damage();
            Projectile.position = Projectile.Center;
            Projectile.Size = oldSize;
            Projectile.Center = Projectile.position;
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
                Dust explosionDust = Dust.NewDustDirect(Projectile.Center, 0, 0, 267, 0f, 0f, 0, brightenedRayColor, 1f);
                explosionDust.position = Projectile.Center;
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
                Dust explosionDust = Dust.NewDustDirect(Projectile.Center, 0, 0, 267, 0f, 0f, 0, brightenedRayColor, 1f);
                explosionDust.position = Projectile.Center;
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

        public override Color? GetAlpha(Color lightColor) => new Color(Projectile.Opacity, Projectile.Opacity, Projectile.Opacity, 0);

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<MiracleBlight>(), 300);

        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(ModContent.BuffType<MiracleBlight>(), 300);
    }
}
