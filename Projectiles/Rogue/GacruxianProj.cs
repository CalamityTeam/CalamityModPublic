using CalamityMod.Dusts;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class GacruxianProj : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Fishing/AstralCatches/GacruxianMollusk";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mollusk");
        }

        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.penetrate = 1;
            projectile.aiStyle = 113;
            projectile.timeLeft = 600;
            aiType = ProjectileID.BoneJavelin;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            if (Main.rand.NextBool(4))
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, ModContent.DustType<AstralOrange>(), projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
            }
            if (projectile.timeLeft % 15 == 0)
            {
                if (projectile.Calamity().stealthStrike)
                {
                    Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<GacruxianHome>(), (int)(projectile.damage * 0.3), projectile.knockBack, projectile.owner);
                }
                else
                {
                    int proj = Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<UltimusCleaverDust>(), (int)(projectile.damage * 0.75), projectile.knockBack, projectile.owner);
                    if (proj.WithinBounds(Main.maxProjectiles))
                        Main.projectile[proj].Calamity().forceRogue = true;
                }
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
