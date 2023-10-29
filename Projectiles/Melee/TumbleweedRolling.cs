using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
namespace CalamityMod.Projectiles.Melee
{
    public class TumbleweedRolling : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override string Texture => "CalamityMod/Projectiles/Melee/TumbleweedFlail";

        public override void SetDefaults()
        {
            Projectile.width = 44;
            Projectile.height = 44;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 8;
            Projectile.timeLeft = 300;
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
        }

        public override void AI()
        {
            if ((Projectile.velocity.X != Projectile.velocity.X && (Projectile.velocity.X < -3f || Projectile.velocity.X > 3f)) ||
                (Projectile.velocity.Y != Projectile.velocity.Y && (Projectile.velocity.Y < -3f || Projectile.velocity.Y > 3f)))
            {
                Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
                SoundEngine.PlaySound(SoundID.NPCHit11, Projectile.position);
            }
            if (Projectile.velocity.X != Projectile.velocity.X)
            {
                Projectile.velocity.X = Projectile.velocity.X * -0.5f;
            }
            if (Projectile.velocity.Y != Projectile.velocity.Y && Projectile.velocity.Y > 1f)
            {
                Projectile.velocity.Y = Projectile.velocity.Y * -0.5f;
            }
            Projectile.rotation += Projectile.velocity.X * 0.05f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // TODO -- Make this use proper i-frame variables.
            target.immune[Projectile.owner] = 5;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath15, Projectile.position);
            for (int i = 0; i < 20; i++)
            {
                int tumbleDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 32, 0f, 0f, 100, default, 1.2f);
                Main.dust[tumbleDust].velocity *= 3f;
                if (Main.rand.NextBool())
                {
                    Main.dust[tumbleDust].scale = 0.5f;
                    Main.dust[tumbleDust].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
            for (int j = 0; j < 30; j++)
            {
                int tumbleDust2 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 85, 0f, 0f, 100, default, 1.7f);
                Main.dust[tumbleDust2].noGravity = true;
                Main.dust[tumbleDust2].velocity *= 5f;
                tumbleDust2 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 85, 0f, 0f, 100, default, 1f);
                Main.dust[tumbleDust2].velocity *= 2f;
            }
        }
    }
}
