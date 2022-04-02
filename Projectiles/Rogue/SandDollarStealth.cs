using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Rogue
{
    public class SandDollarStealth : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/SandDollar";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sand Dollar");
        }

        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 28;
            projectile.Calamity().rogue = true;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.aiStyle = 3;
            projectile.timeLeft = 300;
            aiType = ProjectileID.Bananarang;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            OnHitEffects();
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            OnHitEffects();
        }

        private void OnHitEffects()
        {
            int coralAmt = Main.rand.Next(1, 4);
            if (projectile.owner == Main.myPlayer && projectile.Calamity().stealthStrike)
            {
                for (int coralCount = 0; coralCount < coralAmt; coralCount++)
                {
                    Vector2 velocity = CalamityUtils.RandomVelocity(100f, 70f, 100f);
                    int coral = Projectile.NewProjectile(projectile.Center, velocity, ModContent.ProjectileType<SmallCoral>(), projectile.damage / 3, 0f, projectile.owner);
                    if (coral.WithinBounds(Main.maxProjectiles))
                        Main.projectile[coral].Calamity().forceRogue = true;
                }
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            projectile.ai[0] += 0.1f;
            if (projectile.velocity.X != oldVelocity.X)
            {
                projectile.velocity.X = -oldVelocity.X;
            }
            if (projectile.velocity.Y != oldVelocity.Y)
            {
                projectile.velocity.Y = -oldVelocity.Y;
            }
            return false;
        }
    }
}
