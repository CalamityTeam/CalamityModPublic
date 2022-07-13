using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Summon
{
    public class IceSentryShard : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Boss/IceRain";

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.aiStyle = 1;
            Projectile.coldDamage = true;
            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.penetrate = 1;
            Projectile.coldDamage = true;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Frost Shard");
            ProjectileID.Sets.SentryShot[Projectile.type] = true;
        }

        public override void AI()
        {
            Projectile.velocity.Y += 0.2f;
            if (Projectile.localAI[0] == 0f || Projectile.localAI[0] == 2f)
            {
                Projectile.scale += 0.01f;
                Projectile.alpha -= 50;
                if (Projectile.alpha <= 0)
                {
                    Projectile.localAI[0] = 1f;
                    Projectile.alpha = 0;
                }
            }
            else if (Projectile.localAI[0] == 1.0)
            {
                Projectile.scale -= 0.01f;
                Projectile.alpha += 50;
                if (Projectile.alpha >= byte.MaxValue)
                {
                    Projectile.localAI[0] = 2f;
                    Projectile.alpha = byte.MaxValue;
                }
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(200, 200, 200, Projectile.alpha);
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item27, Projectile.position);
            for (int index1 = 0; index1 < 3; ++index1)
            {
                int index2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 76);
                Main.dust[index2].noGravity = true;
                Main.dust[index2].noLight = true;
                Main.dust[index2].scale = 0.7f;
            }
        }
    }
}
