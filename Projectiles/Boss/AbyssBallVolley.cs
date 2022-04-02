using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class AbyssBallVolley : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Abyss Ball Volley");
        }

        public override void SetDefaults()
        {
            projectile.Calamity().canBreakPlayerDefense = true;
            projectile.width = 30;
            projectile.height = 30;
            projectile.hostile = true;
            projectile.penetrate = 1;
            projectile.alpha = 60;
            projectile.tileCollide = false;
            projectile.timeLeft = (CalamityWorld.malice || BossRushEvent.BossRushActive) ? 780 : CalamityWorld.death ? 600 : CalamityWorld.revenge ? 540 : Main.expertMode ? 480 : 300;
            projectile.Calamity().affectedByMaliceModeVelocityMultiplier = true;
        }

        public override void AI()
        {
            if (projectile.velocity.Length() < 16f && (Main.expertMode || BossRushEvent.BossRushActive))
            {
                float velocityMult = (CalamityWorld.malice || BossRushEvent.BossRushActive) ? 1.025f : CalamityWorld.death ? 1.015f : CalamityWorld.revenge ? 1.0125f : Main.expertMode ? 1.01f : 1f;
                projectile.velocity *= velocityMult;
            }

            if (projectile.timeLeft < 60)
                projectile.Opacity = MathHelper.Clamp(projectile.timeLeft / 60f, 0f, 1f);

            if (projectile.ai[1] == 0f)
            {
                projectile.ai[1] = 1f;
                Main.PlaySound(SoundID.Item, (int)projectile.position.X, (int)projectile.position.Y, 33);
            }

            if (Main.rand.NextBool(2))
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 173, 0f, 0f);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(projectile.Center, 12f, targetHitbox);

        public override bool CanHitPlayer(Player target) => projectile.timeLeft >= 60;

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (projectile.timeLeft < 60)
                return;

            target.AddBuff(BuffID.Weak, 120);
        }
    }
}
