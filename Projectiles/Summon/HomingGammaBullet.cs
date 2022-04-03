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
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.minionSlots = 0f;
            Projectile.alpha = 255;
            Projectile.timeLeft = 360;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            Projectile.alpha = Utils.Clamp(Projectile.alpha - 35, 0, 255);
            Projectile.rotation = Projectile.velocity.ToRotation();

            NPC potentialTarget = Projectile.Center.MinionHoming(1000f, Main.player[Projectile.owner]);
            if (potentialTarget != null && !Projectile.WithinRange(potentialTarget.Center, 100f) && Projectile.timeLeft < 290)
                Projectile.velocity = (Projectile.velocity * 6f + Projectile.SafeDirectionTo(potentialTarget.Center) * 15f) / 7f;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit) => target.AddBuff(ModContent.BuffType<SulphuricPoisoning>(), 240);

        public override void Kill(int timeLeft)
        {
            if (Main.dedServ)
                return;

            for (int i = 0; i < 3; i++)
            {
                Dust sulphuricAcid = Dust.NewDustDirect(Projectile.position, 8, 8, (int)CalamityDusts.SulfurousSeaAcid, 0, 0, 0, default, 0.75f);
                sulphuricAcid.noGravity = true;
                sulphuricAcid.velocity *= 1.8f;

                sulphuricAcid = Dust.CloneDust(sulphuricAcid);
                sulphuricAcid.velocity *= -1f;
            }
        }
    }
}
