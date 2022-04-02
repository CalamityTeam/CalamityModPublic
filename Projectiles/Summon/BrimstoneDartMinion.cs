using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class BrimstoneDartMinion : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Boss/BrimstoneBarrage";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Brimfire");
            Main.projFrames[projectile.type] = 4;
            ProjectileID.Sets.MinionShot[projectile.type] = true;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 2;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 18;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.timeLeft = 300;
            projectile.penetrate = 1;
            projectile.minion = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
            projectile.alpha = 255;
        }

        public override void AI()
        {
            projectile.frameCounter++;
            projectile.frame = projectile.frameCounter / 5 % Main.projFrames[projectile.type];
            projectile.Opacity = Utils.InverseLerp(300f, 285f, projectile.timeLeft, true) * Utils.InverseLerp(0f, 15f, projectile.timeLeft, true);
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;

            if (projectile.velocity.Length() < 35f)
                projectile.velocity *= 1.036f;

            Lighting.AddLight(projectile.Center, 0.75f, 0f, 0f);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit) => target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 240);

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            lightColor.R = (byte)(255 * projectile.Opacity);
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }
    }
}
