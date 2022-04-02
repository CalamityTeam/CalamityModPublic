using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class AureusBomber : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Aureus Bomber");
            Main.projFrames[projectile.type] = 3;
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 1;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 60;
            projectile.height = 60;
            projectile.netImportant = true;
            projectile.friendly = true;
            projectile.minionSlots = 0;
            projectile.timeLeft = 300;
            projectile.penetrate = 1;
            projectile.minion = true;
            projectile.tileCollide = false;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            int dustType = Utils.SelectRandom(Main.rand, new int[]
            {
                ModContent.DustType<AstralBlue>(),
                ModContent.DustType<AstralOrange>()
            });

            //on spawn effects && minion flexibility
            if (projectile.localAI[0] == 0f)
            {
                projectile.Calamity().spawnedPlayerMinionDamageValue = player.MinionDamage();
                projectile.Calamity().spawnedPlayerMinionProjectileDamageValue = projectile.damage;
                int num226 = 36;
                for (int num227 = 0; num227 < num226; num227++)
                {
                    Vector2 vector6 = Vector2.Normalize(projectile.velocity) * new Vector2((float)projectile.width / 2f, (float)projectile.height) * 0.75f;
                    vector6 = vector6.RotatedBy((double)((float)(num227 - (num226 / 2 - 1)) * 6.28318548f / (float)num226), default) + projectile.Center;
                    Vector2 vector7 = vector6 - projectile.Center;
                    int num228 = Dust.NewDust(vector6 + vector7, 0, 0, dustType, vector7.X * 1.5f, vector7.Y * 1.5f, 100, default, 1.4f);
                    Main.dust[num228].noGravity = true;
                    Main.dust[num228].noLight = true;
                    Main.dust[num228].velocity = vector7;
                }
                projectile.localAI[0] += 1f;
            }
            if (player.MinionDamage() != projectile.Calamity().spawnedPlayerMinionDamageValue)
            {
                int damage2 = (int)((float)projectile.Calamity().spawnedPlayerMinionProjectileDamageValue /
                    projectile.Calamity().spawnedPlayerMinionDamageValue *
                    player.MinionDamage());
                projectile.damage = damage2;
            }

            //anti sticking movement
            projectile.MinionAntiClump();

            //get in a line
            Vector2 idlePosition = player.Center;
            idlePosition.Y -= 48f;
            float minionPositionOffsetX = (10 + projectile.minionPos * 40) * -player.direction;
            idlePosition.X += minionPositionOffsetX;

            Vector2 vectorToIdlePosition = idlePosition - projectile.Center;
            float idleDistance = vectorToIdlePosition.Length();

            //dust effects
            if (Main.rand.NextBool(10))
            {
                int index = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, dustType, 0.0f, 0.0f, 100, Color.Transparent, 2f);
                Main.dust[index].velocity *= 0.3f;
                Main.dust[index].noGravity = true;
                Main.dust[index].noLight = true;
            }

            //frames
            projectile.frameCounter++;
            if (projectile.frameCounter > 6)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame >= 3)
            {
                projectile.frame = 0;
            }

            //direction
            if (projectile.velocity.X > 0f)
                projectile.spriteDirection = projectile.direction = -1;
            else if (projectile.velocity.X < 0f)
                projectile.spriteDirection = projectile.direction = 1;

            //tile collision
            projectile.tileCollide = true;
            if (projectile.ai[0] == 1f)
                projectile.tileCollide = false;

            //find nearby NPCs
            Vector2 objectivePos = projectile.position;
            float minDist = 400f;
            bool enemyFound = false;
            int npcIndex = -1;
            NPC targetedNPC = projectile.OwnerMinionAttackTargetNPC;
            if (targetedNPC != null && targetedNPC.CanBeChasedBy((object) projectile, false))
            {
                float distToEnemy = Vector2.Distance(targetedNPC.Center, projectile.Center);
                if (((double) Vector2.Distance(projectile.Center, objectivePos) > (double) distToEnemy && (double) distToEnemy < (double) minDist || !enemyFound) && Collision.CanHitLine(projectile.position, projectile.width, projectile.height, targetedNPC.position, targetedNPC.width, targetedNPC.height))
                {
                    minDist = distToEnemy;
                    objectivePos = targetedNPC.Center;
                    enemyFound = true;
                    npcIndex = targetedNPC.whoAmI;
                }
            }
            if (!enemyFound)
            {
                for (int index = 0; index < Main.npc.Length; ++index)
                {
                    NPC npc = Main.npc[index];
                    if (npc.CanBeChasedBy((object) projectile, false))
                    {
                        float distToEnemy = Vector2.Distance(npc.Center, projectile.Center);
                        if (((double) Vector2.Distance(projectile.Center, objectivePos) > (double) distToEnemy && (double) distToEnemy < (double) minDist || !enemyFound) && Collision.CanHitLine(projectile.position, projectile.width, projectile.height, npc.position, npc.width, npc.height))
                        {
                            minDist = distToEnemy;
                            objectivePos = npc.Center;
                            enemyFound = true;
                            npcIndex = index;
                        }
                    }
                }
            }

            int maxDistToEnemy = 500;
            if (enemyFound)
                maxDistToEnemy = 1000;

            //if too far, return to the player
            if ((double) Vector2.Distance(player.Center, projectile.Center) > (double) maxDistToEnemy)
            {
                projectile.ai[0] = 1f;
                projectile.netUpdate = true;
            }

            //movement if NPC found
            if (enemyFound && projectile.ai[0] == 0f)
            {
                float homingStrength = 15f;
                Vector2 projPos = new Vector2(projectile.position.X + (float)projectile.width * 0.5f, projectile.position.Y + (float)projectile.height * 0.5f);
                float distX = objectivePos.X - projPos.X;
                float distY = objectivePos.Y - projPos.Y;
                float distToTarget = (float)Math.Sqrt((double)(distX * distX + distY * distY));
                distToTarget = homingStrength / distToTarget;
                distX *= distToTarget;
                distY *= distToTarget;
                projectile.velocity.X = (projectile.velocity.X * 20f + distX) / 21f;
                projectile.velocity.Y = (projectile.velocity.Y * 20f + distY) / 21f;
            }
            else
            {
                //if player not in line of sight, go through tiles
                if (!Collision.CanHitLine(projectile.Center, 1, 1, player.Center, 1, 1))
                    projectile.ai[0] = 1f;

                float speedToPlayer = 6f;
                if (projectile.ai[0] == 1f)
                    speedToPlayer = 15f;

                Vector2 center = projectile.Center;
                Vector2 playerPos = player.Center - center + new Vector2(0.0f, -60f);
                playerPos = player.Center - center;
                float distToPlayer = playerPos.Length();
                if (distToPlayer > 200f && speedToPlayer < 9f)
                    speedToPlayer = 9f;
                speedToPlayer *= 0.75f;

                //idle distance
                if (distToPlayer < idleDistance && projectile.ai[0] == 1f && !Collision.SolidCollision(projectile.position, projectile.width, projectile.height))
                {
                    projectile.ai[0] = 0f;
                    projectile.netUpdate = true;
                }

                //way too far so teleport memes
                if (distToPlayer > 2000f)
                {
                    projectile.position.X = player.Center.X - (float) (projectile.width / 2);
                    projectile.position.Y = player.Center.Y - (float) (projectile.width / 2);
                }

                if (distToPlayer > 10f)
                {
                    playerPos.Normalize();
                    if (distToPlayer < 50f)
                        speedToPlayer /= 2f;
                    projectile.velocity = (projectile.velocity * 20f + playerPos * speedToPlayer) / 21f;
                }
                else
                {
                    projectile.direction = player.direction;
                    projectile.velocity = projectile.velocity * 0.9f;
                }
            }

            //stop if you trying to return to the player
            if (projectile.ai[0] != 0f)
                return;

            //stop if you didn't find an enemy
            if (!enemyFound)
                return;

            //face said enemy
            if ((objectivePos - projectile.Center).X > 0f)
                projectile.spriteDirection = projectile.direction = -1;
            else if ((objectivePos - projectile.Center).X < 0f)
                projectile.spriteDirection = projectile.direction = 1;
        }

        public override void Kill(int timeLeft)
        {
            projectile.position = projectile.Center;
            projectile.width = projectile.height = 150;
            projectile.position.X = projectile.position.X - (float)(projectile.width / 2);
            projectile.position.Y = projectile.position.Y - (float)(projectile.height / 2);
            projectile.maxPenetrate = -1;
            projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
            projectile.Damage();
            Main.PlaySound(SoundID.Item, (int)projectile.Center.X, (int)projectile.Center.Y, 14);
            for (int num193 = 0; num193 < 2; num193++)
            {
                Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, ModContent.DustType<AstralOrange>(), 0f, 0f, 50, default, 1f);
            }
            for (int num194 = 0; num194 < 20; num194++)
            {
                int num195 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, ModContent.DustType<AstralOrange>(), 0f, 0f, 0, default, 1.5f);
                Main.dust[num195].noGravity = true;
                Main.dust[num195].velocity *= 3f;
                num195 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, ModContent.DustType<AstralBlue>(), 0f, 0f, 50, default, 1f);
                Main.dust[num195].velocity *= 2f;
                Main.dust[num195].noGravity = true;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity) => false;

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 120);
        }
    }
}
