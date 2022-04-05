using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
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
            Projectile.width = 16;
            Projectile.height = 22;
            Projectile.friendly = true;
            Projectile.timeLeft = 600;
            Projectile.penetrate = 3;
            Projectile.Calamity().rogue = true;
            Projectile.ignoreWater = true;
        }
        private void Explode()
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            if (Main.myPlayer == Projectile.owner)
            {
                Projectile.NewProjectile(Projectile.GetProjectileSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<BettyExplosion>(), Projectile.damage, 8f, Projectile.owner);
                if (Projectile.Calamity().stealthStrike)
                {
                    int projectileCount = 12;
                    for (int i = 0; i < projectileCount; i++)
                    {
                        if (Main.rand.NextBool(2))
                        {
                            Vector2 shrapnelVelocity = (Vector2.UnitY * Main.rand.NextFloat(-12f, -4f)).RotatedByRandom(MathHelper.ToRadians(30f));
                            Projectile.NewProjectile(Projectile.GetProjectileSource_FromThis(), Projectile.Center, Projectile.velocity + shrapnelVelocity, ModContent.ProjectileType<BouncingBettyShrapnel>(), (int)(Projectile.damage * 0.5f), 3f, Projectile.owner);
                        }
                        else
                        {
                            Vector2 fireVelocity = (Vector2.UnitY * Main.rand.NextFloat(-12f, -4f)).RotatedByRandom(MathHelper.ToRadians(40f));
                            Projectile fire = Projectile.NewProjectileDirect(Projectile.GetProjectileSource_FromThis(), Projectile.Center, Projectile.velocity + fireVelocity, ModContent.ProjectileType<TotalityFire>(), (int)(Projectile.damage * 0.6f), 1f, Projectile.owner);
                            fire.localNPCHitCooldown = 9;
                            fire.timeLeft = 240;
                        }
                    }
                }
            }
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Point tileCoords = Projectile.Bottom.ToTileCoordinates();
            if (Main.tile[tileCoords.X, tileCoords.Y + 1].HasUnactuatedTile &&
                WorldGen.SolidTile(Main.tile[tileCoords.X, tileCoords.Y + 1]) &&
                Projectile.timeLeft < 575)
            {
                Explode();
                Projectile.Kill();
            }
            else
            {
                Projectile.velocity.Y += 0.4f;
                if (Projectile.velocity.Y > 16f)
                    Projectile.velocity.Y = 16f;
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity *= -1f;
            return false;
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            Explode();
            Projectile.velocity *= -1f;
        }
    }
}
