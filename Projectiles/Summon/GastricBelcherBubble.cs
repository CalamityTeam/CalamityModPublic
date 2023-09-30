using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
namespace CalamityMod.Projectiles.Summon
{
    public class GastricBelcherBubble : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.width = Projectile.height = 18;
            Projectile.timeLeft = 180;
            Projectile.aiStyle = ProjAIStyleID.Bubble;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item54, Projectile.position);
            Vector2 center = Projectile.Center;
            for (int dustIndex = 0; dustIndex < 10; ++dustIndex)
            {
                int scalar = (int)(10 * Projectile.ai[1]);
                int bubble = Dust.NewDust(Projectile.Center - Vector2.One * scalar, scalar * 2, scalar * 2, 212, 0.0f, 0.0f, 0, new Color(), 1f);
                Dust dust = Main.dust[bubble];
                Vector2 dustVec = Vector2.Normalize(dust.position - Projectile.Center);
                dust.position = Projectile.Center + dustVec * scalar * Projectile.scale;
                dust.velocity = dustIndex >= 30 ? dustVec * Main.rand.Next(45, 91) / 10f : dustVec * dust.velocity.Length();
                dust.color = Main.hslToRgb(0.4f + Main.rand.NextFloat() * 0.2f, 0.9f, 0.5f);
                dust.color = Color.Lerp(dust.color, Color.White, 0.3f);
                dust.noGravity = true;
                dust.scale = 0.7f;
            }
        }
    }
}
