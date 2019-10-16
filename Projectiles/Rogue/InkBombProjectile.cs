using Microsoft.Xna.Framework;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Projectiles
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
            Mod calamity = ModLoader.GetMod("CalamityMod");
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

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            CreateInk();
            return true;
        }

        private void CreateInk()
        {
            Main.PlaySound(2, (int)Main.player[Main.myPlayer].position.X, (int)Main.player[Main.myPlayer].position.Y, 14);
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
                int inkID = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-2f, 2f), inkType, 22, 7, Main.myPlayer);
                Main.projectile[inkID].timeLeft += Main.rand.Next(-20, 25);
            }
            projectile.Kill();
        }
    }
}
