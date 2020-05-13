using CalamityMod.Buffs.StatDebuffs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class CausticStaffProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Thorn");
            ProjectileID.Sets.MinionShot[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.friendly = true;
            projectile.minion = true;
            projectile.minionSlots = 0f;
            projectile.ignoreWater = true;
            projectile.penetrate = 3;
            projectile.timeLeft = 360;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
        }
        public override void AI()
        {
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
            projectile.velocity.X *= 0.99f;
            if (projectile.velocity.Y < 9f)
                projectile.velocity.Y += 0.085f;
            projectile.tileCollide = projectile.timeLeft <= 180;
        }
        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 5; i++)
            {
                Dust dust = Dust.NewDustPerfect(projectile.Center, DustID.Fire);
                dust.noGravity = true;
                dust.velocity = Vector2.One.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(2);
            }
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<MarkedforDeath>(), 180);
            target.AddBuff(BuffID.Ichor, 180);
            target.AddBuff(BuffID.Venom, 180);
            target.AddBuff(BuffID.CursedInferno, 180);
            target.AddBuff(BuffID.OnFire, 180);
        }
    }
}
