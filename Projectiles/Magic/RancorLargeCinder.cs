using CalamityMod.Graphics.Metaballs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class RancorLargeCinder : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public ref float Time => ref Projectile.ai[0];
        public ref float Lifetime => ref Projectile.ai[1];
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 36;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.friendly = true;
            Projectile.hostile = true;
            Projectile.timeLeft = 300;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            // Make a decision for the lifetime for the cinder if one has not yet been made.
            if (Lifetime == 0f)
            {
                Lifetime = Main.rand.Next(45, 150);
                Projectile.netUpdate = true;
            }

            // Calculate the scale of the cinder.
            else
            {
                Projectile.scale = Utils.GetLerpValue(0f, 20f, Time, true) * Utils.GetLerpValue(Lifetime, Lifetime - 20f, Time, true);
                Projectile.scale *= MathHelper.Lerp(0.5f, 1f, Projectile.identity % 6f / 6f);
            }

            // Fall down.
            Projectile.velocity.Y += 0.04f;

            // Create lava particles.
            RancorLavaMetaball.SpawnParticle(Projectile.Center + Main.rand.NextVector2Circular(6f, 6f), Projectile.scale * 36f);

            Time++;
        }

        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            modifiers.SourceDamage *= 0f;
            modifiers.SourceDamage.Flat += Main.rand.Next(80, 90);
        }

        public override Color? GetAlpha(Color lightColor) => Color.White * Projectile.Opacity;
    }
}
