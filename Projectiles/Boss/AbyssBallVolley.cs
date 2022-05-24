using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

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
            Projectile.Calamity().canBreakPlayerDefense = true;
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.hostile = true;
            Projectile.penetrate = 1;
            Projectile.alpha = 60;
            Projectile.tileCollide = false;
            Projectile.timeLeft = (CalamityWorld.malice || BossRushEvent.BossRushActive) ? 780 : CalamityWorld.death ? 600 : CalamityWorld.revenge ? 540 : Main.expertMode ? 480 : 300;
            Projectile.Calamity().affectedByMaliceModeVelocityMultiplier = true;
        }

        public override void AI()
        {
            if (Projectile.velocity.Length() < 12f && (Main.expertMode || BossRushEvent.BossRushActive))
            {
                float velocityMult = (CalamityWorld.malice || BossRushEvent.BossRushActive) ? 1.025f : CalamityWorld.death ? 1.015f : CalamityWorld.revenge ? 1.0125f : Main.expertMode ? 1.01f : 1f;
                Projectile.velocity *= velocityMult;
            }

            if (Projectile.timeLeft < 60)
                Projectile.Opacity = MathHelper.Clamp(Projectile.timeLeft / 60f, 0f, 1f);

            if (Projectile.ai[1] == 0f)
            {
                Projectile.ai[1] = 1f;
                SoundEngine.PlaySound(SoundID.Item33, Projectile.position);
            }

            if (Main.rand.NextBool(2))
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 173, 0f, 0f);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(Projectile.Center, 12f, targetHitbox);

        public override bool CanHitPlayer(Player target) => Projectile.timeLeft >= 60;

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (Projectile.timeLeft < 60)
                return;

            target.AddBuff(BuffID.Weak, 180);
        }
    }
}
