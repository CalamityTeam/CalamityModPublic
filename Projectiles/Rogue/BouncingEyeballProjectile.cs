using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static System.Math;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Rogue
{
    public class BouncingEyeballProjectile : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/BouncingEyeball";

        private int Bounces = 5;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 26;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 300;
            Projectile.penetrate = 2;
            Projectile.DamageType = RogueDamageClass.Instance;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            SoundEngine.PlaySound(SoundID.NPCHit19 with { Volume = SoundID.NPCHit19.Volume * 0.7f}, Projectile.Center);
            Bounces--;
            if (Bounces <= 0)
            {
                Projectile.Kill();
            }
            else
            {
                if (Projectile.velocity.X != oldVelocity.X)
                {
                    Projectile.velocity.X = -oldVelocity.X;
                }
                if (Projectile.velocity.Y != oldVelocity.Y)
                {
                    Projectile.velocity.Y = -oldVelocity.Y;
                }
            }
            return false;
        }
        public override void AI()
        {
            if (Projectile.velocity.Y <= 10f)
                Projectile.velocity.Y += 0.15f;
            Projectile.rotation += MathHelper.ToRadians(5f) * Sign(Projectile.velocity.X);
        }
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.NPCHit19 with { Volume = SoundID.NPCHit19.Volume * 0.7f}, Projectile.Center);
            int dustCount = Main.rand.Next(8, 16);
            for (int index = 0; index < dustCount; index++)
            {
                Vector2 velocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(4f, 9f) + Projectile.velocity / 2f;
                Dust.NewDust(Projectile.Center, 4, 4, DustID.Blood, velocity.X, velocity.Y);
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 2);
            return false;
        }
    }
}
