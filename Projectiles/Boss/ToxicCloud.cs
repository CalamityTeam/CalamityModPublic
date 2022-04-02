using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class ToxicCloud : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Enemy/ToxicMinnowCloud";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Toxic Cloud");
            Main.projFrames[projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 2;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.Calamity().canBreakPlayerDefense = true;
            projectile.width = 52;
            projectile.height = 48;
            projectile.hostile = true;
            projectile.Opacity = 0f;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.timeLeft = 900;
            projectile.Calamity().affectedByMaliceModeVelocityMultiplier = true;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, 0.1f, 0.7f, 0f);

            bool malice = CalamityWorld.malice || BossRushEvent.BossRushActive;
            if (projectile.velocity.Length() < (malice ? 6.25f : 5f))
                projectile.velocity *= 1.01f;

            projectile.frameCounter++;
            if (projectile.frameCounter > 9)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame > 3)
                projectile.frame = 0;

            projectile.ai[0] += 1f;
            if (projectile.ai[0] >= 860f)
            {
                projectile.damage = 0;
                if (projectile.Opacity > 0f)
                {
                    projectile.Opacity -= 0.02f;
                    if (projectile.Opacity <= 0f)
                    {
                        projectile.Opacity = 0f;
                        projectile.Kill();
                    }
                }
            }
            else if (projectile.Opacity < 0.9f)
            {
                projectile.Opacity += 0.12f;
                if (projectile.Opacity > 0.9f)
                    projectile.Opacity = 0.9f;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            lightColor.R = (byte)(255 * projectile.Opacity);
            lightColor.G = (byte)(255 * projectile.Opacity);
            lightColor.B = (byte)(255 * projectile.Opacity);
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(projectile.Center, 20f, targetHitbox);

        public override bool CanHitPlayer(Player target) => projectile.Opacity == 0.9f;

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (projectile.Opacity == 0.9f)
            {
                target.AddBuff(BuffID.Poisoned, 120);
                target.AddBuff(ModContent.BuffType<Irradiated>(), 120);
            }
        }
    }
}
