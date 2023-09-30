using CalamityMod.Items.Weapons.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Rogue
{
    public class PhantasmalSoul : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        private const int Lifetime = 300;
        private const int NoHomingFrames = 35;
        private const int NoHitFrames = 10;
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
                int dustID = 173;
                Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, dustID);
                d.velocity *= 0.1f;
                d.scale = 1.3f;
                d.noGravity = true;
                d.noLight = true;
            }

            // Purple light
            Lighting.AddLight(Projectile.Center, 0.5f, 0.2f, 0.9f);

            // Make sure the soul is always facing forwards
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;

            float inertia = 40f * Projectile.ai[1]; // ranges from 20 to 60
            float homingSpeed = 8f * Projectile.ai[1]; // ranges from 4 to 12
            float playerHomingRange = 900f;

            Player owner = Main.player[Projectile.owner];
            if (owner.active && !owner.dead)
            {
                // If you are too far away from the projectiles, they home in on you.
                if (Projectile.Distance(Main.player[Projectile.owner].Center) > playerHomingRange)
                {
                    Vector2 moveDirection = Projectile.SafeDirectionTo(Main.player[Projectile.owner].Center, Vector2.UnitY);
                    Projectile.velocity = (Projectile.velocity * (inertia - 1f) + moveDirection * homingSpeed) / inertia;
                    return;
                }

                // Otherwise, if homing on enemies is enabled, they home in on enemies.
                if (Projectile.ai[0] == 1f)
                    CalamityUtils.HomeInOnNPC(Projectile, true, 600f, 10f, 20f);
            }

            // If the owner is dead these projectiles disappear rapidly.
            else if (Projectile.timeLeft > 30)
                Projectile.timeLeft = 30;
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
            int dustID = 173;
            for (int i = 0; i < dustCount; i++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(1f, 1f);
                velocity = 12f * velocity + Projectile.velocity * 0.1f;
                Dust d = Dust.NewDustDirect(Projectile.Center, 0, 0, dustID);
                d.velocity = velocity;
                d.noGravity = true;
                d.noLight = true;
            }

            // Special quieter version of this noise used by Soul Edge and similar
            SoundEngine.PlaySound(SoulEdge.ProjectileDeathSound, Projectile.Center);
        }

        // Cannot deal damage for the first several frames of existence.
        public override bool CanHitPvp(Player target) => Projectile.timeLeft < Lifetime - NoHitFrames;
    }
}
