using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Buffs.StatDebuffs;
namespace CalamityMod.Projectiles.Magic
{
    public class MiasmaGas : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gas");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 3;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 44;
            projectile.height = 28;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.timeLeft = 240;
            projectile.penetrate = 10;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 15;
            projectile.magic = true;
        }

        public override void AI()
        {
            if (projectile.localAI[0] == 0f)
            {
                projectile.alpha = Main.rand.Next(35, 75);
                projectile.localAI[0] = 1f;
            }
            projectile.ai[0] += 1f;
            if (projectile.ai[0] >= 360f)
            {
                projectile.ai[1] = 1f;
            }
            if (projectile.ai[1] == 1f)
            {
                projectile.velocity *= 0.9865f;
            }
            projectile.rotation += projectile.velocity.X * 0.0003f;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Irradiated>(), 180);
            projectile.ai[1] = 1f;
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Irradiated>(), 180);
            projectile.ai[1] = 1f;
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 25; i++)
            {
                Dust dust = Dust.NewDustDirect(projectile.position, 48, 30, 189, 0f, 0f);
                dust.velocity = projectile.velocity.RotatedByRandom(MathHelper.ToRadians(39f));
                dust.alpha = 127;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 2);
            return false;
        }
    }
}
