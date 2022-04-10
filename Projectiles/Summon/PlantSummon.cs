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
    public class PlantSummon : ModProjectile
    {
        private bool initialized = false;
        private bool enraged = false;
        private int pinkSeed = ModContent.ProjectileType<PlantSeed>();
        private int greenSeed = ModContent.ProjectileType<PlantSeedGreen>();
        private int thornBall = ModContent.ProjectileType<PlantThornBall>();
        private int sporeClouds = ModContent.ProjectileType<PlantSporeCloud>();

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Plantera");
            Main.projFrames[Projectile.type] = 8;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
            ProjectileID.Sets.NeedsUUID[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 38;
            Projectile.height = 38;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.minionSlots = 3f;
            Projectile.timeLeft = 18000;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft *= 5;
            Projectile.minion = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 12;
            Projectile.extraUpdates = 0;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            CalamityGlobalProjectile modProj = Projectile.Calamity();

            if (player.statLife <= (int)(player.statLifeMax2 * 0.75))
            {
                if (Main.myPlayer == Projectile.owner)
                {
                    enraged = true;
                }
            }
            else
            {
                enraged = false;
                Projectile.extraUpdates = 0;
            }

            Framing();

            if (!initialized)
            {
                SpawnDust();
                SpawnTentacles();
                initialized = true;
            }
            bool correctMinion = Projectile.type == ModContent.ProjectileType<PlantSummon>();
            player.AddBuff(ModContent.BuffType<PlantationBuff>(), 3600);
            if (correctMinion)
            {
                if (player.dead)
                {
                    modPlayer.plantera = false;
                }
                if (modPlayer.plantera)
                {
                    Projectile.timeLeft = 2;
                }
            }

            float range = 1000f;

            //shouldn't need anti clump because there can only be one

            if (!enraged)
            {
                if (Projectile.ai[0] >= 2f)
                    Projectile.ai[0] = 0f;
                Vector2 targetVec = Projectile.position;
                bool foundTarget = false;
                if (player.HasMinionAttackTargetNPC)
                {
                    NPC npc = Main.npc[player.MinionAttackTargetNPC];
                    if (npc.CanBeChasedBy(Projectile, false))
                    {
                        float extraDist = (npc.width / 2) + (npc.height / 2);
                        //Calculate distance between target and the projectile to know if it's too far or not
                        float npcDist = Vector2.Distance(npc.Center, Projectile.Center);
                        if (!foundTarget && npcDist < (range + extraDist))
                        {
                            range = npcDist;
                            targetVec = npc.Center;
                            foundTarget = true;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        NPC npc = Main.npc[i];
                        if (npc.CanBeChasedBy(Projectile, false))
                        {
                            float extraDist = (npc.width / 2) + (npc.height / 2);
                            //Calculate distance between target and the projectile to know if it's too far or not
                            float npcDist = Vector2.Distance(npc.Center, Projectile.Center);
                            if (!foundTarget && npcDist < (range + extraDist))
                            {
                                range = npcDist;
                                targetVec = npc.Center;
                                foundTarget = true;
                            }
                        }
                    }
                }

                CheckIfShouldReturnToPlayer(foundTarget);

                if (foundTarget && Projectile.ai[0] == 0f)
                {
                    StayCertainDistFromTarget(targetVec);
                }
                else
                {
                    PassiveAI();
                }

                HandleRotation(foundTarget, targetVec);

                IncrementAttackCounter();
                if (Projectile.ai[0] == 0f)
                {
                    float projSpeed = 6f;
                    int projType = Main.rand.NextBool(2) ? greenSeed : pinkSeed;
                    int projDmg = (int)(Projectile.damage * 0.7f);
                    float speedMult = 1f;
                    if (Main.rand.NextBool(4))
                    {
                        projType = thornBall;
                    }
                    if (projType == thornBall)
                    {
                        speedMult = 2f;
                        projDmg = (int)(Projectile.damage * 1.2f);
                    }
                    if (Projectile.ai[1] == 0f && foundTarget && range < 500f)
                    {
                        SoundEngine.PlaySound(SoundID.Item20, Projectile.position);
                        Projectile.ai[1] += 1f;
                        if (Main.myPlayer == Projectile.owner)
                        {
                            Vector2 velocity = targetVec - Projectile.Center;
                            if (projType != thornBall && Main.rand.NextBool(3))
                            {
                                FireShotgun(velocity, 0.7f);
                            }
                            else
                            {
                                velocity.Normalize();
                                velocity *= projSpeed;
                                velocity *= speedMult;
                                int p = Projectile.NewProjectile(Projectile.GetItemSource_FromThis(), Projectile.Center, velocity, projType, projDmg, Projectile.knockBack, Projectile.owner, 0f, 0f);
                                if (Main.projectile.IndexInRange(p))
                                    Main.projectile[p].originalDamage = Projectile.originalDamage;
                            }
                            Projectile.netUpdate = true;
                        }
                    }
                }
            }
            else //enraged
            {
                bool charging = false;
                if (Projectile.ai[0] == 2f)
                {
                    Projectile.ai[1] += 1f;
                    Projectile.extraUpdates = 1;
                    if (Projectile.ai[1] > 30f)
                    {
                        Projectile.ai[1] = 1f;
                        Projectile.ai[0] = 0f;
                        Projectile.extraUpdates = 0;
                        Projectile.numUpdates = 0;
                        Projectile.netUpdate = true;
                    }
                    else
                    {
                        charging = true;
                    }
                }
                if (charging)
                {
                    return;
                }
                Vector2 targetVec = Projectile.position;
                bool foundTarget = false;
                if (player.HasMinionAttackTargetNPC)
                {
                    NPC npc = Main.npc[player.MinionAttackTargetNPC];
                    if (npc.CanBeChasedBy(Projectile, false))
                    {
                        float extraDist = (npc.width / 2) + (npc.height / 2);
                        //Calculate distance between target and the projectile to know if it's too far or not
                        float npcDist = Vector2.Distance(npc.Center, Projectile.Center);
                        if (!foundTarget && npcDist < (range + extraDist))
                        {
                            range = npcDist;
                            targetVec = npc.Center;
                            foundTarget = true;
                        }
                    }
                }
                if (!foundTarget)
                {
                    for (int num645 = 0; num645 < Main.maxNPCs; num645++)
                    {
                        NPC npc = Main.npc[num645];
                        if (npc.CanBeChasedBy(Projectile, false))
                        {
                            float extraDist = (npc.width / 2) + (npc.height / 2);
                            //Calculate distance between target and the projectile to know if it's too far or not
                            float npcDist = Vector2.Distance(npc.Center, Projectile.Center);
                            if (!foundTarget && npcDist < (range + extraDist))
                            {
                                range = npcDist;
                                targetVec = npc.Center;
                                foundTarget = true;
                            }
                        }
                    }
                }

                HandleRotation(foundTarget, targetVec);

                CheckIfShouldReturnToPlayer(foundTarget);

                if (foundTarget && Projectile.ai[0] == 0f)
                {
                    StayCertainDistFromTarget(targetVec);
                }
                else
                {
                    PassiveAI();
                }
                IncrementAttackCounter();
                if (Projectile.ai[0] == 0f)
                {
                    if (Projectile.ai[1] == 0f && foundTarget && range < 500f)
                    {
                        Projectile.ai[1] += 1f;
                        if (Main.myPlayer == Projectile.owner)
                        {
                            Projectile.ai[0] = 2f;
                            Vector2 whereIsTarget = targetVec - Projectile.Center;
                            whereIsTarget.Normalize();
                            int projType = thornBall;
                            if (Main.rand.NextBool(2))
                            {
                                Vector2 projVelocity = whereIsTarget * 2f;
                                int projDmg = (int)(Projectile.damage * 1.5f);
                                int p = Projectile.NewProjectile(Projectile.GetItemSource_FromThis(), Projectile.Center, projVelocity, projType, projDmg, Projectile.knockBack, Projectile.owner, 0f, 1f);
                                if (Main.projectile.IndexInRange(p))
                                    Main.projectile[p].originalDamage = Projectile.originalDamage;
                            }
                            if (Main.rand.NextBool(3))
                            {
                                FireShotgun(whereIsTarget, 0.8f);
                            }
                            float chargeSpeed = 8f;
                            Projectile.velocity = whereIsTarget * chargeSpeed;
                            Projectile.netUpdate = true;
                        }
                    }
                }
            }
        }

        private void CheckIfShouldReturnToPlayer(bool targetLocated)
        {
            Player player = Main.player[Projectile.owner];
            float separationAnxietyDist = 1300f;
            if (targetLocated)
            {
                separationAnxietyDist = 2600f;
            }
            if (Vector2.Distance(player.Center, Projectile.Center) > separationAnxietyDist)
            {
                Projectile.ai[0] = 1f;
                Projectile.netUpdate = true;
            }
        }

        private void StayCertainDistFromTarget(Vector2 whereIsTarget)
        {
            Vector2 targetPos = whereIsTarget - Projectile.Center;
            float targetDist = targetPos.Length();
            targetPos.Normalize();
            if (targetDist > 200f)
            {
                float speedMult = 8f;
                targetPos *= speedMult;
                Projectile.velocity = (Projectile.velocity * 40f + targetPos) / 41f;
            }
            else
            {
                float reverseSpeedMult = 4f;
                targetPos *= -reverseSpeedMult;
                Projectile.velocity = (Projectile.velocity * 40f + targetPos) / 41f;
            }
        }

        private void PassiveAI()
        {
            Player player = Main.player[Projectile.owner];
            bool returningToPlayer = false;
            if (!returningToPlayer)
            {
                returningToPlayer = Projectile.ai[0] == 1f;
            }
            float returnSpeed = 12f;
            if (returningToPlayer)
            {
                returnSpeed = 30f;
            }
            Vector2 playerVec = player.Center - Projectile.Center + new Vector2(0f, -120f);
            float playerDist = playerVec.Length();
            if (playerDist > 200f && returnSpeed < 16f)
            {
                returnSpeed = 16f;
            }
            if (playerDist < 600f && returningToPlayer)
            {
                Projectile.ai[0] = 0f;
                Projectile.netUpdate = true;
            }
            if (playerDist > 2000f)
            {
                Projectile.position.X = player.Center.X - (float)(Projectile.width / 2);
                Projectile.position.Y = player.Center.Y - (float)(Projectile.height / 2);
                Projectile.netUpdate = true;
            }
            if (playerDist > 70f)
            {
                playerVec.Normalize();
                playerVec *= returnSpeed;
                Projectile.velocity = (Projectile.velocity * 40f + playerVec) / 41f;
            }
            else if (Projectile.velocity.X == 0f && Projectile.velocity.Y == 0f)
            {
                Projectile.velocity.X = -0.15f;
                Projectile.velocity.Y = -0.05f;
            }
        }

        private void IncrementAttackCounter()
        {
            if (Projectile.ai[1] > 0f)
            {
                Projectile.ai[1] += (float)Main.rand.Next(1, 4);
            }
            if (Projectile.ai[1] > 40f)
            {
                Projectile.ai[1] = 0f;
                Projectile.netUpdate = true;
            }
        }

        private void FireShotgun(Vector2 whereIsTarget, float attackMult)
        {
            whereIsTarget.Normalize();
            float projSpeedMult = 3f;
            int projType = pinkSeed;
            if (Main.rand.NextBool(2) && CalamityUtils.CountProjectiles(sporeClouds) < 9)
            {
                projType = sporeClouds;
                projSpeedMult = 10f;
            }
            else
            {
                projType = Main.rand.NextBool(2) ? greenSeed : pinkSeed;
            }
            int projDmg = (int)(Projectile.damage * attackMult);
            Vector2 projVelocity = whereIsTarget * projSpeedMult;
            for (int i = -8; i <= 8; i += 8)
            {
                Vector2 perturbedSpeed = projVelocity.RotatedBy(MathHelper.ToRadians(i));
                int p = Projectile.NewProjectile(Projectile.GetItemSource_FromThis(), Projectile.Center, perturbedSpeed, projType, projDmg, Projectile.knockBack * attackMult, Projectile.owner, Main.rand.Next(3), 1f);
                if (Main.projectile.IndexInRange(p))
                    Main.projectile[p].originalDamage = Projectile.originalDamage;
            }
        }

        private void HandleRotation(bool targetFound, Vector2 whereIsTarget)
        {
            if (targetFound && !enraged)
            {
                Projectile.rotation = Projectile.rotation.AngleTowards(Projectile.AngleTo(whereIsTarget) + MathHelper.Pi, 0.1f);
            }
            else
            {
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.Pi;
            }
        }

        private void Framing()
        {
            if (Projectile.frameCounter++ % 8 == 7)
            {
                Projectile.frame++;
            }
            if (!enraged)
            {
                if (Projectile.frame >= 4)
                {
                    Projectile.frame = 0;
                }
            }
            else
            {
                if (Projectile.frame >= 8)
                {
                    Projectile.frame = 4;
                }
            }
        }

        private void SpawnDust()
        {
            int dustAmt = 36;
            for (int dustIndex = 0; dustIndex < dustAmt; dustIndex++)
            {
                Vector2 source = Vector2.Normalize(Projectile.velocity) * new Vector2((float)Projectile.width / 2f, (float)Projectile.height) * 0.75f;
                source = source.RotatedBy((double)((float)(dustIndex - (dustAmt / 2 - 1)) * MathHelper.TwoPi / (float)dustAmt), default) + Projectile.Center;
                Vector2 dustVel = source - Projectile.Center;
                int terra = Dust.NewDust(source + dustVel, 0, 0, 107, dustVel.X * 1.75f, dustVel.Y * 1.75f, 100, default, 1.1f);
                Main.dust[terra].noGravity = true;
                Main.dust[terra].velocity = dustVel;
            }
        }

        private void SpawnTentacles()
        {
            if (Projectile.owner == Main.myPlayer)
            {
                int tentacleAmt = 6;
                for (int tentacleIndex = 0; tentacleIndex < tentacleAmt; tentacleIndex++)
                {
                    int p = Projectile.NewProjectile(Projectile.GetItemSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<PlantTentacle>(), Projectile.damage, Projectile.knockBack, Projectile.owner, tentacleIndex, Projectile.GetByUUID(Projectile.owner, Projectile.whoAmI));
                    if (Main.projectile.IndexInRange(p))
                        Main.projectile[p].originalDamage = Projectile.originalDamage;
                }
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Poisoned, 180);
            target.AddBuff(BuffID.Venom, 90);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Poisoned, 180);
            target.AddBuff(BuffID.Venom, 90);
        }

        public override bool? CanDamage() => enraged;

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            int height = texture.Height / Main.projFrames[Projectile.type];
            int frameHeight = height * Projectile.frame;
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (Projectile.spriteDirection == -1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, frameHeight, texture.Width, height)), Projectile.GetAlpha(lightColor), Projectile.rotation, new Vector2((float)texture.Width / 2f, (float)height / 2f), Projectile.scale, spriteEffects, 0);
            return false;
        }
    }
}
