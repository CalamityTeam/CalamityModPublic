using CalamityMod.BiomeManagers;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Dusts;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Placeables.Furniture.BossRelics;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Projectiles.Enemy;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;
using Terraria.WorldBuilding;

namespace CalamityMod.NPCs.AcidRain
{
    [AutoloadBossHead]
    public class CragmawMire : ModNPC
    {
        public enum CragmawAttackState
        {
            ReleaseBurstsOfSpikes,
            AcidExplosionSlam,
            DigAndReleaseLaser,
            CreateVibeCheckTether
        }

        public Player Target => Main.player[NPC.target];
        public CragmawAttackState CurrentAttack
        {
            get => (CragmawAttackState)(int)NPC.ai[0];
            set => NPC.ai[0] = (int)value;
        }
        public ref float AttackTimer => ref NPC.ai[1];
        public bool HasMadeShellBreakGore
        {
            get => NPC.localAI[0] == 1f;
            set => NPC.localAI[0] = value.ToInt();
        }
        public bool InPhase2
        {
            get
            {
                float phase2CeilingRatio = CalamityWorld.revenge ? 0.85f : 0.7f;
                return NPC.life / (float)NPC.lifeMax < phase2CeilingRatio && DownedBossSystem.downedPolterghast;
            }
        }

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 2;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
        }

