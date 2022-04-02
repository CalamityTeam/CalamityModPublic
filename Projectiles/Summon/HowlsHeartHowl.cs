using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class HowlsHeartHowl : ModProjectile
    {
        public bool initialized = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Howl");
            Main.projFrames[projectile.type] = 5;
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 72;
            projectile.height = 54;
            projectile.netImportant = true;
            projectile.friendly = true;
            projectile.minionSlots = 0f;
            projectile.timeLeft = 18000;
            projectile.penetrate = -1;
            projectile.timeLeft *= 5;
            projectile.minion = true;
            projectile.tileCollide = false;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();

            //Spawn dust and record initial damage values
            if (!initialized)
            {
                projectile.Calamity().spawnedPlayerMinionDamageValue = player.MinionDamage();
                projectile.Calamity().spawnedPlayerMinionProjectileDamageValue = projectile.damage;
                int dustAmt = 36;
                for (int dustIndex = 0; dustIndex < dustAmt; dustIndex++)
                {
                    Vector2 vector6 = Vector2.Normalize(projectile.velocity) * new Vector2((float)projectile.width / 2f, (float)projectile.height) * 0.75f;
                    vector6 = vector6.RotatedBy((double)((float)(dustIndex - (dustAmt / 2 - 1)) * MathHelper.TwoPi / (float)dustAmt), default) + projectile.Center;
                    Vector2 vector7 = vector6 - projectile.Center;
                    int dusty = Dust.NewDust(vector6 + vector7, 0, 0, 191, vector7.X * 1f, vector7.Y * 1f, 100, default, 1.1f);
                    Main.dust[dusty].noGravity = true;
                    Main.dust[dusty].noLight = true;
                    Main.dust[dusty].velocity = vector7;
                }
                initialized = true;
            }

            //if minion damage changes, update it
            if (player.MinionDamage() != projectile.Calamity().spawnedPlayerMinionDamageValue)
            {
                int damage2 = (int)((float)projectile.Calamity().spawnedPlayerMinionProjectileDamageValue /
                    projectile.Calamity().spawnedPlayerMinionDamageValue *
                    player.MinionDamage());
                projectile.damage = damage2;
            }

            //If the correct minion, set bools
            bool correctMinion = projectile.type == ModContent.ProjectileType<HowlsHeartHowl>();
            if (!modPlayer.howlsHeart && !modPlayer.howlsHeartVanity || !player.active)
            {
                projectile.active = false;
                return;
            }
            if (correctMinion)
            {
                if (player.dead)
                {
                    modPlayer.howlTrio = false;
                }
                if (modPlayer.howlTrio)
                {
                    projectile.timeLeft = 2;
                }
            }

            //Anti sticky movement although there should only be one

            //Set tile collision for only when trying to return to the player
            projectile.tileCollide = projectile.ai[0] != 1f;

            //Find an enemy
            Vector2 targetPos = projectile.position;
            float maxRange = 900f;
            bool foundEnemy = false;
            int targetIndex = -1;
            //If the player has targetted an enemy, choose that one
            NPC target = projectile.OwnerMinionAttackTargetNPC;
            if (target != null && target.CanBeChasedBy(projectile, false) && !modPlayer.howlsHeartVanity)
            {
                float targetDist = Vector2.Distance(target.Center, projectile.Center);
                if (!foundEnemy && targetDist < maxRange && Collision.CanHitLine(projectile.position, projectile.width, projectile.height, target.position, target.width, target.height))
                {
                    maxRange = targetDist;
                    targetPos = target.Center;
                    foundEnemy = true;
                    targetIndex = target.whoAmI;
                }
            }
            //else, search through all available NPCs
            if (!foundEnemy && !modPlayer.howlsHeartVanity)
            {
                for (int index = 0; index < Main.maxNPCs; ++index)
                {
                    NPC npc = Main.npc[index];
                    if (npc.CanBeChasedBy(projectile, false))
                    {
                        float targetDist = Vector2.Distance(npc.Center, projectile.Center);
                        if (!foundEnemy && targetDist < maxRange && Collision.CanHitLine(projectile.position, projectile.width, projectile.height, npc.position, npc.width, npc.height))
                        {
                            maxRange = targetDist;
                            targetPos = npc.Center;
                            foundEnemy = true;
                            targetIndex = index;
                        }
                    }
                }
            }

            //If the minion is too far away, return to the player. Max distance increases while attacking
            float maxDistanceFromPlayer = 1300f;
            if (foundEnemy)
                maxDistanceFromPlayer = 1600f;
            if (Vector2.Distance(player.Center, projectile.Center) > maxDistanceFromPlayer && projectile.ai[0] != 1f)
            {
                projectile.ai[0] = 1f;
                projectile.netUpdate = true;
            }

            //Move toward an enemy if one has been found
            if (foundEnemy && projectile.ai[0] == 0f)
            {
                Vector2 homeInVector = targetPos - projectile.Center;
                float targetDist = homeInVector.Length();
                homeInVector.Normalize();
                if (targetDist > 200f)
                {
                    float velocity = 6f;
                    projectile.velocity = (projectile.velocity * 40f + homeInVector * velocity) / 41f;
                }
                else
                {
                    if (targetDist < 150f)
                    {
                        float velocity = -4f;
                        projectile.velocity = (projectile.velocity * 40f + homeInVector * velocity) / 41f;
                    }
                    else
                        projectile.velocity *= 0.97f;
                }
            }
            else
            {
                //Return to the player if you can't see the player
                if (!Collision.CanHitLine(projectile.Center, 1, 1, player.Center, 1, 1))
                    projectile.ai[0] = 1f;

                //Set speed to home in on the player. If returning to the player, go faster
                float speed = 9f;
                if (projectile.ai[0] == 1f)
                    speed = 22f;

                //Find the player and align accordingly.  Get in a line if there's more than one.
                Vector2 playerPos = player.Center - projectile.Center;
                projectile.netUpdate = true;
                int minionPosition = 1;
                for (int index = 0; index < projectile.whoAmI; ++index)
                {
                    Projectile proj = Main.projectile[index];
                    if (proj.active && proj.owner == projectile.owner && proj.type == projectile.type)
                        ++minionPosition;
                }
                playerPos.X -= 10f * player.direction;
                playerPos.X -= minionPosition * 60f * player.direction;
                playerPos.Y -= 10f;

                //Calculate player distance
                float playerDist = playerPos.Length();
                //If too far, increase speed
                if (playerDist > 200f && speed < 15f)
                    speed = 15f;
                //If you were trying to chase the player but are close enough now, return to normal
                if (playerDist < 100f && projectile.ai[0] == 1f && !Collision.SolidCollision(projectile.position, projectile.width, projectile.height))
                {
                    projectile.ai[0] = 0f;
                    projectile.netUpdate = true;
                }
                //Teleport to the player if really, really far
                if (playerDist > 2000f)
                {
                    projectile.position.X = player.Center.X - projectile.width / 2f;
                    projectile.position.Y = player.Center.Y - projectile.width / 2f;
                    projectile.netUpdate = true;
                }
                //Home in on the player
                if (playerDist > 10f)
                {
                    playerPos.Normalize();
                    if (playerDist < 50f)
                        speed /= 2f;
                    projectile.velocity = (projectile.velocity * 20f + playerPos * speed) / 21f;
                }
                else
                {
                    projectile.direction = -player.direction;
                    projectile.velocity *= 0.9f;
                }
            }

            //Rotation
            projectile.rotation = projectile.velocity.X * 0.05f;

            //Cycle through animation
            projectile.frameCounter++;
            if (projectile.frameCounter >= 6)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame >= Main.projFrames[projectile.type])
            {
                projectile.frame = 0;
            }

            //Set projectile direction based on its movement
            if (projectile.velocity.X > 0f)
                projectile.spriteDirection = projectile.direction = -1;
            else if (projectile.velocity.X < 0f)
                projectile.spriteDirection = projectile.direction = 1;

            //Increment firing cooldown
            if (projectile.ai[1] > 0f)
            {
                projectile.ai[1] += Main.rand.Next(1,4);
                if (Main.rand.NextBool(3))
                    ++projectile.ai[1];
            }
            //Determine if it should shoot
            if (projectile.ai[1] > 90f)
            {
                projectile.ai[1] = 0f;
                projectile.netUpdate = true;
            }

            //Return if trying to return to the player or no enemy has been found
            if (projectile.ai[0] != 0f || !foundEnemy)
                return;

            //Set projectile direction based on target location
            Vector2 targetVec = targetPos - projectile.Center;
            if (targetVec.X > 0f)
                projectile.spriteDirection = projectile.direction = -1;
            else if (targetVec.X < 0f)
                projectile.spriteDirection = projectile.direction = 1;

            //Return if firing cooldown isn't perfect
            if (projectile.ai[1] != 0f)
                return;
            ++projectile.ai[1];
            if (Main.myPlayer != projectile.owner)
                return;

            //Fire projectile
            float speedMult = 16f;
            targetVec.Normalize();
            targetVec *= speedMult;
            Main.PlaySound(SoundID.Item20, projectile.position);
            int fireball = Projectile.NewProjectile(projectile.Center, targetVec, ModContent.ProjectileType<HowlsHeartFireball>(), projectile.damage, projectile.knockBack, projectile.owner, targetIndex, 0f);
            Main.projectile[fireball].netUpdate = true;
            Main.projectile[fireball].frame = Main.rand.Next(4);
            projectile.netUpdate = true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity) => false;

        public override bool CanDamage() => false;
    }
}
