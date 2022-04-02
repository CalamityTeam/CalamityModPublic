using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class SwirlingFire : ModProjectile
    {
        public ref float AngularTurnSpeed => ref projectile.ai[0];
        public ref float Time => ref projectile.ai[1];
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fire");
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 10;
            projectile.hostile = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.alpha = 255;
            projectile.penetrate = -1;
            projectile.timeLeft = 180;
            cooldownSlot = 1;
        }

        public override void AI()
        {
            Vector2 fireVelocity = (Time / 6f).ToRotationVector2() * Main.rand.NextFloat(1.7f, 2.2f);
            Dust fire = Dust.NewDustPerfect(projectile.Center + Main.rand.NextVector2Circular(3f, 3f), Main.dayTime ? 6 : 267);
            fire.velocity = fireVelocity.RotatedBy(MathHelper.PiOver2);
            fire.scale = Main.rand.NextFloat(1.3f, 1.45f);
            fire.noGravity = true;
            if (!Main.dayTime)
                fire.color = Color.Lerp(Color.Cyan, Color.BlueViolet, Main.rand.NextFloat());

            Dust.CloneDust(fire).velocity = fireVelocity.RotatedBy(-MathHelper.PiOver2);

            projectile.velocity = projectile.velocity.RotatedBy(AngularTurnSpeed);
            Time++;
        }
    }
}
