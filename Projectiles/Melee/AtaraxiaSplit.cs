using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Melee
{
    public class AtaraxiaSplit : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Still Not Exoblade");
        }

        public override void SetDefaults()
        {
            projectile.width = 8;
            projectile.height = 8;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.penetrate = -1;
            projectile.usesIDStaticNPCImmunity = true;
            projectile.idStaticNPCHitCooldown = 6;
            projectile.timeLeft = 25;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.alpha = 0;
        }

        public override void AI()
        {
            drawOffsetX = -5;
            drawOriginOffsetY = -1;
            drawOriginOffsetX = 0;
            projectile.rotation = projectile.velocity.ToRotation();

            // Slow down exponentially and fade away
            projectile.velocity *= 0.93f;
            projectile.alpha += 5;

            // Light which scales down as it fades
            float lightFactor = (255f - (float)projectile.alpha) / 255f;
            Lighting.AddLight(projectile.Center, 0.3f * lightFactor, 0.05f * lightFactor, 0.2f * lightFactor);

            // Spawn dust, with less dust as it fades away
            if (Main.rand.Next(256) > projectile.alpha - 60)
            {
                int idx = Dust.NewDust(projectile.Center, 1, 1, 71);
                Main.dust[idx].position = projectile.Center - projectile.velocity * 0.7f;
                Main.dust[idx].noGravity = true;
                Main.dust[idx].velocity *= 0.3f;
                Main.dust[idx].velocity += projectile.velocity * 0.4f;
                Main.dust[idx].scale = Main.rand.NextFloat(0.5f, 1.0f);
                Main.dust[idx].alpha = Main.rand.Next(80, 200);
            }
        }
    }
}
