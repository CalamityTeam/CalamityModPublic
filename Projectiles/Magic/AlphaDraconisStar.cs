using ReLogic.Content;
using System;
using System.Linq;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Graphics.Primitives;
using CalamityMod.Particles;
using CalamityMod.Utilities.Daybreak;
using CalamityMod.Utilities.Daybreak.Buffers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class AlphaDraconisStar : ModProjectile, ILocalizedModType
    {
        private static Asset<Texture2D> _cachedTexScarletDevilStreak;
        private static Asset<Texture2D> _cachedTexSylvestaffStreak;

        public new string LocalizationCategory => "Projectiles.Magic";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 16;
            ProjectileID.Sets.TrailingMode[Type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.penetrate = 5;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.extraUpdates = 4;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10 * Projectile.MaxUpdates;
            Projectile.scale = 0.5f;
            Projectile.tileCollide = false;
            Projectile.timeLeft = Projectile.MaxUpdates * 300;
            Projectile.stopsDealingDamageAfterPenetrateHits = true;
        }

        public override void OnSpawn(IEntitySource source)
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 8;
        }

        NPC target = null;
        Player closestPlayer = null;
        public override void AI()
        {
            Vector2 goalPos = new Vector2(Projectile.ai[0], Projectile.ai[1]);
            float velLength = Projectile.velocity.Length();
            if (Projectile.ai[2] == 0)
            {
                float maxDist = 57600; //240^2
                if (Projectile.FinalExtraUpdate())
                {
                    target = null;
                    foreach (var item in Main.ActiveNPCs)
                    {
                        if (item.CanBeChasedBy(item) && item.DistanceSQ(goalPos) < maxDist)
                        {
                            maxDist = item.DistanceSQ(goalPos);
                            target = item;
                        }
                    }
                }
                if (target != null)
                {
                    if (target.active)
                    {
                        goalPos = target.Center;
                        Projectile.ai[0] = goalPos.X;
                        Projectile.ai[1] = goalPos.Y;
                        Projectile.Calamity().HomingTarget = target.whoAmI;
                    }
                    else
                    {
                        target = null;
                    }
                } else
                {
                    Projectile.Calamity().HomingTarget = -1;
                }

                    Projectile.velocity += Projectile.DirectionTo(goalPos);
                Projectile.velocity.Normalize();
                if (Projectile.velocity.HasNaNs())
                    Projectile.velocity = Vector2.UnitY;
                Projectile.velocity *= velLength;

                if (Projectile.Center.Y > goalPos.Y + 100)
                    Projectile.ai[2] = 1;
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            }
            else
            {
                Projectile.Calamity().HomingTarget = -1;
                Projectile.velocity *= 0.98f;
                if (Projectile.velocity.Length() < 1 && Projectile.ai[2] == 1)
                    Projectile.Kill();
                if (Projectile.ai[2] == 2)
                {
                    Projectile.rotation += 0.05f;
                    Projectile.scale = MathHelper.Min(Projectile.scale + 0.02f, 1);
                    if (Projectile.FinalExtraUpdate())
                    {
                        closestPlayer = null;
                        float closestDis = 320*320;
                        foreach (var player in Main.ActivePlayers)
                        {
                            var dis = Projectile.DistanceSQ(player.Center);
                            if (dis < closestDis)
                            {
                                closestDis = dis;
                                closestPlayer = player;
                            }
                        }
                    }
                    if (closestPlayer != null)
                    {
                        Projectile.velocity += Projectile.DirectionTo(closestPlayer.Center);
                        if (Projectile.Distance(closestPlayer.Center) < 32)
                        {
                            closestPlayer.Calamity().StratusStarburst++;
                            Projectile.Kill();
                        }
                    }
                }
            }

            // Emit a small amount of dust while traveling
            if (Main.rand.NextBool(10))
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, Effects.ArsenalEffects.ArsenalPlasmaDust, -Projectile.velocity);
                dust.scale = Main.rand.NextFloat(0.4f, 1f);
                dust.velocity = -Projectile.velocity.RotatedByRandom(0.3f) * Main.rand.NextFloat(0.1f, 0.7f);
                dust.noGravity = true;
                dust.color = Color.CadetBlue;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.ai[2] == 0)
            {
                if (Main.rand.NextFloat() <= 0.33f) //33% chance to become pick-up-able stars
                {
                    Projectile.ai[2] = 2;
                    Projectile.timeLeft = 600 * Projectile.MaxUpdates;
                }
                else
                    Projectile.ai[2] = 1;
                Projectile.velocity *= Main.rand.NextFloat(0.9f, 1.1f);
                Projectile.netUpdate = true;
            }
        }

        public override void OnKill(int timeLeft)
        {
            if (Projectile.ai[2] == 2)
                return;
            for (int i = 0; i < 4; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<SquashDustPixelated>(), -Projectile.velocity);
                dust.scale = Main.rand.NextFloat(0.4f, 0.6f);
                dust.velocity = (new Vector2(10, 10).RotatedByRandom(100) * Main.rand.NextFloat(0.2f, 1f));
                dust.color = Color.CadetBlue;
            }
        }




        public override bool PreDraw(ref Color lightColor)
        {
            // Glow FX
            Texture2D mainTexture = ModContent.Request<Texture2D>(Texture).Value;
            SpriteEffects spriteEffects = SpriteEffects.None;

            Vector2 drawOrigin = mainTexture.Size() / 2f;
            float drawScale = Projectile.scale;
            float drawRotation = Projectile.rotation;
            Vector2 drawPosition;

            float glowSine = ((float)Math.Sin(Main.GlobalTimeWrappedHourly * 14f) + 1f) / 2f;
            float pulse = MathHelper.Lerp(0.25f, 0.8f, glowSine); // Least protruding to most protruding
            drawPosition = Projectile.Center - Main.screenPosition;

            for (int i = 0; i < 12; i++)
            {
                Vector2 unitVector = (MathHelper.TwoPi * i / 12f).ToRotationVector2();

                Vector2 innerOffset = unitVector * (1.4f * pulse);
                Main.spriteBatch.Draw(mainTexture, drawPosition + innerOffset, null, Color.SkyBlue with { A = 0 } * 0.225f, drawRotation, drawOrigin, drawScale, spriteEffects, 0f);

                Vector2 outerOffset = unitVector * (1.75f * pulse);
                Main.spriteBatch.Draw(mainTexture, drawPosition + outerOffset, null, Color.DeepSkyBlue with { A = 0 } * 0.1f, drawRotation, drawOrigin, drawScale, spriteEffects, 0f);
            }

            // Trail rendering
            Main.spriteBatch.End(out var ss);

            var device = Main.instance.GraphicsDevice;
            using var lease = RenderTargetPool.Shared.Rent(
                device,
                Main.screenWidth / 2,
                Main.screenHeight / 2,
                RenderTargetDescriptor.Default
            );

            using (lease.Scope(clearColor: Color.Transparent))
            {
                if (Projectile.velocity.Length() > 1) // Will otherwise cause noticeable frame drops
                {
                    GameShaders.Misc["CalamityMod:ImpFlameTrail"].SetShaderTexture((_cachedTexScarletDevilStreak ??= ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Trails/ScarletDevilStreak")));
                    PrimitiveRenderer.RenderTrail(Projectile.oldPos, new(FireWidthFunction, FireColorFunction, (_, _) => Projectile.Size * 0.5f, smoothen: true, pixelate: false, shader: GameShaders.Misc["CalamityMod:ImpFlameTrail"], useUnscaledMatrices: true), Projectile.oldPos.Length + 32);

                    Vector2[] fireCoreLength = Projectile.oldPos.Take(8).ToArray();
                    GameShaders.Misc["CalamityMod:ImpFlameTrail"].SetShaderTexture((_cachedTexSylvestaffStreak ??= ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Trails/SylvestaffStreak")));
                    PrimitiveRenderer.RenderTrail(fireCoreLength, new(FireCoreWidthFunction, FireCoreColorFunction, (_, _) => Projectile.Size * 0.5f, smoothen: true, pixelate: false, shader: GameShaders.Misc["CalamityMod:ImpFlameTrail"], useUnscaledMatrices: true), fireCoreLength.Length + 24);
                }
            }

            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            Main.spriteBatch.Draw(lease.Target, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0f);
            Main.spriteBatch.End();

            Main.spriteBatch.Begin(ss);
            return false;
        }

        public float FireWidthFunction(float completion, Vector2 pos)
        {
            float width;
            float maxBodyWidth = 32f * Projectile.scale;
            float curveRatio = 0.05f;
            // Crop the tip of the trail into a conic shape.
            if (completion < curveRatio)
                width = MathF.Pow(completion / curveRatio, 0.5f) * maxBodyWidth;
            else
                width = Utils.Remap(completion, curveRatio, 1f, maxBodyWidth, 0f);

            // Pulse inwards and outwards over time.
            float pulseInterpolant = MathF.Cos(MathHelper.Pi * completion - Main.GlobalTimeWrappedHourly * 20f) * 0.5f + 0.5f;
            float additionalPulseWidth = MathHelper.Lerp(0f, 12f, pulseInterpolant);
            return (width + additionalPulseWidth) * CalamityUtils.CountNonZeroVectors(Projectile.oldPos) / (float)ProjectileID.Sets.TrailCacheLength[Type];
        }

        public Color FireColorFunction(float completion, Vector2 pos)
        {
            Color mainColor = Color.CadetBlue * 1.1f;
            Color endColor = Color.Lerp(mainColor, Color.Transparent, Utils.GetLerpValue(0.8f, 1f, completion, true));
            return Color.Lerp(mainColor, endColor, completion) * Projectile.Opacity;
        }

        public float FireCoreWidthFunction(float completion, Vector2 pos)
        {
            float width;
            float maxBodyWidth = Projectile.scale * 8;
            float curveRatio = 0.1f;
            if (completion < curveRatio)
                width = MathF.Sin(completion / curveRatio * MathHelper.PiOver2) * maxBodyWidth + curveRatio;
            else
                width = Utils.Remap(completion, curveRatio, 1f, maxBodyWidth, 0f);
            return width * CalamityUtils.CountNonZeroVectors(Projectile.oldPos) / (float)ProjectileID.Sets.TrailCacheLength[Type];
        }

        public Color FireCoreColorFunction(float completion, Vector2 pos)
        {
            Color mainColor = Color.CadetBlue;
            Color tipColor = Color.Lerp(mainColor, Color.Transparent, Utils.GetLerpValue(0.2f, 1f, completion, true));
            Color fullBodyColor = Color.Lerp(mainColor, tipColor, completion);
            return Color.Lerp(fullBodyColor, Color.White, 0.175f) * Projectile.Opacity;
        }
    }
}
