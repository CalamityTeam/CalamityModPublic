using CalamityMod.Buffs.StatDebuffs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Rogue
{
    public class FossilShardThrown : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Ranged/FossilShard";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shard");
        }

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.aiStyle = 1;
            Projectile.timeLeft = 90;
            AIType = ProjectileID.WoodenArrowFriendly;
            Projectile.DamageType = RogueDamageClass.Instance;
        }

        public override void AI()
        {
            Projectile.rotation += Projectile.velocity.Y;
            Projectile.velocity.Y *= 1.05f;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<ArmorCrunch>(), 60);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<ArmorCrunch>(), 60);
        }

        public override void Kill(int timeLeft)
        {
            Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 32, Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.5f);
        }
    }
}
