using System;
using CalamityMod.Graphics.Primitives;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Particles;
using CalamityMod.Enums;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class SHPB : ModProjectile, ILocalizedModType, IPixelatedPrimitiveRenderer
    {
        public GeneralDrawLayer LayerToRenderTo => GeneralDrawLayer.BeforeProjectiles;

        public ref float ExplodeTimer => ref Projectile.ai[2];

        public bool CanExplodeFromProximity
        {
            get => Projectile.ai[1] == 1f;
            set => Projectile.ai[1] = value.ToInt();
        }

        public new string LocalizationCategory => "Projectiles.Magic";

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 4;
            ProjectileID.Sets.TrailCacheLength[Type] = 12;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.alpha = 0;
            Projectile.scale = 0f;
            Projectile.timeLeft = 300;
            Projectile.DamageType = DamageClass.Magic;
        }

        // This is reused for all SHPC projectiles
        #region General SHPC Projectile Functions
        public enum SoulType
        {
            Light, // Larger explosion
            Night, // Explosion lasts longer
            Flight, // Restores flight time on direct hits
            Might, // Launches enemies
            Sight, // Weakly homes
            Fright // Deals extra flat damage
        }

        public static SoulType GetSoulEffects(int projai)
        {
            switch (projai)
            {
                case 0:
                    return SoulType.Light;
                case 1:
                    return SoulType.Night;
                case 2:
                    return SoulType.Flight;
                case 3:
                    return SoulType.Might;
                case 4:
                    return SoulType.Sight;
                case 5:
                    return SoulType.Fright;

                default:
                    return SoulType.Light;
            }
        }

        public static Color FindColorForSoul(int projai)
        {
            switch (projai)
            {
                case 0:
                    return new(240, 29, 196);
                case 1:
                    return new(123, 29, 220);
                case 2:
                    return new(106, 240, 250);
                case 3:
                    return new(4, 51, 222);
                case 4:
                    return new(79, 255, 124);
                case 5:
                    return new(255, 96, 20);

                default:
                    return new(0, 0, 0);
            }
        }
        #endregion General SHPC Projectile Functions

        public override void AI()
        {
            // Light and fade in
            float lights = (float)Main.rand.Next(90, 111) * 0.01f;
            lights *= Main.essScale;
            Lighting.AddLight(Projectile.Center, 1f * lights, 0.2f * lights, 0.75f * lights);

            // Animation
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 4)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame > 3)
                    Projectile.frame = 0;
            }

            // Sight has weak homing
            if (GetSoulEffects((int)Projectile.ai[0]) == SoulType.Sight)
            {
                float npcDistCheck = SHPC.SightHomingRange;
                int index = -1;
                foreach (NPC n in Main.ActiveNPCs)
                {
                    if (!n.CanBeChasedBy(Projectile))
                        continue;

                    float currentNPCDist = Vector2.Distance(n.Center, Projectile.Center);
                    if (currentNPCDist < npcDistCheck)
                    {
                        npcDistCheck = currentNPCDist;
                        index = n.whoAmI;
                    }
                }

                if (index != -1)
                    Projectile.velocity = (Projectile.velocity * 17f + Utils.DirectionTo(Projectile.Center, Main.npc[index].Center) * 20f) / 18f;
                else
                    Projectile.velocity *= 0.9875f; // Slow down over time if not homing
            }
            else if (GetSoulEffects((int)Projectile.ai[0]) != SoulType.Flight) // Always slow down if not Flight
                Projectile.velocity *= 0.9875f;

            // As soon as a plasma orb is in range of an enemy, it will explode in 1 second.
            float explodeRange = 250f;
            foreach (NPC n in Main.ActiveNPCs)
            {
                if (n.CanBeChasedBy(Projectile, false) && Collision.CanHit(Projectile.Center, 1, 1, n.Center, 1, 1))
                {
                    float npcX = n.position.X + (float)(n.width / 2);
                    float npcY = n.position.Y + (float)(n.height / 2);
                    float npcDist = Math.Abs(Projectile.position.X + (float)(Projectile.width / 2) - npcX) + Math.Abs(Projectile.position.Y + (float)(Projectile.height / 2) - npcY);
                    if (npcDist < explodeRange)
                    {
                        explodeRange = npcDist;
                        CanExplodeFromProximity = true;
                    }
                }
            }

            if (CanExplodeFromProximity)
            {
                ExplodeTimer++;
                if (ExplodeTimer >= 60f)
                    Projectile.Kill();

                // Quickly scale down before exploding.
                bool lightSoul = GetSoulEffects((int)Projectile.ai[0]) == SoulType.Light;
                float minScale = 0.5f * (lightSoul ? SHPC.LightExplosionSizeMult : 1f);
                float maxScale = 1.9f * (lightSoul ? SHPC.LightExplosionSizeMult : 1f);
                Projectile.scale = MathHelper.Clamp(Projectile.scale - 0.075f, minScale, maxScale);
            }
            else
            {
                // Size control.
                // Scale up to the minimum scale at the start.
                if (Projectile.timeLeft >= 285)
                {
                    bool lightSoul = GetSoulEffects((int)Projectile.ai[0]) == SoulType.Light;
                    float minScale = 1.5f * (lightSoul ? SHPC.LightExplosionSizeMult : 1f);
                    Projectile.scale += 0.125f;
                    if (Projectile.scale > minScale)
                        Projectile.scale = minScale;
                }
                // Pulse occasionally afterwards.
                else if (Projectile.timeLeft <= 285 && Projectile.timeLeft >= 45)
                {
                    bool lightSoul = GetSoulEffects((int)Projectile.ai[0]) == SoulType.Light;
                    float minScale = 1.5f * (lightSoul ? SHPC.LightExplosionSizeMult : 1f);
                    float maxScale = 1.9f * (lightSoul ? SHPC.LightExplosionSizeMult : 1f);

                    Projectile.localAI[0]++;
                    Projectile.scale = MathHelper.Lerp(minScale, maxScale, CalamityUtils.SineBumpEasing(Projectile.localAI[0] / 75f, 1));
                }
                // Scale down once the projectile is about to die.
                else
                {
                    // Save the last scale value of the projectile so that we can seamlessly shrink the projectile.
                    if (Projectile.localAI[1] == 0f)
                        Projectile.localAI[1] = Projectile.scale;

                    Projectile.scale = Utils.Remap(Projectile.timeLeft, 45, 0, Projectile.localAI[1], 0.35f, true);
                }
            }

            Projectile.ExpandHitboxBy((int)(24 * Projectile.scale));
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            // Miscellaneous visual additions.
            float deathInterpolant = ExplodeTimer > 0 ? Utils.GetLerpValue(0, 50, ExplodeTimer, true) : Utils.GetLerpValue(60, 10, Projectile.timeLeft, true);
            if (Main.rand.NextBool(3) && deathInterpolant <= 0f)
            {
                for (int i = 0; i < 4; i++)
                {
                    Vector2 dustSpawnPosition = Projectile.Center + Main.rand.NextVector2Circular(Projectile.width * 0.75f, Projectile.height * 0.75f);
                    Vector2 dustVelocity = Projectile.velocity * -1.2f; 
                    Dust dust = Dust.NewDustDirect(dustSpawnPosition, 1, 1, DustID.TintableDustLighted, dustVelocity.X, dustVelocity.Y);
                    dust.scale = Main.rand.NextFloat(0.8f, 1.2f);
                    dust.noGravity = true;
                    dust.noLight = false;
                    dust.color = FindColorForSoul((int)Projectile.ai[0]);
                    dust.velocity *= 0.9f;
                }
            }

            // Start emitting particles on death.
            if (deathInterpolant > 0f)
            {
                float speedMultiplier = MathHelper.Lerp(1f, 3f, deathInterpolant);
                float scaleMultiplier = MathHelper.Lerp(1f, 1.65f, deathInterpolant);
                for (int i = 0; i < 3; i++)
                {
                    Vector2 dustVelocity = Main.rand.NextVector2Circular(1f, 1f) * 2.5f * speedMultiplier;
                    Dust dust = Dust.NewDustDirect(Projectile.Center, 1, 1, DustID.TintableDustLighted, dustVelocity.X, dustVelocity.Y);
                    dust.scale = Main.rand.NextFloat(1.4f, 1.8f) * scaleMultiplier;
                    dust.color = FindColorForSoul((int)Projectile.ai[0]);
                    dust.noGravity = true;
                    dust.noLight = false;
                }

                Vector2 plasmaVelocity = Main.rand.NextVector2Circular(1f, 1f) * 1.35f * speedMultiplier;
                Color plasmaColor = Color.Lerp(FindColorForSoul((int)Projectile.ai[0]), Color.White, deathInterpolant);

                float plasmaScale = Main.rand.NextFloat(0.2f, 0.4f) * scaleMultiplier;
                int plasmaLifetime = Main.rand.Next(30, 45);

                SquishyLightParticle plasma = new(Projectile.Center, plasmaVelocity, plasmaScale, plasmaColor, plasmaLifetime);
                GeneralParticleHandler.SpawnParticle(plasma);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player owner = Main.player[Projectile.owner];
            // Flight restores flight time on direct hit
            if (GetSoulEffects((int)Projectile.ai[0]) == SoulType.Flight)
            {
                if (owner.wingTime < owner.wingTimeMax)
                    owner.wingTime += SHPC.FlightDirectHitFlightBoost;

                if (owner.wingTime > owner.wingTimeMax)
                    owner.wingTime = owner.wingTimeMax;
            }

            // Might launches enemies
            if (GetSoulEffects((int)Projectile.ai[0]) == SoulType.Might && target.CanBeMoved())
            {
                // 14NOV2024: Ozzatron: clamped mouse position unnecessary, only used for direction
                Vector2 launchVel = Utils.DirectionTo(owner.Center, owner.Calamity().mouseWorld) - Vector2.UnitY * 5f;
                target.MoveNPC(launchVel, SHPC.MightKnockbackStrength, false, owner);
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            // Fright deals extra flat damage
            if (GetSoulEffects((int)Projectile.ai[0]) == SoulType.Fright)
                modifiers.SourceDamage.Flat += SHPC.FrightFlatDamage;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item105, Projectile.Center);
            float screenshake = GetSoulEffects((int)Projectile.ai[0]) == SoulType.Light ? 5f : 3.5f;
            Main.LocalPlayer.SetScreenshake(screenshake);

            if (Projectile.owner == Main.myPlayer)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<SHPExplosion>(), Projectile.damage, Projectile.knockBack, Projectile.owner, Projectile.ai[0], 0f);

                for (int i = 0; i < 8; i++)
                {
                    bool pickup = i >= 5;
                    Vector2 soulVelocity = -Vector2.UnitY.RotatedByRandom(MathHelper.Pi) * (pickup ? 2.75f : Main.rand.NextFloat(6f, 9f));
                    int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, soulVelocity, ModContent.ProjectileType<SHPS>(), (int)(Projectile.damage * 0.33f), 0f, Projectile.owner, Main.rand.Next(6), 0f, pickup ? 1f : 0f);
                    if (pickup)
                        Main.projectile[p].timeLeft *= 3;
                }
            }

            // Explosion visuals.
            for (int i = 0; i < 25; i++)
            {
                Vector2 plasmaVelocity = Vector2.One.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(6f, 11f);
                Color plasmaColor = Color.Lerp(FindColorForSoul((int)Projectile.ai[0]), Color.White, Main.rand.NextFloat());

                float plasmaScale = Main.rand.NextFloat(0.8f, 1.4f);
                int plasmaLifetime = Main.rand.Next(30, 45);

                SquishyLightParticle plasma = new(Projectile.Center, plasmaVelocity, plasmaScale, plasmaColor, plasmaLifetime);
                GeneralParticleHandler.SpawnParticle(plasma);
            }

            for (int i = 0; i < 15; i++)
            {
                Vector2 dustVelocity = Main.rand.NextVector2Circular(1f, 1f) * 6f;
                float dustScale = Main.rand.NextFloat(1.8f, 2.4f);
                Color dustColor = Color.Lerp(FindColorForSoul((int)Projectile.ai[0]), Color.White, Main.rand.NextFloat());

                Dust dust = Dust.NewDustDirect(Projectile.Center, 1, 1, DustID.TintableDustLighted, dustVelocity.X, dustVelocity.Y, 0, dustColor, dustScale);
                dust.noGravity = true;
                dust.noLight = false;
                dust.noLightEmittence = false;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTexture = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D bloomCircleTexture = ModContent.Request<Texture2D>("CalamityMod/Particles/BloomCircle").Value;
            Rectangle frame = mainTexture.Frame(1, Main.projFrames[Type], 0, Projectile.frame);

            // Start to shake before exploding.
            float deathInterpolant = ExplodeTimer > 0 ? Utils.GetLerpValue(0, 50, ExplodeTimer, true) : Utils.GetLerpValue(60, 10, Projectile.timeLeft, true);
            float shakeStrength = MathHelper.Lerp(0f, 6f, deathInterpolant);
            Vector2 drawPosition = Projectile.Center + Main.rand.NextVector2Circular(shakeStrength, shakeStrength) - Main.screenPosition;

            // Turn the entire projectile white while exploding.
            Color drawColor = Color.Lerp(FindColorForSoul((int)Projectile.ai[0]), Color.White, deathInterpolant);
            Color bloomColor = Color.Lerp(drawColor, Color.White, 0.25f);

            Main.spriteBatch.SetBlendState(BlendState.Additive);

            Main.EntitySpriteDraw(bloomCircleTexture, drawPosition, null, Projectile.GetAlpha(bloomColor) * 0.85f, Projectile.rotation, bloomCircleTexture.Size() / 2f, Projectile.scale * 0.65f, SpriteEffects.None);
            Main.EntitySpriteDraw(mainTexture, drawPosition, frame, Projectile.GetAlpha(drawColor), Projectile.rotation, frame.Size() / 2f, Projectile.scale, SpriteEffects.None);
            Main.EntitySpriteDraw(mainTexture, drawPosition, frame, Projectile.GetAlpha(Color.White), Projectile.rotation, frame.Size() / 2f, Projectile.scale * 0.8f, SpriteEffects.None);

            Main.spriteBatch.SetBlendState(BlendState.AlphaBlend);
            return false;
        }

        public float PlasmaBallWidthFunction(float completion, Vector2 vertexPos)
        {
            float deathInterpolant = ExplodeTimer > 0 ? Utils.GetLerpValue(0, 50, ExplodeTimer, true) : Utils.GetLerpValue(60, 10, Projectile.timeLeft, true);

            float width;
            float maxBodyWidth = Projectile.scale * 58f;
            float curveRatio = 0.15f;

            if (completion < curveRatio)
                width = MathF.Sin(completion / curveRatio * MathHelper.PiOver2) * maxBodyWidth + curveRatio;
            else
                width = Utils.Remap(completion, curveRatio, 1f, maxBodyWidth, 0f);

            return width * MathHelper.Lerp(1f, 0f, deathInterpolant);
        }

        public Color PlasmaBallColorFunction(float completion, Vector2 vertexPos)
        {
            Color bodyColor = Color.Lerp(Color.Transparent, FindColorForSoul((int)Projectile.ai[0]), Utils.GetLerpValue(0f, 0.35f, completion));
            Color endColor = Color.Lerp(bodyColor, FindColorForSoul((int)Projectile.ai[0]), Utils.GetLerpValue(0.35f, 1f, completion));

            // Change to white when the ball is about to explode.
            float deathInterpolant = ExplodeTimer > 0 ? Utils.GetLerpValue(0, 50, ExplodeTimer, true) : Utils.GetLerpValue(60, 10, Projectile.timeLeft, true);
            return Color.Lerp(endColor, Color.White, deathInterpolant);
        }

        public void RenderPixelatedPrimitives(SpriteBatch spriteBatch, GeneralDrawLayer layer)
        {
            GameShaders.Misc["CalamityMod:ImpFlameTrail"].SetShaderTexture(ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Trails/SylvestaffStreak"));
            PrimitiveRenderer.RenderTrail(Projectile.oldPos, new(PlasmaBallWidthFunction, PlasmaBallColorFunction, (_,_) => Projectile.Size * 0.5f, true, true, GameShaders.Misc["CalamityMod:ImpFlameTrail"]), Projectile.oldPos.Length * 2);
        }
    }
}
