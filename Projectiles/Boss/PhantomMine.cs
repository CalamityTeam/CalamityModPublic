using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Boss
{
    public class PhantomMine : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Phantom Orb");
        }

        public override void SetDefaults()
        {
            projectile.Calamity().canBreakPlayerDefense = true;
            projectile.width = 30;
            projectile.height = 30;
            projectile.hostile = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.timeLeft = 480;
            cooldownSlot = 1;
            projectile.Calamity().affectedByMaliceModeVelocityMultiplier = true;
        }

        public override void AI()
        {
            if (projectile.velocity.Length() < projectile.ai[0])
                projectile.velocity *= projectile.ai[1];
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(200, 200, 200, projectile.alpha);
        }

        public override bool CanHitPlayer(Player target) => projectile.velocity.Length() >= projectile.ai[0];

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(projectile.Center, 12f, targetHitbox);

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item14, projectile.position);
            projectile.position.X = projectile.position.X + (projectile.width / 2);
            projectile.position.Y = projectile.position.Y + (projectile.height / 2);
            projectile.width = 150;
            projectile.height = 150;
            projectile.position.X = projectile.position.X - (projectile.width / 2);
            projectile.position.Y = projectile.position.Y - (projectile.height / 2);
            for (int num621 = 0; num621 < 15; num621++)
            {
                int num622 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 180, 0f, 0f, 100, default, 1.2f);
                Main.dust[num622].velocity *= 3f;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[num622].scale = 0.5f;
                    Main.dust[num622].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                }
            }
            for (int num623 = 0; num623 < 30; num623++)
            {
                int num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 180, 0f, 0f, 100, default, 1.7f);
                Main.dust[num624].noGravity = true;
                Main.dust[num624].velocity *= 5f;
                num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 60, 0f, 0f, 100, default, 1f);
                Main.dust[num624].velocity *= 2f;
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (projectile.velocity.Length() >= projectile.ai[0])
            {
                target.AddBuff(ModContent.BuffType<Nightwither>(), 180);
                target.AddBuff(ModContent.BuffType<WhisperingDeath>(), 180);
            }
        }

        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)
        {
            target.Calamity().lastProjectileHit = projectile;
        }
    }
}
