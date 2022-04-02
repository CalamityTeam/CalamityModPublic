using CalamityMod.Buffs.StatDebuffs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Ranged
{
    public class FossilShard : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shard");
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.penetrate = 1;
            projectile.aiStyle = 1;
            projectile.timeLeft = 120;
            aiType = ProjectileID.WoodenArrowFriendly;
        }

        public override void AI()
        {
            if (projectile.velocity.Y <= 0f)
                projectile.velocity.Y = 0.1f;
            projectile.rotation += projectile.velocity.Y;
            projectile.velocity.Y *= 1.05f;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<ArmorCrunch>(), 60);
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i <= 2; i++)
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 32, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
            }
        }
    }
}
