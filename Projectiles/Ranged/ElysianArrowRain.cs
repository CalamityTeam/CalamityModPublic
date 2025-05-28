using System;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Graphics.Primitives;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class ElysianArrowRain : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public static int MaxUpdate = 7;
        private int Lifetime = 110;
        private static Color ShaderColorOne = Color.Khaki;
        private static Color ShaderColorTwo = Color.White;
        private static Color ShaderEndColor = Color.Orange;
        private Vector2 altSpawn;

        public override void SetStaticDefaults()
        {
            // While this projectile doesn't have afterimages, it keeps track of old positions for its primitive drawcode.
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 21;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 24;
            Projectile.arrow = true;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = Lifetime;
            Projectile.MaxUpdates = MaxUpdate;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override bool? CanDamage() => Projectile.numHits >= 1 ? false : null;

        public override void AI()
        {
            Projectile.ai[0]++;
            if (Projectile.ai[0] == 45)
                altSpawn = Projectile.Center;
            if (Projectile.timeLeft <= 5)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(9, 9) - Projectile.velocity * 5, Main.rand.NextBool() ? 262 : 87, (Projectile.velocity * 30) * Main.rand.NextFloat(0.1f, 0.95f));
                dust.noGravity = true;
                dust.scale = Main.rand.NextFloat(0.9f, 1.45f);
                dust.alpha = 235;
            }
            if (Projectile.timeLeft <= 80)
                Projectile.velocity *= 0.96f;
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 90);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {

            if (Projectile.damage > 1 && Projectile.numHits < 1)
                Projectile.damage = (int)(Projectile.damage * 0.8f);


            target.AddBuff(ModContent.BuffType<HolyFlames>(), 90);
            Projectile.timeLeft = 80;
        }

        private float PrimitiveWidthFunction(float completionRatio)
        {
            float arrowheadCutoff = 0.36f;
            float width = 24f;
            float minHeadWidth = 0.03f;
            float maxHeadWidth = width;
            if (completionRatio <= arrowheadCutoff)
                width = MathHelper.Lerp(minHeadWidth, maxHeadWidth, Utils.GetLerpValue(0f, arrowheadCutoff, completionRatio, true));
            return width;
        }

        private Color PrimitiveColorFunction(float completionRatio)
        {
            float endFadeRatio = 0.41f;

            float completionRatioFactor = 2.7f;
            float globalTimeFactor = 5.3f;
            float endFadeFactor = 3.2f;
            float endFadeTerm = Utils.GetLerpValue(0f, endFadeRatio * 0.5f, completionRatio, true) * endFadeFactor;
            float cosArgument = completionRatio * completionRatioFactor - Main.GlobalTimeWrappedHourly * globalTimeFactor + endFadeTerm;
            float startingInterpolant = (float)Math.Cos(cosArgument) * 0.5f + 0.5f;

            float colorLerpFactor = 0.6f;
            // Color 1, Color 2
            Color startingColor = Color.Lerp(ShaderColorOne, ShaderColorTwo, startingInterpolant * colorLerpFactor);
            // End Color
            return Color.Lerp(startingColor, ShaderEndColor, MathHelper.SmoothStep(0f, 1f, Utils.GetLerpValue(0f, endFadeRatio, completionRatio, true)));
        }

        public override bool PreDraw(ref Color lightColor)
        {
            GameShaders.Misc["CalamityMod:TrailStreak"].SetShaderTexture(ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Trails/SylvestaffStreak"));
            Vector2 overallOffset = Projectile.Size * 0.5f;
            overallOffset += Projectile.velocity * 1.4f;
            int numPoints = 46;
            PrimitiveRenderer.RenderTrail(Projectile.oldPos, new(PrimitiveWidthFunction, PrimitiveColorFunction, (_) => overallOffset, shader: GameShaders.Misc["CalamityMod:TrailStreak"]), numPoints);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            if (Projectile.ai[1] == 0)
            {
                Player Owner = Main.player[Projectile.owner];
                float targetDist = Vector2.Distance(Owner.Center, Projectile.Center);

                NPC target = Projectile.Center.ClosestNPCAt(2000);
                Vector2 targetPosition = Projectile.numHits == 1 ? (target == null ? Projectile.Center : target.Center) : altSpawn;
                Vector2 spawnSpot = (Projectile.numHits == 1 ? (target == null ? Projectile.Center : target.Center) : altSpawn) + new Vector2(Main.rand.NextFloat(-450, 450), Main.rand.NextFloat(750, 950));

                Vector2 velocity;
                if (target == null)
                    velocity = (targetPosition - spawnSpot).SafeNormalize(Vector2.UnitX) * 20;
                else
                    velocity = CalamityUtils.CalculatePredictiveAimToTargetMaxUpdates(spawnSpot, target, 20f, MaxUpdate);

                if (targetDist < 1400f)
                {
                    int Dusts = 8;
                    float radians = MathHelper.TwoPi / Dusts;
                    Vector2 spinningPoint = Vector2.Normalize(new Vector2(-1f, -1f));
                    for (int i = 0; i < Dusts; i++)
                    {
                        Vector2 dustVelocity = spinningPoint.RotatedBy(radians * i) * 3.5f;
                        Dust dust = Dust.NewDustPerfect(spawnSpot, Main.rand.NextBool() ? 262 : 87, dustVelocity, 0, default, 0.9f);
                        dust.noGravity = true;

                        Dust dust2 = Dust.NewDustPerfect(spawnSpot, Main.rand.NextBool() ? 262 : 87, dustVelocity * 0.6f, 0, default, 1.2f);
                        dust2.noGravity = true;
                    }
                }

                Projectile.NewProjectile(Projectile.GetSource_FromThis(), spawnSpot, velocity, ModContent.ProjectileType<ElysianArrowRain>(), Projectile.damage, 0f, Projectile.owner, 0f, 1f);

            }

            //SoundStyle onKill = new("CalamityMod/Sounds/Custom/ProfanedGuardians/GuardianDash");
            //SoundEngine.PlaySound(onKill with { Volume = 0.4f, Pitch = 0.4f }, Projectile.position);
        }
    }
}
