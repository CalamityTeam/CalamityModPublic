using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Weapons.Magic;
using Microsoft.Xna.Framework;

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
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
            Projectile.DamageType = DamageClass.Magic;
        }

        public override void AI()
        {
            Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 6, Projectile.velocity.X * 0.1f, Projectile.velocity.Y * 0.1f);
        }

        public override void Kill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];

            int infernadoDamage = (int)player.GetDamage<GenericDamageClass>().CombineWith(player.GetDamage<MagicDamageClass>()).ApplyTo(TheWand.BaseDamage);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<InfernadoMarkFriendly>(), infernadoDamage, Projectile.knockBack, Projectile.owner);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Daybreak, 120);
            target.AddBuff(BuffID.OnFire, 240);
        }
    }
}
