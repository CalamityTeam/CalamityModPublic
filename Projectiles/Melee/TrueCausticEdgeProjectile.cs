using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Melee
{
    public class TrueCausticEdgeProjectile : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.aiStyle = ProjAIStyleID.Beam;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 600;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0f, 0.35f, 0f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.timeLeft > 595)
                return false;

            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 2);
            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(0, 100, 0, 0);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.penetrate--;
            if (Projectile.penetrate <= 0)
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
                SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
            }
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(BuffID.Venom, 90);

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
            int inc;
            for (int i = 4; i < 31; i = inc + 1)
            {
                float oldXVel = Projectile.oldVelocity.X * (30f / (float)i);
                float oldYVel = Projectile.oldVelocity.Y * (30f / (float)i);
                int causticDust = Dust.NewDust(new Vector2(Projectile.oldPosition.X - oldXVel, Projectile.oldPosition.Y - oldYVel), 8, 8, 44, Projectile.oldVelocity.X, Projectile.oldVelocity.Y, 100, default, 1.8f);
                Main.dust[causticDust].noGravity = true;
                Dust dust = Main.dust[causticDust];
                dust.velocity *= 0.5f;
                causticDust = Dust.NewDust(new Vector2(Projectile.oldPosition.X - oldXVel, Projectile.oldPosition.Y - oldYVel), 8, 8, 44, Projectile.oldVelocity.X, Projectile.oldVelocity.Y, 100, default, 1.4f);
                dust = Main.dust[causticDust];
                dust.velocity *= 0.05f;
                inc = i;
            }
        }
    }
}
