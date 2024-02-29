using CalamityMod.Graphics.Metaballs;
using CalamityMod.Graphics.Primitives;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class SpeedBlasterShot : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";

        public bool DashShot => Projectile.ai[1] == 3f; // the big shot
        public bool PostDashShot => Projectile.ai[1] == 2f; // the higher velocity post dash shots

        public static readonly SoundStyle ShotImpact = new("CalamityMod/Sounds/Item/SplatshotImpact") { PitchVariance = 0.3f, Volume = 2.5f };
        public static readonly SoundStyle ShotImpactBig = new("CalamityMod/Sounds/Item/SplatshotBigImpact") { PitchVariance = 0.3f, Volume = 4f };

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.MaxUpdates = 2;
            Projectile.timeLeft = 150 * Projectile.MaxUpdates;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.Calamity().pointBlankShotDuration = CalamityGlobalProjectile.DefaultPointBlankDuration;
        }

        public override void AI()
        {
            if (Projectile.ai[2] != 1f)
            {
                if (DashShot)
                {
                    Projectile.scale = 2f;
                    Projectile.penetrate = 4;
                    Projectile.MaxUpdates = 30;
                    Projectile.usesLocalNPCImmunity = true;
                    Projectile.localNPCHitCooldown = -1;
                    Projectile.timeLeft = 60 * Projectile.MaxUpdates;
                }
                else if (PostDashShot)
                    Projectile.MaxUpdates = 3;

                Projectile.ai[2] = 1f;
            }

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Projectile.velocity *= (DashShot ? 1f : PostDashShot ? 0.985f : 0.97f);
            Projectile.velocity.Y += (DashShot ? 0f : PostDashShot ? 0.15f : 0.25f);
            Color ColorUsed = GetColor(Projectile.ai[0]);

            if (Main.rand.NextBool(20) && !DashShot)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, 192);
                dust.noLight = true;
                dust.noGravity = false;
                dust.scale = 1.2f;
                dust.velocity = new Vector2(Main.rand.Next(-1, 1), 3);
                dust.color = ColorUsed;
                dust.alpha = 75;
            }
            if (DashShot)
            {
                Dust dust2 = Dust.NewDustPerfect(Projectile.Center + new Vector2(Main.rand.Next(-5, 5), Main.rand.Next(-5, 5)), 279);
                dust2.noLightEmittence = true;
                dust2.noGravity = true;
                dust2.scale = Main.rand.NextFloat(2.5f, 3.8f);
                dust2.velocity = Vector2.Zero;
                dust2.color = ColorUsed;
                dust2.alpha = 15;

                for (int i = 0; i <= 2; i++)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center, 192);
                    dust.noLight = true;
                    dust.noGravity = false;
                    dust.scale = Main.rand.NextFloat(1.3f, 1.5f);
                    dust.velocity = new Vector2(Main.rand.Next(-1, 1), Main.rand.Next(0, 8)).RotatedByRandom(MathHelper.ToRadians(10f)) * Main.rand.NextFloat(0.05f, 0.3f);
                    dust.color = ColorUsed;
                    dust.alpha = Main.rand.Next(145, 240);
                }
            }
            if (Projectile.timeLeft == 300 && DashShot)
            {
                for (int i = 0; i <= 10; i++)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center, 192, Projectile.velocity.RotatedByRandom(MathHelper.ToRadians(30f)) * Main.rand.NextFloat(0.4f, 1.2f));
                    dust.noGravity = true;
                    dust.color = ColorUsed;
                    dust.alpha = Main.rand.Next(40, 90);
                    dust.scale = Main.rand.NextFloat(1.2f, 2.3f);
                    dust.noLight = true;
                }
            }
            if (Projectile.timeLeft == 300 && !DashShot)
            {
                for (int i = 0; i <= 7; i++)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center, 192, Projectile.velocity.RotatedByRandom(MathHelper.ToRadians(PostDashShot ? 13f : 23f)) * Main.rand.NextFloat(0.4f, 1.2f));
                    dust.noGravity = true;
                    dust.color = ColorUsed;
                    dust.alpha = Main.rand.Next(40, 90);
                    dust.scale = Main.rand.NextFloat(0.7f, 1.6f);
                    dust.noLight = true;
                }
            }

        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Color ColorUsed = GetColor(Projectile.ai[0]);
            if (DashShot)
            {
                if (Projectile.numHits == 0)
                    SoundEngine.PlaySound(ShotImpactBig, Projectile.position);

                for (int i = 0; i < 2; i++)
                {
                    CritSpark spark = new CritSpark(Projectile.Center, Vector2.Zero, Color.White, ColorUsed, 5.5f, 7, 10f, 4.5f);
                    GeneralParticleHandler.SpawnParticle(spark);
                }
            }
        }
        public override void OnKill(int timeLeft)
        {
            if (!DashShot)
                SoundEngine.PlaySound(ShotImpact, Projectile.position);

            for (int i = 0; i <= 8; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.position, 192, Projectile.velocity.RotatedByRandom(MathHelper.ToRadians(20f)) * Main.rand.NextFloat(0.05f, 0.3f), 0, default, Main.rand.NextFloat(0.6f, 1.2f));
                dust.noLight = true;
                dust.noGravity = false;
                dust.color = GetColor(Projectile.ai[0]);
                dust.alpha = 75;
            }

            for (int i = 0; i < 3; i++)
            {
                Vector2 paintPos = Projectile.Center + Main.rand.NextVector2Circular(12f, 12f) + (Projectile.velocity.SafeNormalize(Vector2.UnitY)).RotatedByRandom(MathHelper.ToRadians(30f)) * Main.rand.NextFloat(4f, 20f);
                float paintSize = Main.rand.NextFloat(60f, 100f);
                switch (Projectile.ai[0])
                {
                    case 0:
                    default:
                        ModContent.GetInstance<CyanPaint>().SpawnParticle(paintPos, paintSize);
                        break;
                    case 1:
                        ModContent.GetInstance<BluePaint>().SpawnParticle(paintPos, paintSize);
                        break;
                    case 2:
                        ModContent.GetInstance<MagentaPaint>().SpawnParticle(paintPos, paintSize);
                        break;
                    case 3:
                        ModContent.GetInstance<LimePaint>().SpawnParticle(paintPos, paintSize);
                        break;
                    case 4:
                        ModContent.GetInstance<YellowPaint>().SpawnParticle(paintPos, paintSize);
                        break;
                }
            }
        }

        public static Color GetColor(float type) => type == 0 ? Color.Aqua : type == 1 ? Color.Blue : type == 2 ? Color.Fuchsia : type == 3 ? Color.Lime : Color.Yellow;

        public override Color? GetAlpha(Color drawColor) => GetColor(Projectile.ai[0]) * drawColor.A * Projectile.Opacity;
        internal float WidthFunction(float completionRatio) => (1f - completionRatio) * Projectile.scale * 6f;
        internal Color ColorFunction(float completionRatio) => GetColor(Projectile.ai[0]) * Projectile.Opacity;
        public override bool PreDraw(ref Color lightColor)
        {
            PrimitiveSet.Prepare(Projectile.oldPos, new(WidthFunction, ColorFunction, (_) => Projectile.Size * 0.5f), 20);
            return true;
        }
    }
}
