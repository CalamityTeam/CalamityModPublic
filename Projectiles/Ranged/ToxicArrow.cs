using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Ranged
{
    public class ToxicArrow : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Toxic Arrow");
        }

        public override void SetDefaults()
        {
            projectile.width = 14;
            projectile.height = 14;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.arrow = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 240;
            projectile.Calamity().pointBlankShotDuration = CalamityGlobalProjectile.basePointBlankShotDuration;
        }

        public override void AI()
        {
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }

        public override void Kill(int timeLeft)
        {
            projectile.position = projectile.Center;
            projectile.width = projectile.height = 32;
            projectile.position = projectile.position - projectile.Size / 2f;
            projectile.maxPenetrate = -1;
            projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
            projectile.damage /= 2;
            projectile.Damage();
            Main.PlaySound(SoundID.Item92, projectile.position);
            int count = Main.rand.Next(6, 15);
            for (int i = 0; i < count; i++)
            {
                int idx = Dust.NewDust(projectile.Center - projectile.velocity / 2f, 0, 0, (int)CalamityDusts.SulfurousSeaAcid, 0f, 0f, 100, default, 2f);
                Main.dust[idx].velocity *= 2f;
                Main.dust[idx].noGravity = true;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<SulphuricPoisoning>(), 120);
        }
    }
}
