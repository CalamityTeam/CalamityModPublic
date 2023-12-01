using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Melee
{
    public class Flarefrost : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.alpha = 255;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 180;
            Projectile.DamageType = DamageClass.Melee;
        }

        public override void AI()
        {
            Vector2 rotateVector = new Vector2(5f, 10f);
            Lighting.AddLight(Projectile.Center, 0.25f, 0f, 0.25f);
            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] == 48f)
            {
                Projectile.localAI[0] = 0f;
            }
            else
            {
                for (int i = 0; i < 2; i++)
                {
                    int dustType = i == 0 ? 67 : 174;
                    Vector2 dustRotate = Vector2.UnitX * -12f;
                    dustRotate = -Vector2.UnitY.RotatedBy((double)(Projectile.localAI[0] * 0.1308997f + (float)i * 3.14159274f), default) * rotateVector;
                    int flarefrost = Dust.NewDust(Projectile.Center, 0, 0, dustType, 0f, 0f, 160, default, 1f);
                    Main.dust[flarefrost].scale = dustType == 67 ? 1.5f : 1f;
                    Main.dust[flarefrost].noGravity = true;
                    Main.dust[flarefrost].position = Projectile.Center + dustRotate;
                    Main.dust[flarefrost].velocity = Projectile.velocity;
                    int frostflare = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, dustType, 0f, 0f, 100, default, 0.8f);
                    Main.dust[frostflare].noGravity = true;
                    Main.dust[frostflare].velocity *= 0f;
                }
            }

            CalamityUtils.HomeInOnNPC(Projectile, !Projectile.tileCollide, 250f, 12f, 20f);
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item27, Projectile.Center);
            for (int k = 0; k < 2; k++)
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 67, Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.5f);
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 174, Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.5f);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire3, 120);
            target.AddBuff(BuffID.Frostburn2, 120);
        }
    }
}
