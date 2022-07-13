using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Summon
{
    public class MagicRifle : ModProjectile
    {
        private int counter = 0;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rifle");
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.minionSlots = 0f;
            Projectile.timeLeft = 180;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.minion = true;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            //Set player namespace
            Player player = Main.player[Projectile.owner];

            //Anti sticky movement to prevent overlapping minions
            Projectile.MinionAntiClump();

            //Try not to do anything at first
            counter++;
            if (counter == 30)
            {
                Projectile.netUpdate = true;
            }
            else if (counter < 30)
            {
                return;
            }

            float homingRange = MagicHat.Range;
            Vector2 targetVec = Projectile.position;
            bool foundTarget = false;
            //If targeting something, prioritize that enemy
            if (player.HasMinionAttackTargetNPC)
            {
                NPC npc = Main.npc[player.MinionAttackTargetNPC];
                if (npc.CanBeChasedBy(Projectile, false))
                {
                    float extraDist = (npc.width / 2) + (npc.height / 2);
                    //Calculate distance between target and the projectile to know if it's too far or not
                    float targetDist = Vector2.Distance(npc.Center, Projectile.Center);
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
                    if (npc.CanBeChasedBy(Projectile, false))
                    {
                        float extraDist = (npc.width / 2) + (npc.height / 2);
                        //Calculate distance between target and the projectile to know if it's too far or not
                        float targetDist = Vector2.Distance(npc.Center, Projectile.Center);
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
            if (Vector2.Distance(player.Center, Projectile.Center) > separationAnxietyDist)
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
                    float speedMult = 18f; //12
                    vecToTarget *= speedMult;
                    Projectile.velocity = (Projectile.velocity * 40f + vecToTarget) / 41f;
                }
                //Otherwise, back it up slowly
                else
                {
                    float speedMult = -9f;
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
                float speedMult = 12f;
                if (returningToPlayer)
                {
                    speedMult = 30f;
                }
                Vector2 vecToPlayer = player.Center - Projectile.Center + new Vector2(0f, -120f);
                float playerDist = vecToPlayer.Length();
                //Speed up if near the player
                if (playerDist < 200f && speedMult < 16f)
                {
                    speedMult = 16f;
                }
                //If close enough to the player, return to normal
                if (playerDist < 600f && returningToPlayer)
                {
                    Projectile.ai[0] = 0f;
                    Projectile.netUpdate = true;
                }
                //If abnormally far, teleport to the player
                if (playerDist > 2000f)
                {
                    Projectile.position.X = player.Center.X - (float)(Projectile.width / 2);
                    Projectile.position.Y = player.Center.Y - (float)(Projectile.height / 2);
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

            //Update rotation
            if (foundTarget)
            {
                Projectile.spriteDirection = Projectile.direction = ((targetVec.X - Projectile.Center.X) > 0).ToDirectionInt();
                Projectile.rotation = Projectile.rotation.AngleTowards(Projectile.AngleTo(targetVec) + (Projectile.spriteDirection == 1 ? MathHelper.ToRadians(45) : MathHelper.ToRadians(135)), 0.1f);
            }
            else
            {
                Projectile.spriteDirection = Projectile.direction = (Projectile.velocity.X > 0).ToDirectionInt();
                Projectile.rotation = Projectile.velocity.ToRotation() + (Projectile.spriteDirection == 1 ? MathHelper.ToRadians(45) : MathHelper.ToRadians(135));
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

            //Shoot a bullet
            if (Main.myPlayer == Projectile.owner)
            {
                float projSpeed = 6f;
                int projType = ModContent.ProjectileType<MagicBullet>();
                if (Main.rand.NextBool(6))
                {
                    SoundEngine.PlaySound(SoundID.Item20 with { Volume = SoundID.Item20.Volume * 0.1f}, Projectile.position);
                }
                Projectile.ai[1] += 1f;
                if (Main.myPlayer == Projectile.owner)
                {
                    Vector2 velocity = targetVec - Projectile.Center;
                    velocity.Normalize();
                    velocity *= projSpeed;
                    SoundEngine.PlaySound(SoundID.Item40, Projectile.position);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, projType, Projectile.damage, 0f, Projectile.owner);
                    Projectile.netUpdate = true;
                }
            }
        }

        public override Color? GetAlpha(Color lightColor) => new Color(148, 0, 211, Projectile.alpha);

        public override bool? CanDamage() => false;

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 10; i++)
            {
                Vector2 dspeed = new Vector2(Main.rand.NextFloat(-7f, 7f), Main.rand.NextFloat(-7f, 7f));
                int dust = Dust.NewDust(Projectile.Center, 1, 1, 66, dspeed.X, dspeed.Y, 160, new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB), 0.75f);
                Main.dust[dust].noGravity = true;
            }
        }
    }
}
