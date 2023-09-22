using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.DraedonsArsenal
{
    public class StarSwallowerAcid : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Misc";
        public override string Texture => "CalamityMod/Projectiles/Environment/AcidDrop";

        public const float Gravity = 0.25f;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = false;
            Projectile.timeLeft = 300;
            Projectile.alpha = 255;
            Projectile.DamageType = DamageClass.Summon;
        }
        public override void AI()
        {
            if (Projectile.velocity.Y <= 10f)
            {
                Projectile.velocity.Y += Gravity;
            }
            Projectile.ai[0]++;
            if (Projectile.ai[0] < 10)
            {
                Projectile.alpha = (int)MathHelper.Lerp(255, 0, Projectile.ai[0] / 10f);
            }
            Projectile.tileCollide = Projectile.timeLeft <= 260;
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            // Water drip
            for (int i = 0; i < 4; i++)
            {
                int idx = Dust.NewDust(Projectile.position - Projectile.velocity, 2, 2, 154, 0f, 0f, 0, new Color(112, 150, 42, 127), 1f);
                Dust dust = Main.dust[idx];
                dust.position.X -= 2f;
                Main.dust[idx].alpha = 38;
                Main.dust[idx].velocity *= 0.1f;
                Main.dust[idx].velocity -= Projectile.velocity * 0.025f;
                Main.dust[idx].scale = 2f;
            }
            return true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], new Color(255, 255, 255, 127) * Projectile.Opacity, 2);
            return false;
        }
    }
}
