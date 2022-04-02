using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class MadAlchemistsCocktailShrapnel : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shrapnel");
        }

        public override void SetDefaults()
        {
            projectile.width = 8;
            projectile.height = 8;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.tileCollide = false;
            projectile.penetrate = 1;
            projectile.timeLeft = 180;
            projectile.extraUpdates = 1;
        }

        public override bool? CanHitNPC(NPC target) => projectile.timeLeft < 150 && target.CanBeChasedBy(projectile);

        public override void AI()
        {
            Vector2 value7 = new Vector2(6f, 12f);
            projectile.localAI[0] += 1f;
            if (projectile.localAI[0] == 48f)
            {
                projectile.localAI[0] = 0f;
            }
            else
            {
                for (int num41 = 0; num41 < 2; num41++)
                {
                    Vector2 value8 = Vector2.UnitX * -15f;
                    value8 = -Vector2.UnitY.RotatedBy((double)(projectile.localAI[0] * 0.1308997f + (float)num41 * 3.14159274f), default) * value7 * 0.75f;
                    int num42 = Dust.NewDust(projectile.Center, 0, 0, 173, 0f, 0f, 160, default, 0.75f);
                    Main.dust[num42].noGravity = true;
                    Main.dust[num42].position = projectile.Center + value8;
                    Main.dust[num42].velocity = projectile.velocity;
                }
            }

            int num458 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 173, 0f, 0f, 100, default, 1f);
            Main.dust[num458].noGravity = true;

            if (projectile.timeLeft < 150)
                CalamityGlobalProjectile.HomeInOnNPC(projectile, true, 600f, 12f, 20f);
        }
    }
}
