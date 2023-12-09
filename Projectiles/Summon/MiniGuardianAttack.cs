using System;
using System.IO;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.Summon.Whips;
using CalamityMod.NPCs.Providence;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class MiniGuardianAttack : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public Player Owner => Main.player[Projectile.owner];

        public bool SpawnedFromPSC => Projectile.ai[0] == 1f;

        internal int phaseTimer
        {
            get => (int)Projectile.ai[1];
            set
            {
                Projectile.ai[1] = value;
                Projectile.netUpdate = true;
            }
        }

        private int attackDelay = 0;
        
        public enum MiniOffenseAIState
        {
            Vanity,
            Psa,
            Spears,
            Charges,
            Fireballs
        }

        public MiniOffenseAIState getAiState => SpawnedFromPSC ? 
            (MiniOffenseAIState)Math.Clamp(Projectile.ai[2], (int)MiniOffenseAIState.Spears, (int)MiniOffenseAIState.Fireballs) : 
            MiniOffenseAIState.Psa;

        public MiniOffenseAIState updateAiState(Player player, MiniOffenseAIState currentAIState)
        {
            MiniOffenseAIState? result = null;
            if (SpawnedFromPSC && ForcedVanity)
                result = MiniOffenseAIState.Vanity;
            else if (!SpawnedFromPSC)
                result = MiniOffenseAIState.Psa;
            if (result != null)
            {
                Projectile.ai[2] = (int)result;
                return (MiniOffenseAIState)result;
                //Return early as the phaseTimer would overwrite the spears on hit counter
            }

            int currentPhase = (int)currentAIState;
            
            if (phaseTimer <= 0)
            {
                currentPhase++;
                if (currentPhase > (int)MiniOffenseAIState.Fireballs) //if it is beyond the last possible phase (in terms of enum order) reset to beginning
                    currentPhase = (int)MiniOffenseAIState.Spears;
                result = (MiniOffenseAIState)currentPhase;
                bool whip = player.HasBuff<ProfanedCrystalWhipBuff>();
                int newPhaseTimer = result == MiniOffenseAIState.Charges ? 60 * (whip ? 10 : 6) : //charge time
                    result == MiniOffenseAIState.Fireballs ? 60 * (whip ? 10 : 6) : //fireball time
                    60 * (whip ? 8 : 6); //spears time
                phaseTimer = newPhaseTimer;
            }
            else
            {
                result = (MiniOffenseAIState)currentPhase;
                phaseTimer--;
            }
            
            Projectile.ai[2] = (int)result;
            return (MiniOffenseAIState)result;
        }
        
        public bool ForcedVanity => SpawnedFromPSC && !Owner.Calamity().profanedCrystalBuffs;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.netImportant = true;
            Projectile.tileCollide = false;
            Projectile.width = 60;
            Projectile.height = 88;
            Projectile.minion = true;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
        }

        private void BaseAI(NPC potentialTarget)
        {
            if (potentialTarget != null && !ForcedVanity)
            {
                Vector2 targetDestination = potentialTarget.Center - Projectile.Center;
                float targetDist = targetDestination.Length();
                // Moves faster if from PSC
                float baseSpeed = (targetDist < 100f ? 28f : 24f) * (SpawnedFromPSC ? 2f : 0.95f);
                float inertia = SpawnedFromPSC ? 20f : 12f;

                targetDist = baseSpeed / targetDist;
                targetDestination *= targetDist;
                Projectile.velocity = (Projectile.velocity * inertia + targetDestination) / (inertia + 1f);
            }
            else // Idle movement
            {
                //Set rotation in case it's offset from the buffed charging
                Projectile.rotation = (float)(Math.Atan(0));
                Vector2 playerDestination = Owner.Center - Projectile.Center;
                playerDestination.X += Main.rand.NextFloat(-10f, 20f) - (60f * Owner.direction);
                playerDestination.Y += Main.rand.NextFloat(-10f, 20f) - 60f;
                float playerDist = playerDestination.Length();
                float acceleration = 0.5f;
                float returnSpeed = 28f;

                // Teleport if too far
                if (playerDist > 2000f)
                {
                    Projectile.position = Owner.position;
                    Projectile.netUpdate = true;
                }
                // Slow down a lot when close
                else if (playerDist < 50f)
                {
                    acceleration = 0.01f;
                    if (Math.Abs(Projectile.velocity.X) > 2f || Math.Abs(Projectile.velocity.Y) > 2f)
                        Projectile.velocity *= 0.9f;
                }
                else
                {
                    if (playerDist < 100f)
                        acceleration = 0.1f;
                    
                    if (playerDist > 300f)
                        acceleration = 1f;

                    playerDist = returnSpeed / playerDist;
                    playerDestination *= playerDist;
                }

                // Turning (wtf is this)
                // idk ask phup lmao
                if (Projectile.velocity.X < playerDestination.X)
                {
                    Projectile.velocity.X += acceleration;
                    if (acceleration > 0.05f && Projectile.velocity.X < 0f)
                        Projectile.velocity.X += acceleration;
                }
                if (Projectile.velocity.X > playerDestination.X)
                {
                    Projectile.velocity.X -= acceleration;
                    if (acceleration > 0.05f && Projectile.velocity.X > 0f)
                        Projectile.velocity.X -= acceleration;
                }
                if (Projectile.velocity.Y < playerDestination.Y)
                {
                    Projectile.velocity.Y += acceleration;
                    if (acceleration > 0.05f && Projectile.velocity.Y < 0f)
                        Projectile.velocity.Y += acceleration * 2f;
                }
                if (Projectile.velocity.Y > playerDestination.Y)
                {
                    Projectile.velocity.Y -= acceleration;
                    if (acceleration > 0.05f && Projectile.velocity.Y > 0f)
                        Projectile.velocity.Y -= acceleration * 2f;
                }
                
            }
        }

        public void AdvancedAI(NPC potentialTarget, Player owner, MiniOffenseAIState aiState)
        {
            bool buffedAi = owner.HasBuff<ProfanedCrystalWhipBuff>();
                
            Vector2 targetDestination = potentialTarget.Center - Projectile.Center;
            if (attackDelay > 0)
                attackDelay--;
            switch (aiState)
            {
                case MiniOffenseAIState.Charges:
                    break;
                case MiniOffenseAIState.Fireballs:
                    targetDestination.X += Main.rand.NextFloat(-5f, 5f);
                    targetDestination.Y += Main.rand.NextFloat(155f, 160f);
                    break;
                case MiniOffenseAIState.Spears:
                    targetDestination.X += Main.rand.NextFloat(-5f, 5f);
                    targetDestination.Y += Main.rand.NextFloat(155f, 160f);
                    break;
            }

            if (aiState != MiniOffenseAIState.Charges)
            {

                Projectile.rotation = (float)(Math.Atan(0));
                float targetDist = targetDestination.Length();
            
                float baseSpeed = (targetDist < 100f ? 28f : 24f) * 2f;
                float inertia = 20f;

                targetDist = baseSpeed / targetDist;
                targetDestination *= targetDist;
                Projectile.velocity = (Projectile.velocity * inertia + targetDestination) / (inertia + 1f);

                if (aiState == MiniOffenseAIState.Fireballs)
                {
                    if (attackDelay == 0)
                    {
                        if (buffedAi)
                        {
                            attackDelay = 100;
                            Projectile.velocity = Projectile.Center - potentialTarget.Center;
                            Projectile.velocity.Normalize();
                            Projectile.velocity *= 29f;
                            int damage = (int)Owner.GetTotalDamage<SummonDamageClass>().ApplyTo(Projectile.originalDamage);
                            var velocity = CalamityUtils.CalculatePredictiveAimToTarget(Projectile.Center, potentialTarget, 28f);
                            int shotCount = 3;
                            int spread = -20;
                            for (int i = 0; i < shotCount; i++)
                            {
                                Vector2 perturbedspeed = new Vector2(velocity.X, velocity.Y).RotatedBy(MathHelper.ToRadians(spread));
                                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, perturbedspeed, ModContent.ProjectileType<MiniGuardianFireball>(), damage, 1f, Projectile.owner, 1f);
                                spread += 20;
                            }
                        }
                        else
                        {
                            Projectile.velocity = Projectile.Center - potentialTarget.Center;
                            Projectile.velocity.Normalize();
                            Projectile.velocity *= 20f;
                            attackDelay = 75;
                            int damage = (int)Owner.GetTotalDamage<SummonDamageClass>().ApplyTo(Projectile.originalDamage);
                            var velocity = CalamityUtils.CalculatePredictiveAimToTarget(Projectile.Center, potentialTarget, 25f);
                            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<MiniGuardianFireball>(), damage, 1f, Projectile.owner);
                        }
                    }
                }
                else //Spears
                {
                    if (attackDelay % (buffedAi ? 6 : 8) == 0)
                    {
                        //fire spear
                        SoundEngine.PlaySound(SoundID.Item20, Projectile.Center);
                        
                        bool shouldShotGun = attackDelay == 0;
                        if (shouldShotGun)
                        {
                            int numProj = 5;
                            var velocity = CalamityUtils.CalculatePredictiveAimToTarget(Projectile.Center, potentialTarget, buffedAi ? 28f : 20f);
                            int spread = buffedAi ? -10 : -20;
                            for (int i = 0; i < numProj; i++)
                            {
                                Vector2 perturbedspeed = velocity.RotatedBy(MathHelper.ToRadians(spread));
                                int separation = (i * 4) - 8;
                                int spearBaseDamage = (int)(Projectile.originalDamage * 0.5f);
                                int spearDamage = (int)Owner.GetTotalDamage<SummonDamageClass>().ApplyTo(spearBaseDamage);
                                int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y - separation, perturbedspeed.X, perturbedspeed.Y, ModContent.ProjectileType<MiniGuardianSpear>(), spearDamage, 1f, Projectile.owner, 1f, 1f);
                                if (proj.WithinBounds(Main.maxProjectiles))
                                {
                                    Main.projectile[proj].DamageType = DamageClass.Summon;
                                    Main.projectile[proj].originalDamage = spearBaseDamage;
                                }
                                spread += buffedAi ? 5 : 10;
                            }
                        }
                        else
                        {
                            int spearBaseDamage = Projectile.originalDamage;
                            int spearDamage = (int)Owner.GetTotalDamage<SummonDamageClass>().ApplyTo(spearBaseDamage);
                            var velocity = CalamityUtils.CalculatePredictiveAimToTarget(Projectile.Center, potentialTarget, buffedAi ? 28f : 20f);
                            var proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<MiniGuardianSpear>(), spearDamage, 1f, Projectile.owner, 1f, 1f);
                            if (proj.WithinBounds(Main.maxProjectiles))
                            {
                                Main.projectile[proj].DamageType = DamageClass.Summon;
                                Main.projectile[proj].originalDamage = Projectile.originalDamage;
                            }
                        }
                    }

                    if (attackDelay == 0)
                        attackDelay = buffedAi ? 30 : 40;
                }
            }
            else
            {
                var shouldDrawDust = buffedAi || attackDelay > 0;
                if (buffedAi)
                {
                    if (attackDelay <= 0)
                        owner.Calamity().rollBabSpears(1, true);
                    float distToTarget = Projectile.Distance(potentialTarget.Center) + .01f;
                    Projectile.velocity = Projectile.rotation.ToRotationVector2() * (28f + (28f / (distToTarget * .01f)));
                    Projectile.velocity = Vector2.Clamp(Projectile.velocity, Vector2.One * -50f, Vector2.One * 50f);
                    Projectile.rotation = Projectile.rotation.AngleTowards(Projectile.AngleTo(potentialTarget.Center), .001f * distToTarget);
                }
                else
                {
                    if (attackDelay == 24)
                    {
                        Projectile.velocity = Projectile.SuperhomeTowardsTarget(potentialTarget, 35f, 1f);
                        Projectile.velocity *= 1.369f;
                    }
                        
                }
                if (attackDelay <= 0)
                    attackDelay = buffedAi ? 20 : 25;

                if (shouldDrawDust)
                {
                    int pscState = (int)(Main.dayTime ? Providence.BossMode.Day : Providence.BossMode.Night);
                    var shouldAdjust = !Main.dayTime && buffedAi;
                    int dustId = ProvUtils.GetDustID(pscState);
                    for (int i = 0; i < 6; i++)
                    {
                        Dust dust = Dust.NewDustPerfect(Projectile.Center + (Projectile.Size / 2f).RotatedBy(Projectile.rotation), dustId);
                        dust.velocity = Projectile.velocity.RotatedBy(MathHelper.ToRadians(20f * (i % 2 == 0).ToDirectionInt()));
                        dust.noGravity = true;
                        dust.fadeIn = shouldAdjust ? 0.9f : 1.8f;
                        dust.scale = Main.dayTime ? dust.scale : 0.45f;
                    }
                }
            }
        }

        public override void AI()
        {
            Player owner = Owner;
            // Despawn properly
            if (owner.Calamity().pSoulGuardians)
                Projectile.timeLeft = 2;
            if (!owner.Calamity().pSoulArtifact || owner.dead || !owner.active)
            {
                owner.Calamity().pSoulGuardians = false;
                Projectile.active = false;
                return;
            }
            
            var psc = owner.Calamity().profanedCrystal;
            if (psc && !SpawnedFromPSC || !psc && SpawnedFromPSC)
            {
                Projectile.active = false;
            }
            
            // Dynamically update stats here, originalDamage can be found in MiscEffects
            Projectile.damage = (int)Owner.GetTotalDamage<SummonDamageClass>().ApplyTo(Projectile.originalDamage);
            Projectile.damage = Owner.ApplyArmorAccDamageBonusesTo(Projectile.damage);
            Projectile.localNPCHitCooldown = SpawnedFromPSC ? 6 : 9;

            var currentAIState = getAiState;

            if (owner.Calamity().profanedCrystalAnim != -1)
                currentAIState = MiniOffenseAIState.Vanity;
            
            var newAIState = updateAiState(owner, currentAIState);

            if (newAIState != currentAIState)
            {
                Projectile.netUpdate = true;
            }

            // Find minion and charge if possible, make sure vanity minions are not chasing
            NPC potentialTarget = Projectile.Center.MinionHoming(3000f, owner);

            switch (currentAIState)
            {
                case MiniOffenseAIState.Vanity:
                case MiniOffenseAIState.Psa:
                    BaseAI(potentialTarget);
                    break;
                default:
                    if (potentialTarget != null && !ForcedVanity)
                        AdvancedAI(potentialTarget, owner, newAIState);
                    else
                        BaseAI(null);
                    break;
            }

            // Direction and frames
            if (Math.Abs(Projectile.velocity.X) > 0.2f)
                Projectile.direction = Projectile.spriteDirection = Math.Sign(Projectile.velocity.X);

            Projectile.frameCounter++;
            Projectile.frame = Projectile.frameCounter / 6 % Main.projFrames[Projectile.type];
        }

        // Vanity stuff can't damage
        public override bool? CanDamage() => !ForcedVanity;

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Owner.Calamity().angelicAlliance)
                target.AddBuff(ModContent.BuffType<BanishingFire>(), 300);
            if (!Owner.Calamity().profanedCrystal)
            {
                if (Projectile.ai[1] == 0f)
                    Owner.Calamity().rollBabSpears(1, target.chaseable);
                Projectile.ai[1] -= 1f;
                if (Projectile.ai[1] < 0f)
                    Projectile.ai[1] = 15;
            }
            else
            {
                var buffedAI = Owner.HasBuff<ProfanedCrystalWhipBuff>();
                var state = getAiState;
                if (state == MiniOffenseAIState.Charges && !buffedAI)
                {
                    Owner.Calamity().rollBabSpears(1, true);
                }
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (Owner.Calamity().angelicAlliance)
                target.AddBuff(ModContent.BuffType<BanishingFire>(), 300);
            if (!Owner.Calamity().profanedCrystal)
            {
                if (Projectile.ai[1] == 0f)
                    Owner.Calamity().rollBabSpears(1, true);
                Projectile.ai[1] -= 1f;
                if (Projectile.ai[1] < 0f)
                    Projectile.ai[1] = 15;
            }
            else
            {
                var buffedAI = Owner.HasBuff<ProfanedCrystalWhipBuff>();
                var state = getAiState;
                if (state == MiniOffenseAIState.Charges && !buffedAI)
                {
                    Owner.Calamity().rollBabSpears(1, true);
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            // Has afterimages if maximum empowerment
            if (!ForcedVanity && SpawnedFromPSC)
            {
                CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
                return false;
            }
            return true;
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            attackDelay = reader.ReadInt32();
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(attackDelay);
        }
    }
}
