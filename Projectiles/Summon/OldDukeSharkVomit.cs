using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
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
            projectile.rotation = projectile.velocity.ToRotation();
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
                    if (dist < maxHomingRange)
                    {
                        targetIdx = player.MinionAttackTargetNPC;
                        maxHomingRange = dist;
                        hasHomingTarget = true;
                    }
                }
            }
            else
            {
                for (int i = 0; i < Main.npc.Length; ++i)
                {
                    NPC npc = Main.npc[i];
                    if (npc == null || !npc.active)
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
                Vector2 homingVector = (target.Center - projectile.Center).SafeNormalize(Vector2.Zero) * 25f;
                float homingRatio = 20f;
                projectile.velocity = (projectile.velocity * homingRatio + homingVector) / (homingRatio + 1f);
            }
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
