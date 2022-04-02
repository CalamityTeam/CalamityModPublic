using CalamityMod.Items.Weapons.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class PhantasmalSoul : ModProjectile
    {
        private const int Lifetime = 300;
        private const int NoHomingFrames = 40;
        private const int NoHitFrames = 10;
        private const int NoDrawFrames = 5;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Phantasmal Soul");
            Main.projFrames[projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 3;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 36;
            projectile.alpha = 100;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.Calamity().rogue = true;
            projectile.extraUpdates = 1;
            projectile.penetrate = 1;
            projectile.timeLeft = Lifetime;
        }

        public override bool? CanHitNPC(NPC target) => projectile.timeLeft < Lifetime - NoHomingFrames && target.CanBeChasedBy(projectile);

        public override void AI()
        {
            // Handle animation
            projectile.frameCounter++;
            if (projectile.frameCounter > 6)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame > 3)
                projectile.frame = 0;

            // Activate homing on enemies after a brief delay
            if (projectile.timeLeft < Lifetime - NoHomingFrames)
                projectile.ai[0] = 1f;

            // Occasional dust
            if (Main.rand.NextBool(4))
            {
                int dustID = 173;
                Dust d = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, dustID);
                d.velocity *= 0.1f;
                d.scale = 1.3f;
                d.noGravity = true;
                d.noLight = true;
            }

            // Purple light
            Lighting.AddLight(projectile.Center, 0.5f, 0.2f, 0.9f);

            // Make sure the soul is always facing forwards
            projectile.rotation = projectile.velocity.ToRotation() - MathHelper.PiOver2;

            float inertia = 40f * projectile.ai[1]; // ranges from 20 to 60
            float homingSpeed = 8f * projectile.ai[1]; // ranges from 4 to 12
            float playerHomingRange = 900f;

            Player owner = Main.player[projectile.owner];
            if (owner.active && !owner.dead)
            {
                // If you are too far away from the projectiles, they home in on you.
                if (projectile.Distance(Main.player[projectile.owner].Center) > playerHomingRange)
                {
                    Vector2 moveDirection = projectile.SafeDirectionTo(Main.player[projectile.owner].Center, Vector2.UnitY);
                    projectile.velocity = (projectile.velocity * (inertia - 1f) + moveDirection * homingSpeed) / inertia;
                    return;
                }

                // Otherwise, if homing on enemies is enabled, they home in on enemies.
                if (projectile.ai[0] == 1f)
                    CalamityGlobalProjectile.HomeInOnNPC(projectile, true, 600f, 10f, 20f);
            }

            // If the owner is dead these projectiles disappear rapidly.
            else if (projectile.timeLeft > 30)
                projectile.timeLeft = 30;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (projectile.timeLeft > Lifetime - NoDrawFrames)
                return false;

            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }

        // Get darker when about to disappear
        public override Color? GetAlpha(Color lightColor)
        {
            if (projectile.timeLeft < 85)
            {
                byte b2 = (byte)(projectile.timeLeft * 3);
                byte a2 = (byte)(100f * ((float)b2 / 255f));
                return new Color((int)b2, (int)b2, (int)b2, (int)a2);
            }
            return new Color(255, 255, 255, 100);
        }

        public override void Kill(int timeLeft)
        {
            int dustCount = 36;
            int dustID = 173;
            for (int i = 0; i < dustCount; i++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(1f, 1f);
                velocity = 12f * velocity + projectile.velocity * 0.1f;
                Dust d = Dust.NewDustDirect(projectile.Center, 0, 0, dustID);
                d.velocity = velocity;
                d.noGravity = true;
                d.noLight = true;
            }

            // Special quieter version of this noise used by Soul Edge and similar
            Main.PlaySound(SoulEdge.ProjectileDeathSound, projectile.Center);
        }

        // Cannot deal damage for the first several frames of existence.
        public override bool CanHitPvp(Player target) => projectile.timeLeft < Lifetime - NoHitFrames;
    }
}
