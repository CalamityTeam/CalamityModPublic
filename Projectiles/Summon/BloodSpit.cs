using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class BloodSpit : ModProjectile
    {
        public const int OnDeathHealValue = 1;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spit");
            Main.projFrames[projectile.type] = 3;
            ProjectileID.Sets.MinionShot[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 8;
            projectile.height = 8;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.minion = true;
            projectile.minionSlots = 0f;
            projectile.timeLeft = 150;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 6;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, projectile.Opacity * 0.77f, projectile.Opacity * 0.15f, projectile.Opacity * 0.08f);
            projectile.rotation = projectile.velocity.ToRotation() - MathHelper.PiOver2;
            if (projectile.frameCounter++ > 4)
            {
                projectile.frame = (projectile.frame + 1) % Main.projFrames[projectile.type];
                projectile.frameCounter = 0;
            }
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 15; i++)
            {
                Dust dust = Dust.NewDustDirect(projectile.position + projectile.velocity, projectile.width, projectile.height, 5, projectile.velocity.X * 0.1f, projectile.velocity.Y * 0.1f);
                dust.velocity = Utils.NextVector2Unit(Main.rand) * Main.rand.NextFloat(1f, 2f);
                dust.noGravity = true;
            }

            Main.player[projectile.owner].HealEffect(OnDeathHealValue, false);
            Main.player[projectile.owner].statLife += OnDeathHealValue;
            if (Main.player[projectile.owner].statLife > Main.player[projectile.owner].statLifeMax2)
                Main.player[projectile.owner].statLife = Main.player[projectile.owner].statLifeMax2;
        }
    }
}
