using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class IceSentry : ModProjectile
    {
        private bool setDamage = true;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ice Sentry");
            Main.projFrames[projectile.type] = 18;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 94;
            projectile.height = 94;
            projectile.timeLeft = Projectile.SentryLifeTime;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.sentry = true;
            projectile.coldDamage = true;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            if (setDamage)
            {
                projectile.Calamity().spawnedPlayerMinionDamageValue = player.MinionDamage();
                projectile.Calamity().spawnedPlayerMinionProjectileDamageValue = projectile.damage;
                setDamage = false;
            }
            if (player.MinionDamage() != projectile.Calamity().spawnedPlayerMinionDamageValue)
            {
                int damage2 = (int)((float)projectile.Calamity().spawnedPlayerMinionProjectileDamageValue /
                    projectile.Calamity().spawnedPlayerMinionDamageValue *
                    player.MinionDamage());
                projectile.damage = damage2;
            }

            projectile.velocity = Vector2.Zero;

            projectile.frameCounter++;
            if (projectile.frameCounter > 6)
            {
                projectile.frameCounter = 0;
                projectile.frame++;
            }

            if (projectile.ai[1] < 300f)
            {
                projectile.localAI[1] = 1f;
                if (projectile.frame >= 9)
                    projectile.frame = 0;
            }
            else
            {
                if (projectile.frame >= 18)
                    projectile.frame = 9;

                projectile.localAI[1]++;
                if (projectile.localAI[1] > 2)
                {
                    projectile.localAI[1] = 0;

                    if (projectile.owner == Main.myPlayer)
                    {
                        Vector2 speed = new Vector2(Main.rand.Next(-1000, 1001), Main.rand.Next(-1000, 1001));
                        speed.Normalize();
                        speed *= 15f;
                        int shard = Projectile.NewProjectile(projectile.Center + speed, speed, ModContent.ProjectileType<IceSentryShard>(), projectile.damage / 2, projectile.knockBack / 2, projectile.owner);
                    }
                }
            }

            NPC minionAttackTargetNpc = projectile.OwnerMinionAttackTargetNPC;
            if (minionAttackTargetNpc != null && projectile.ai[0] != minionAttackTargetNpc.whoAmI && minionAttackTargetNpc.CanBeChasedBy(projectile) && Collision.CanHit(projectile.Center, 0, 0, minionAttackTargetNpc.position, minionAttackTargetNpc.width, minionAttackTargetNpc.height))
            {
                projectile.ai[0] = minionAttackTargetNpc.whoAmI;
                projectile.ai[1] = 0f;
                projectile.localAI[0] = 0f;
                projectile.netUpdate = true;
            }

            if (projectile.ai[0] >= 0 && projectile.ai[0] < Main.maxNPCs)
            {
                NPC npc = Main.npc[(int)projectile.ai[0]];

                bool rememberTarget = npc.CanBeChasedBy(projectile);
                if (rememberTarget)
                {
                    projectile.localAI[0]++;

                    if (projectile.ai[1] < 300f)
                        projectile.ai[1]++;

                    float delay = 60f - projectile.ai[1] / 60f * 10f;
                    if (projectile.localAI[0] > delay)
                    {
                        projectile.localAI[0] = 0f;

                        rememberTarget = Collision.CanHit(projectile.Center, 0, 0, npc.position, npc.width, npc.height);
                        if (rememberTarget && projectile.owner == Main.myPlayer)
                        {
                            Vector2 speed = npc.Center - projectile.Center;
                            speed.Normalize();
                            speed *= 8f;
                            if (projectile.ai[1] >= 300f)
                                speed = speed.RotatedBy(MathHelper.ToRadians(Main.rand.Next(-5, 6))) * 1.5f + npc.velocity / 2f;
                            Projectile.NewProjectile(projectile.Center, speed + npc.velocity / 2f, ModContent.ProjectileType<IceSentryFrostBolt>(), projectile.damage, projectile.knockBack, projectile.owner);
                        }
                    }
                }

                if (!rememberTarget)
                {
                    projectile.ai[0] = -1f;
                    projectile.ai[1] = 0f;
                    projectile.netUpdate = true;
                }
            }
            else
            {
                projectile.localAI[0] = 0f;

                float maxDistance = 1000f;
                int possibleTarget = -1;

                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (npc.CanBeChasedBy(projectile))
                    {
                        float npcDistance = projectile.Distance(npc.Center);
                        if (npcDistance < maxDistance)
                        {
                            maxDistance = npcDistance;
                            possibleTarget = i;
                        }
                    }
                }

                if (possibleTarget > 0)
                {
                    //Main.NewText("new target acquired");
                    projectile.ai[0] = possibleTarget;
                    projectile.ai[1] = 0f;
                    projectile.netUpdate = true;
                }
            }
        }

        public override bool CanDamage()
        {
            return false;
        }
    }
}
