using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Rogue
{
    public class InkBombProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ink Bomb");
        }

        public override void SetDefaults()
        {
            projectile.width = 14;
            projectile.height = 22;
            projectile.friendly = true;
            projectile.alpha = 0;
            projectile.penetrate = 1;
            projectile.tileCollide = true;
            projectile.ignoreWater = true;
            projectile.timeLeft = 50;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            projectile.velocity.Y += 0.1f;
            projectile.rotation += projectile.velocity.X * 0.1f;

            if (projectile.timeLeft == 1)
                CreateInk();
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (!target.friendly)
                CreateInk();
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            CreateInk();
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            CreateInk();
            return true;
        }

        private void CreateInk()
        {
            Player player = Main.player[projectile.owner];
            Main.PlaySound(SoundID.Item14, projectile.position);
            for (int i = 0; i < 20; i++)
            {
                int projType = Main.rand.Next(0, 3);
                int inkType;
                switch (projType)
                {
                    case 0:
                        inkType = ModContent.ProjectileType<InkCloud>();
                        break;
                    case 1:
                        inkType = ModContent.ProjectileType<InkCloud2>();
                        break;
                    default:
                        inkType = ModContent.ProjectileType<InkCloud3>();
                        break;
                }
                int inkID = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-2f, 2f), inkType, (int)(22 * player.RogueDamage()), 7, projectile.owner);
                Main.projectile[inkID].timeLeft += Main.rand.Next(-20, 25);
            }
            projectile.Kill();
        }
    }
}
