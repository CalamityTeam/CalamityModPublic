using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class PoseidonTyphoon : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Poseidon Typhoon");
        }

        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 30;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.penetrate = 10; //randomized in the item file now, penetrates 4 to 10 times
            projectile.timeLeft = 300;
            projectile.ignoreWater = true;
        }

        public override void AI()
        {
            projectile.rotation += projectile.velocity.X * 0.05f;

            CalamityGlobalProjectile.HomeInOnNPC(projectile, !projectile.tileCollide, 600f, 6f, 30f);
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item10, projectile.position);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(200, 200, 200, 0);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            projectile.penetrate--;
            if (projectile.penetrate <= 0)
            {
                projectile.Kill();
            }
            else
            {
                if (projectile.velocity.X != oldVelocity.X)
                {
                    projectile.velocity.X = -oldVelocity.X;
                }
                if (projectile.velocity.Y != oldVelocity.Y)
                {
                    projectile.velocity.Y = -oldVelocity.Y;
                }
            }
            return false;
        }
    }
}
