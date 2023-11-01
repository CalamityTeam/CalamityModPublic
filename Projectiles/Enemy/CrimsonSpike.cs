using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Enemy
{
    public class CrimsonSpike : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Enemy";
        public override void SetDefaults()
        {
            Projectile.width = 6;
            Projectile.height = 6;
            Projectile.hostile = true;
            Projectile.Opacity = 0f;
            Projectile.aiStyle = ProjAIStyleID.Arrow;
            Projectile.penetrate = -1;
        }

        public override void AI()
        {
            if (Projectile.Opacity == 1f && Main.rand.NextBool(3))
            {
                Color dustColor = Color.Crimson;
                dustColor.A = 150;
                int redSpiky = Dust.NewDust(Projectile.position - Projectile.velocity * 3f, Projectile.width, Projectile.height, 260, 0f, 0f, 50, dustColor, 1.2f);
                Main.dust[redSpiky].velocity *= 0.3f;
                Main.dust[redSpiky].velocity += Projectile.velocity * 0.3f;
                Main.dust[redSpiky].noGravity = true;
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

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (info.Damage <= 0)
                return;

            target.AddBuff(BuffID.Darkness, 90);
        }

        public override void OnKill(int timeLeft)
        {
            Color dustColor = Color.Crimson;
            dustColor.A = 150;
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 260, Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.5f, 50, dustColor, 1f);
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
