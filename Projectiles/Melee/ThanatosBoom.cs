using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Melee
{
    public class ThanatosBoom : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults() => DisplayName.SetDefault("Boom");

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 54;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.timeLeft = 45;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 4;
            projectile.melee = true;
        }

        public override void AI()
        {
            if (projectile.localAI[0] == 0f)
            {
                CreateExplosionDust();
                projectile.localAI[0] = 1f;
            }
        }

        public void CreateExplosionDust()
        {
            if (Main.dedServ)
                return;

            for (float speed = 2f; speed <= 6f; speed += 0.7f)
            {
                float lifePersistance = Main.rand.NextFloat(0.8f, 1.7f);
                for (int i = 0; i < 60; i++)
                {
                    Dust energy = Dust.NewDustPerfect(projectile.Center, 267);
                    energy.velocity = (MathHelper.TwoPi * i / 60f).ToRotationVector2() * speed;
                    energy.noGravity = true;
                    energy.color = Main.hslToRgb(Main.rand.NextFloat(), 0.7f, 0.625f);
                    energy.fadeIn = lifePersistance;
                    energy.scale = 1.4f;
                }
            }
        }
    }
}
