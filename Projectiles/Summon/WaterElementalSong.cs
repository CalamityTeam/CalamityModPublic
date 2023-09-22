using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Summon
{
    public class WaterElementalSong : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 26;
            Projectile.friendly = true;
            Projectile.timeLeft = 600;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            Projectile.velocity.X *= 0.985f;
            Projectile.velocity.Y *= 0.985f;
            if (Projectile.localAI[0] == 0f)
            {
                Projectile.scale += 0.02f;
                if (Projectile.scale >= 1.25f)
                {
                    Projectile.localAI[0] = 1f;
                }
            }
            else if (Projectile.localAI[0] == 1f)
            {
                Projectile.scale -= 0.02f;
                if (Projectile.scale <= 0.75f)
                {
                    Projectile.localAI[0] = 0f;
                }
            }
            if (Projectile.ai[1] == 0f)
            {
                Projectile.ai[1] = 1f;
                float soundPitch = (Main.rand.NextFloat() - 0.5f) * 0.5f;
                Main.musicPitch = soundPitch;
                SoundEngine.PlaySound(SoundID.Item26, Projectile.position);
            }
            Lighting.AddLight(Projectile.Center, 0f, 0f, 1.2f);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 0);
        }
    }
}
