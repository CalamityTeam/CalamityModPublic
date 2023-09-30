using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;

namespace CalamityMod.Projectiles.Enemy
{
    public class IceClasperEnemyProjectile : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Enemy";

        public override string Texture => "CalamityMod/Projectiles/Melee/DarkIceZero";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 28;
            Projectile.height = 28;
            Projectile.aiStyle = ProjAIStyleID.Arrow;
            AIType = ProjectileID.Bullet;
            Projectile.timeLeft = 600;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.ignoreWater = true;
            Projectile.coldDamage = true;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            Projectile.velocity *= 1.01f;

            // Trail dust.
            int trailDust = Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, 172, Projectile.velocity.X, Projectile.velocity.Y, 0, default, 1.5f);
            Main.dust[trailDust].noGravity = true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(BuffID.Frostburn2, 180);

        public override void OnKill(int timeLeft)
        {
            if (timeLeft > 0)
                SoundEngine.PlaySound(SoundID.Item27, Projectile.Center);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (CalamityConfig.Instance.Afterimages)
                CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor);
            
            return true;
        }
    }
}
