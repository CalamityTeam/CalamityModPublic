using CalamityMod.World;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Typeless
{
    public class AstralSpray : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.alpha = 255;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 2;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public override bool? CanDamage() => false;

        public override bool PreAI()
        {
            if (Main.myPlayer == Projectile.owner)
            {
                int x = (int)(Projectile.Center.X / 16f);
                int y = (int)(Projectile.Center.Y / 16f);

                AstralBiome.ConvertToAstral(x - 1, x + 1, y - 1, y + 1);
            }
            if (Projectile.timeLeft > 133)
            {
                Projectile.timeLeft = 133;
            }
            if (Projectile.ai[0] > 7f)
            {
                float scalar = 1f;
                if (Projectile.ai[0] == 8f)
                {
                    scalar = 0.2f;
                }
                else if (Projectile.ai[0] == 9f)
                {
                    scalar = 0.4f;
                }
                else if (Projectile.ai[0] == 10f)
                {
                    scalar = 0.6f;
                }
                else if (Projectile.ai[0] == 11f)
                {
                    scalar = 0.8f;
                }
                Projectile.ai[0]++;
                for (int i = 0; i < 1; i++)
                {
                    int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 118, Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f, 100);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].scale *= 1.75f * scalar;
                    Main.dust[d].velocity.X *= 2f;
                    Main.dust[d].velocity.Y *= 2f;
                }
            }
            else
            {
                Projectile.ai[0]++;
            }
            Projectile.rotation += 0.3f * Projectile.direction;
            return false;
        }
    }
}
