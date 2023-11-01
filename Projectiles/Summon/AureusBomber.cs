using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Summon
{
    public class AureusBomber : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 3;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 1;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 60;
            Projectile.height = 60;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.minionSlots = 0;
            Projectile.timeLeft = 300;
            Projectile.penetrate = 1;
            Projectile.minion = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            int dustType = Utils.SelectRandom(Main.rand, new int[]
            {
                ModContent.DustType<AstralBlue>(),
                ModContent.DustType<AstralOrange>()
            });

            //on spawn effects && minion flexibility
            if (Projectile.localAI[0] == 0f)
            {
                int constant = 36;
                for (int i = 0; i < constant; i++)
                {
                    Vector2 rotate = Vector2.Normalize(Projectile.velocity) * new Vector2((float)Projectile.width / 2f, (float)Projectile.height) * 0.75f;
                    rotate = rotate.RotatedBy((double)((float)(i - (constant / 2 - 1)) * 6.28318548f / (float)constant), default) + Projectile.Center;
                    Vector2 faceDirection = rotate - Projectile.Center;
                    int astralDust = Dust.NewDust(rotate + faceDirection, 0, 0, dustType, faceDirection.X * 1.5f, faceDirection.Y * 1.5f, 100, default, 1.4f);
                    Main.dust[astralDust].noGravity = true;
                    Main.dust[astralDust].noLight = true;
                    Main.dust[astralDust].velocity = faceDirection;
                }
                Projectile.localAI[0] += 1f;
            }

            //anti sticking movement
            Projectile.MinionAntiClump();

            //get in a line
            Vector2 idlePosition = player.Center;
            idlePosition.Y -= 48f;
            float minionPositionOffsetX = (10 + Projectile.minionPos * 40) * -player.direction;
            idlePosition.X += minionPositionOffsetX;

            Vector2 vectorToIdlePosition = idlePosition - Projectile.Center;
            float idleDistance = vectorToIdlePosition.Length();

            //dust effects
            if (Main.rand.NextBool(10))
            {
                int index = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dustType, 0.0f, 0.0f, 100, Color.Transparent, 2f);
                Main.dust[index].velocity *= 0.3f;
                Main.dust[index].noGravity = true;
                Main.dust[index].noLight = true;
            }

            //frames
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 6)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame >= 3)
            {
                Projectile.frame = 0;
            }

            //direction
            if (Projectile.velocity.X > 0f)
                Projectile.spriteDirection = Projectile.direction = -1;
            else if (Projectile.velocity.X < 0f)
                Projectile.spriteDirection = Projectile.direction = 1;

            //tile collision
            Projectile.tileCollide = true;
            if (Projectile.ai[0] == 1f)
                Projectile.tileCollide = false;

            //find nearby NPCs
            Vector2 objectivePos = Projectile.position;
            float minDist = 400f;
            bool enemyFound = false;
            NPC targetedNPC = Projectile.OwnerMinionAttackTargetNPC;
            if (targetedNPC != null && targetedNPC.CanBeChasedBy(Projectile, false))
            {
                float distToEnemy = Vector2.Distance(targetedNPC.Center, Projectile.Center);
                if (((double) Vector2.Distance(Projectile.Center, objectivePos) > (double) distToEnemy && (double) distToEnemy < (double) minDist || !enemyFound) && Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, targetedNPC.position, targetedNPC.width, targetedNPC.height))
                {
                    minDist = distToEnemy;
                    objectivePos = targetedNPC.Center;
                    enemyFound = true;
                }
            }
            if (!enemyFound)
            {
                for (int index = 0; index < Main.npc.Length; ++index)
                {
                    NPC npc = Main.npc[index];
                    if (npc.CanBeChasedBy(Projectile, false))
                    {
                        float distToEnemy = Vector2.Distance(npc.Center, Projectile.Center);
                        if (((double) Vector2.Distance(Projectile.Center, objectivePos) > (double) distToEnemy && (double) distToEnemy < (double) minDist || !enemyFound) && Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, npc.position, npc.width, npc.height))
                        {
                            minDist = distToEnemy;
                            objectivePos = npc.Center;
                            enemyFound = true;
                        }
                    }
                }
            }

            int maxDistToEnemy = 500;
            if (enemyFound)
                maxDistToEnemy = 1000;

            //if too far, return to the player
            if (Vector2.Distance(player.Center, Projectile.Center) > maxDistToEnemy)
            {
                Projectile.ai[0] = 1f;
                Projectile.netUpdate = true;
            }

            //movement if NPC found
            if (enemyFound && Projectile.ai[0] == 0f)
            {
                float homingStrength = 15f;
                float distX = objectivePos.X - Projectile.Center.X;
                float distY = objectivePos.Y - Projectile.Center.Y;
                float distToTarget = (float)Math.Sqrt((double)(distX * distX + distY * distY));
                distToTarget = homingStrength / distToTarget;
                distX *= distToTarget;
                distY *= distToTarget;
                Projectile.velocity.X = (Projectile.velocity.X * 20f + distX) / 21f;
                Projectile.velocity.Y = (Projectile.velocity.Y * 20f + distY) / 21f;
            }
            else
            {
                //if player not in line of sight, go through tiles
                if (!Collision.CanHitLine(Projectile.Center, 1, 1, player.Center, 1, 1))
                    Projectile.ai[0] = 1f;

                float speedToPlayer = 6f;
                if (Projectile.ai[0] == 1f)
                    speedToPlayer = 15f;

                Vector2 center = Projectile.Center;
                Vector2 playerPos = player.Center - center;
                float distToPlayer = playerPos.Length();
                if (distToPlayer > 200f && speedToPlayer < 9f)
                    speedToPlayer = 9f;
                speedToPlayer *= 0.75f;

                //idle distance
                if (distToPlayer < idleDistance && Projectile.ai[0] == 1f && !Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
                {
                    Projectile.ai[0] = 0f;
                    Projectile.netUpdate = true;
                }

                //way too far so teleport memes
                if (distToPlayer > 2000f)
                {
                    Projectile.position.X = player.Center.X - Projectile.width / 2;
                    Projectile.position.Y = player.Center.Y - Projectile.width / 2;
                }

                if (distToPlayer > 10f)
                {
                    playerPos.Normalize();
                    if (distToPlayer < 50f)
                        speedToPlayer /= 2f;
                    Projectile.velocity = (Projectile.velocity * 20f + playerPos * speedToPlayer) / 21f;
                }
                else
                {
                    Projectile.direction = player.direction;
                    Projectile.velocity = Projectile.velocity * 0.9f;
                }
            }

            //stop if you trying to return to the player
            if (Projectile.ai[0] != 0f)
                return;

            //stop if you didn't find an enemy
            if (!enemyFound)
                return;

            //face said enemy
            if ((objectivePos - Projectile.Center).X > 0f)
                Projectile.spriteDirection = Projectile.direction = -1;
            else if ((objectivePos - Projectile.Center).X < 0f)
                Projectile.spriteDirection = Projectile.direction = 1;
        }

        public override void OnKill(int timeLeft)
        {
            Projectile.position = Projectile.Center;
            Projectile.width = Projectile.height = 150;
            Projectile.position.X = Projectile.position.X - Projectile.width / 2;
            Projectile.position.Y = Projectile.position.Y - Projectile.height / 2;
            Projectile.maxPenetrate = -1;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.Damage();
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            for (int j = 0; j < 2; j++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<AstralOrange>(), 0f, 0f, 50, default, 1f);
            }
            for (int k = 0; k < 20; k++)
            {
                int moreAstralDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<AstralOrange>(), 0f, 0f, 0, default, 1.5f);
                Main.dust[moreAstralDust].noGravity = true;
                Main.dust[moreAstralDust].velocity *= 3f;
                moreAstralDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<AstralBlue>(), 0f, 0f, 50, default, 1f);
                Main.dust[moreAstralDust].velocity *= 2f;
                Main.dust[moreAstralDust].noGravity = true;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity) => false;

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 120);
        }
    }
}
