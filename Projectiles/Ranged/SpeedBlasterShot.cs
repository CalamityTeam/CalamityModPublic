using CalamityMod.Particles;
using CalamityMod.Items.Weapons.Ranged;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using System;

namespace CalamityMod.Projectiles.Ranged
{
    public class SpeedBlasterShot : ModProjectile, ILocalizedModType
    {
        public bool FirstFrameSettings = true;
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override string Texture => "CalamityMod/Projectiles/Ranged/SpeedBlasterShot";
        public ref float ColorValue => ref Main.player[Projectile.owner].Calamity().ColorValue;

        public static readonly SoundStyle ShotImpact = new("CalamityMod/Sounds/Item/SplatshotImpact") { PitchVariance = 0.3f, Volume = 2.5f };
        public static readonly SoundStyle ShotImpactBig = new("CalamityMod/Sounds/Item/SplatshotBigImpact") { PitchVariance = 0.3f, Volume = 4f };
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }
        public static int DashShot = 3; //used with projectile.ai[1] to fire the big shot
        public static int PostDashShot = 2; //used with projectile.ai[1] to fire the higher velocity post dash shots
        public bool FirstHit = true;
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.alpha = 0;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 1;
            Projectile.timeLeft = 300;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.Calamity().pointBlankShotDuration = CalamityGlobalProjectile.DefaultPointBlankDuration;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 50;
        }

        public override void AI()
        {
            ColorValue = Projectile.ai[0];
            if (Projectile.ai[1] == DashShot)
            {
                if (FirstFrameSettings)
                {
                    Projectile.penetrate = 3;
                    Projectile.extraUpdates = 28;
                    Projectile.timeLeft = 1500;
                    Projectile.Size *= 2f;
                    FirstFrameSettings = false;
                }
            }
            else if (Projectile.ai[1] == PostDashShot)
                Projectile.extraUpdates = 2;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Projectile.velocity *= (Projectile.ai[1] == DashShot ? 1f : Projectile.ai[1] == PostDashShot ? 0.985f : 0.97f);
            Projectile.velocity.Y += (Projectile.ai[1] == DashShot ? 0f : Projectile.ai[1] == PostDashShot ? 0.19f : 0.28f);
            Color ColorUsed = (Projectile.ai[0] == 0 ? Color.Aqua : Projectile.ai[0] == 1 ? Color.Blue : Projectile.ai[0] == 2 ? Color.Fuchsia : Projectile.ai[0] == 3 ? Color.Lime : Projectile.ai[0] == 4 ? Color.Yellow : Color.White);

            if (Main.rand.NextBool(20) && Projectile.ai[1] != DashShot)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, 192);
                dust.noLight = true;
                dust.noGravity = false;
                dust.scale = 1.2f;
                dust.velocity = new Vector2(Main.rand.Next(-1, 1), 3);
                dust.color = ColorUsed;
                dust.alpha = 75;
            }
            if (Projectile.ai[1] == DashShot)
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
            if (Projectile.timeLeft == 300 && Projectile.ai[1] == DashShot)
            {
                for (int i = 0; i <= 10; i++)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center, 192, Projectile.velocity.RotatedByRandom(MathHelper.ToRadians(30f)) * Main.rand.NextFloat(0.4f, 1.2f));
                    dust.noGravity = true;
                    dust.color = ColorUsed;
                    dust.alpha = 60;
                    dust.scale = Main.rand.NextFloat(1.2f, 2.3f);
                    dust.noLight = true;
                }
            }

        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Color ColorUsed = (Projectile.ai[0] == 0 ? Color.Aqua : Projectile.ai[0] == 1 ? Color.Blue : Projectile.ai[0] == 2 ? Color.Fuchsia : Projectile.ai[0] == 3 ? Color.Lime : Projectile.ai[0] == 4 ? Color.Yellow : Color.White);
            if (Projectile.ai[1] == DashShot)
            {
                if (FirstHit)
                {
                    FirstHit = false;
                    SoundEngine.PlaySound(ShotImpactBig, Projectile.position);
                }

                for (int i = 0; i < 2; i++)
                {
                    CritSpark spark = new CritSpark(Projectile.Center, Vector2.Zero, Color.White, ColorUsed, 5.5f, 7, 10f, 4.5f);
                    GeneralParticleHandler.SpawnParticle(spark);
                }
            }
        }
        public override void Kill(int timeLeft)
        {
            if (Projectile.ai[1] != DashShot)
                SoundEngine.PlaySound(ShotImpact, Projectile.position);
            
            for (int i = 0; i <= 8; i++)
            {
                Color ColorUsed = (Projectile.ai[0] == 0 ? Color.Aqua : Projectile.ai[0] == 1 ? Color.Blue : Projectile.ai[0] == 2 ? Color.Fuchsia : Projectile.ai[0] == 3 ? Color.Lime : Projectile.ai[0] == 4 ? Color.Yellow : Color.White);
                Dust dust = Dust.NewDustPerfect(Projectile.position, 192, Projectile.velocity.RotatedByRandom(MathHelper.ToRadians(20f)) * Main.rand.NextFloat(0.05f, 0.3f), 0, default, Main.rand.NextFloat(0.6f, 1.2f));
                dust.noLight = true;
                dust.noGravity = false;
                dust.color = ColorUsed;
                dust.alpha = 75;
            }
        }
        public override Color? GetAlpha(Color drawColor)
        {
            Color ColorUsed = (Projectile.ai[0] == 0 ? Color.Aqua : Projectile.ai[0] == 1 ? Color.Blue : Projectile.ai[0] == 2 ? Color.Fuchsia : Projectile.ai[0] == 3 ? Color.Lime : Projectile.ai[0] == 4 ? Color.Yellow : Color.White);
            Color lightColor = ColorUsed * drawColor.A;
            return lightColor * Projectile.Opacity;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 2);
            return false;
        }
    }
}
