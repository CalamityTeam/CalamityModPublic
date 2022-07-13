using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class BrimstoneLaserSummon : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Melee/BrimstoneLaser";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Laser");
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.aiStyle = 1;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.minion = true;
            Projectile.minionSlots = 0f;
            Projectile.penetrate = 1;
            Projectile.alpha = 120;
            Projectile.timeLeft = 300;
            AIType = ProjectileID.DeathLaser;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, Projectile.Opacity * 0.5f, Projectile.Opacity * 0.05f, Projectile.Opacity * 0.05f);
            Projectile.velocity.X *= 1.05f;
            Projectile.velocity.Y *= 1.05f;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 120);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 120);
        }

        public override Color? GetAlpha(Color lightColor) => new Color(250, 50, 50, Projectile.alpha);
    }
}
