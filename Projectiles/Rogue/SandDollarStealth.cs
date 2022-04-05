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
            Projectile.width = 30;
            Projectile.height = 28;
            Projectile.Calamity().rogue = true;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.aiStyle = 3;
            Projectile.timeLeft = 300;
            aiType = ProjectileID.Bananarang;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
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
            if (Projectile.owner == Main.myPlayer && Projectile.Calamity().stealthStrike)
            {
                for (int coralCount = 0; coralCount < coralAmt; coralCount++)
                {
                    Vector2 velocity = CalamityUtils.RandomVelocity(100f, 70f, 100f);
                    int coral = Projectile.NewProjectile(Projectile.GetProjectileSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<SmallCoral>(), Projectile.damage / 3, 0f, Projectile.owner);
                    if (coral.WithinBounds(Main.maxProjectiles))
                        Main.projectile[coral].Calamity().forceRogue = true;
                }
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.ai[0] += 0.1f;
            if (Projectile.velocity.X != oldVelocity.X)
            {
                Projectile.velocity.X = -oldVelocity.X;
            }
            if (Projectile.velocity.Y != oldVelocity.Y)
            {
                Projectile.velocity.Y = -oldVelocity.Y;
            }
            return false;
        }
    }
}
