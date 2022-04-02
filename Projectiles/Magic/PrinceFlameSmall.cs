using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class PrinceFlameSmall : ModProjectile
    {
        public ref float Time => ref projectile.ai[0];
        public const int AttackDelay = 12;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Holy Fire");
            Main.projFrames[projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 3;
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 16;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.timeLeft = 240;
            projectile.penetrate = 1;
            projectile.magic = true;
            projectile.extraUpdates = 2;
        }

        public override void AI()
        {
            Time++;
            projectile.frameCounter++;
            projectile.frame = projectile.frameCounter / 5 % Main.projFrames[projectile.type];
            projectile.rotation = projectile.velocity.ToRotation() - MathHelper.PiOver2;
            projectile.Opacity = Utils.InverseLerp(0f, 15f, projectile.timeLeft, true);

            if (Time > AttackDelay)
                CalamityGlobalProjectile.HomeInOnNPC(projectile, false, 600f, 14f, 32f);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            lightColor = Color.Lerp(lightColor, Color.White, 0.8f);
            lightColor.A /= 3;
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (Time > AttackDelay)
                return null;
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 60);
        }
    }
}
