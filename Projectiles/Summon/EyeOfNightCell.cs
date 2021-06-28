using CalamityMod.Dusts;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class EyeOfNightCell : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cell");
            ProjectileID.Sets.MinionShot[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.friendly = true;
            projectile.width = projectile.height = 10;
            projectile.minion = true;
            projectile.minionSlots = 0f;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.timeLeft = 180;
            projectile.extraUpdates = 1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 19;
        }

        public override void AI()
        {
            if (!Main.dedServ && projectile.velocity.Length() > 5f)
                Dust.NewDustPerfect(projectile.Center, (int)CalamityDusts.SulfurousSeaAcid).noGravity = true;

            projectile.StickyProjAI(360);
        }
        public override void Kill(int timeLeft)
        {
            CalamityGlobalProjectile.ExpandHitboxBy(projectile, 60);
            projectile.Damage();

            if (!Main.dedServ)
                for (int i = 0; i < 10; i++)
                    Dust.NewDustDirect(projectile.position, 36, 36, (int)CalamityDusts.SulfurousSeaAcid).noGravity = true;
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection) => projectile.ModifyHitNPCSticky(10, false);

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit) => target.AddBuff(BuffID.CursedInferno, 120);

        public override void OnHitPvp(Player target, int damage, bool crit) => target.AddBuff(BuffID.CursedInferno, 120);
    }
}
