using CalamityMod.Particles;
using CalamityMod.Items.Weapons.Ranged;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Ranged
{
    public class SpeedBlasterShot : ModProjectile, ILocalizedModType
    {
        public bool FirstFrameNoDraw = true;
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override string Texture => "CalamityMod/Projectiles/Ranged/SpeedBlasterShot";

        public static readonly SoundStyle ShotImpact = new("CalamityMod/Sounds/Item/SplatshotImpact") { PitchVariance = 0.3f, Volume = 2.0f };
        public static readonly SoundStyle ShotImpactBig = new("CalamityMod/Sounds/Item/SplatshotBigImpact") { PitchVariance = 0.3f, Volume = 2.0f };
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }
        public int DashShot = 3; //used with projectile.ai[1] to fire the big shot
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
            if (Projectile.ai[1] == DashShot)
                Projectile.extraUpdates = 2;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Projectile.velocity *= (Projectile.ai[1] == DashShot ? 1f : 0.97f);
            Projectile.velocity.Y += (Projectile.ai[1] == DashShot ? 0f : 0.28f);
            Color ColorUsed = (Projectile.ai[0] == 0 ? Color.Aqua : Projectile.ai[0] == 1 ? Color.Blue : Projectile.ai[0] == 2 ? Color.Fuchsia : Projectile.ai[0] == 3 ? Color.Lime : Projectile.ai[0] == 4 ? Color.Yellow : Color.White);

            if (Main.rand.NextBool(20))
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, 192);
                dust.noLight = true;
                dust.noGravity = false;
                dust.scale = 1.2f;
                dust.velocity = new Vector2(Main.rand.Next(-1, 1), 3);
                dust.color = ColorUsed;
                dust.alpha = 75;
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            //target.AddBuff(BuffID.OnFire, 60);
        }
        public override void Kill(int timeLeft)
        {
            if (Projectile.ai[1] == DashShot)
                SoundEngine.PlaySound(ShotImpactBig, Projectile.position);
            else
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
