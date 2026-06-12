using System;
using System.Linq;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Graphics.Primitives;
using CalamityMod.Particles;
using CalamityMod.Enums;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class DoGFire : ModProjectile, ILocalizedModType, IPixelatedPrimitiveRenderer
    {
        public bool IsAPassiveFireball
        {
            get => Projectile.ai[0] == 2f;
            set => Projectile.ai[0] = value.ToInt();
        }

        public new string LocalizationCategory => "Projectiles.Boss";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        public static readonly SoundStyle SpawnSound = new("CalamityMod/Sounds/Custom/DoGFireball");

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 10;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.width = 34;
            Projectile.height = 34;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 1;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 300;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 15;
            CooldownSlot = ImmunityCooldownID.Bosses;
        }

        public override void OnSpawn(IEntitySource source)
        {
            if (IsAPassiveFireball)
                SoundEngine.PlaySound(SpawnSound, Projectile.Center);
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Main.rand.NextBool(12))
            {
                for (int i = 0; i < 2; i++)
                {
                    Vector2 dustSpawnPosition = Projectile.Center + Main.rand.NextVector2Circular(6f, 6f);
                    Vector2 dustVelocity = Projectile.velocity * -1.2f;
                    float dustScale = Main.rand.NextFloat(0.6f, 0.8f) * Projectile.scale;
                    Color dustColor = Color.Lerp(Color.White, IsAPassiveFireball ? Color.SkyBlue : Color.Purple, Main.rand.NextFloat(0.5f, 1f));

                    Dust dust = Dust.NewDustDirect(dustSpawnPosition, 1, 1, DustID.TintableDustLighted, dustVelocity.X, dustVelocity.Y, 0, dustColor, dustScale);
                    dust.noGravity = true;
                    dust.noLight = false;
                    dust.noLightEmittence = false;
                }
            }

            if (Main.rand.NextBool(2))
            {
                for (int i = 0; i < 3; i++)
                {
                    Vector2 smokeVelocity = -Projectile.velocity * 0.7f + Main.rand.NextVector2Circular(1f, 1f) * 0.65f;
                    int smokeLifetime = Main.rand.Next(10, 15) * (Projectile.friendly ? 1 : 2);
                    float smokeScale = Main.rand.NextFloat(0.25f, 0.45f) * Projectile.scale;
                    float smokeOpacity = Main.rand.NextFloat(0.7f, 0.9f);
                    Color flameColor = Color.Lerp(Color.White, IsAPassiveFireball ? Color.SkyBlue : Color.Purple, Main.rand.NextFloat(0.5f, 1f));

                    HeavySmokeParticle ghastlySmoke = new(Projectile.Center + Main.rand.NextVector2Circular(8f, 8f), smokeVelocity, flameColor, smokeLifetime, smokeScale, smokeOpacity, 0.02f, true);
                    GeneralParticleHandler.SpawnParticle(ghastlySmoke);
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 360);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (info.Damage <= 0)
                return;

            target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), CalamityUtils.SecondsToFrames(5));
        }

        public override void OnKill(int timeLeft)
        {
            // Spawn a burst of dust at the center.
            for (int i = 0; i < 12; i++)
            {
                Vector2 dustVelocity = Main.rand.NextVector2Circular(1f, 1f) * 6f;
                float dustScale = Main.rand.NextFloat(3f, 5f);
                Color dustColor = Color.Lerp(Color.White, IsAPassiveFireball ? Color.SkyBlue : Color.Purple, Main.rand.NextFloat(0.5f, 1f));

                Dust dust = Dust.NewDustDirect(Projectile.Center, 1, 1, DustID.TintableDustLighted, dustVelocity.X, dustVelocity.Y, 0, dustColor, dustScale);
                dust.noGravity = true;
                dust.noLight = false;
                dust.noLightEmittence = false;
            }
            Projectile.Damage();
        }

        public override bool PreDraw(ref Color lightColor) => false;

        public float FireWidthFunction(float completion, Vector2 vertexPos)
        {
            float width;
            float maxBodyWidth = 72f * Projectile.scale;
            float curveRatio = 0.2f;

            // Crop the tip of the trail into a conic shape.
            if (completion < curveRatio)
                width = MathF.Pow(completion/curveRatio,0.5f) * maxBodyWidth;
            else
                width = Utils.Remap(completion, curveRatio, 1f, maxBodyWidth, 0f);

            // Pulse inwards and outwards over time.
            float pulseInterpolant = MathF.Cos(MathHelper.Pi * completion - Main.GlobalTimeWrappedHourly * 20f) * 0.5f + 0.5f;
            float additionalPulseWidth = MathHelper.Lerp(0f, 12f, pulseInterpolant);
            return width + additionalPulseWidth;
        }

        public Color FireColorFunction(float completion, Vector2 vertexPos)
        {
            Color mainColor = IsAPassiveFireball ? Color.Cyan : Color.Purple * 1.3f;
            Color endColor = Color.Lerp(mainColor, Color.Transparent, Utils.GetLerpValue(0.8f, 1f, completion, true));
            return Color.Lerp(mainColor, endColor, completion);
        }

        public float FireCoreWidthFunction(float completion, Vector2 vertexPos)
        {
            float width;
            float maxBodyWidth = Projectile.scale * (IsAPassiveFireball ? 24f : 64f);
            float curveRatio = 0.25f;

            if (completion < curveRatio)
                width = MathF.Sin(completion / curveRatio * MathHelper.PiOver2) * maxBodyWidth + curveRatio;
            else
                width = Utils.Remap(completion, curveRatio, 1f, maxBodyWidth, 0f);
            return width;
        }

        public Color FireCoreColorFunction(float completion, Vector2 vertexPos)
        {
            Color mainColor = IsAPassiveFireball ? Color.SkyBlue : Color.Fuchsia;
            Color tipColor = Color.Lerp(mainColor, Color.Transparent, Utils.GetLerpValue(0.8f, 1f, completion, true));
            Color fullBodyColor = Color.Lerp(mainColor, tipColor, completion);
            return Color.Lerp(fullBodyColor, Color.White, 0.175f);
        }

        public void RenderPixelatedPrimitives(SpriteBatch spriteBatch, GeneralDrawLayer layer)
        {
            // Render the main trail for the body for the flame.
            GameShaders.Misc["CalamityMod:ImpFlameTrail"].SetShaderTexture(ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Trails/ScarletDevilStreak"));
            PrimitiveRenderer.RenderTrail(Projectile.oldPos, new(FireWidthFunction, FireColorFunction, (_,_) => Projectile.Size * 0.5f + Projectile.velocity.SafeNormalize(Vector2.Zero)*24f, true, true, GameShaders.Misc["CalamityMod:ImpFlameTrail"]), Projectile.oldPos.Length + 12);

            // Render a smaller, pure white trail in the same position to represent the glowing core of the flame.
            int coreLength = IsAPassiveFireball ? 6 : 7;
            GameShaders.Misc["CalamityMod:ImpFlameTrail"].SetShaderTexture(ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Trails/SylvestaffStreak"));
            PrimitiveRenderer.RenderTrail(Projectile.oldPos[..coreLength], new(FireCoreWidthFunction, FireCoreColorFunction, (_,_) => Projectile.Size * 0.5f + Projectile.velocity.SafeNormalize(Vector2.Zero) * 24f, true, true, GameShaders.Misc["CalamityMod:ImpFlameTrail"]), coreLength + 8);
        }
    }
}
