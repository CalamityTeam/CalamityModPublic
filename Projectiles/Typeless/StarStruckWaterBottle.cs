using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Projectiles.Typeless
{
    public class StarStruckWaterBottle : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Typeless/StarStruckWater";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bottle");
        }

        public override void SetDefaults()
        {
            projectile.width = 14;
            projectile.height = 14;
            projectile.aiStyle = 2;
            projectile.friendly = true;
            projectile.penetrate = 1;
        }

        public override void AI()
        {
            projectile.ai[0] += 1f;
            if (projectile.ai[0] >= 10f)
            {
                projectile.velocity.Y += 0.1f;
                projectile.velocity.X *= 0.998f;
            }
        }

        public override void Kill(int timeLeft)
        {
            if (projectile.owner == Main.myPlayer)
            {
                Main.PlaySound(SoundID.Shatter, (int)projectile.position.X, (int)projectile.position.Y, 1, 1f, 0f);
                for (int index = 0; index < 5; ++index)
                    Dust.NewDust(projectile.position, projectile.width, projectile.height, 13, 0f, 0f, 0, new Color(), 1f);
                for (int index1 = 0; index1 < 30; ++index1)
                {
                    int index2 = Dust.NewDust(projectile.position, projectile.width, projectile.height, ModContent.DustType<AstralBlue>(), 0f, -2f, 0, new Color(), 1.1f);
                    Dust dust = Main.dust[index2];
                    dust.alpha = 100;
                    dust.velocity.X *= 1.5f;
                    dust.velocity *= 3f;
                }
                Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<WaterConvertor>(), 0, 0f, projectile.owner, 4f);
            }
        }
    }
}
