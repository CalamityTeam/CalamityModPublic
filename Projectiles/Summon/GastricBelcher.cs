using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class GastricBelcher : ModProjectile
    {
        private bool initialized = false;
        private int bubbleCounter = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gastric Belcher");
            Main.projFrames[projectile.type] = 4;
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 40;
            projectile.height = 40;
            projectile.netImportant = true;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.minionSlots = 1f;
            projectile.timeLeft = 18000;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.timeLeft *= 5;
            projectile.minion = true;
        }

        public override void AI()
        {
            //Set namespaces
            Player player = Main.player[projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            CalamityGlobalProjectile modProj = projectile.Calamity();

            //On spawn effects
            if (!initialized)
            {
                //Set constants
                modProj.spawnedPlayerMinionDamageValue = player.MinionDamage();
                modProj.spawnedPlayerMinionProjectileDamageValue = projectile.damage;
                //Spawn dust
                int dustAmt = 36;
                for (int dustIndex = 0; dustIndex < dustAmt; dustIndex++)
                {
                    int randomDust = Utils.SelectRandom(Main.rand, new int[]
                    {
                        33,
                        89
                    });
                    Vector2 direction = Vector2.Normalize(projectile.velocity) * new Vector2(projectile.width / 2f, projectile.height) * 0.75f;
                    direction = direction.RotatedBy((double)((dustIndex - (dustAmt / 2f - 1f)) * MathHelper.TwoPi / dustAmt), default) + projectile.Center;
                    Vector2 dustVel = direction - projectile.Center;
                    int water = Dust.NewDust(direction + dustVel, 0, 0, randomDust, dustVel.X * 1.75f, dustVel.Y * 1.75f, 100, default, 1.1f);
                    Main.dust[water].noGravity = true;
                    Main.dust[water].velocity = dustVel;
                }
                initialized = true;
            }

            //Flexible minion damage update
            if (player.MinionDamage() != modProj.spawnedPlayerMinionDamageValue)
            {
                int damage2 = (int)(modProj.spawnedPlayerMinionProjectileDamageValue /
                    modProj.spawnedPlayerMinionDamageValue * player.MinionDamage());
                projectile.damage = damage2;
            }

            //Update frames
            if (projectile.frameCounter++ % 6 == 0)
            {
                projectile.frame++;
            }
            if (projectile.frame >= Main.projFrames[projectile.type])
            {
                projectile.frame = 0;
            }

            //Set up buff and timeLeft
            bool typeCheck = projectile.type == ModContent.ProjectileType<GastricBelcher>();
            player.AddBuff(ModContent.BuffType<GastricBelcherBuff>(), 3600);
            if (typeCheck)
            {
                if (player.dead)
                {
                    modPlayer.gastricBelcher = false;
                }
                if (modPlayer.gastricBelcher)
                {
                    projectile.timeLeft = 2;
                }
            }

            //Anti sticky movement to prevent overlapping minions
            projectile.MinionAntiClump();

            //Find a target
            float maxDistance = 700f;
            Vector2 targetVec = projectile.position;
            bool foundTarget = false;
            //If targeting something, prioritize that enemy
            if (player.HasMinionAttackTargetNPC)
            {
                NPC npc = Main.npc[player.MinionAttackTargetNPC];
                if (npc.CanBeChasedBy(projectile, false))
                {
                    float extraDist = (npc.width / 2) + (npc.height / 2);
                    float targetDist = Vector2.Distance(npc.Center, projectile.Center);
                    bool canHit = true;
                    if (extraDist < maxDistance)
                        canHit = Collision.CanHit(projectile.Center, 1, 1, npc.Center, 1, 1);
                    if (!foundTarget && targetDist < (maxDistance + extraDist) && canHit)
                    {
                        maxDistance = targetDist;
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
                        float targetDist = Vector2.Distance(npc.Center, projectile.Center);
                        bool canHit = true;
                        if (extraDist < maxDistance)
                            canHit = Collision.CanHit(projectile.Center, 1, 1, npc.Center, 1, 1);
                        if (!foundTarget && targetDist < (maxDistance + extraDist) && canHit)
                        {
                            maxDistance = targetDist;
                            targetVec = npc.Center;
                            foundTarget = true;
                        }
                    }
                }
            }

            //If too far, make the minion start returning to the player.
            float returnDist = 1000f;
            if (foundTarget)
            {
                //Max travel distance increases if targeting something
                returnDist = 2200f;
            }
            if (Vector2.Distance(player.Center, projectile.Center) > returnDist)
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
                    float speedMult = (targetDist > 400f) ? 12f : (targetDist > 250) ? 6f : 3f;
                    vecToTarget *= speedMult;
                    projectile.velocity = (projectile.velocity * 40f + vecToTarget) / 41f;
                }
                //Otherwise, back it up slowly
                else
                {
                    float speedMult = -3f;
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
                float speedMult = 10f;
                if (returningToPlayer)
                {
                    speedMult = 21f;
                }
                Vector2 vecToPlayer = player.Center - projectile.Center + new Vector2(0f, -60f);
                float playerDist = vecToPlayer.Length();
                //Slow down if near the player
                if (playerDist < 200f && speedMult > 8f)
                {
                    speedMult = 1f;
                }
                //If close enough to the player, return to normal
                if (playerDist < 150f && returningToPlayer && !Collision.SolidCollision(projectile.position, projectile.width, projectile.height))
                {
                    projectile.ai[0] = 0f;
                    projectile.netUpdate = true;
                }
                //If abnormally far, teleport to the player
                if (playerDist > 2000f)
                {
                    projectile.position.X = player.Center.X - (projectile.width / 2f);
                    projectile.position.Y = player.Center.Y - (projectile.height / 2f);
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

            //Update sprite direction and rotation
            if (foundTarget)
            {
                projectile.spriteDirection = projectile.direction = ((targetVec.X - projectile.Center.X) > 0).ToDirectionInt();
                projectile.rotation = projectile.rotation.AngleTowards(projectile.AngleTo(targetVec) + (projectile.spriteDirection == 1 ? 0f : MathHelper.Pi), 0.1f);
            }
            else
            {
                projectile.spriteDirection = projectile.direction = (projectile.velocity.X > 0).ToDirectionInt();
                projectile.rotation = projectile.velocity.ToRotation() + (projectile.spriteDirection == 1 ? 0f : MathHelper.Pi);
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

            if (Main.myPlayer == projectile.owner)
            {
                //Play vomit sound
                if (modPlayer.soundCooldown <= 0)
                {
                    Main.PlaySound(SoundID.NPCKilled, (int)projectile.Center.X, (int)projectile.Center.Y, 13, 0.5f, 0f);
                    modPlayer.soundCooldown = Main.rand.Next(120, 180);
                }

                //Increment the attack counter
                projectile.ai[1]++;

                int projType = ModContent.ProjectileType<GastricBelcherVomit>();

                //Calculate the general velocity
                Vector2 velocity = targetVec - projectile.Center;
                velocity.Normalize();

                //Add some inaccuracy for the vomit projectiles
                float vomitSpeedMult = 16f;
                Vector2 vomitVel = velocity * vomitSpeedMult;
                vomitVel.Y += Main.rand.NextFloat(-30f, 30f) * 0.05f;
                vomitVel.X += Main.rand.NextFloat(-30f, 30f) * 0.05f;

                //Fire the vomit projectile
                Projectile.NewProjectile(projectile.Center, vomitVel, projType, projectile.damage, projectile.knockBack, projectile.owner, Main.rand.Next(3), 0f);

                //Fire 5 bubbles for every three attacks
                if (bubbleCounter++ % 3 == 2)
                {
                    for (int projCount = 0; projCount < 5; projCount++)
                    {
                        //Add a shotgun spread to the bubbles
                        float bubbleSpeedMult = 14f;
                        Vector2 bubbleVel = velocity * bubbleSpeedMult;
                        bubbleVel.Y += Main.rand.NextFloat(-50f, 50f) * 0.05f;
                        bubbleVel.X += Main.rand.NextFloat(-50f, 50f) * 0.05f;

                        Projectile.NewProjectile(projectile.Center, bubbleVel, ModContent.ProjectileType<GastricBelcherBubble>(), projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
                    }
                }

                projectile.netUpdate = true;
            }
        }

        //This minion does no contact damage
        public override bool CanDamage() => false;

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = Main.projectileTexture[projectile.type];
            int frameHeight = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type];
            int y6 = frameHeight * projectile.frame;
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (projectile.spriteDirection == -1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y6, texture.Width, frameHeight)), projectile.GetAlpha(lightColor), projectile.rotation, new Vector2(texture.Width / 2f, frameHeight / 2f), projectile.scale, spriteEffects, 0f);
            return false;
        }
    }
}
