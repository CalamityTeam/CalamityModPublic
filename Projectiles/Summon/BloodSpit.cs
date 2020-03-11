using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class BloodSpit : ModProjectile
    {
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
            Lighting.AddLight(projectile.Center, (255 - projectile.alpha) * 0.77f / 255f, (255 - projectile.alpha) * 0.15f / 255f, (255 - projectile.alpha) * 0.08f / 255f);
            projectile.rotation = projectile.velocity.ToRotation() - MathHelper.PiOver2;
            projectile.frameCounter++;
            if (projectile.frameCounter > 4)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame >= Main.projFrames[projectile.type])
            {
                projectile.frame = 0;
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
            Main.player[projectile.owner].HealEffect(1, false);
            Main.player[projectile.owner].statLife += 1;
            if (Main.player[projectile.owner].statLife > Main.player[projectile.owner].statLifeMax2)
            {
                Main.player[projectile.owner].statLife = Main.player[projectile.owner].statLifeMax2;
            }
        }
    }
}
