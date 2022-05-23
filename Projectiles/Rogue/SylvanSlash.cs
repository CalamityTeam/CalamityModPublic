using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Rogue
{
    public class SylvanSlash : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Slash");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.penetrate = 2;
            Projectile.alpha = 120;
            Projectile.timeLeft = 200;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            if (Projectile.localAI[0] == 0f)
            {
                SoundEngine.PlaySound(SoundID.Item92, Projectile.position);
                Projectile.localAI[0] += 1f;
            }
            Lighting.AddLight(Projectile.Center, 0.1f, 1f, 2f);
            Projectile.spriteDirection = Projectile.direction = (Projectile.velocity.X > 0).ToDirectionInt();
            Projectile.rotation = Projectile.velocity.ToRotation() + (Projectile.spriteDirection == 1 ? 0f : MathHelper.Pi) + MathHelper.ToRadians(90) * Projectile.direction;
            Projectile.velocity.Y += Projectile.ai[0];
            if (Main.rand.NextBool(2))
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 111, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.timeLeft > 195)
                return false;

            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 2);
            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(200, 200, 200, Projectile.alpha);
        }

        public override void Kill(int timeLeft)
        {
            for (int k = 0; k < 3; k++)
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 111, Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.5f);
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Projectile.velocity *= 0.5f;
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            Projectile.velocity *= 0.5f;
        }
    }
}
