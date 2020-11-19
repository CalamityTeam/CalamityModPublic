using CalamityMod.Dusts;
using CalamityMod.Projectiles.Melee;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class GacruxianProj : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Fishing/AstralCatches/GacruxianMollusk";

        private int sparkTrailTimer = 10;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mollusk");
        }

        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.aiStyle = 113;
            projectile.timeLeft = 600;
            aiType = ProjectileID.BoneJavelin;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            sparkTrailTimer--;
            if (Main.rand.NextBool(4))
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, ModContent.DustType<AstralOrange>(), projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
            }
            if (sparkTrailTimer == 0)
            {
                if (projectile.Calamity().stealthStrike == true)
                {
                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, projectile.velocity.X * 0f, projectile.velocity.Y * 0f, ModContent.ProjectileType<GacruxianHome>(), (int)((double)projectile.damage * 0.3), projectile.knockBack, projectile.owner, 0f, 0f);
                }
                else
                {
                    int proj = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, projectile.velocity.X * 0f, projectile.velocity.Y * 0f, ModContent.ProjectileType<UltimusCleaverDust>(), (int)((double)projectile.damage * 0.75), projectile.knockBack, projectile.owner, 0f, 0f);
                    Main.projectile[proj].Calamity().forceRogue = true;
                    Main.projectile[proj].localNPCHitCooldown = 10;
                    Main.projectile[proj].penetrate = 3;
                }
                sparkTrailTimer = 10;
            }
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
