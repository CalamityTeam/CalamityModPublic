using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class SandBlade : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
            Projectile.DamageType = DamageClass.Melee;
        }

        public override void AI()
        {
            Projectile.rotation += 0.5f;
            Projectile.ai[1] += 1f;
            if (Projectile.ai[1] <= 30f)
            {
                Projectile.velocity.X *= 0.925f;
                Projectile.velocity.Y *= 0.925f;
            }
            else if (Projectile.ai[1] > 30f && Projectile.ai[1] <= 59f)
            {
                Projectile.velocity.X *= 1.15f;
                Projectile.velocity.Y *= 1.15f;
            }
            else if (Projectile.ai[1] == 60f)
            {
                Projectile.ai[1] = 0f;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 2);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 159, 0f, 0f);
            }
        }
    }
}
