using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class HomingGammaBullet : ModProjectile
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
            projectile.timeLeft = 360;
            projectile.tileCollide = true;
            projectile.ignoreWater = true;
        }

        public override void AI()
        {
            projectile.alpha = Utils.Clamp(projectile.alpha - 35, 0, 255);
            projectile.rotation = projectile.velocity.ToRotation();

            NPC potentialTarget = projectile.Center.MinionHoming(1000f, Main.player[projectile.owner]);
            if (potentialTarget != null && !projectile.WithinRange(potentialTarget.Center, 100f) && projectile.timeLeft < 290)
                projectile.velocity = (projectile.velocity * 6f + projectile.SafeDirectionTo(potentialTarget.Center) * 15f) / 7f;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit) => target.AddBuff(ModContent.BuffType<SulphuricPoisoning>(), 240);

        public override void Kill(int timeLeft)
        {
            if (Main.dedServ)
                return;

            for (int i = 0; i < 3; i++)
            {
                Dust sulphuricAcid = Dust.NewDustDirect(projectile.position, 8, 8, (int)CalamityDusts.SulfurousSeaAcid, 0, 0, 0, default, 0.75f);
                sulphuricAcid.noGravity = true;
                sulphuricAcid.velocity *= 1.8f;

                sulphuricAcid = Dust.CloneDust(sulphuricAcid);
                sulphuricAcid.velocity *= -1f;
            }
        }
    }
}
