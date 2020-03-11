using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class EternityCircle : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Eternity");
        }

        public override void SetDefaults()
        {
            projectile.width = 2;
            projectile.height = 2;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.timeLeft = EternityHex.trueTimeLeft;
            projectile.alpha = 255;
            projectile.magic = true;
        }

        public override void AI()
        {
            // Circle around the enemy
            if (!Main.npc[(int)projectile.ai[0]].active)
            {
                projectile.active = false;
            }
            if (projectile.localAI[0] == 0f)
            {
                projectile.localAI[0] = Main.rand.NextFloat(MathHelper.TwoPi);
            }
            projectile.position = Main.npc[(int)projectile.ai[0]].Center + projectile.ai[1].ToRotationVector2() * 670f;
            projectile.ai[1] += MathHelper.ToRadians(3.5f);

            projectile.localAI[0] += 0.18f;

            if (projectile.localAI[1] >= Main.projectile.Length || projectile.localAI[0] < 0)
            {
                projectile.Kill();
                return;
            }

            Projectile book = Main.projectile[(int)projectile.localAI[1]];

            if (!book.active)
            {
                projectile.Kill();
                return;
            }

            float angle = projectile.velocity.ToRotation() + MathHelper.PiOver2;
            float pulse = (float)Math.Sin(projectile.localAI[0] * 3f);
            float radius = 8f;
            Vector2 offset = angle.ToRotationVector2() * pulse * radius;
            Dust dust = Dust.NewDustPerfect(projectile.Center + offset, 132, Vector2.Zero);
            dust.noGravity = true;
            dust = Dust.NewDustPerfect(projectile.Center - offset, 133, Vector2.Zero);
            dust.noGravity = true;
        }
    }
}