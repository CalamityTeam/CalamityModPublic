using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Rogue
{
    public class SeafoamBombProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Seafoam Bomb");
        }

        public override void SetDefaults()
        {
            projectile.width = 14;
            projectile.height = 20;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 240;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            projectile.rotation += projectile.velocity.X * 0.1f;
            projectile.velocity.Y = projectile.velocity.Y + 0.15f;
            projectile.velocity.X = projectile.velocity.X * 0.99f;
        }

        public override void Kill(int timeLeft)
        {
            projectile.position = projectile.Center;
            projectile.width = projectile.height = projectile.Calamity().stealthStrike ? 256 : 128;
            projectile.position.X = projectile.position.X - (float)(projectile.width / 2);
            projectile.position.Y = projectile.position.Y - (float)(projectile.height / 2);
            projectile.maxPenetrate = -1;
            projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
            projectile.Damage();
            Main.PlaySound(2, (int)projectile.Center.X, (int)projectile.Center.Y, 14);

            for (int i = 0; i < (projectile.Calamity().stealthStrike ? 5 : 1); i++)
            {
                float posX = projectile.Center.X + (projectile.Calamity().stealthStrike ? Main.rand.Next(-50, 51) : 0);
                float posY = projectile.Center.Y + (projectile.Calamity().stealthStrike ? Main.rand.Next(-50, 51) : 0);
                Projectile.NewProjectile(posX, posY, 0f, 0f, ModContent.ProjectileType<SeafoamBubble>(), (int)((double)projectile.damage * 0.4), 0f, projectile.owner, 0f, 0f);
            }

			CalamityUtils.ExplosionGores(projectile, (projectile.Calamity().stealthStrike ? 6 : 3));
        }
    }
}
