using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class IceSentryShard : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Boss/IceRain";

        public override void SetDefaults()
        {
            projectile.width = 12;
            projectile.height = 12;
            projectile.aiStyle = 1;
            projectile.coldDamage = true;
            projectile.friendly = true;
            projectile.minion = true;
            projectile.penetrate = 1;
            projectile.coldDamage = true;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Frost Shard");
            ProjectileID.Sets.SentryShot[projectile.type] = true;
        }

        public override void AI()
        {
            projectile.velocity.Y += 0.2f;
            if (projectile.localAI[0] == 0f || projectile.localAI[0] == 2f)
            {
                projectile.scale += 0.01f;
                projectile.alpha -= 50;
                if (projectile.alpha <= 0)
                {
                    projectile.localAI[0] = 1f;
                    projectile.alpha = 0;
                }
            }
            else if (projectile.localAI[0] == 1.0)
            {
                projectile.scale -= 0.01f;
                projectile.alpha += 50;
                if (projectile.alpha >= byte.MaxValue)
                {
                    projectile.localAI[0] = 2f;
                    projectile.alpha = byte.MaxValue;
                }
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(200, 200, 200, projectile.alpha);
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item27, projectile.position);
            for (int index1 = 0; index1 < 3; ++index1)
            {
                int index2 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 76);
                Main.dust[index2].noGravity = true;
                Main.dust[index2].noLight = true;
                Main.dust[index2].scale = 0.7f;
            }
        }
    }
}
