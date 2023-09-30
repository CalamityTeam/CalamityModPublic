using CalamityMod.Items.Weapons.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Rogue
{
    public class PhantasmalSoulBlue : ModProjectile, ILocalizedModType
    {
        public static readonly SoundStyle HitSound = new("CalamityMod/Sounds/Item/PhantomSpirit") { Volume = 0.2f };
        public new string LocalizationCategory => "Projectiles.Rogue";
        private const int Lifetime = 300;
        private const int NoHomingFrames = 45;
        private const int NoHitFrames = 20;
        private const int NoDrawFrames = 5;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 3;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 36;
            Projectile.alpha = 100;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.extraUpdates = 1;
            Projectile.penetrate = 1;
            Projectile.timeLeft = Lifetime;
        }

        public override bool? CanHitNPC(NPC target) => Projectile.timeLeft < Lifetime - NoHomingFrames && target.CanBeChasedBy(Projectile);

        public override void AI()
        {
            // Handle animation
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 6)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame > 3)
                Projectile.frame = 0;

            // Activate homing on enemies after a brief delay
            if (Projectile.timeLeft < Lifetime - NoHomingFrames)
                Projectile.ai[0] = 1f;

            // Occasional dust
            if (Main.rand.NextBool(4))
            {
                int dustID = 180;
                Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, dustID);
                d.velocity *= 0.1f;
                d.scale = 1.3f;
                d.noGravity = true;
                d.noLight = true;
            }

            // Blue light
            Lighting.AddLight(Projectile.Center, 0.2f, 0.2f, 0.7f);

            // Make sure the soul is always facing forwards
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;

            if (Projectile.ai[0] == 1f)
                CalamityUtils.HomeInOnNPC(Projectile, true, 900f, 15f, 20f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.timeLeft > Lifetime - NoDrawFrames)
                return false;

            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }

        // Get darker when about to disappear
        public override Color? GetAlpha(Color lightColor)
        {
            if (Projectile.timeLeft < 85)
            {
                byte b2 = (byte)(Projectile.timeLeft * 3);
                byte a2 = (byte)(100f * ((float)b2 / 255f));
                return new Color((int)b2, (int)b2, (int)b2, (int)a2);
            }
            return new Color(255, 255, 255, 100);
        }

        public override void OnKill(int timeLeft)
        {
            int dustCount = 36;
            int dustID = 180;
            for (int i = 0; i < dustCount; i++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(1f, 1f);
                velocity = 12f * velocity + Projectile.velocity * 0.1f;
                Dust d = Dust.NewDustDirect(Projectile.Center, 0, 0, dustID);
                d.velocity = velocity;
                d.noGravity = true;
                d.noLight = true;
            }

            SoundEngine.PlaySound(HitSound with { PitchVariance = 0.4f }, Projectile.position);
        }

        // Cannot deal damage for the first several frames of existence.
        public override bool CanHitPvp(Player target) => Projectile.timeLeft < Lifetime - NoHitFrames;
    }
}
