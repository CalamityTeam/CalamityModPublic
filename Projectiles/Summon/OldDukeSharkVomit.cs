using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class OldDukeSharkVomit : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Puke");
            ProjectileID.Sets.MinionShot[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.friendly = true;
            projectile.width = 36;
            projectile.height = 36;
            projectile.minion = true;
            projectile.minionSlots = 0f;
            projectile.ignoreWater = true;
            projectile.tileCollide = true;
            projectile.timeLeft = 360;
            projectile.alpha = 255;
        }

        public override void AI()
        {
            projectile.ai[0] += 1f;
            if (projectile.ai[0] < 10f)
            {
                projectile.alpha = 255 - (int)(255 * projectile.ai[0] / 10f);
            }
            projectile.velocity.Y += 0.2f;
            if (projectile.velocity.X < 0f)
            {
                projectile.spriteDirection = -1;
                projectile.rotation = (float)Math.Atan2((double)-(double)projectile.velocity.Y, (double)-(double)projectile.velocity.X);
            }
            else
            {
                projectile.spriteDirection = 1;
                projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X);
            }
            //Homing
            if (projectile.ai[0] > 20f)
                HomingAI();
        }

        private void HomingAI()
        {
            Player player = Main.player[projectile.owner];
            int targetIdx = -1;
            float maxHomingRange = 600f;
            bool hasHomingTarget = false;
            if (player.HasMinionAttackTargetNPC)
            {
                NPC npc = Main.npc[player.MinionAttackTargetNPC];
                if (npc.CanBeChasedBy(projectile, false))
                {
                    float dist = (projectile.Center - npc.Center).Length();
                    if (dist < maxHomingRange && Collision.CanHit(projectile.Center, projectile.width, projectile.height, npc.Center, npc.width, npc.height))
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
                        if (dist < maxHomingRange && Collision.CanHit(projectile.Center, projectile.width, projectile.height, npc.Center, npc.width, npc.height))
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
                Vector2 homingVector = (target.Center - projectile.Center).SafeNormalize(Vector2.Zero) * 25f;
                float homingRatio = 20f;
                projectile.velocity = (projectile.velocity * homingRatio + homingVector) / (homingRatio + 1f);
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            SpriteEffects spriteEffects = projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Texture2D texture = Main.projectileTexture[projectile.type];
            Main.spriteBatch.Draw(texture, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, 0, texture.Width, texture.Height)), projectile.GetAlpha(lightColor), projectile.rotation, texture.Size() / 2f, projectile.scale, spriteEffects, 0f);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < Main.rand.Next(28, 41); i++)
            {
                Dust.NewDustPerfect(projectile.Center + Utils.NextVector2Unit(Main.rand) * Main.rand.NextFloat(10f),
                    (int)CalamityDusts.SulfurousSeaAcid,
                    Utils.NextVector2Unit(Main.rand) * Main.rand.NextFloat(1f, 4f));
            }
        }
    }
}