        public override void SetDefaults()
        {
            NPC.Calamity().canBreakPlayerDefense = true;

            NPC.width = 68;
            NPC.height = 54;
            NPC.aiStyle = AIType = -1;

            NPC.damage = 66;
            NPC.lifeMax = 4000;
            NPC.defense = 25;

            if (DownedBossSystem.downedPolterghast)
            {
                NPC.damage = 160;
                NPC.lifeMax = 80630;
                NPC.defense = 80;
            }

            NPC.behindTiles = true;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 3, 60, 0);
            NPC.lavaImmune = false;
            NPC.noGravity = false;
            NPC.noTileCollide = false;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.Calamity().VulnerableToHeat = false;
            NPC.Calamity().VulnerableToSickness = false;
            NPC.Calamity().VulnerableToElectricity = true;
            NPC.Calamity().VulnerableToWater = false;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<AcidRainBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] 
            {
				new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.CragmawMire")
            });
        }

        public override void AI()
        {
            NPC.TargetClosest(false);

            // Reset things every frame. They may be changed in the attack states below.
            NPC.noGravity = false;
            NPC.noTileCollide = false;
            NPC.dontTakeDamage = false;

            // Handle the phase 2 transition.
            if (InPhase2 && !HasMadeShellBreakGore)
            {
                SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, NPC.Center);
                if (Main.netMode != NetmodeID.Server)
                {
                    Gore.NewGore(NPC.GetSource_FromAI(), NPC.position, -Vector2.UnitY.RotatedByRandom(0.4f) * 4f, Mod.Find<ModGore>("CragmawMireP1Gore").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_FromAI(), NPC.position, -Vector2.UnitY.RotatedByRandom(0.4f) * 4f, Mod.Find<ModGore>("CragmawMireP1Gore2").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_FromAI(), NPC.position, -Vector2.UnitY.RotatedByRandom(0.4f) * 4f, Mod.Find<ModGore>("CragmawMireP1Gore3").Type, NPC.scale);
                }
                HasMadeShellBreakGore = true;
            }

            switch (CurrentAttack)
            {
                case CragmawAttackState.ReleaseBurstsOfSpikes:
                    DoBehavior_ReleaseBurstsOfSpikes();
                    break;
                case CragmawAttackState.AcidExplosionSlam:
                    DoBehavior_AcidExplosionSlam();
                    break;
                case CragmawAttackState.DigAndReleaseLaser:
                    DoBehavior_DigAndReleaseLaser();
                    break;
                case CragmawAttackState.CreateVibeCheckTether:
                    DoBehavior_CreateVibeCheckTether();
                    break;
            }

            AttackTimer++;
        }

        public void DoBehavior_ReleaseBurstsOfSpikes()
        {
            int spikesPerBurst = 5;
            float burstSpeed = 8f;
            int burstShootRate = 92;
            if (DownedBossSystem.downedPolterghast)
            {
                spikesPerBurst += 3;
                burstSpeed += 3.25f;
                burstShootRate -= 20;
            }
            if (InPhase2)
            {
                spikesPerBurst++;
                burstShootRate -= 10;
            }

            if (Collision.SolidCollision(NPC.position, NPC.width, NPC.height - 6))
                NPC.position.Y -= 4f;

            // Release bursts of spikes and play a fire sound.
            float wrappedAttackTimer = AttackTimer % burstShootRate;

            // Create some charge-up dust before firing.
            if (wrappedAttackTimer > burstShootRate * 0.65f)
            {
                for (int i = 0; i < 4; i++)
                {
                    Dust rock = Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Circular(75f, 75f), 1);
                    rock.color = Color.Yellow;
                    rock.velocity = (NPC.Center - rock.position) * Main.rand.NextFloat(0.06f, 0.09f);
                    rock.scale = Main.rand.NextFloat(1f, 1.3f);
                    rock.noGravity = true;
                }
            }

            if (wrappedAttackTimer == burstShootRate - 1f)
            {
                // Play the sound.
                SoundEngine.PlaySound(SoundID.Item92, NPC.Center);

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int damage = DownedBossSystem.downedPolterghast ? 52 : 33;
                    float shootOffsetAngle = Main.rand.NextFloat(MathHelper.TwoPi);
                    for (int i = 0; i < spikesPerBurst; i++)
                    {
                        Vector2 spikeShootVelocity = (MathHelper.TwoPi * i / spikesPerBurst + shootOffsetAngle).ToRotationVector2() * burstSpeed;
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + spikeShootVelocity * 1.6f, spikeShootVelocity, ModContent.ProjectileType<CragmawSpike>(), damage, 0f);
                    }
                }
            }

            if (AttackTimer > 240f)
                SelectNextAttack();
        }

        public void DoBehavior_AcidExplosionSlam()
        {
            float opacityFadeoutIncrement = 0.03f;
            float verticalTeleportOffset = 415f;
            float slamAcceleration = 0.375f;
            float maxSlamSpeed = 19f;
            int slamCount = 2;
            if (DownedBossSystem.downedPolterghast)
            {
                opacityFadeoutIncrement += 0.03f;
                slamAcceleration += 0.07f;
                maxSlamSpeed += 5f;
            }
            if (InPhase2)
            {
                opacityFadeoutIncrement += 0.01f;
                slamAcceleration += 0.03f;
                maxSlamSpeed += 1.15f;
            }

            ref float attackSubstate = ref NPC.ai[2];
            ref float slamCounter = ref NPC.ai[3];

            switch ((int)attackSubstate)
            {
                // Dig in anticipation of the slam teleport.
                case 0:
                    // Disable tile collision and normal gravity, as the cragmaw will dig into the ground.
                    NPC.noTileCollide = true;
                    NPC.noGravity = true;

                    // Dig into the ground.
                    NPC.position.Y += 3f;

                    // Fade out.
                    NPC.Opacity = MathHelper.Clamp(NPC.Opacity - opacityFadeoutIncrement * 0.6f, 0f, 1f);

                    // Stop taking damage.
                    NPC.dontTakeDamage = true;

                    // Teleport once completely transparent.
                    if (NPC.Opacity <= 0f)
                    {
                        attackSubstate = 1f;
                        NPC.Center = Target.Center - Vector2.UnitY * verticalTeleportOffset;
                        NPC.velocity = Vector2.Zero;
                        NPC.netUpdate = true;
                    }
                    break;

                // Fade in again.
                case 1:
                    // Disable default gravity and tile collision, as it is expected that the cragmaw is in the air at the moment.
                    NPC.noGravity = true;
                    NPC.noTileCollide = true;

                    // Disable contact damage. This is done to prevent cheap hits from the teleport, either in multiplayer or due to fast upward movement.
                    NPC.damage = 0;

                    // Fade in.
                    NPC.Opacity = MathHelper.Clamp(NPC.Opacity + opacityFadeoutIncrement, 0f, 1f);

                    // Do the slam once completely faded in.
                    if (NPC.Opacity >= 1f)
                    {
                        attackSubstate = 2f;
                        AttackTimer = 0f;
                        NPC.velocity = Vector2.UnitY * maxSlamSpeed * 0.251f;
                        NPC.netUpdate = true;
                    }
                    break;

                // Do the slam.
                case 2:
                    // Disable default gravity and tile collision. Both of them will be calculated manually.
                    NPC.noGravity = true;
                    NPC.noTileCollide = true;

                    // Slam downward.
                    NPC.velocity.Y = MathHelper.Clamp(NPC.velocity.Y + slamAcceleration, 0f, maxSlamSpeed);

                    // Register ground collision from the slam.
                    // Once it has been hit a nuke explosion projectile is created on the ground, along with homing nuclear drops.
                    if ((NPC.Bottom.Y > Target.Bottom.Y && Collision.SolidCollision(NPC.BottomLeft, NPC.width, 1)) || AttackTimer > 180f)
                    {
                        NPC.velocity = Vector2.Zero;

                        if (AttackTimer == 1f)
                        {
                            SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, NPC.Center);
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                int nukeDamage = DownedBossSystem.downedPolterghast ? 72 : 38;
                                int dropletDamage = (int)(nukeDamage * 0.6f);
                                int explosion = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<CragmawExplosion>(), nukeDamage, 0f);
                                if (Main.projectile.IndexInRange(explosion))
                                    Main.projectile[explosion].Bottom = NPC.Bottom + Vector2.UnitY * 4f;

                                for (int i = 0; i < 12; i++)
                                {
                                    Vector2 dropletVelocity = -Vector2.UnitY.RotatedByRandom(0.71f) * Main.rand.NextFloat(8f, 12f);
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, dropletVelocity, ModContent.ProjectileType<CragmawAcidDrop>(), dropletDamage, 0f);
                                }
                            }
                        }
                        AttackTimer++;
                    }
                    else
                        AttackTimer = 0f;

                    // Sit in place for a moment after hitting the ground.
                    if (AttackTimer > 35f)
                    {
                        slamCounter++;
                        if (slamCounter >= slamCount)
                            SelectNextAttack();
                        else
                        {
                            attackSubstate = 0f;
                            NPC.netUpdate = true;
                        }
                    }
                    break;
            }
        }

        public void DoBehavior_DigAndReleaseLaser()
        {
            int digReapperTime = 30;
            float digReapperSpeed = 4f;
            int chargeupTelegraphTime = 60;
            float opacityFadeoutIncrement = 0.025f;
            if (DownedBossSystem.downedPolterghast)
            {
                opacityFadeoutIncrement += 0.025f;
                chargeupTelegraphTime -= 15;
            }
            if (InPhase2)
            {
                opacityFadeoutIncrement += 0.01f;
            }

            ref float attackSubstate = ref NPC.ai[2];

            switch ((int)attackSubstate)
            {
                // Dig to reposition.
                case 0:
                    // Disable tile collision and normal gravity, as the cragmaw will dig into the ground.
                    NPC.noTileCollide = true;
                    NPC.noGravity = true;

                    // Dig into the ground.
                    NPC.position.Y += 3f;

                    // Fade out.
                    NPC.Opacity = MathHelper.Clamp(NPC.Opacity - opacityFadeoutIncrement * 0.6f, 0f, 1f);

                    // Stop taking damage.
                    NPC.dontTakeDamage = true;

                    // Teleport once completely transparent.
                    if (NPC.Opacity <= 0f)
                    {
                        attackSubstate = 1f;
                        if (WorldUtils.Find(Target.Center.ToTileCoordinates(), Searches.Chain(new Searches.Down(1000), new CustomConditions.SolidOrPlatform()), out Point teleportPosition))
                            NPC.Bottom = teleportPosition.ToWorldCoordinates() + Vector2.UnitY * digReapperTime * digReapperSpeed;
                        else
                            SelectNextAttack();

                        AttackTimer = 0f;
                        NPC.velocity = Vector2.Zero;
                        NPC.netUpdate = true;
                    }
                    break;

                // Prepare and fire the laser.
                case 1:
                    // Come out of the ground.
                    if (AttackTimer < digReapperTime)
                    {
                        NPC.noTileCollide = true;
                        NPC.noGravity = true;
                        NPC.position.Y -= digReapperSpeed;
                    }
                    if (AttackTimer == digReapperTime)
                        NPC.netUpdate = true;

                    // Fade in.
                    NPC.Opacity = MathHelper.Clamp(NPC.Opacity + opacityFadeoutIncrement, 0f, 1f);

                    // Create some charge dust.
                    if (AttackTimer > digReapperTime && AttackTimer < digReapperTime + chargeupTelegraphTime)
                    {
                        Dust chargeupDust = Dust.NewDustPerfect(NPC.Center, 267);
                        chargeupDust.position -= Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi) * NPC.height * 0.4f;
                        chargeupDust.velocity = -Vector2.UnitY.RotatedByRandom(0.31f) * Main.rand.NextFloat(2f, 6f);
                        chargeupDust.color = Color.Lerp(Color.Green, Color.Yellow, Main.rand.NextFloat());
                        chargeupDust.scale = Main.rand.NextFloat(0.95f, 1.35f);
                        chargeupDust.noGravity = true;
                    }

                    // Release the laser.
                    if (AttackTimer == digReapperTime + chargeupTelegraphTime)
                    {
                        // Play a sound and create dust.
                        SoundEngine.PlaySound(SoundID.Zombie104, NPC.Center);
                        for (int i = 0; i < 40; i++)
                        {
                            Dust burstDust = Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Circular(75f, 75f), 267);
                            burstDust.velocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(2f, 6f);
                            burstDust.color = Color.Lerp(Color.Green, Color.Yellow, Main.rand.NextFloat());
                            burstDust.scale = Main.rand.NextFloat(1.1f, 1.45f);
                            burstDust.noGravity = true;
                        }

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int laserbeamDamage = DownedBossSystem.downedPolterghast ? 120 : 40;
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, -Vector2.UnitY, ModContent.ProjectileType<CragmawBeam>(), laserbeamDamage, 0f, Main.myPlayer, 0f, NPC.whoAmI);
                        }
                    }

                    if (AttackTimer == digReapperTime + chargeupTelegraphTime + CragmawBeam.Lifetime)
                        SelectNextAttack();
                    break;
            }
        }

        public void DoBehavior_CreateVibeCheckTether()
        {
            if (Main.netMode != NetmodeID.MultiplayerClient && AttackTimer == 30f)
                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, -Vector2.UnitY * 4f, ModContent.ProjectileType<CragmawVibeCheckChain>(), 0, 0f, Main.myPlayer, NPC.whoAmI, NPC.target);
            if (AttackTimer > CragmawVibeCheckChain.Lifetime + 30f)
                SelectNextAttack();
        }

        public void SelectNextAttack()
        {
            // Reset the attack timer and optional attack variables.
            AttackTimer = 0f;
            NPC.ai[2] = 0f;
            NPC.ai[3] = 0f;

            CragmawAttackState oldAttack = CurrentAttack;
            WeightedRandom<CragmawAttackState> attackSelector = new WeightedRandom<CragmawAttackState>(Main.rand);
            attackSelector.Add(CragmawAttackState.ReleaseBurstsOfSpikes);
            attackSelector.Add(CragmawAttackState.AcidExplosionSlam);
            attackSelector.Add(CragmawAttackState.DigAndReleaseLaser);
            if (InPhase2)
                attackSelector.Add(CragmawAttackState.CreateVibeCheckTether);

            // Select the new attack.
            // It is random, but will never perform the same attack twice in succession.
            do
                CurrentAttack = attackSelector.Get();
            while (oldAttack == CurrentAttack);

            NPC.netUpdate = true;
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.8f * balance);
            NPC.damage = (int)(NPC.damage * 0.85f);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = InPhase2 ? ModContent.Request<Texture2D>("CalamityMod/NPCs/AcidRain/CragmawMire2").Value : ModContent.Request<Texture2D>("CalamityMod/NPCs/AcidRain/CragmawMire").Value;
            Main.EntitySpriteDraw(texture, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() * 0.5f, NPC.scale, 0, 0);
            return false;
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter % 6 == 5)
                NPC.frame.Y += frameHeight;
            if (NPC.frame.Y >= frameHeight * 2)
                NPC.frame.Y = 0;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 5; k++)
                Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.SulfurousSeaAcid, hit.HitDirection, -1f, 0, default, 1f);
            if (NPC.life <= 0)
            {
                if (Main.netMode != NetmodeID.Server)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, -Vector2.UnitY.RotatedByRandom(0.4f) * 4f, Mod.Find<ModGore>("CragmawMireP2Gore").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, -Vector2.UnitY.RotatedByRandom(0.4f) * 4f, Mod.Find<ModGore>("CragmawMireP2Gore2").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, -Vector2.UnitY.RotatedByRandom(0.4f) * 4f, Mod.Find<ModGore>("CragmawMireP2Gore3").Type, NPC.scale);
                }
            }
        }

        public override void OnKill()
        {
            // Mark Cragmaw Mire as dead
            DownedBossSystem.downedCragmawMire = true;
            CalamityNetcode.SyncWorld();
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage > 0)
                target.AddBuff(ModContent.BuffType<Irradiated>(), 300);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            // If post-Polter, the drop rates are 10%. Otherwise they're 100%.
			// This is accomplished by adding rules if the CONDITION "Post-Polter" fails.
            LeadingConditionRule postPolter = npcLoot.DefineConditionalDropSet(() => DownedBossSystem.downedPolterghast);
            postPolter.Add(ModContent.ItemType<NuclearFuelRod>(), 10, hideLootReport: !DownedBossSystem.downedPolterghast);
            postPolter.Add(ModContent.ItemType<SpentFuelContainer>(), 10, hideLootReport: !DownedBossSystem.downedPolterghast);
            postPolter.AddFail(ModContent.ItemType<NuclearFuelRod>(), hideLootReport: DownedBossSystem.downedPolterghast);
            postPolter.AddFail(ModContent.ItemType<SpentFuelContainer>(), hideLootReport: DownedBossSystem.downedPolterghast);

            npcLoot.Add(ModContent.ItemType<CragmawMireTrophy>(), 10);
            npcLoot.DefineConditionalDropSet(DropHelper.RevAndMaster).Add(ModContent.ItemType<CragmawMireRelic>());
        }
    }
}
