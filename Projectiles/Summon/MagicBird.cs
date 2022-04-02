using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class MagicBird : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bird");
            Main.projFrames[projectile.type] = 7;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
            ProjectileID.Sets.MinionShot[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.friendly = true;
            projectile.width = 14;
            projectile.height = 14;
            projectile.minion = true;
            projectile.minionSlots = 0f;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.extraUpdates = 1;
            projectile.alpha = 255;
            projectile.timeLeft = 180;
        }

        public override void AI()
        {
            projectile.spriteDirection = projectile.direction = (projectile.velocity.X > 0).ToDirectionInt();
            projectile.rotation = projectile.velocity.ToRotation() + (projectile.spriteDirection == 1 ? 0f : MathHelper.Pi);

            projectile.frameCounter++;
            if (projectile.frameCounter > 6)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame >= 7)
            {
                projectile.frame = 0;
            }

            if (projectile.ai[0]++ % 20f == 0f)
            {
                Vector2 dspeed = projectile.velocity * Main.rand.NextFloat(0.3f,0.6f);
                int dust = Dust.NewDust(projectile.Center, 1, 1, 67, dspeed.X, dspeed.Y, 50, default, 1.2f);
                Main.dust[dust].noGravity = true;
            }

            //Homing
            if (projectile.ai[1]++ > 30f)
                HomingAI();

            //Fade in
            if (projectile.alpha > 0)
            {
                projectile.alpha -= 15;
            }
            if (projectile.alpha < 0)
            {
                projectile.alpha = 0;
            }
        }

        private void HomingAI()
        {
            Player player = Main.player[projectile.owner];
            int targetIdx = -1;
            float maxHomingRange = MagicHat.Range;
            bool hasHomingTarget = false;
            if (player.HasMinionAttackTargetNPC)
            {
                NPC npc = Main.npc[player.MinionAttackTargetNPC];
                if (npc.CanBeChasedBy(projectile, false))
                {
                    float dist = (projectile.Center - npc.Center).Length();
                    if (dist < maxHomingRange)
                    {
                        targetIdx = player.MinionAttackTargetNPC;
                        maxHomingRange = dist;
                        hasHomingTarget = true;
                    }
                }
            }
            if (!hasHomingTarget)
            {
                for (int i = 0; i < Main.npc.Length; ++i)
                {
                    NPC npc = Main.npc[i];
                    if (npc is null || !npc.active)
                        continue;

                    if (npc.CanBeChasedBy(projectile, false))
                    {
                        float dist = (projectile.Center - npc.Center).Length();
                        if (dist < maxHomingRange)
                        {
                            targetIdx = i;
                            maxHomingRange = dist;
                            hasHomingTarget = true;
                        }
                    }
                }
            }

            // Home in on said closest NPC.
            if (hasHomingTarget)
            {
                NPC target = Main.npc[targetIdx];
                Vector2 homingVector = (target.Center - projectile.Center).SafeNormalize(Vector2.Zero) * 32f;
                float homingRatio = 15f;
                projectile.velocity = (projectile.velocity * homingRatio + homingVector) / (homingRatio + 1f);
            }
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 10; i++)
            {
                Vector2 dspeed = new Vector2(Main.rand.NextFloat(-7f, 7f), Main.rand.NextFloat(-7f, 7f));
                int dust = Dust.NewDust(projectile.Center, 1, 1, 67, dspeed.X, dspeed.Y, 50, default, 1.2f);
                Main.dust[dust].noGravity = true;
            }
        }

        public override Color? GetAlpha(Color lightColor) => new Color(255, 239, 0, projectile.alpha);

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }
    }
}
