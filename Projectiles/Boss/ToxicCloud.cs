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
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 2;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.Calamity().canBreakPlayerDefense = true;
            Projectile.width = 52;
            Projectile.height = 48;
            Projectile.hostile = true;
            Projectile.Opacity = 0f;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 900;
            Projectile.Calamity().affectedByMaliceModeVelocityMultiplier = true;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0.1f, 0.7f, 0f);

            bool malice = CalamityWorld.malice || BossRushEvent.BossRushActive;
            if (Projectile.velocity.Length() < (malice ? 6.25f : 5f))
                Projectile.velocity *= 1.01f;

            Projectile.frameCounter++;
            if (Projectile.frameCounter > 9)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame > 3)
                Projectile.frame = 0;

            Projectile.ai[0] += 1f;
            if (Projectile.ai[0] >= 860f)
            {
                Projectile.damage = 0;
                if (Projectile.Opacity > 0f)
                {
                    Projectile.Opacity -= 0.02f;
                    if (Projectile.Opacity <= 0f)
                    {
                        Projectile.Opacity = 0f;
                        Projectile.Kill();
                    }
                }
            }
            else if (Projectile.Opacity < 0.9f)
            {
                Projectile.Opacity += 0.12f;
                if (Projectile.Opacity > 0.9f)
                    Projectile.Opacity = 0.9f;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            lightColor.R = (byte)(255 * Projectile.Opacity);
            lightColor.G = (byte)(255 * Projectile.Opacity);
            lightColor.B = (byte)(255 * Projectile.Opacity);
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(Projectile.Center, 20f, targetHitbox);

        public override bool CanHitPlayer(Player target) => Projectile.Opacity == 0.9f;

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (Projectile.Opacity == 0.9f)
            {
                target.AddBuff(BuffID.Poisoned, 240);
                target.AddBuff(ModContent.BuffType<Irradiated>(), 240);
            }
        }
    }
}
