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
            ProjectileID.Sets.MinionShot[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 2;
            projectile.height = 2;
            projectile.aiStyle = 1;
            aiType = ProjectileID.DeathLaser;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.minion = true;
            projectile.minionSlots = 0f;
            projectile.penetrate = 1;
            projectile.alpha = 120;
            projectile.timeLeft = 300;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, (255 - projectile.alpha) * 0.5f / 255f, (255 - projectile.alpha) * 0.05f / 255f, (255 - projectile.alpha) * 0.05f / 255f);
            projectile.velocity.X *= 1.05f;
            projectile.velocity.Y *= 1.05f;
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 120);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 120);
        }

        public override Color? GetAlpha(Color lightColor) => new Color(250, 50, 50, projectile.alpha);
    }
}
