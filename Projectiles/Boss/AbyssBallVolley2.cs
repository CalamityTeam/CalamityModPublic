using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class AbyssBallVolley2 : ModProjectile
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
            Projectile.Opacity = 0.8f;
            Projectile.tileCollide = false;
            Projectile.timeLeft = (CalamityWorld.malice || BossRushEvent.BossRushActive) ? 640 : CalamityWorld.death ? 490 : CalamityWorld.revenge ? 440 : Main.expertMode ? 390 : 240;
        }

        public override void AI()
        {
            // Fly up and then fall down
            if (Projectile.ai[0] == 1f)
            {
                if (Projectile.ai[1] < 60f)
                {
                    Projectile.ai[1] += 1f;
                }
                else
                {
                    if (Projectile.velocity.Y < 12f)
                        Projectile.velocity.Y += 0.2f;
                }
            }

            // Accelerate
            else
            {
                if (Projectile.velocity.Length() < 15f && (Main.expertMode || BossRushEvent.BossRushActive))
                {
                    float velocityMult = (CalamityWorld.malice || BossRushEvent.BossRushActive) ? 1.025f : CalamityWorld.death ? 1.015f : CalamityWorld.revenge ? 1.0125f : Main.expertMode ? 1.01f : 1.005f;
                    Projectile.velocity *= velocityMult;
                }
            }

            if (Projectile.timeLeft < 60)
                Projectile.Opacity = MathHelper.Lerp(0f, 0.8f, Projectile.timeLeft / 60f);

            if (Main.rand.NextBool())
            {
                int dust = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 4, 0f, 0f, Projectile.alpha, Color.Crimson);
                Main.dust[dust].noGravity = true;
            }

            Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.05f;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(Projectile.Center, 12f, targetHitbox);

        public override bool CanHitPlayer(Player target) => Projectile.timeLeft >= 60;

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (Projectile.timeLeft < 60)
                return;

            target.AddBuff(BuffID.Darkness, 180);
        }
    }
}
