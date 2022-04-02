using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Rogue
{
    public class BouncingBettyProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bouncing Betty");
        }

        public override void SetDefaults()
        {
            projectile.width = 16;
            projectile.height = 22;
            projectile.friendly = true;
            projectile.timeLeft = 600;
            projectile.penetrate = 3;
            projectile.Calamity().rogue = true;
            projectile.ignoreWater = true;
        }
        private void Explode()
        {
            Main.PlaySound(SoundID.Item14, projectile.Center);
            if (Main.myPlayer == projectile.owner)
            {
                Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<BettyExplosion>(), projectile.damage, 8f, projectile.owner);
                if (projectile.Calamity().stealthStrike)
                {
                    int projectileCount = 12;
                    for (int i = 0; i < projectileCount; i++)
                    {
                        if (Main.rand.NextBool(2))
                        {
                            Vector2 shrapnelVelocity = (Vector2.UnitY * Main.rand.NextFloat(-12f, -4f)).RotatedByRandom(MathHelper.ToRadians(30f));
                            Projectile.NewProjectile(projectile.Center, projectile.velocity + shrapnelVelocity, ModContent.ProjectileType<BouncingBettyShrapnel>(), (int)(projectile.damage * 0.5f), 3f, projectile.owner);
                        }
                        else
                        {
                            Vector2 fireVelocity = (Vector2.UnitY * Main.rand.NextFloat(-12f, -4f)).RotatedByRandom(MathHelper.ToRadians(40f));
                            Projectile fire = Projectile.NewProjectileDirect(projectile.Center, projectile.velocity + fireVelocity, ModContent.ProjectileType<TotalityFire>(), (int)(projectile.damage * 0.6f), 1f, projectile.owner);
                            fire.localNPCHitCooldown = 9;
                            fire.timeLeft = 240;
                        }
                    }
                }
            }
        }
        public override void AI()
        {
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Point tileCoords = projectile.Bottom.ToTileCoordinates();
            if (Main.tile[tileCoords.X, tileCoords.Y + 1].nactive() &&
                WorldGen.SolidTile(Main.tile[tileCoords.X, tileCoords.Y + 1]) &&
                projectile.timeLeft < 575)
            {
                Explode();
                projectile.Kill();
            }
            else
            {
                projectile.velocity.Y += 0.4f;
                if (projectile.velocity.Y > 16f)
                    projectile.velocity.Y = 16f;
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            projectile.velocity *= -1f;
            return false;
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Main.PlaySound(SoundID.Item14, projectile.Center);
            Explode();
            projectile.velocity *= -1f;
        }
    }
}
