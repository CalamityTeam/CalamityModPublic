using CalamityMod.Buffs.StatDebuffs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Enemy
{
    public class MaulerAcidDrop : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Environment/AcidDrop";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Acid");
        }

        public override void SetDefaults()
        {
            projectile.width = 16;
            projectile.height = 16;
            projectile.penetrate = -1;
            projectile.hostile = true;
            projectile.timeLeft = 240;
            projectile.tileCollide = false;
            projectile.ignoreWater = false;
        }
        public override void AI()
        {
            float homingSpeed = 16f;
            Player target = Main.player[Player.FindClosest(projectile.Center, 1, 1)];
            if (projectile.WithinRange(target.Center, 1200f) && projectile.timeLeft < 210)
                projectile.velocity = (projectile.velocity * 59f + projectile.SafeDirectionTo(target.Center) * homingSpeed) / 60f;
            projectile.Opacity = Utils.InverseLerp(0f, 25f, projectile.timeLeft, true);
            projectile.rotation = projectile.velocity.ToRotation() - MathHelper.PiOver2;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Irradiated>(), 60);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            lightColor = Color.White;
            lightColor.A = 64;
            return lightColor * projectile.Opacity;
        }
    }
}
