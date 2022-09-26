using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
namespace CalamityMod.Projectiles.Magic
{
    public class WaywasherProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Waywasher Blast");
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.alpha = 0;
            Projectile.penetrate = 2;
            Projectile.timeLeft = 300;
            Projectile.DamageType = DamageClass.Magic;
        }

        public override void AI()
        {
            Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.02f * (float)Projectile.direction;
            Lighting.AddLight(Projectile.Center, 0f, 0.1f, 0.7f);
            for (int num105 = 0; num105 < 2; num105++)
            {
                float num99 = Projectile.velocity.X / 3f * (float)num105;
                float num100 = Projectile.velocity.Y / 3f * (float)num105;
                int num101 = 4;
                int num102 = Dust.NewDust(new Vector2(Projectile.position.X + (float)num101, Projectile.position.Y + (float)num101), Projectile.width - num101 * 2, Projectile.height - num101 * 2, 33, 0f, 0f, 0, new Color(64, 224, 208), 1.2f);
                Dust dust = Main.dust[num102];
                dust.noGravity = true;
                dust.velocity *= 0.1f;
                dust.velocity += Projectile.velocity * 0.1f;
                dust.position.X -= num99;
                dust.position.Y -= num100;
            }
            if (Main.rand.NextBool(5))
            {
                int num103 = 4;
                int num104 = Dust.NewDust(new Vector2(Projectile.position.X + (float)num103, Projectile.position.Y + (float)num103), Projectile.width - num103 * 2, Projectile.height - num103 * 2, 33, 0f, 0f, 0, new Color(64, 224, 208), 0.6f);
                Main.dust[num104].velocity *= 0.25f;
                Main.dust[num104].velocity += Projectile.velocity * 0.5f;
            }
            if (Projectile.velocity.Y > 16f)
            {
                Projectile.velocity.Y = 16f;
            }
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
            for (int k = 0; k < 10; k++)
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 33, Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.5f, 0, new Color(0, 142, 255), 1f);
            }
        }
    }
}
