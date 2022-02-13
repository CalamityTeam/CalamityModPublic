using CalamityMod.Dusts;
using CalamityMod.Items.Accessories;
using CalamityMod.Projectiles.Enemy;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Items.Weapons.Rogue;
using Terraria.Utilities;
using Terraria.World.Generation;

namespace CalamityMod.NPCs.AcidRain
{
    public class CragmawMire : ModNPC
    {
        public enum CragmawAttackState
        {
            ReleaseBurstsOfSpikes,
            AcidExplosionSlam,
            DigAndReleaseLaser,
            CreateVibeCheckTether
        }

        public Player Target => Main.player[npc.target];
        public CragmawAttackState CurrentAttack
        {
            get => (CragmawAttackState)(int)npc.ai[0];
            set => npc.ai[0] = (int)value;
        }
        public ref float AttackTimer => ref npc.ai[1];
        public bool HasMadeShellBreakGore
        {
            get => npc.localAI[0] == 1f;
            set => npc.localAI[0] = value.ToInt();
        }
        public bool InPhase2
        {
            get
            {
                float phase2CeilingRatio = CalamityWorld.revenge ? 0.85f : 0.7f;
                return npc.life / (float)npc.lifeMax < phase2CeilingRatio && CalamityWorld.downedPolterghast;
            }
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cragmaw Mire");
            Main.npcFrameCount[npc.type] = 2;
        }

        public override void SetDefaults()
        {
            npc.Calamity().canBreakPlayerDefense = true;

            npc.width = 68;
            npc.height = 54;
            npc.aiStyle = aiType = -1;

            npc.damage = 66;
            npc.lifeMax = 4000;
            npc.defense = 25;

            if (CalamityWorld.downedPolterghast)
            {
                npc.damage = 160;
                npc.lifeMax = 80630;
                npc.defense = 80;
            }

            npc.behindTiles = true;
            npc.knockBackResist = 0f;
            npc.value = Item.buyPrice(0, 3, 60, 0);
            npc.lavaImmune = false;
            npc.noGravity = false;
            npc.noTileCollide = false;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            npc.Calamity().VulnerableToHeat = false;
            npc.Calamity().VulnerableToSickness = false;
            npc.Calamity().VulnerableToElectricity = true;
            npc.Calamity().VulnerableToWater = false;
        }

