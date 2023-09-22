using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Summon
{
    public class GastricBelcher : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        private bool initialized = false;
        private int bubbleCounter = 0;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.minionSlots = 1f;
            Projectile.timeLeft = 18000;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft *= 5;
            Projectile.minion = true;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            //Set namespaces
            Player player = Main.player[Projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            CalamityGlobalProjectile modProj = Projectile.Calamity();

            //On spawn effects
            if (!initialized)
            {
                //Spawn dust
                int dustAmt = 36;
                for (int dustIndex = 0; dustIndex < dustAmt; dustIndex++)
                {
                    int randomDust = Utils.SelectRandom(Main.rand, new int[]
                    {
                        33,
                        89
                    });
                    Vector2 direction = Vector2.Normalize(Projectile.velocity) * new Vector2(Projectile.width / 2f, Projectile.height) * 0.75f;
                    direction = direction.RotatedBy((double)((dustIndex - (dustAmt / 2f - 1f)) * MathHelper.TwoPi / dustAmt), default) + Projectile.Center;
                    Vector2 dustVel = direction - Projectile.Center;
                    int water = Dust.NewDust(direction + dustVel, 0, 0, randomDust, dustVel.X * 1.75f, dustVel.Y * 1.75f, 100, default, 1.1f);
                    Main.dust[water].noGravity = true;
                    Main.dust[water].velocity = dustVel;
                }
                initialized = true;
            }

            //Update frames
            if (Projectile.frameCounter++ % 6 == 0)
            {
                Projectile.frame++;
            }
            if (Projectile.frame >= Main.projFrames[Projectile.type])
            {
                Projectile.frame = 0;
            }

            //Set up buff and timeLeft
            bool typeCheck = Projectile.type == ModContent.ProjectileType<GastricBelcher>();
            player.AddBuff(ModContent.BuffType<GastricAberrationBuff>(), 3600);
            if (typeCheck)
            {
                if (player.dead)
                {
                    modPlayer.gastricBelcher = false;
                }
                if (modPlayer.gastricBelcher)
                {
                    Projectile.timeLeft = 2;
                }
            }

            //Anti sticky movement to prevent overlapping minions
            Projectile.MinionAntiClump();

            //Find a target
            float maxDistance = 700f;
            Vector2 targetVec = Projectile.position;
            bool foundTarget = false;
            //If targeting something, prioritize that enemy
            if (player.HasMinionAttackTargetNPC)
            {
                NPC npc = Main.npc[player.MinionAttackTargetNPC];
                if (npc.CanBeChasedBy(Projectile, false))
                {
                    float extraDist = (npc.width / 2) + (npc.height / 2);
                    float targetDist = Vector2.Distance(npc.Center, Projectile.Center);
                    bool canHit = true;
                    if (extraDist < maxDistance)
                        canHit = Collision.CanHit(Projectile.Center, 1, 1, npc.Center, 1, 1);
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
                    if (npc.CanBeChasedBy(Projectile, false))
                    {
                        float extraDist = (npc.width / 2) + (npc.height / 2);
                        float targetDist = Vector2.Distance(npc.Center, Projectile.Center);
                        bool canHit = true;
                        if (extraDist < maxDistance)
                            canHit = Collision.CanHit(Projectile.Center, 1, 1, npc.Center, 1, 1);
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
            if (Vector2.Distance(player.Center, Projectile.Center) > returnDist)
            {
                Projectile.ai[0] = 1f;
                Projectile.netUpdate = true;
            }

            //If a target is found, move toward it
            if (foundTarget && Projectile.ai[0] == 0f)
            {
                Vector2 vecToTarget = targetVec - Projectile.Center;
                float targetDist = vecToTarget.Length();
                vecToTarget.Normalize();
                //If farther than 200 pixels, move toward it
                if (targetDist > 200f)
                {
                    float speedMult = (targetDist > 400f) ? 12f : (targetDist > 250) ? 6f : 3f;
                    vecToTarget *= speedMult;
                    Projectile.velocity = (Projectile.velocity * 40f + vecToTarget) / 41f;
                }
                //Otherwise, back it up slowly
                else
                {
                    float speedMult = -3f;
                    vecToTarget *= speedMult;
                    Projectile.velocity = (Projectile.velocity * 40f + vecToTarget) / 41f;
                }
            }

            //If not targeting something, act passively
            else
            {
                bool returningToPlayer = false;
                if (!returningToPlayer)
                {
                    returningToPlayer = Projectile.ai[0] == 1f;
                }
                //Move faster if actively returning to the player
                float speedMult = 10f;
                if (returningToPlayer)
                {
                    speedMult = 21f;
                }
                Vector2 vecToPlayer = player.Center - Projectile.Center + new Vector2(0f, -60f);
                float playerDist = vecToPlayer.Length();
                //Slow down if near the player
                if (playerDist < 200f && speedMult > 8f)
                {
                    speedMult = 1f;
                }
                //If close enough to the player, return to normal
                if (playerDist < 150f && returningToPlayer && !Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
                {
                    Projectile.ai[0] = 0f;
                    Projectile.netUpdate = true;
                }
                //If abnormally far, teleport to the player
                if (playerDist > 2000f)
                {
                    Projectile.position.X = player.Center.X - (Projectile.width / 2f);
                    Projectile.position.Y = player.Center.Y - (Projectile.height / 2f);
                    Projectile.netUpdate = true;
                }
                //Move toward player if more than 70 pixels away
                if (playerDist > 70f)
                {
                    vecToPlayer.Normalize();
                    vecToPlayer *= speedMult;
                    Projectile.velocity = (Projectile.velocity * 40f + vecToPlayer) / 41f;
                }
                //Move if still
                else if (Projectile.velocity.X == 0f && Projectile.velocity.Y == 0f)
                {
                    Projectile.velocity.X = -0.15f;
                    Projectile.velocity.Y = -0.05f;
                }
            }

            //Update sprite direction and rotation
            if (foundTarget)
            {
                Projectile.spriteDirection = Projectile.direction = ((targetVec.X - Projectile.Center.X) > 0).ToDirectionInt();
                Projectile.rotation = Projectile.rotation.AngleTowards(Projectile.AngleTo(targetVec) + (Projectile.spriteDirection == 1 ? 0f : MathHelper.Pi), 0.1f);
            }
            else
            {
                Projectile.spriteDirection = Projectile.direction = (Projectile.velocity.X > 0).ToDirectionInt();
                Projectile.rotation = Projectile.velocity.ToRotation() + (Projectile.spriteDirection == 1 ? 0f : MathHelper.Pi);
            }

            //Increment attack cooldown
            if (Projectile.ai[1] > 0f)
            {
                Projectile.ai[1] += Main.rand.Next(1, 4);
            }
            //Set the minion to be ready for attack
            if (Projectile.ai[1] > 90f)
            {
                Projectile.ai[1] = 0f;
                Projectile.netUpdate = true;
            }

            //Return if on attack cooldown, has no target, or returning to the player
            if (Projectile.ai[0] != 0f || !foundTarget || Projectile.ai[1] != 0f)
                return;

            if (Main.myPlayer == Projectile.owner)
            {
                //Play vomit sound
                if (modPlayer.soundCooldown <= 0)
                {
                    SoundEngine.PlaySound(SoundID.NPCDeath13 with { Volume = SoundID.NPCDeath13.Volume * 0.5f }, Projectile.Center);
                    modPlayer.soundCooldown = Main.rand.Next(120, 180);
                }

                //Increment the attack counter
                Projectile.ai[1]++;

                int projType = ModContent.ProjectileType<GastricBelcherVomit>();

                //Calculate the general velocity
                Vector2 velocity = targetVec - Projectile.Center;
                velocity.Normalize();

                //Add some inaccuracy for the vomit projectiles
                float vomitSpeedMult = 16f;
                Vector2 vomitVel = velocity * vomitSpeedMult;
                vomitVel.Y += Main.rand.NextFloat(-30f, 30f) * 0.05f;
                vomitVel.X += Main.rand.NextFloat(-30f, 30f) * 0.05f;

                //Fire the vomit projectile
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, vomitVel, projType, Projectile.damage, Projectile.knockBack, Projectile.owner, Main.rand.Next(3), 0f);

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

                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, bubbleVel, ModContent.ProjectileType<GastricBelcherBubble>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    }
                }

                Projectile.netUpdate = true;
            }
        }

        //This minion does no contact damage
        public override bool? CanDamage() => false;

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int y6 = frameHeight * Projectile.frame;
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (Projectile.spriteDirection == -1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y6, texture.Width, frameHeight)), Projectile.GetAlpha(lightColor), Projectile.rotation, new Vector2(texture.Width / 2f, frameHeight / 2f), Projectile.scale, spriteEffects, 0);
            return false;
        }
    }
}
