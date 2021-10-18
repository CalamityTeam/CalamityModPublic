using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class RancorLargeCinder : ModProjectile
    {
        public ref float Time => ref projectile.ai[0];
        public ref float Lifetime => ref projectile.ai[1];
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        public override void SetStaticDefaults() => DisplayName.SetDefault("Lava Cinder");

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 36;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.magic = true;
            projectile.friendly = true;
            projectile.hostile = true;
            projectile.timeLeft = 300;
            projectile.ignoreWater = true;
        }

        public override void AI()
        {
            // Make a decision for the lifetime for the cinder if one has not yet been made.
            if (Lifetime == 0f)
            {
                Lifetime = Main.rand.Next(45, 150);
                projectile.netUpdate = true;
            }

            // Calculate the scale of the cinder.
            else
            {
                projectile.scale = Utils.InverseLerp(0f, 20f, Time, true) * Utils.InverseLerp(Lifetime, Lifetime - 20f, Time, true);
                projectile.scale *= MathHelper.Lerp(0.5f, 1f, projectile.identity % 6f / 6f);
            }

            // Fall down.
            projectile.velocity.Y += 0.04f;

            // Create lava particles.
            FusableParticleManager.GetParticleSetByType<RancorLavaParticleSet>().SpawnParticle(projectile.Center + Main.rand.NextVector2Circular(6f, 6f), projectile.scale * 36f);

            Time++;
        }

        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)
        {
            damage = Main.rand.Next(80, 90);
        }

        public override Color? GetAlpha(Color lightColor) => Color.White * projectile.Opacity;
    }
}
