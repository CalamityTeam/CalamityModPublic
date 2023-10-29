using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class CausticStaffSummon : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public bool initialized = false;
        private float debuffToInflict = 0f;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 26;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.minionSlots = 1f;
            Projectile.timeLeft = 18000;
            Projectile.penetrate = -1;
            Projectile.timeLeft *= 5;
            Projectile.minion = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();

            //Spawn dust and record initial damage values
            if (!initialized)
            {
                int dustAmt = 36;
                for (int dustIndex = 0; dustIndex < dustAmt; dustIndex++)
                {
                    Vector2 rotate = Vector2.Normalize(Projectile.velocity) * new Vector2((float)Projectile.width / 2f, (float)Projectile.height) * 0.75f;
                    rotate = rotate.RotatedBy((double)((float)(dustIndex - (dustAmt / 2 - 1)) * MathHelper.TwoPi / (float)dustAmt), default) + Projectile.Center;
                    Vector2 faceDirection = rotate - Projectile.Center;
                    int dusty = Dust.NewDust(rotate + faceDirection, 0, 0, 6, faceDirection.X * 1f, faceDirection.Y * 1f, 100, default, 1.1f);
                    Main.dust[dusty].noGravity = true;
                    Main.dust[dusty].noLight = true;
                    Main.dust[dusty].velocity = faceDirection;
                }
                initialized = true;
            }

            //If the correct minion, set bools and apply buffs
            bool correctMinion = Projectile.type == ModContent.ProjectileType<CausticStaffSummon>();
            player.AddBuff(ModContent.BuffType<CausticStaffBuff>(), 3600);
            if (correctMinion)
            {
                if (player.dead)
                {
                    modPlayer.causticDragon = false;
                }
                if (modPlayer.causticDragon)
                {
                    Projectile.timeLeft = 2;
                }
            }

            //Anti sticky movement to prevent minions from stacking
            Projectile.MinionAntiClump();

            //Set tile collision for only when trying to return to the player
            Projectile.tileCollide = Projectile.ai[0] != 1f;

            //Find an enemy
            Vector2 targetPos = Projectile.position;
            float maxRange = 900f;
            bool foundEnemy = false;
            int targetIndex = -1;
            //If the player has targetted an enemy, choose that one
            NPC target = Projectile.OwnerMinionAttackTargetNPC;
            if (target != null && target.CanBeChasedBy(Projectile, false))
            {
                float targetDist = Vector2.Distance(target.Center, Projectile.Center);
                if (!foundEnemy && targetDist < maxRange && Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, target.position, target.width, target.height))
                {
                    maxRange = targetDist;
                    targetPos = target.Center;
                    foundEnemy = true;
                    targetIndex = target.whoAmI;
                }
            }
            //else, search through all available NPCs
            if (!foundEnemy)
            {
                for (int index = 0; index < Main.maxNPCs; ++index)
                {
                    NPC npc = Main.npc[index];
                    if (npc.CanBeChasedBy(Projectile, false))
                    {
                        float targetDist = Vector2.Distance(npc.Center, Projectile.Center);
                        if (!foundEnemy && targetDist < maxRange && Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, npc.position, npc.width, npc.height))
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
            if (Vector2.Distance(player.Center, Projectile.Center) > maxDistanceFromPlayer && Projectile.ai[0] != 1f)
            {
                Projectile.ai[0] = 1f;
                Projectile.netUpdate = true;
            }

            //Move toward an enemy if one has been found
            if (foundEnemy && Projectile.ai[0] == 0f)
            {
                Vector2 homeInVector = targetPos - Projectile.Center;
                float targetDist = homeInVector.Length();
                homeInVector.Normalize();
                if (targetDist > 200f)
                {
                    float velocity = 6f;
                    Projectile.velocity = (Projectile.velocity * 40f + homeInVector * velocity) / 41f;
                }
                else
                {
                    if (targetDist < 150f)
                    {
                        float velocity = -4f;
                        Projectile.velocity = (Projectile.velocity * 40f + homeInVector * velocity) / 41f;
                    }
                    else
                        Projectile.velocity *= 0.97f;
                }
            }
            else
            {
                //Return to the player if you can't see the player
                if (!Collision.CanHitLine(Projectile.Center, 1, 1, player.Center, 1, 1))
                    Projectile.ai[0] = 1f;

                //Set speed to home in on the player. If returning to the player, go faster
                float speed = 9f;
                if (Projectile.ai[0] == 1f)
                    speed = 22f;

                //Find the player and align accordingly.  Get in a line if there's more than one.
                Vector2 playerPos = player.Center - Projectile.Center;
                Projectile.netUpdate = true;
                int minionPosition = 1;
                for (int index = 0; index < Projectile.whoAmI; ++index)
                {
                    Projectile proj = Main.projectile[index];

                    // Short circuits to make the loop as fast as possible
                    if (!proj.active || proj.owner != Projectile.owner || !proj.minion)
                        continue;

                    if (proj.type == Projectile.type)
                        ++minionPosition;
                }
                playerPos.X -= 10f * player.direction;
                playerPos.X -= minionPosition * 40f * player.direction;
                playerPos.Y -= 10f;

                //Calculate player distance
                float playerDist = playerPos.Length();
                //If too far, increase speed
                if (playerDist > 200f && speed < 15f)
                    speed = 15f;
                //If you were trying to chase the player but are close enough now, return to normal
                if (playerDist < 100f && Projectile.ai[0] == 1f && !Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
                {
                    Projectile.ai[0] = 0f;
                    Projectile.netUpdate = true;
                }
                //Teleport to the player if really, really far
                if (playerDist > 2000f)
                {
                    Projectile.position.X = player.Center.X - Projectile.width / 2f;
                    Projectile.position.Y = player.Center.Y - Projectile.width / 2f;
                    Projectile.netUpdate = true;
                }
                //Home in on the player
                if (playerDist > 10f)
                {
                    playerPos.Normalize();
                    if (playerDist < 50f)
                        speed /= 2f;
                    Projectile.velocity = (Projectile.velocity * 20f + playerPos * speed) / 21f;
                }
                else
                {
                    Projectile.direction = player.direction;
                    Projectile.velocity *= 0.9f;
                }
            }

            //Rotation
            Projectile.rotation = Projectile.velocity.X * 0.05f;

            //Cycle through animation
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 6)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame >= Main.projFrames[Projectile.type])
            {
                Projectile.frame = 0;
            }

            //Occasionally spawn fiery dust
            if (Main.rand.NextBool(6))
            {
                int fire = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 6, 0f, 0f, 100, new Color(), 2f);
                Main.dust[fire].velocity *= 0.3f;
                Main.dust[fire].noGravity = true;
                Main.dust[fire].noLight = true;
            }

            //Set projectile direction based on its movement
            if (Projectile.velocity.X > 0f)
                Projectile.spriteDirection = Projectile.direction = -1;
            else if (Projectile.velocity.X < 0f)
                Projectile.spriteDirection = Projectile.direction = 1;

            //Increment firing cooldown
            if (Projectile.ai[1] > 0f)
            {
                Projectile.ai[1] += Main.rand.Next(1,4);
                if (Main.rand.NextBool(3))
                    ++Projectile.ai[1];
            }
            //Determine if it should shoot
            if (Projectile.ai[1] > 90f)
            {
                Projectile.ai[1] = 0f;
                Projectile.netUpdate = true;
            }

            //Return if trying to return to the player or no enemy has been found
            if (Projectile.ai[0] != 0f || !foundEnemy)
                return;

            //Set projectile direction based on target location
            Vector2 targetVec = targetPos - Projectile.Center;
            if (targetVec.X > 0f)
                Projectile.spriteDirection = Projectile.direction = -1;
            else if (targetVec.X < 0f)
                Projectile.spriteDirection = Projectile.direction = 1;

            //Return if firing cooldown isn't perfect
            if (Projectile.ai[1] != 0f)
                return;
            ++Projectile.ai[1];
            if (Main.myPlayer != Projectile.owner)
                return;

            //Fire projectile
            float speedMult = 16f;
            targetVec.Normalize();
            targetVec *= speedMult;
            int spike = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, targetVec, ModContent.ProjectileType<CausticStaffProjectile>(), Projectile.damage, Projectile.knockBack, Projectile.owner, debuffToInflict, 0f);
            debuffToInflict++;
            if (debuffToInflict >= 5f)
                debuffToInflict = 0f;
            Main.projectile[spike].netUpdate = true;
            Projectile.netUpdate = true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity) => false;

        public override bool? CanDamage() => false;
    }
}
