using CalamityMod.Dusts;
using CalamityMod.Projectiles.Melee;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
	public class GacruxianHome : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Fishing/AstralCatches/GacruxianMollusk";

        private int stealthTrailTimer = 10;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mollusk");
        }

        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.aiStyle = 18;
            projectile.friendly = true;
            projectile.Calamity().rogue = true;
            projectile.penetrate = 5;
            projectile.timeLeft = 180;
            projectile.ignoreWater = true;
            aiType = ProjectileID.DeathSickle;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 5;
        }

        public override void AI()
        {
            stealthTrailTimer--;
            if (Main.rand.NextBool(4))
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, ModContent.DustType<AstralOrange>(), projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
            }
            if (stealthTrailTimer == 0 && projectile.owner == Main.myPlayer)
            {
                int proj = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, projectile.velocity.X * 0f, projectile.velocity.Y * 0f, ModContent.ProjectileType<UltimusCleaverDust>(), (int)((double)projectile.damage * 0.5), projectile.knockBack, projectile.owner, 0f, 0f);
                Main.projectile[proj].Calamity().forceRogue = true;
                Main.projectile[proj].localNPCHitCooldown = 10;
                Main.projectile[proj].penetrate = 3;
                stealthTrailTimer = 10;
            }
			CalamityGlobalProjectile.HomeInOnNPC(projectile, false, 500f, 16f, 20f);
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i <= 10; i++)
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, ModContent.DustType<AstralOrange>(), projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
            }
        }
    }
}
