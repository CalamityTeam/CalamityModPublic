using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class SilvaBurst : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Silva Burst");
        }

        public override void SetDefaults()
        {
            Projectile.width = 96;
            Projectile.height = 96;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.alpha = 255;
            Projectile.timeLeft = 2;
        }

        public override void AI()
        {
            // Create a bright burst of Silva light.
            float brightness = 1.6f;
            Lighting.AddLight(Projectile.Center, 0.27f * brightness, 0.82f * brightness, 0.157f * brightness);

            // Produce a bunch of dust.
            int dustID = 157;
            for (int d = 0; d < 3; d++)
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dustID, 0f, 0f, 100, new Color(Main.DiscoR, 203, 103), 1.5f);

            for (int d = 0; d < 30; d++)
            {
                int explode = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dustID, 0f, 0f, 0, new Color(Main.DiscoR, 203, 103), 2.5f);
                Main.dust[explode].noGravity = true;
                Main.dust[explode].velocity *= 3f;
                explode = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dustID, 0f, 0f, 100, new Color(Main.DiscoR, 203, 103), 1.5f);
                Main.dust[explode].velocity *= 2f;
                Main.dust[explode].noGravity = true;
            }
        }
    }
}
