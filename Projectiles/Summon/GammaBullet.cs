using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class GammaBullet : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Enemy/NuclearBulletMedium";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gamma Bullet");
            ProjectileID.Sets.MinionShot[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 24;
            projectile.height = 12;
            projectile.friendly = true;
            projectile.minion = true;
            projectile.minionSlots = 0f;
            projectile.alpha = 255;
            projectile.timeLeft = 180;
            projectile.tileCollide = true;
            projectile.ignoreWater = true;
        }
        public override void AI()
        {
            Lighting.AddLight(projectile.Center, Color.White.ToVector3() * 1.25f);
            projectile.ai[1]++;
            if (projectile.ai[1] <= 20f)
            {
                projectile.alpha = (int)MathHelper.Lerp(255, 0, projectile.ai[1] / 20f);
            }
            if (projectile.ai[1] % 10f == 9f)
            {
                for (int i = 0; i < 24; i++)
                {
                    float angle = MathHelper.TwoPi / 24f * i;
                    Dust dust = Dust.NewDustPerfect(projectile.Center + angle.ToRotationVector2().RotatedBy(projectile.rotation) * new Vector2(7f, 4f), (int)CalamityDusts.SulfurousSeaAcid);
                    dust.scale = 0.9f;
                    dust.alpha = projectile.alpha;
                    dust.noGravity = true;
                }
            }
            projectile.velocity *= 1.03f;
            projectile.rotation = projectile.velocity.ToRotation();
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<SulphuricPoisoning>(), 240);
        }
        public override void Kill(int timeLeft)
        {
            for (int i = 0; i <= 2; i++)
            {
                int idx = Dust.NewDust(projectile.position, 8, 8, (int)CalamityDusts.SulfurousSeaAcid, 0, 0, 0, default, 0.75f);
                Main.dust[idx].noGravity = true;
                Main.dust[idx].velocity *= 3f;
                idx = Dust.NewDust(projectile.position, 8, 8, (int)CalamityDusts.SulfurousSeaAcid, 0, 0, 0, default, 0.75f);
                Main.dust[idx].noGravity = true;
                Main.dust[idx].velocity *= 3f;
            }
        }
    }
}
