using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Ranged
{
    public class TerraBulletMain : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 2;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.aiStyle = ProjAIStyleID.Arrow;
            AIType = ProjectileID.Bullet;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
            Projectile.alpha = 255;
            Projectile.extraUpdates = 3;
            Projectile.Calamity().pointBlankShotDuration = CalamityGlobalProjectile.DefaultPointBlankDuration;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0f, 0.4f, 0f);

            // localAI is used as an invisibility counter. The bullet fades into existence after 15 startup frames.
            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] > 4f)
            {
                // After 15 frames, the alpha will be exactly 0
                if (Projectile.alpha > 0)
                    Projectile.alpha -= 17;

                // Dust type 74, scale 0.8, no gravity, no light, no velocity
                Vector2 pos = Projectile.Center - Projectile.velocity * 0.1f;
                Dust d = Dust.NewDustDirect(pos, 0, 0, 74, Scale: 0.8f);
                d.position = pos;
                d.velocity = Vector2.Zero;
                d.noGravity = true;
                d.noLight = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesFromEdge(Projectile, 0, lightColor);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            if (Projectile.owner == Main.myPlayer)
            {
                for (int b = 0; b < 2; b++)
                {
                    Vector2 velocity = CalamityUtils.RandomVelocity(100f, 70f, 100f);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<TerraBulletSplit>(), (int)(Projectile.damage * 0.3), 0f, Projectile.owner, 0f, 0f);
                }
            }
            SoundEngine.PlaySound(SoundID.Item118, Projectile.position);
        }
    }
}
