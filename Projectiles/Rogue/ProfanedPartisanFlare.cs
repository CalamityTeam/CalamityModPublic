using System;
using System.Linq;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Graphics.Primitives;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Boss;
using CalamityMod.Utilities.Daybreak;
using CalamityMod.Utilities.Daybreak.Buffers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class ProfanedPartisanFlare : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public int timer = 0;
        public override string Texture => "CalamityMod/Projectiles/Boss/HolyFire";
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 4;
            ProjectileID.Sets.CultistIsResistantTo[Type] = true;
        }
        public override void SetDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 8;
            ProjectileID.Sets.TrailingMode[Type] = 2;
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.penetrate = 3;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 120);
        }

        NPC target = null;
        public override void AI()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 3)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame >= Main.projFrames[Type])
                Projectile.frame = 0;

            if (Projectile.originalDamage == 0)
            {
                Projectile.originalDamage = ProfanedPartisan.StarBaseDamage;
                Projectile.ContinuouslyUpdateDamageStats = true;
            }
            if (target != null && Projectile.localNPCImmunity[target.whoAmI] <= 0 && target.active && !target.dontTakeDamage)
            {
                Projectile.Calamity().HomingTarget = target.whoAmI;
                var dis = Projectile.Distance(target.Center);
                Projectile.velocity = Projectile.velocity.ToRotation().AngleLerp(Projectile.DirectionTo(target.Center).ToRotation(), (Projectile.Distance(target.Center) < 160 ? 0.1f + (1 - dis / 160) * 0.4f : 0.1f)).ToRotationVector2() * Projectile.velocity.Length();
            }
            else
            {
                target = GetTargetInRange(2000);
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }
        public override void OnKill(int timeLeft)
        {
            Color hiColor = new Color(255, 155, 25, 255);
            Color loColor = new Color(255, 0, 0, 0);

            for (int i = 0; i < 25; i++)
            {
                GeneralParticleHandler.SpawnParticle(new GlowOrbParticle(Projectile.Center, new Vector2(Main.rand.NextFloat(10), 0).RotatedByRandom(MathHelper.TwoPi), false, 10, Main.rand.NextFloat(0.8f, 1.2f), hiColor));
            }

            GeneralParticleHandler.SpawnParticle(new CustomPulse(Projectile.Center, Vector2.Zero, Color.White, "CalamityMod/Particles/BloomCircle", Vector2.One, 0f, 0.5f, 0.1f, 4));

            for (float i = 0; i < 1; i += 0.25f)
            {
                GeneralParticleHandler.SpawnParticle(new CustomPulse(Projectile.Center, Vector2.Zero, hiColor, "CalamityMod/Particles/SoftRoundExplosion", Vector2.One, Main.rand.NextFloat(MathHelper.TwoPi), 0.02f * i, 0.075f * i, 24));
            }
            GeneralParticleHandler.SpawnParticle(new CustomPulse(Projectile.Center, Vector2.Zero, hiColor, "CalamityMod/Particles/ShatteredExplosion", Vector2.One, Main.rand.NextFloat(MathHelper.TwoPi), 0.02f, 0.045f, 16));

            SoundEngine.PlaySound(SoundID.DD2_BetsyFireballImpact.WithPitchOffset(0.6f), Projectile.Center);
            SoundEngine.PlaySound(SoundID.Item100.WithPitchOffset(0.4f), Projectile.Center);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Color hiColor = new Color(255, 155, 25, 255);
            Color loColor = new Color(255, 0, 0, 0);

            Projectile.Opacity = 1;
            Main.spriteBatch.End(out var ss);
            var device = Main.instance.GraphicsDevice;
            using var trailLease = RenderTargetPool.Shared.Rent(
                device,
                Main.screenWidth / 2,
                Main.screenHeight / 2,
                RenderTargetDescriptor.Default
            );
            //Everything is drawn to this lease, which then is drawn to screen with the desired opacity.
            //This is used to allow projectile opacity to scale nicely
            using var mainLease = RenderTargetPool.Shared.Rent(
                device,
                Main.screenWidth,
                Main.screenHeight,
                RenderTargetDescriptor.Default
            );

            using (mainLease.Scope(clearColor: Color.Transparent))
            {
                using (trailLease.Scope(clearColor: Color.Transparent))
                {
                    GameShaders.Misc["CalamityMod:ImpFlameTrail"].SetShaderTexture(ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Trails/ScarletDevilStreak"));
                    PrimitiveRenderer.RenderTrail(Projectile.oldPos.Take(5).ToArray(), new(FireWidthFunction, FireColorFunction, (_, _) => Projectile.Size * 0.5f, smoothen: true, pixelate: false, shader: GameShaders.Misc["CalamityMod:ImpFlameTrail"], useUnscaledMatrices: true), Projectile.oldPos.Length + 32);
                }

                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null, Matrix.Identity);
                Main.spriteBatch.Draw(trailLease.Target, Vector2.Zero, null, Color.White * 0.75f, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0f);


                float lerpMult = MathHelper.Lerp(0.5f, 1.5f, Math.Abs(MathF.Sin(Projectile.localAI[1] / 10f)));

                Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[ModContent.ProjectileType<HolyBurnOrb>()].Value;
                Vector2 drawPos = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
                Color baseColor = Color.Lerp(hiColor, loColor, 0.5f);
                Color baseColor2 = hiColor;
                baseColor.A = 0;
                baseColor *= lerpMult;
                baseColor2 *= lerpMult;
                Vector2 origin = texture.Size() / 2f;
                Vector2 scale = new Vector2(0.5f, 1f) * ((lerpMult - 1) * 0.5f + 1f) * 1.2f * Projectile.scale;

                SpriteEffects spriteEffects = SpriteEffects.None;
                if (Projectile.spriteDirection == -1)
                    spriteEffects = SpriteEffects.FlipHorizontally;

                Projectile.rotation += MathHelper.ToRadians(lerpMult * 2f);

                float upRight = MathHelper.PiOver4;
                float up = MathHelper.PiOver2;
                float upLeft = 3f * MathHelper.PiOver4;
                float left = MathHelper.Pi;
                Main.EntitySpriteDraw(texture, drawPos, null, baseColor, upLeft + Projectile.rotation, origin, scale, spriteEffects, 0);
                Main.EntitySpriteDraw(texture, drawPos, null, baseColor, upRight - Projectile.rotation, origin, scale, spriteEffects, 0);
                Main.EntitySpriteDraw(texture, drawPos, null, baseColor2, upLeft + Projectile.rotation, origin, scale * 0.6f, spriteEffects, 0);
                Main.EntitySpriteDraw(texture, drawPos, null, baseColor2, upRight - Projectile.rotation, origin, scale * 0.6f, spriteEffects, 0);
                Main.EntitySpriteDraw(texture, drawPos, null, baseColor, up + Projectile.rotation, origin, scale * 0.6f, spriteEffects, 0);
                Main.EntitySpriteDraw(texture, drawPos, null, baseColor, left - Projectile.rotation, origin, scale * 0.6f, spriteEffects, 0);
                Main.EntitySpriteDraw(texture, drawPos, null, baseColor2, up + Projectile.rotation, origin, scale * 0.36f, spriteEffects, 0);
                Main.EntitySpriteDraw(texture, drawPos, null, baseColor2, left - Projectile.rotation, origin, scale * 0.36f, spriteEffects, 0);

                scale = new Vector2(1f, 1f);
                texture = ModContent.Request<Texture2D>("CalamityMod/Particles/GlowOrbParticle").Value;
                using (Main.spriteBatch.Scope())
                {

                    Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null, Matrix.Identity);
                    Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, baseColor2, 0, texture.Size() * 0.5f, 1f, 0, 0f);
                    Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White, 0, texture.Size() * 0.5f, 0.5f, 0, 0f);
                    Main.spriteBatch.End();
                }
                Main.spriteBatch.End();
            }

            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
            Main.spriteBatch.Draw(mainLease.Target, Vector2.Zero, null, Color.White * Projectile.Opacity, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(ss);
            return false;
        }
        public float FireWidthFunction(float completion, Vector2 pos)
        {
            float width;
            float maxBodyWidth = 16f * Projectile.scale;
            float curveRatio = 0.2f;
            var positions = Projectile.oldPos.ToList();
            positions.RemoveAll(x => x == Vector2.Zero);
            // Crop the tip of the trail into a conic shape.
            if (completion < curveRatio)
                width = MathF.Pow(completion / curveRatio, 0.5f) * maxBodyWidth;
            else
                width = Utils.Remap(completion, curveRatio, 1f, maxBodyWidth, 0f);

            // Pulse inwards and outwards over time.
            float pulseInterpolant = MathF.Cos(MathHelper.Pi * completion - Main.GlobalTimeWrappedHourly * 20f) * 0.5f + 0.5f;
            float additionalPulseWidth = MathHelper.Lerp(0f, 12f, pulseInterpolant);
            return (width + additionalPulseWidth) * positions.Count() / (float)ProjectileID.Sets.TrailCacheLength[Type];
        }

        public Color FireColorFunction(float completion, Vector2 pos)
        {
            Color mainColor = new Color(255, 155, 25, 255);
            Color endColor = Color.Lerp(mainColor, Color.Transparent, Utils.GetLerpValue(0.8f, 1f, completion, true));
            return Color.Lerp(mainColor, Color.Transparent, completion);
        }

        NPC GetTargetInRange(float range)
        {
            var player = Main.player[Projectile.owner];
            {
                NPC gotTarget = null;
                float currentDistance = range;
                foreach (var npc in Main.ActiveNPCs)
                {
                    if (Projectile.localNPCImmunity[npc.whoAmI] > 0)
                        continue;
                    var myDistance = npc.Distance(Projectile.Center);

                    if (npc.CanBeChasedBy() && myDistance < currentDistance)
                    {
                        currentDistance = myDistance;
                        gotTarget = npc;
                    }
                }
                return gotTarget;
            }
        }
    }
}
