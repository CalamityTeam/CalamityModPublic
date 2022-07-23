using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Enemy
{
    public class CrimsonSpike : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spike");
        }

        public override void SetDefaults()
        {
            Projectile.width = 6;
            Projectile.height = 6;
            Projectile.hostile = true;
            Projectile.Opacity = 0f;
            Projectile.aiStyle = 1;
            Projectile.penetrate = -1;
        }

        public override void AI()
        {
            if (Projectile.Opacity == 1f && Main.rand.NextBool(3))
            {
                int num67 = Dust.NewDust(Projectile.position - Projectile.velocity * 3f, Projectile.width, Projectile.height, 260, 0f, 0f, 50, new Color(255, 136, 78, 150), 1.2f);
                Main.dust[num67].velocity *= 0.3f;
                Main.dust[num67].velocity += Projectile.velocity * 0.3f;
                Main.dust[num67].noGravity = true;
            }

            Projectile.Opacity += 0.2f;
            if (Projectile.Opacity > 1f)
                Projectile.Opacity = 1f;

            if (Projectile.ai[1] == 0f)
            {
                Projectile.ai[1] = 1f;
                SoundEngine.PlaySound(SoundID.Item17, Projectile.position);
            }

            Projectile.ai[0] += 1f;
            if (Projectile.ai[0] >= 5f)
            {
                Projectile.ai[0] = 5f;
                Projectile.velocity.Y = Projectile.velocity.Y + 0.15f;
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Darkness, 90);
        }

        public override void Kill(int timeLeft)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 260, Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.5f);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            lightColor.R = (byte)(255 * Projectile.Opacity);
            lightColor.G = (byte)(255 * Projectile.Opacity);
            lightColor.B = (byte)(255 * Projectile.Opacity);
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }
    }
}
