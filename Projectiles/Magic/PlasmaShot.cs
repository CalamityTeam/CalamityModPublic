using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class PlasmaShot : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shot");
        }

        public override void SetDefaults()
        {
            projectile.width = 6;
            projectile.height = 6;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.penetrate = 1;
            projectile.alpha = 255;
            projectile.timeLeft = 600;
            projectile.extraUpdates = 10;
        }

        public override void AI()
        {
            projectile.ai[0] += 1f;
            if (projectile.ai[0] > 6f)
            {
                for (int d = 0; d < 10; d++)
                {
                    Dust dust = Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, 107, projectile.velocity.X, projectile.velocity.Y, 100, default, 1f)];
                    dust.velocity = Vector2.Zero;
                    dust.position -= projectile.velocity / 5f * d;
                    dust.noGravity = true;
                    dust.noLight = true;
                }
            }
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item14, projectile.position);
            int cloudAmt = Main.rand.Next(20, 31);
            if (projectile.owner == Main.myPlayer)
            {
                for (int i = 0; i < cloudAmt; i++)
                {
                    Vector2 velocity = CalamityUtils.RandomVelocity(100f, 10f, 200f, 0.01f);
                    Projectile cloud = Projectile.NewProjectileDirect(projectile.Center, velocity, ProjectileID.ToxicCloud + Main.rand.Next(3), projectile.damage, 1f, projectile.owner, 0f, Main.rand.Next(-45, 1));
                    cloud.usesIDStaticNPCImmunity = true;
                    cloud.idStaticNPCHitCooldown = 10;
                }
            }
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 107, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
            }
        }
    }
}
