using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class GammaCanister : ModProjectile
    {
        public const float Gravity = 0.2f;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gamma Canister");
            ProjectileID.Sets.MinionShot[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 28;
            projectile.friendly = true;
            projectile.minion = true;
            projectile.minionSlots = 0f;
            projectile.alpha = 255;
            projectile.timeLeft = 300;
            projectile.tileCollide = true;
            projectile.ignoreWater = true;
        }
        public override void AI()
        {
            NPC potentialTarget = projectile.Center.MinionHoming(1450f, Main.player[projectile.owner]);
            if (projectile.timeLeft >= 180)
                projectile.velocity.Y += Gravity;

            else if (potentialTarget != null)
            {
                // This looks quite stupid but the weapon is going to be useless otherwise and I'm not reworking this thing a 3rd time.
                projectile.velocity = Vector2.Lerp(projectile.velocity, projectile.DirectionTo(potentialTarget.Center) * 18f, 0.18f);
            }
            projectile.rotation = projectile.velocity.ToRotation() - MathHelper.PiOver2;
            projectile.alpha = Utils.Clamp(projectile.alpha - 22, 0, 255);
        }

        public override void Kill(int timeLeft)
        {
            if (Main.myPlayer != projectile.owner)
                return;

            for (int i = 0; i < 12; i++)
            {
                Vector2 shootVelocity = (MathHelper.TwoPi * i / 12f).ToRotationVector2() * Main.rand.NextFloat(6f, 17f);
                Projectile.NewProjectile(projectile.Center + shootVelocity * 2f, shootVelocity, ModContent.ProjectileType<HomingGammaBullet>(), projectile.damage / 2, projectile.knockBack * 0.4f, projectile.owner);
            }
        }
    }
}
