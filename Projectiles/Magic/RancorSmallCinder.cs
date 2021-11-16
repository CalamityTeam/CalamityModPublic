using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class RancorSmallCinder : ModProjectile
    {
        public ref float Time => ref projectile.ai[0];
        public ref float Lifetime => ref projectile.ai[1];
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cinder");
            Main.projFrames[projectile.type] = 3;
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 4;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.magic = true;
            projectile.timeLeft = 300;
            projectile.ignoreWater = true;
        }

        public override void AI()
        {
            // Decide a frame to use on the first frame this projectile exists.
            if (projectile.localAI[0] == 0f)
            {
                projectile.frame = Main.rand.Next(Main.projFrames[projectile.type]);
                projectile.localAI[0] = 1f;
            }

            // Make a decision for the lifetime for the cinder if one has not yet been made.
            if (Lifetime == 0f)
            {
                Lifetime = Main.rand.Next(60, 210);
                projectile.netUpdate = true;
            }

            // Calculate scale of the cinder.
            else
            {
                projectile.scale = Utils.InverseLerp(0f, 20f, Time, true) * Utils.InverseLerp(Lifetime, Lifetime - 20f, Time, true);
                projectile.scale *= MathHelper.Lerp(0.5f, 1f, projectile.identity % 6f / 6f);
            }

            if (Time >= Lifetime)
                projectile.Kill();

            Time++;
        }

        public override Color? GetAlpha(Color lightColor) => Color.White * projectile.Opacity;
    }
}
