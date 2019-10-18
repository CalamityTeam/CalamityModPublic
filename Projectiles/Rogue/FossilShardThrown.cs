using Terraria;
using Terraria.ModLoader;
using CalamityMod.Buffs;
namespace CalamityMod.Projectiles.Rogue
{
    public class FossilShardThrown : ModProjectile
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
            projectile.penetrate = 1;
            projectile.aiStyle = 1;
            projectile.timeLeft = 90;
            aiType = 1;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            projectile.rotation += projectile.velocity.Y;
            projectile.velocity.Y *= 1.05f;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<ArmorCrunch>(), 60);
        }

        public override void Kill(int timeLeft)
        {
            Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 32, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
        }
    }
}
