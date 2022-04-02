using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Weapons.Magic;
namespace CalamityMod.Projectiles.Magic
{
    public class SparkInfernal : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spark");
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 300;
            projectile.magic = true;
        }

        public override void AI()
        {
            Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 6, projectile.velocity.X * 0.1f, projectile.velocity.Y * 0.1f);
        }

        public override void Kill(int timeLeft)
        {
            Player player = Main.player[projectile.owner];
            Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, ModContent.ProjectileType<InfernadoMarkFriendly>(), (int)(TheWand.BaseDamage * (player.allDamage + player.magicDamage - 1f)), projectile.knockBack, projectile.owner, 0f, 0f);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Daybreak, 120);
            target.AddBuff(BuffID.OnFire, 240);
        }
    }
}
