using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class ShadecrystalProjectile : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public override void SetDefaults()
        {
            Projectile.width = 6;
            Projectile.height = 6;
            Projectile.scale = 1.5f;
            Projectile.friendly = true;
            Projectile.alpha = 50;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
            Projectile.DamageType = DamageClass.Magic;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0.15f, 0f, 0.15f);

            Projectile.rotation += Projectile.velocity.X * 0.2f;

            if (Main.rand.NextBool(4))
            {
                int crystalDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 70, 0f, 0f, 0, default, 1f);
                Main.dust[crystalDust].noGravity = true;
                Main.dust[crystalDust].velocity *= 0.2f;
                Main.dust[crystalDust].scale *= 0.8f;
            }

            Projectile.velocity *= 0.99f;

            Projectile.ai[1] += 1f;
            if (Projectile.ai[1] > 90f)
            {
                Projectile.scale -= 0.05f;
                if (Projectile.scale <= 0.2)
                {
                    Projectile.scale = 0.2f;
                    Projectile.Kill();
                }
                Projectile.width = (int)(6f * Projectile.scale);
                Projectile.height = (int)(6f * Projectile.scale);
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != oldVelocity.X)
                Projectile.velocity.X = -oldVelocity.X;

            if (Projectile.velocity.Y != oldVelocity.Y)
                Projectile.velocity.Y = -oldVelocity.Y;

            return false;
        }

        public override void OnKill(int timeLeft)
        {
            for (int k = 0; k < 5; k++)
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 70, Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.5f);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Frostburn2, 120);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(200, 200, 200, 0);
        }
    }
}