        public override void AI()
        {
            npc.TargetClosest(false);

            // Reset things every frame. They may be changed in the attack states below.
            npc.noGravity = false;
            npc.noTileCollide = false;
            npc.dontTakeDamage = false;

            // Handle the phase 2 transition.
            if (InPhase2 && !HasMadeShellBreakGore)
            {
                Main.PlaySound(SoundID.DD2_ExplosiveTrapExplode, npc.Center);
                Gore.NewGore(npc.position, -Vector2.UnitY.RotatedByRandom(0.4f) * 4f, mod.GetGoreSlot("Gores/AcidRain/CragmawMireP1Gore"), npc.scale);
                Gore.NewGore(npc.position, -Vector2.UnitY.RotatedByRandom(0.4f) * 4f, mod.GetGoreSlot("Gores/AcidRain/CragmawMireP1Gore2"), npc.scale);
                Gore.NewGore(npc.position, -Vector2.UnitY.RotatedByRandom(0.4f) * 4f, mod.GetGoreSlot("Gores/AcidRain/CragmawMireP1Gore3"), npc.scale);
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
            if (CalamityWorld.downedPolterghast)
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

            if (Collision.SolidCollision(npc.position, npc.width, npc.height - 6))
                npc.position.Y -= 4f;

            // Release bursts of spikes and play a fire sound.
            float wrappedAttackTimer = AttackTimer % burstShootRate;

            // Create some charge-up dust before firing.
            if (wrappedAttackTimer > burstShootRate * 0.65f)
            {
                for (int i = 0; i < 4; i++)
                {
                    Dust rock = Dust.NewDustPerfect(npc.Center + Main.rand.NextVector2Circular(75f, 75f), 1);
                    rock.color = Color.Yellow;
                    rock.velocity = (npc.Center - rock.position) * Main.rand.NextFloat(0.06f, 0.09f);
                    rock.scale = Main.rand.NextFloat(1f, 1.3f);
                    rock.noGravity = true;
                }
            }

            if (wrappedAttackTimer == burstShootRate - 1f)
            {
                // Play the sound.
                Main.PlaySound(SoundID.Item92, npc.Center);

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int damage = CalamityWorld.downedPolterghast ? 52 : 33;
                    float shootOffsetAngle = Main.rand.NextFloat(MathHelper.TwoPi);
                    for (int i = 0; i < spikesPerBurst; i++)
                    {
                        Vector2 spikeShootVelocity = (MathHelper.TwoPi * i / spikesPerBurst + shootOffsetAngle).ToRotationVector2() * burstSpeed;
                        Projectile.NewProjectile(npc.Center + spikeShootVelocity * 1.6f, spikeShootVelocity, ModContent.ProjectileType<CragmawSpike>(), damage, 0f);
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
            if (CalamityWorld.downedPolterghast)
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

            ref float attackSubstate = ref npc.ai[2];
            ref float slamCounter = ref npc.ai[3];

            switch ((int)attackSubstate)
            {
                // Dig in anticipation of the slam teleport.
                case 0:
                    // Disable tile collision and normal gravity, as the cragmaw will dig into the ground.
                    npc.noTileCollide = true;
                    npc.noGravity = true;

                    // Dig into the ground.
                    npc.position.Y += 3f;

                    // Fade out.
                    npc.Opacity = MathHelper.Clamp(npc.Opacity - opacityFadeoutIncrement * 0.6f, 0f, 1f);

                    // Stop taking damage.
                    npc.dontTakeDamage = true;

                    // Teleport once completely transparent.
                    if (npc.Opacity <= 0f)
                    {
                        attackSubstate = 1f;
                        npc.Center = Target.Center - Vector2.UnitY * verticalTeleportOffset;
                        npc.velocity = Vector2.Zero;
                        npc.netUpdate = true;
                    }
                    break;

                // Fade in again.
                case 1:
                    // Disable default gravity and tile collision, as it is expected that the cragmaw is in the air at the moment.
                    npc.noGravity = true;
                    npc.noTileCollide = true;

                    // Disable contact damage. This is done to prevent cheap hits from the teleport, either in multiplayer or due to fast upward movement.
                    npc.damage = 0;

                    // Fade in.
                    npc.Opacity = MathHelper.Clamp(npc.Opacity + opacityFadeoutIncrement, 0f, 1f);

                    // Do the slam once completely faded in.
                    if (npc.Opacity >= 1f)
                    {
                        attackSubstate = 2f;
                        AttackTimer = 0f;
                        npc.velocity = Vector2.UnitY * maxSlamSpeed * 0.251f;
                        npc.netUpdate = true;
                    }
                    break;

                // Do the slam.
                case 2:
                    // Disable default gravity and tile collision. Both of them will be calculated manually.
                    npc.noGravity = true;
                    npc.noTileCollide = true;

                    // Slam downward.
                    npc.velocity.Y = MathHelper.Clamp(npc.velocity.Y + slamAcceleration, 0f, maxSlamSpeed);

                    // Register ground collision from the slam.
                    // Once it has been hit a nuke explosion projectile is created on the ground, along with homing nuclear drops.
                    if ((npc.Bottom.Y > Target.Bottom.Y && Collision.SolidCollision(npc.BottomLeft, npc.width, 1)) || AttackTimer > 180f)
                    {
                        npc.velocity = Vector2.Zero;

                        if (AttackTimer == 1f)
                        {
                            Main.PlaySound(SoundID.DD2_ExplosiveTrapExplode, npc.Center);
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                int nukeDamage = CalamityWorld.downedPolterghast ? 72 : 38;
                                int dropletDamage = (int)(nukeDamage * 0.6f);
                                int explosion = Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<CragmawExplosion>(), nukeDamage, 0f);
                                if (Main.projectile.IndexInRange(explosion))
                                    Main.projectile[explosion].Bottom = npc.Bottom + Vector2.UnitY * 4f;

                                for (int i = 0; i < 12; i++)
                                {
                                    Vector2 dropletVelocity = -Vector2.UnitY.RotatedByRandom(0.71f) * Main.rand.NextFloat(8f, 12f);
                                    Projectile.NewProjectile(npc.Center, dropletVelocity, ModContent.ProjectileType<CragmawAcidDrop>(), dropletDamage, 0f);
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
                            npc.netUpdate = true;
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
            if (CalamityWorld.downedPolterghast)
            {
                opacityFadeoutIncrement += 0.025f;
                chargeupTelegraphTime -= 15;
            }
            if (InPhase2)
            {
                opacityFadeoutIncrement += 0.01f;
            }

            ref float attackSubstate = ref npc.ai[2];

            switch ((int)attackSubstate)
            {
                // Dig to reposition.
                case 0:
                    // Disable tile collision and normal gravity, as the cragmaw will dig into the ground.
                    npc.noTileCollide = true;
                    npc.noGravity = true;

                    // Dig into the ground.
                    npc.position.Y += 3f;

                    // Fade out.
                    npc.Opacity = MathHelper.Clamp(npc.Opacity - opacityFadeoutIncrement * 0.6f, 0f, 1f);

                    // Stop taking damage.
                    npc.dontTakeDamage = true;

                    // Teleport once completely transparent.
                    if (npc.Opacity <= 0f)
                    {
                        attackSubstate = 1f;
                        if (WorldUtils.Find(Target.Center.ToTileCoordinates(), Searches.Chain(new Searches.Down(1000), new CustomConditions.SolidOrPlatform()), out Point teleportPosition))
                            npc.Bottom = teleportPosition.ToWorldCoordinates() + Vector2.UnitY * digReapperTime * digReapperSpeed;
                        else
                            SelectNextAttack();

                        AttackTimer = 0f;
                        npc.velocity = Vector2.Zero;
                        npc.netUpdate = true;
                    }
                    break;

                // Prepare and fire the laser.
                case 1:
                    // Come out of the ground.
                    if (AttackTimer < digReapperTime)
                    {
                        npc.noTileCollide = true;
                        npc.noGravity = true;
                        npc.position.Y -= digReapperSpeed;
                    }
                    if (AttackTimer == digReapperTime)
                        npc.netUpdate = true;

                    // Fade in.
                    npc.Opacity = MathHelper.Clamp(npc.Opacity + opacityFadeoutIncrement, 0f, 1f);

                    // Create some charge dust.
                    if (AttackTimer > digReapperTime && AttackTimer < digReapperTime + chargeupTelegraphTime)
                    {
                        Dust chargeupDust = Dust.NewDustPerfect(npc.Center, 267);
                        chargeupDust.position -= Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi) * npc.height * 0.4f;
                        chargeupDust.velocity = -Vector2.UnitY.RotatedByRandom(0.31f) * Main.rand.NextFloat(2f, 6f);
                        chargeupDust.color = Color.Lerp(Color.Green, Color.Yellow, Main.rand.NextFloat());
                        chargeupDust.scale = Main.rand.NextFloat(0.95f, 1.35f);
                        chargeupDust.noGravity = true;
                    }

                    // Release the laser.
                    if (AttackTimer == digReapperTime + chargeupTelegraphTime)
                    {
                        // Play a sound and create dust.
                        Main.PlaySound(SoundID.Zombie, npc.Center, 104);
                        for (int i = 0; i < 40; i++)
                        {
                            Dust burstDust = Dust.NewDustPerfect(npc.Center + Main.rand.NextVector2Circular(75f, 75f), 267);
                            burstDust.velocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(2f, 6f);
                            burstDust.color = Color.Lerp(Color.Green, Color.Yellow, Main.rand.NextFloat());
                            burstDust.scale = Main.rand.NextFloat(1.1f, 1.45f);
                            burstDust.noGravity = true;
                        }

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int laserbeamDamage = CalamityWorld.downedPolterghast ? 120 : 40;
                            Projectile.NewProjectile(npc.Center, -Vector2.UnitY, ModContent.ProjectileType<CragmawBeam>(), laserbeamDamage, 0f, Main.myPlayer, 0f, npc.whoAmI);
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
                Projectile.NewProjectile(npc.Center, -Vector2.UnitY * 4f, ModContent.ProjectileType<CragmawVibeCheckChain>(), 0, 0f, Main.myPlayer, npc.whoAmI, npc.target);
            if (AttackTimer > CragmawVibeCheckChain.Lifetime + 30f)
                SelectNextAttack();
        }

        public void SelectNextAttack()
        {
            // Reset the attack timer and optional attack variables.
            AttackTimer = 0f;
            npc.ai[2] = 0f;
            npc.ai[3] = 0f;

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

            npc.netUpdate = true;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * 0.8f * bossLifeScale);
            npc.damage = (int)(npc.damage * 0.85f);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            Texture2D texture = InPhase2 ? ModContent.GetTexture("CalamityMod/NPCs/AcidRain/CragmawMire2") : ModContent.GetTexture("CalamityMod/NPCs/AcidRain/CragmawMire");
            CalamityMod.DrawTexture(spriteBatch, texture, 0, npc, npc.GetAlpha(drawColor), true);
            return false;
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter++;
            if (npc.frameCounter % 6 == 5)
                npc.frame.Y += frameHeight;
            if (npc.frame.Y >= frameHeight * 2)
                npc.frame.Y = 0;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
                Dust.NewDust(npc.position, npc.width, npc.height, (int)CalamityDusts.SulfurousSeaAcid, hitDirection, -1f, 0, default, 1f);
            if (npc.life <= 0)
            {
                Gore.NewGore(npc.position, -Vector2.UnitY.RotatedByRandom(0.4f) * 4f, mod.GetGoreSlot("Gores/AcidRain/CragmawMireP2Gore"), npc.scale);
                Gore.NewGore(npc.position, -Vector2.UnitY.RotatedByRandom(0.4f) * 4f, mod.GetGoreSlot("Gores/AcidRain/CragmawMireP2Gore2"), npc.scale);
                Gore.NewGore(npc.position, -Vector2.UnitY.RotatedByRandom(0.4f) * 4f, mod.GetGoreSlot("Gores/AcidRain/CragmawMireP2Gore3"), npc.scale);
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit) => target.AddBuff(ModContent.BuffType<Irradiated>(), 300);

        public override void NPCLoot()
        {
            DropHelper.DropItemChance(npc, ModContent.ItemType<NuclearRod>(), CalamityWorld.downedPolterghast ? 0.1f : 1f);
            DropHelper.DropItemChance(npc, ModContent.ItemType<SpentFuelContainer>(), CalamityWorld.downedPolterghast ? 0.1f : 1f);
        }
    }
}
