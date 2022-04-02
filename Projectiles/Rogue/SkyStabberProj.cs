using Terraria;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using CalamityMod.Projectiles.Typeless;
using CalamityMod.CalPlayer;

namespace CalamityMod.Projectiles.Rogue
{
    public class SkyStabberProj : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/SkyStabber";

        private static int Lifetime = 1200;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("SkyStabberProj");
        }

        public override void SetDefaults()
        {
            projectile.width = 15;
            projectile.height = 15;
            projectile.friendly = true;
            projectile.Calamity().rogue = true;
            projectile.tileCollide = true;
            projectile.penetrate = 20;
            projectile.timeLeft = Lifetime;

            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 30;
        }

        public override void AI()
        {
            projectile.ai[0] += 1f;

            if (projectile.ai[0] >= 90f)
            {
                projectile.velocity.X *= 0.98f;
                projectile.velocity.Y *= 0.98f;
            }
            else
            {
                projectile.rotation += 0.3f * (float)projectile.direction;
            }

            Player player = Main.player[projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            if (modPlayer.killSpikyBalls)
            {
                projectile.active = false;
                projectile.netUpdate = true;
            }
        }

        // Makes the projectile bounce infinitely, as it stops mid-air anyway.
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(projectile.position + projectile.velocity, projectile.velocity, projectile.width, projectile.height);
            if (projectile.velocity.X != oldVelocity.X)
            {
                projectile.velocity.X = -oldVelocity.X * 0.6f;
            }
            if (projectile.velocity.Y != oldVelocity.Y)
            {
                projectile.velocity.Y = -oldVelocity.Y * 0.6f;
            }
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            OnHitEffects(target.Center);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            OnHitEffects(target.Center);
        }

        private void OnHitEffects(Vector2 targetPos)
        {
            if (projectile.Calamity().stealthStrike)
            {
                for (int n = 0; n < 4; n++)
                {
                    Projectile feather = CalamityUtils.ProjectileRain(targetPos, 400f, 100f, 500f, 800f, 20f, ModContent.ProjectileType<StickyFeatherAero>(), (int)(projectile.damage * 0.25), projectile.knockBack * 0.25f, projectile.owner);
                    if (feather.whoAmI.WithinBounds(Main.maxProjectiles))
                        feather.Calamity().forceRogue = true;
                }
            }
        }
    }
}
