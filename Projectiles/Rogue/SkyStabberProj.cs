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
            Projectile.width = 15;
            Projectile.height = 15;
            Projectile.friendly = true;
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.tileCollide = true;
            Projectile.penetrate = 20;
            Projectile.timeLeft = Lifetime;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
        }

        public override void AI()
        {
            Projectile.ai[0] += 1f;

            if (Projectile.ai[0] >= 90f)
            {
                Projectile.velocity.X *= 0.98f;
                Projectile.velocity.Y *= 0.98f;
            }
            else
            {
                Projectile.rotation += 0.3f * (float)Projectile.direction;
            }

            Player player = Main.player[Projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            if (modPlayer.killSpikyBalls)
            {
                Projectile.active = false;
                Projectile.netUpdate = true;
            }
        }

        // Makes the projectile bounce infinitely, as it stops mid-air anyway.
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
            if (Projectile.velocity.X != oldVelocity.X)
            {
                Projectile.velocity.X = -oldVelocity.X * 0.6f;
            }
            if (Projectile.velocity.Y != oldVelocity.Y)
            {
                Projectile.velocity.Y = -oldVelocity.Y * 0.6f;
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
            if (Projectile.Calamity().stealthStrike)
            {
                var source = Projectile.GetSource_FromThis();
                for (int n = 0; n < 4; n++)
                {
                    Projectile feather = CalamityUtils.ProjectileRain(source, targetPos, 400f, 100f, 500f, 800f, 20f, ModContent.ProjectileType<StickyFeatherAero>(), (int)(Projectile.damage * 0.25), Projectile.knockBack * 0.25f, Projectile.owner);
                    if (feather.whoAmI.WithinBounds(Main.maxProjectiles))
                        feather.DamageType = RogueDamageClass.Instance;
                }
            }
        }
    }
}
