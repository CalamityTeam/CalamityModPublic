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
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 6;
            Projectile.timeLeft = 25;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.alpha = 0;
        }

        public override void AI()
        {
            drawOffsetX = -5;
            drawOriginOffsetY = -1;
            drawOriginOffsetX = 0;
            Projectile.rotation = Projectile.velocity.ToRotation();

            // Slow down exponentially and fade away
            Projectile.velocity *= 0.93f;
            Projectile.alpha += 5;

            // Light which scales down as it fades
            float lightFactor = (255f - (float)Projectile.alpha) / 255f;
            Lighting.AddLight(Projectile.Center, 0.3f * lightFactor, 0.05f * lightFactor, 0.2f * lightFactor);

            // Spawn dust, with less dust as it fades away
            if (Main.rand.Next(256) > Projectile.alpha - 60)
            {
                int idx = Dust.NewDust(Projectile.Center, 1, 1, 71);
                Main.dust[idx].position = Projectile.Center - Projectile.velocity * 0.7f;
                Main.dust[idx].noGravity = true;
                Main.dust[idx].velocity *= 0.3f;
                Main.dust[idx].velocity += Projectile.velocity * 0.4f;
                Main.dust[idx].scale = Main.rand.NextFloat(0.5f, 1.0f);
                Main.dust[idx].alpha = Main.rand.Next(80, 200);
            }
        }
    }
}
