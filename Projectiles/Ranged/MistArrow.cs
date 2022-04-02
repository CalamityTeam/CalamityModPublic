using CalamityMod.Buffs.StatDebuffs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Ranged
{
    public class MistArrow : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Arrow");
            Main.projFrames[projectile.type] = 3;
        }

        public override void SetDefaults()
        {
            projectile.width = 14;
            projectile.height = 14;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.alpha = 255;
            projectile.penetrate = 1;
            projectile.aiStyle = 1;
            projectile.timeLeft = 300;
            aiType = ProjectileID.WoodenArrowFriendly;
            projectile.arrow = true;
            projectile.coldDamage = true;
            projectile.Calamity().pointBlankShotDuration = CalamityGlobalProjectile.basePointBlankShotDuration;
        }

        public override void AI()
        {
            //Animation
            projectile.frameCounter++;
            if (projectile.frameCounter > 9)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame > 2)
            {
                projectile.frame = 0;
            }

            //Fade in
            if (projectile.alpha > 5)
                projectile.alpha -= 15;
            if (projectile.alpha < 5)
                projectile.alpha = 5;

            //Rotation
            projectile.spriteDirection = projectile.direction = (projectile.velocity.X > 0).ToDirectionInt();
            projectile.rotation = projectile.velocity.ToRotation() + (projectile.spriteDirection == 1 ? 0f : MathHelper.Pi) + MathHelper.ToRadians(90) * projectile.direction;

            projectile.localAI[0] += 1f;
            if (projectile.localAI[0] > 4f)
            {             
                Vector2 dspeed = -projectile.velocity * Main.rand.NextFloat(0.3f, 0.6f);
                int num469 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 31, 0f, 0f, 100, new Color(237, 242, 242, 200), 1.2f);
                Main.dust[num469].noGravity = true;
                Main.dust[num469].velocity = dspeed;            
            }
        }

        public override void Kill(int timeLeft)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 31, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f, 100, new Color(237, 242, 242, 200), 1f);
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Frostburn, 180);
            target.AddBuff(ModContent.BuffType<GlacialState>(), 30);
        }
    }
}
