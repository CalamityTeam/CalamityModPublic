using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class MagicRifle : ModProjectile
    {
        private int counter = 0;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rifle");
            ProjectileID.Sets.MinionShot[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 30;
            projectile.netImportant = true;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.minionSlots = 0f;
            projectile.timeLeft = 180;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.minion = true;
        }

        public override void AI()
        {
            //Set player namespace
            Player player = Main.player[projectile.owner];

            //Anti sticky movement to prevent overlapping minions
            projectile.MinionAntiClump();

            //Try not to do anything at first
            counter++;
            if (counter == 30)
            {
                projectile.netUpdate = true;
            }
            else if (counter < 30)
            {
                return;
            }

            float homingRange = MagicHat.Range;
            Vector2 targetVec = projectile.position;
            bool foundTarget = false;
            //If targeting something, prioritize that enemy
            if (player.HasMinionAttackTargetNPC)
            {
                NPC npc = Main.npc[player.MinionAttackTargetNPC];
                if (npc.CanBeChasedBy(projectile, false))
                {
                    float extraDist = (npc.width / 2) + (npc.height / 2);
                    //Calculate distance between target and the projectile to know if it's too far or not
                    float targetDist = Vector2.Distance(npc.Center, projectile.Center);
                    if (!foundTarget && targetDist < (homingRange + extraDist))
                    {
                        homingRange = targetDist;
                        targetVec = npc.Center;
                        foundTarget = true;
                    }
                }
            }
            if (!foundTarget)
            {
                for (int npcIndex = 0; npcIndex < Main.maxNPCs; npcIndex++)
                {
                    NPC npc = Main.npc[npcIndex];
                    if (npc.CanBeChasedBy(projectile, false))
                    {
                        float extraDist = (npc.width / 2) + (npc.height / 2);
                        //Calculate distance between target and the projectile to know if it's too far or not
                        float targetDist = Vector2.Distance(npc.Center, projectile.Center);
                        if (!foundTarget && targetDist < (homingRange + extraDist))
                        {
                            homingRange = targetDist;
                            targetVec = npc.Center;
                            foundTarget = true;
                        }
                    }
                }
            }

            //If too far, make the minion start returning to the player.
            float separationAnxietyDist = 1600f;
            if (foundTarget)
            {
                //Max travel distance increases if targeting something
                separationAnxietyDist = 2600f;
            }
            if (Vector2.Distance(player.Center, projectile.Center) > separationAnxietyDist)
            {
                projectile.ai[0] = 1f;
                projectile.netUpdate = true;
            }

            //If a target is found, move toward it
            if (foundTarget && projectile.ai[0] == 0f)
            {
                Vector2 vecToTarget = targetVec - projectile.Center;
                float targetDist = vecToTarget.Length();
                vecToTarget.Normalize();
                //If farther than 200 pixels, move toward it
                if (targetDist > 200f)
                {
                    float speedMult = 18f; //12
                    vecToTarget *= speedMult;
                    projectile.velocity = (projectile.velocity * 40f + vecToTarget) / 41f;
                }
                //Otherwise, back it up slowly
                else
                {
                    float speedMult = -9f;
                    vecToTarget *= speedMult;
                    projectile.velocity = (projectile.velocity * 40f + vecToTarget) / 41f;
                }
            }

            //If not targeting something, act passively
            else
            {
                bool returningToPlayer = false;
                if (!returningToPlayer)
                {
                    returningToPlayer = projectile.ai[0] == 1f;
                }
                //Move faster if actively returning to the player
                float speedMult = 12f;
                if (returningToPlayer)
                {
                    speedMult = 30f;
                }
                Vector2 vecToPlayer = player.Center - projectile.Center + new Vector2(0f, -120f);
                float playerDist = vecToPlayer.Length();
                //Speed up if near the player
                if (playerDist < 200f && speedMult < 16f)
                {
                    speedMult = 16f;
                }
                //If close enough to the player, return to normal
                if (playerDist < 600f && returningToPlayer)
                {
                    projectile.ai[0] = 0f;
                    projectile.netUpdate = true;
                }
                //If abnormally far, teleport to the player
                if (playerDist > 2000f)
                {
                    projectile.position.X = player.Center.X - (float)(projectile.width / 2);
                    projectile.position.Y = player.Center.Y - (float)(projectile.height / 2);
                    projectile.netUpdate = true;
                }
                //Move toward player if more than 70 pixels away
                if (playerDist > 70f)
                {
                    vecToPlayer.Normalize();
                    vecToPlayer *= speedMult;
                    projectile.velocity = (projectile.velocity * 40f + vecToPlayer) / 41f;
                }
                //Move if still
                else if (projectile.velocity.X == 0f && projectile.velocity.Y == 0f)
                {
                    projectile.velocity.X = -0.15f;
                    projectile.velocity.Y = -0.05f;
                }
            }

            //Update rotation
            if (foundTarget)
            {
                projectile.spriteDirection = projectile.direction = ((targetVec.X - projectile.Center.X) > 0).ToDirectionInt();
                projectile.rotation = projectile.rotation.AngleTowards(projectile.AngleTo(targetVec) + (projectile.spriteDirection == 1 ? MathHelper.ToRadians(45) : MathHelper.ToRadians(135)), 0.1f);
            }
            else
            {
                projectile.spriteDirection = projectile.direction = (projectile.velocity.X > 0).ToDirectionInt();
                projectile.rotation = projectile.velocity.ToRotation() + (projectile.spriteDirection == 1 ? MathHelper.ToRadians(45) : MathHelper.ToRadians(135));
            }

            //Increment attack cooldown
            if (projectile.ai[1] > 0f)
            {
                projectile.ai[1] += Main.rand.Next(1, 4);
            }
            //Set the minion to be ready for attack
            if (projectile.ai[1] > 90f)
            {
                projectile.ai[1] = 0f;
                projectile.netUpdate = true;
            }

            //Return if on attack cooldown, has no target, or returning to the player
            if (projectile.ai[0] != 0f || !foundTarget || projectile.ai[1] != 0f)
                return;

            //Shoot a bullet
            if (Main.myPlayer == projectile.owner)
            {
                float projSpeed = 6f;
                int projType = ModContent.ProjectileType<MagicBullet>();
                if (Main.rand.NextBool(6))
                {
                    Main.PlaySound(SoundID.Item, (int)projectile.position.X, (int)projectile.position.Y, 20, 0.1f);
                }
                projectile.ai[1] += 1f;
                if (Main.myPlayer == projectile.owner)
                {
                    Vector2 velocity = targetVec - projectile.Center;
                    velocity.Normalize();
                    velocity *= projSpeed;
                    Main.PlaySound(SoundID.Item40, projectile.position);
                    Projectile.NewProjectile(projectile.Center, velocity, projType, projectile.damage, 0f, projectile.owner);
                    projectile.netUpdate = true;
                }
            }
        }

        public override Color? GetAlpha(Color lightColor) => new Color(148, 0, 211, projectile.alpha);

        public override bool CanDamage() => false;

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 10; i++)
            {
                Vector2 dspeed = new Vector2(Main.rand.NextFloat(-7f, 7f), Main.rand.NextFloat(-7f, 7f));
                int dust = Dust.NewDust(projectile.Center, 1, 1, 66, dspeed.X, dspeed.Y, 160, new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB), 0.75f);
                Main.dust[dust].noGravity = true;
            }
        }
    }
}
