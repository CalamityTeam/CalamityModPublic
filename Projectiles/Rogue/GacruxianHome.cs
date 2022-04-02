using CalamityMod.Dusts;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class GacruxianHome : ModProjectile
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
            projectile.aiStyle = 18;
            projectile.friendly = true;
            projectile.Calamity().rogue = true;
            projectile.penetrate = 3;
            projectile.timeLeft = 120;
            projectile.ignoreWater = true;
            aiType = ProjectileID.DeathSickle;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 5;
        }

        public override void AI()
        {
            if (Main.rand.NextBool(4))
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, ModContent.DustType<AstralOrange>(), projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
            }
            if (projectile.timeLeft % 30 == 0 && projectile.owner == Main.myPlayer)
            {
                int proj = Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<UltimusCleaverDust>(), (int)(projectile.damage * 0.5), projectile.knockBack, projectile.owner);
                if (proj.WithinBounds(Main.maxProjectiles))
                    Main.projectile[proj].Calamity().forceRogue = true;
            }
            CalamityGlobalProjectile.HomeInOnNPC(projectile, !projectile.tileCollide, 250f, 12f, 20f);
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
