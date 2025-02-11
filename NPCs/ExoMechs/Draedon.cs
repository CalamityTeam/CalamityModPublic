using System;
using System.IO;
using CalamityMod.Events;
using CalamityMod.Items.Weapons.DraedonsArsenal;
using CalamityMod.NPCs.ExoMechs.Ares;
using CalamityMod.NPCs.ExoMechs.Thanatos;
using CalamityMod.Sounds;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using ApolloBoss = CalamityMod.NPCs.ExoMechs.Apollo.Apollo;
using ArtemisBoss = CalamityMod.NPCs.ExoMechs.Artemis.Artemis;

namespace CalamityMod.NPCs.ExoMechs
{
    [LongDistanceNetSync]
    public class Draedon : ModNPC
    {
        public int KillReappearTextCountdown;
        public float DefeatTimer;
        public float ProjectorOffset = 1000;
        public float ProjFrameCounter;
        public float ProjFrameChangeCounter;
        public bool ShouldStartStandingUp;
        public bool exoMechdusa;
        public Vector2 HoverDestinationOffset
        {
            get => new(NPC.ai[1], NPC.ai[2]);
            set
            {
                NPC.ai[1] = value.X;
                NPC.ai[2] = value.Y;
            }
        }
        public Player PlayerToFollow => Main.player[NPC.target];
        public ref float TalkTimer => ref NPC.ai[0];
        public ref float GeneralTimer => ref NPC.ai[3];
        public ref float DialogueType => ref NPC.localAI[0];
        public ref float HologramEffectTimer => ref NPC.localAI[1];
        public bool HasBeenKilled
        {
            get => NPC.localAI[2] == 1f;
            set => NPC.localAI[2] = value.ToInt();
        }
        public ref float KillReappearDelay => ref NPC.localAI[3];
        public static bool ExoMechIsPresent
        {
            get
            {
                if (NPC.AnyNPCs(ModContent.NPCType<ThanatosHead>()))
                    return true;

                if (NPC.AnyNPCs(ModContent.NPCType<AresBody>()))
                    return true;

                if (NPC.AnyNPCs(ModContent.NPCType<ArtemisBoss>()) || NPC.AnyNPCs(ModContent.NPCType<ApolloBoss>()))
                    return true;

                return false;
            }
        }
        public ref float BossRushCounter => ref NPC.Calamity().newAI[0];
        public static readonly Color TextColor = new(155, 255, 255);
        public static readonly Color TextColorEdgy = new(213, 4, 11);
        public const int HologramFadeinTime = 45;
        public const int TalkDelay = 150;
        public const int DelayPerDialogLine = 130;
        public const int ExoMechChooseDelay = TalkDelay + DelayPerDialogLine * 4 + 10;
        public const int ExoMechShakeTime = 100;
        public const int ExoMechPhaseDialogueTime = ExoMechChooseDelay + ExoMechShakeTime;
        public const int DelayBeforeDefeatStandup = 30;

        public static readonly SoundStyle LaughSound = new("CalamityMod/Sounds/Custom/DraedonLaugh");
        public static readonly SoundStyle TeleportSound = new("CalamityMod/Sounds/Custom/DraedonTeleport");
        public static readonly SoundStyle SelectionSound = new("CalamityMod/Sounds/Custom/Codebreaker/ExoMechsIconSelect");

        public static Asset<Texture2D> Texture_Glow;
        public static Asset<Texture2D> HoloTexture;
        public static Asset<Texture2D> ProjectorTexture;
        public static Asset<Texture2D> ProjectorTexture_Glow;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 12;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                PortraitPositionYOverride = 40f,
                Scale = 0.7f,
                PortraitScale = 0.85f,
                SpriteDirection = 1
            };
            value.Position.Y += 45f;
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;
            NPCID.Sets.ShouldBeCountedAsBoss[NPC.type] = true;
            NPCID.Sets.MustAlwaysDraw[NPC.type] = true;
            if (!Main.dedServ)
            {
                Texture_Glow = ModContent.Request<Texture2D>(Texture + "Glowmask", AssetRequestMode.AsyncLoad);
                HoloTexture = ModContent.Request<Texture2D>("CalamityMod/NPCs/ExoMechs/HologramDraedon", AssetRequestMode.AsyncLoad);
                ProjectorTexture = ModContent.Request<Texture2D>(Texture + "Projector", AssetRequestMode.AsyncLoad);
                ProjectorTexture_Glow = ModContent.Request<Texture2D>(Texture + "ProjectorGlowmask", AssetRequestMode.AsyncLoad);
            }
        }

        public override void SetDefaults()
        {
            NPC.damage = 0;
            NPC.width = NPC.height = 86;
            NPC.defense = 100;
            NPC.lifeMax = 16000;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.dontTakeDamage = true;
            NPC.aiStyle = AIType = -1;
            NPC.knockBackResist = 0f;
            NPC.DeathSound = SoundID.NPCDeath14;
            NPC.chaseable = false;
            NPC.Calamity().ProvidesProximityRage = false;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.Draedon")
            });
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(DialogueType);
            writer.Write(DefeatTimer);
            writer.Write(ProjectorOffset);
            writer.Write(ProjFrameCounter);
            writer.Write(ProjFrameChangeCounter);
            writer.Write(HologramEffectTimer);
            writer.Write(KillReappearDelay);
            writer.Write(ShouldStartStandingUp);
            writer.Write(HasBeenKilled);
            writer.Write(BossRushCounter);
            writer.Write(exoMechdusa);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            DialogueType = reader.ReadSingle();
            DefeatTimer = reader.ReadSingle();
            ProjectorOffset = reader.ReadSingle();
            ProjFrameCounter = reader.ReadSingle();
            ProjFrameChangeCounter = reader.ReadSingle();
            HologramEffectTimer = reader.ReadSingle();
            KillReappearDelay = reader.ReadSingle();
            ShouldStartStandingUp = reader.ReadBoolean();
            HasBeenKilled = reader.ReadBoolean();
            BossRushCounter = reader.ReadSingle();
            exoMechdusa = reader.ReadBoolean();
        }

        public override void AI()
        {
            // Be immune to every debuff.
            for (int k = 0; k < NPC.buffImmune.Length; k++)
                NPC.buffImmune[k] = true;

            // Set the whoAmI variable.
            CalamityGlobalNPC.draedon = NPC.whoAmI;
            CalamityGlobalNPC.draedonAmbience = -1;

            // Prevent stupid natural despawns.
            NPC.timeLeft = 3600;

            // Check if Boss Rush is active
            bool bossRush = BossRushEvent.BossRushActive;

            // Emit music. If the battle is ongoing, Draedon emits the battle theme.
            // Otherwise, he emits his trademark ambience.
            // This takes priority over anything except Moon Lord's music fadeout.
            if (!ExoMechIsPresent)
                CalamityGlobalNPC.draedonAmbience = NPC.whoAmI;

            // Decide an initial target and play a teleport sound on the first frame.
            if (TalkTimer == 0f)
            {
                NPC.TargetClosest(false);
                SoundEngine.PlaySound(TeleportSound, PlayerToFollow.Center);
            }

            // Pick someone else to pay attention to if the old target is gone.
            if (PlayerToFollow.dead || !PlayerToFollow.active)
            {
                NPC.TargetClosest(false);

                // Fuck off if no living target exists.
                if (PlayerToFollow.dead || !PlayerToFollow.active)
                {
                    NPC.life = 0;
                    NPC.HitEffect();
                    NPC.active = false;
                    NPC.netUpdate = true;
                    return;
                }
            }

            // Stay within the world.
            NPC.position.Y = MathHelper.Clamp(NPC.position.Y, 150f, Main.maxTilesY * 16f - 150f);

            NPC.spriteDirection = (PlayerToFollow.Center.X < NPC.Center.X).ToDirectionInt();

            // Exo Mechdusa bool setting
            if (!exoMechdusa && CalamityWorld.DraedonMechdusa && Main.zenithWorld)
            {
                exoMechdusa = true;
                CalamityWorld.DraedonMechdusa = false;
                if (Main.netMode != NetmodeID.SinglePlayer)
                {
                    var netMessage = CalamityMod.Instance.GetPacket();
                    netMessage.Write((byte)CalamityModMessageType.CodebreakerSummonStuff);
                    netMessage.Write(CalamityWorld.DraedonSummonCountdown);
                    netMessage.WriteVector2(CalamityWorld.DraedonSummonPosition);
                    netMessage.Write(CalamityWorld.DraedonMechdusa);
                    netMessage.Send();
                }
            }

            // Handle delays when re-appearing after being killed.
            if (KillReappearDelay > 0f)
            {
                if (KillReappearDelay <= 60f)
                    ProjectorOffset -= 14.5f;
                NPC.Opacity = 0f;
                KillReappearDelay--;
                if (KillReappearDelay <= 0f)
                {
                    KillReappearTextCountdown = 96;
                    DefeatTimer = MathHelper.Max(DefeatTimer, DelayBeforeDefeatStandup + TalkDelay * 2f + 120f);
                    NPC.netUpdate = true;
                }
                return;
            }
            if (KillReappearTextCountdown > 0)
            {
                NPC.Opacity = MathHelper.Clamp(NPC.Opacity + 0.05f, 0f, 1f);
                KillReappearTextCountdown--;
                if (KillReappearTextCountdown == 20)
                    CalamityUtils.DisplayLocalizedText("Mods.CalamityMod.Status.Boss.DraedonEndKillAttemptText", TextColor);
                return;
            }

            // Synchronize the hologram effect and talk timer at the beginning.
            if (TalkTimer <= HologramFadeinTime)
            {
                HologramEffectTimer = TalkTimer;

                if (!HasBeenKilled)
                    NPC.Opacity = Utils.GetLerpValue(0f, 8f, TalkTimer, true);
            }

            // Play the stand up animation after teleportation.
            if (TalkTimer == HologramFadeinTime + 5f)
                ShouldStartStandingUp = true;

            // Gloss over the arbitrary details and just get to the Exo Mech selection if Draedon has already been talked to.
            if ((CalamityWorld.TalkedToDraedon || bossRush) && TalkTimer > 70 && TalkTimer < TalkDelay * 4f - 25f && !exoMechdusa)
            {
                TalkTimer = TalkDelay * 4f - 25f;
                NPC.netUpdate = true;
            }

            if (!exoMechdusa)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient && TalkTimer == TalkDelay)
                {
                    CalamityUtils.DisplayLocalizedText("Mods.CalamityMod.Status.Boss.DraedonIntroductionText1", TextColor);
                    NPC.netUpdate = true;
                }

                if (Main.netMode != NetmodeID.MultiplayerClient && TalkTimer == TalkDelay + DelayPerDialogLine)
                {
                    CalamityUtils.DisplayLocalizedText("Mods.CalamityMod.Status.Boss.DraedonIntroductionText2", TextColor);
                    NPC.netUpdate = true;
                }

                if (Main.netMode != NetmodeID.MultiplayerClient && TalkTimer == TalkDelay + DelayPerDialogLine * 2f)
                {
                    CalamityUtils.DisplayLocalizedText("Mods.CalamityMod.Status.Boss.DraedonIntroductionText3", TextColor);
                    NPC.netUpdate = true;
                }

                if (Main.netMode != NetmodeID.MultiplayerClient && TalkTimer == TalkDelay + DelayPerDialogLine * 3f)
                {
                    CalamityUtils.DisplayLocalizedText("Mods.CalamityMod.Status.Boss.DraedonIntroductionText4", TextColor);
                    NPC.netUpdate = true;
                }

                // Inform the player who summoned draedon they may choose the first mech and cause a selection UI to appear over their head.
                if (Main.netMode != NetmodeID.MultiplayerClient && TalkTimer == TalkDelay + DelayPerDialogLine * 4f)
                {
                    if (bossRush)
                        CalamityUtils.DisplayLocalizedText("Mods.CalamityMod.Status.Boss.DraedonBossRushText", TextColorEdgy);
                    else if (CalamityWorld.TalkedToDraedon)
                        CalamityUtils.DisplayLocalizedText("Mods.CalamityMod.Status.Boss.DraedonResummonText", TextColorEdgy);
                    else
                        CalamityUtils.DisplayLocalizedText("Mods.CalamityMod.Status.Boss.DraedonIntroductionText5", TextColorEdgy);

                    // Mark Draedon as talked to.
                    if (!CalamityWorld.TalkedToDraedon)
                    {
                        CalamityWorld.TalkedToDraedon = true;
                        CalamityNetcode.SyncWorld();
                    }

                    NPC.netUpdate = true;
                }

                // Wait for the player to select an exo mech.
                if (TalkTimer >= ExoMechChooseDelay && TalkTimer < ExoMechChooseDelay + 8f && CalamityWorld.DraedonMechToSummon == ExoMech.None)
                {
                    PlayerToFollow.Calamity().AbleToSelectExoMech = true;
                    TalkTimer = ExoMechChooseDelay;
                    if (bossRush)
                    {
                        // Summon a random exo mech if you wait too long
                        BossRushCounter++;
                        if (BossRushCounter > 1200 && CalamityWorld.DraedonMechToSummon == ExoMech.None)
                        {
                            CalamityWorld.DraedonMechToSummon = (ExoMech)Main.rand.Next(1, 4);

                            if (Main.netMode != NetmodeID.SinglePlayer)
                            {
                                var netMessage = CalamityMod.Instance.GetPacket();
                                netMessage.Write((byte)CalamityModMessageType.ExoMechSelection);
                                netMessage.Write((int)CalamityWorld.DraedonMechToSummon);
                                netMessage.Send();
                            }
                        }
                    }
                }
            }
            else
            {
                if (Main.netMode != NetmodeID.MultiplayerClient && TalkTimer == TalkDelay)
                {
                    CalamityUtils.DisplayLocalizedText("Mods.CalamityMod.Status.Boss.DraedonMechdusaBeginText", TextColorEdgy);
                    NPC.netUpdate = true;
                }

                if (Main.netMode != NetmodeID.MultiplayerClient && TalkTimer == TalkDelay + 60)
                {
                    // Mark Draedon as talked to.
                    if (!CalamityWorld.TalkedToDraedon)
                    {
                        CalamityWorld.TalkedToDraedon = true;
                        CalamityNetcode.SyncWorld();
                    }

                    NPC.netUpdate = true;
                }
            }

            // Fly around once the exo mechs have been spawned.
            if (ExoMechIsPresent || DefeatTimer > 0f)
            {
                FlyAroundInGamerChair();
                GeneralTimer++;
            }

            // Make the screen rumble and summon the exo mechs.
            if (TalkTimer > ExoMechChooseDelay + 8f && TalkTimer < ExoMechPhaseDialogueTime)
            {
                Main.LocalPlayer.Calamity().GeneralScreenShakePower = Utils.GetLerpValue(4200f, 1400f, Main.LocalPlayer.Distance(PlayerToFollow.Center), true) * 18f;
                Main.LocalPlayer.Calamity().GeneralScreenShakePower *= Utils.GetLerpValue(ExoMechChooseDelay + 5f, ExoMechPhaseDialogueTime, TalkTimer, true);
            }

            // Summon the selected exo mech.
            if ((TalkTimer == ExoMechChooseDelay + 10f || (TalkTimer == TalkDelay + 60 && exoMechdusa)) && !ExoMechIsPresent)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    SummonExoMech();

                if (Main.netMode != NetmodeID.Server)
                {
                    SoundEngine.PlaySound(CommonCalamitySounds.FlareSound with { Volume = CommonCalamitySounds.FlareSound.Volume * 1.55f }, PlayerToFollow.Center);
                    if (!exoMechdusa)
                        SoundEngine.PlaySound(SelectionSound, PlayerToFollow.Center);
                }
            }

            if (!bossRush && !exoMechdusa)
            {
                // Dialogue lines depending on what phase the exo mechs are at.
                switch ((int)DialogueType)
                {
                    case 1:

                        if (Main.netMode != NetmodeID.MultiplayerClient && TalkTimer == ExoMechPhaseDialogueTime)
                        {
                            CalamityUtils.DisplayLocalizedText("Mods.CalamityMod.Status.Boss.DraedonExoPhase1Text1", TextColor);
                            NPC.netUpdate = true;
                        }

                        if (Main.netMode != NetmodeID.MultiplayerClient && TalkTimer == ExoMechPhaseDialogueTime + DelayPerDialogLine)
                        {
                            CalamityUtils.DisplayLocalizedText("Mods.CalamityMod.Status.Boss.DraedonExoPhase1Text2", TextColor);
                            NPC.netUpdate = true;
                        }

                        break;

                    case 2:

                        if (TalkTimer == ExoMechPhaseDialogueTime)
                        {
                            SoundEngine.PlaySound(LaughSound, PlayerToFollow.Center);
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                CalamityUtils.DisplayLocalizedText("Mods.CalamityMod.Status.Boss.DraedonExoPhase2Text1", TextColor);
                                NPC.netUpdate = true;
                            }
                        }

                        if (Main.netMode != NetmodeID.MultiplayerClient && TalkTimer == ExoMechPhaseDialogueTime + DelayPerDialogLine)
                        {
                            CalamityUtils.DisplayLocalizedText("Mods.CalamityMod.Status.Boss.DraedonExoPhase2Text2", TextColor);
                            NPC.netUpdate = true;
                        }

                        break;

                    case 3:

                        if (Main.netMode != NetmodeID.MultiplayerClient && TalkTimer == ExoMechPhaseDialogueTime)
                        {
                            CalamityUtils.DisplayLocalizedText("Mods.CalamityMod.Status.Boss.DraedonExoPhase3Text1", TextColor);
                            NPC.netUpdate = true;
                        }

                        if (TalkTimer == ExoMechPhaseDialogueTime + DelayPerDialogLine)
                        {
                            SoundEngine.PlaySound(LaughSound, PlayerToFollow.Center);
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                CalamityUtils.DisplayLocalizedText("Mods.CalamityMod.Status.Boss.DraedonExoPhase3Text2", TextColor);
                                NPC.netUpdate = true;
                            }
                        }

                        break;

                    case 4:

                        if (Main.netMode != NetmodeID.MultiplayerClient && TalkTimer == ExoMechPhaseDialogueTime)
                        {
                            CalamityUtils.DisplayLocalizedText("Mods.CalamityMod.Status.Boss.DraedonExoPhase4Text1", TextColor);
                            NPC.netUpdate = true;
                        }

                        if (Main.netMode != NetmodeID.MultiplayerClient && TalkTimer == ExoMechPhaseDialogueTime + DelayPerDialogLine)
                        {
                            CalamityUtils.DisplayLocalizedText("Mods.CalamityMod.Status.Boss.DraedonExoPhase4Text2", TextColor);
                            NPC.netUpdate = true;
                        }

                        break;

                    case 5:

                        if (Main.netMode != NetmodeID.MultiplayerClient && TalkTimer == ExoMechPhaseDialogueTime)
                        {
                            CalamityUtils.DisplayLocalizedText("Mods.CalamityMod.Status.Boss.DraedonExoPhase5Text1", TextColor);
                            NPC.netUpdate = true;
                        }

                        if (Main.netMode != NetmodeID.MultiplayerClient && TalkTimer == ExoMechPhaseDialogueTime + DelayPerDialogLine)
                        {
                            CalamityUtils.DisplayLocalizedText("Mods.CalamityMod.Status.Boss.DraedonExoPhase5Text2", TextColor);
                            NPC.netUpdate = true;
                        }

                        break;

                    case 6:

                        if (Main.netMode != NetmodeID.MultiplayerClient && TalkTimer == ExoMechPhaseDialogueTime)
                        {
                            CalamityUtils.DisplayLocalizedText("Mods.CalamityMod.Status.Boss.DraedonExoPhase6Text1", TextColor);
                            NPC.netUpdate = true;
                        }

                        if (Main.netMode != NetmodeID.MultiplayerClient && TalkTimer == ExoMechPhaseDialogueTime + DelayPerDialogLine)
                        {
                            CalamityUtils.DisplayLocalizedText("Mods.CalamityMod.Status.Boss.DraedonExoPhase6Text2", TextColor);
                            NPC.netUpdate = true;
                        }

                        if (TalkTimer == ExoMechPhaseDialogueTime + DelayPerDialogLine * 2f)
                        {
                            SoundEngine.PlaySound(LaughSound, PlayerToFollow.Center);
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                CalamityUtils.DisplayLocalizedText("Mods.CalamityMod.Status.Boss.DraedonExoPhase6Text3", TextColor);
                                NPC.netUpdate = true;
                            }
                        }

                        break;
                }
            }

            if (TalkTimer > ExoMechChooseDelay + 10f && !ExoMechIsPresent)
            {
                HandleDefeatStuff();
                DefeatTimer++;
            }

            TalkTimer++;

            if (ExoMechIsPresent && Main.zenithWorld && GeneralTimer % 60 == 0 && !exoMechdusa)
            {
                SoundEngine.PlaySound(SoundID.Item33, NPC.Center);
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Vector2 shoot = PlayerToFollow.Center - NPC.Center;
                    shoot.Normalize();
                    shoot *= 4;
                    int p = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center - Vector2.UnitY * 30, shoot, ModContent.ProjectileType<Projectiles.Turret.DraedonLaser>(), 116, 0, Main.myPlayer);
                    if (p.WithinBounds(Main.maxProjectiles))
                    {
                        Main.projectile[p].timeLeft *= 2;
                    }
                }
            }
        }

        // TODO -- Make this work in conjunction with exo mech transitions. This requires that the exo mech AIs be finished.
        public void FlyAroundInGamerChair()
        {
            // Define a hover destination offset if one hasn't been decided yet.
            if (Main.netMode != NetmodeID.MultiplayerClient && HoverDestinationOffset == Vector2.Zero)
            {
                float factor = Main.zenithWorld && !exoMechdusa ? 300f : 700f;
                HoverDestinationOffset = -Vector2.UnitY * factor;
                NPC.netUpdate = true;
            }

            // Switch hover destinations from time to time.
            if (Main.netMode != NetmodeID.MultiplayerClient && GeneralTimer % 480f == 479f)
            {
                Vector2 offsetDirection;
                Vector2 directionToTarget = NPC.SafeDirectionTo(PlayerToFollow.Center);

                // Reroll the offset direction if its direction noticably contrasts the current direction from the target.
                // This is done to prevent Draedon from selecting a position to zoom to that would make him have to move
                // close to the player.
                do
                    offsetDirection = Main.rand.NextVector2Unit();
                while (Vector2.Dot(directionToTarget, offsetDirection) > 0.2f);

                float factormin = Main.zenithWorld && !exoMechdusa ? 300f : 750f;
                float factormax = Main.zenithWorld && !exoMechdusa ? 700f : 1100f;
                HoverDestinationOffset = offsetDirection * Main.rand.NextFloat(factormin, factormax);
                NPC.netUpdate = true;
            }

            // Always hover to the side of the target when defeated.
            if (DefeatTimer > 5f)
                HoverDestinationOffset = Vector2.UnitX * (PlayerToFollow.Center.X < NPC.Center.X).ToDirectionInt() * 325f;

            Vector2 hoverDestination = PlayerToFollow.Center + HoverDestinationOffset;

            // Decide sprite direction based on movement if not close enough to the desination.
            // Not deciding this here results in Draedon using the default of looking at the target he's following.
            if (NPC.WithinRange(hoverDestination, 300f))
            {
                NPC.velocity *= 0.96f;

                float moveSpeed = MathHelper.Lerp(2f, 8f, Utils.GetLerpValue(45f, 275f, NPC.Distance(hoverDestination), true));
                NPC.Center = NPC.Center.MoveTowards(hoverDestination, moveSpeed);
            }
            else
            {
                if (DefeatTimer < DelayBeforeDefeatStandup)
                    NPC.spriteDirection = (NPC.velocity.X < 0f).ToDirectionInt();

                float flySpeed = DefeatTimer > 5f ? 14f : 32f;
                Vector2 idealVelocity = NPC.SafeDirectionTo(hoverDestination) * flySpeed;
                NPC.SimpleFlyMovement(idealVelocity, flySpeed / 400f);
                NPC.velocity = Vector2.Lerp(NPC.velocity, idealVelocity, 0.045f);
            }
        }

        public void SummonExoMech()
        {
            if (exoMechdusa)
            {
                Vector2 aresSpawnPosition = PlayerToFollow.Center - Vector2.UnitY * 1400f;
                NPC Ares = CalamityUtils.SpawnBossBetter(aresSpawnPosition, ModContent.NPCType<AresBody>());
                Ares.ModNPC<AresBody>().exoMechdusa = true;
            }
            else
            {
                switch (CalamityWorld.DraedonMechToSummon)
                {
                    // Summon Thanatos underground.
                    case ExoMech.Destroyer:
                        Vector2 thanatosSpawnPosition = PlayerToFollow.Center + Vector2.UnitY * 2100f;
                        NPC thanatos = CalamityUtils.SpawnBossBetter(thanatosSpawnPosition, ModContent.NPCType<ThanatosHead>());
                        if (thanatos != null)
                            thanatos.velocity = thanatos.SafeDirectionTo(PlayerToFollow.Center) * 40f;
                        break;

                    // Summon Ares in the sky, directly above the player.
                    case ExoMech.Prime:
                        Vector2 aresSpawnPosition = PlayerToFollow.Center - Vector2.UnitY * 1400f;
                        CalamityUtils.SpawnBossBetter(aresSpawnPosition, ModContent.NPCType<AresBody>());
                        break;

                    // Summon Apollo and Artemis above the player to their sides.
                    case ExoMech.Twins:
                        Vector2 artemisSpawnPosition = PlayerToFollow.Center + new Vector2(-1100f, -1600f);
                        Vector2 apolloSpawnPosition = PlayerToFollow.Center + new Vector2(1100f, -1600f);
                        CalamityUtils.SpawnBossBetter(artemisSpawnPosition, ModContent.NPCType<ArtemisBoss>());
                        CalamityUtils.SpawnBossBetter(apolloSpawnPosition, ModContent.NPCType<ApolloBoss>());
                        break;
                }
            }
        }

        public void HandleDefeatStuff()
        {
            // Become vulnerable after being defeated after a certain point.
            NPC.dontTakeDamage = DefeatTimer < TalkDelay * 2f + 50f || HasBeenKilled;
            NPC.Calamity().CanHaveBossHealthBar = !NPC.dontTakeDamage;
            NPC.Calamity().ShouldCloseHPBar = HasBeenKilled;
            NPC.chaseable = BossRushEvent.BossRushActive;

            bool leaving = ((DefeatTimer > DelayBeforeDefeatStandup + TalkDelay * 8f + 200f) || BossRushEvent.BossRushActive) && !exoMechdusa;

            // Fade away and disappear when leaving.
            if (leaving)
            {
                ProjectorOffset -= 9f;
                // Disappears slower if killed to give the projector enough time to fly offscreen
                float disFactor = HasBeenKilled ? 0.4f : 1f;
                HologramEffectTimer = MathHelper.Clamp(HologramEffectTimer - disFactor, 0f, HologramFadeinTime);
                if (HologramEffectTimer <= 0f)
                {
                    Main.BestiaryTracker.Kills.RegisterKill(NPC);
                    NPC.life = 0;
                    NPC.HitEffect();
                    NPC.active = false;
                    NPC.netUpdate = true;
                    // Die you lil piece of shit stop creating endless loop in BR
                    if (BossRushEvent.BossRushActive)
                        NPC.NPCLoot();
                }
            }

            // Fade back in as a hologram if the player tried to kill Draedon.
            else if (HasBeenKilled)
            {
                if (KillReappearDelay <= 0f)
                {
                    Lighting.AddLight(NPC.Center, 0.5f, 1.25f, 1.25f);
                    if (ProjFrameChangeCounter == 0)
                    {
                        Dust d = Main.dust[Dust.NewDust(new Vector2(NPC.Center.X - 45, NPC.Center.Y - 70), NPC.width, (int)(NPC.height * 1.5f), DustID.Vortex, 0, Main.rand.Next(-2, -1), 60)];
                        d.noGravity = true;
                    }
                }
                HologramEffectTimer = MathHelper.Clamp(HologramEffectTimer + 1f, 0f, HologramFadeinTime - 5f);
            }

            // Adjust opacity.
            if (!HasBeenKilled)
                NPC.Opacity = HologramEffectTimer / HologramFadeinTime;

            // Stand up in awe after a small amount of time has passed.
            if (DefeatTimer > DelayBeforeDefeatStandup && DefeatTimer < TalkDelay * 2f + 50f)
                ShouldStartStandingUp = true;

            // Different text if Exo Mechdusa
            if (exoMechdusa && Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (DefeatTimer == DelayBeforeDefeatStandup + 50f)
                    CalamityUtils.DisplayLocalizedText("Mods.CalamityMod.Status.Boss.DraedonMechdusaEndText1", TextColor);

                if (DefeatTimer == DelayBeforeDefeatStandup + TalkDelay + 50f)
                    CalamityUtils.DisplayLocalizedText("Mods.CalamityMod.Status.Boss.DraedonMechdusaEndText2", TextColor);
            }
            // Otherwise do normal text
            else if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (DefeatTimer == DelayBeforeDefeatStandup + 50f)
                    CalamityUtils.DisplayLocalizedText("Mods.CalamityMod.Status.Boss.DraedonEndText1", TextColor);

                if (DefeatTimer == DelayBeforeDefeatStandup + TalkDelay + 50f)
                    CalamityUtils.DisplayLocalizedText("Mods.CalamityMod.Status.Boss.DraedonEndText2", TextColor);

                // After this point Draedon becomes vulnerable.
                // He sits back down as well as he thinks for a bit.
                // Killing him will cause gore to appear but also for Draedon to come back as a hologram.
                if (DefeatTimer == DelayBeforeDefeatStandup + TalkDelay * 2f + 50f)
                    CalamityUtils.DisplayLocalizedText("Mods.CalamityMod.Status.Boss.DraedonEndText3", TextColor);

                if (DefeatTimer == DelayBeforeDefeatStandup + TalkDelay * 3f + 165f)
                    CalamityUtils.DisplayLocalizedText("Mods.CalamityMod.Status.Boss.DraedonEndText4", TextColor);

                if (DefeatTimer == DelayBeforeDefeatStandup + TalkDelay * 4f + 165f)
                    CalamityUtils.DisplayLocalizedText("Mods.CalamityMod.Status.Boss.DraedonEndText5", TextColor);

                if (DefeatTimer == DelayBeforeDefeatStandup + TalkDelay * 5f + 165f)
                    CalamityUtils.DisplayLocalizedText("Mods.CalamityMod.Status.Boss.DraedonEndText6", TextColor);

                if (DefeatTimer == DelayBeforeDefeatStandup + TalkDelay * 6f + 165f)
                    CalamityUtils.DisplayLocalizedText("Mods.CalamityMod.Status.Boss.DraedonEndText7", TextColor);

                if (DefeatTimer == DelayBeforeDefeatStandup + TalkDelay * 7f + 165f)
                    CalamityUtils.DisplayLocalizedText("Mods.CalamityMod.Status.Boss.DraedonEndText8", TextColor);

                if (DefeatTimer == DelayBeforeDefeatStandup + TalkDelay * 8f + 165f)
                    CalamityUtils.DisplayLocalizedText("Mods.CalamityMod.Status.Boss.DraedonEndText9", TextColor);
            }
        }

        // Draedon should not manually despawn.
        public override bool CheckActive() => false;

        public override Color? GetAlpha(Color drawColor)
        {
            float teleportFade = Utils.GetLerpValue(0f, HologramFadeinTime, HologramEffectTimer, true);
            Color color = Color.Lerp(drawColor, Color.Cyan, 1f - (float)Math.Pow(teleportFade, 5D));
            color.A = (byte)(int)(teleportFade * 255f);

            return color * NPC.Opacity;
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frame.Width = 100;

            int xFrame = NPC.frame.X / NPC.frame.Width;
            int yFrame = NPC.frame.Y / frameHeight;
            int frame = xFrame * Main.npcFrameCount[NPC.type] + yFrame;

            // Prepare to stand up if called for and not already doing so.
            if (ShouldStartStandingUp && frame > 23)
                frame = 0;

            int frameChangeDelay = 7;
            bool shouldNotSitDown = (DefeatTimer > DelayBeforeDefeatStandup && DefeatTimer < TalkDelay * 2f + 10f) || (exoMechdusa && DefeatTimer > 0);

            NPC.frameCounter++;
            if (NPC.frameCounter >= frameChangeDelay)
            {
                frame++;

                if (!ShouldStartStandingUp && (frame < 23 || frame > 47))
                    frame = 23;

                // Do the sit animation infinitely if Draedon should not sit down again.
                if (shouldNotSitDown && frame >= 16)
                    frame = 11;

                if (frame >= 23 && ShouldStartStandingUp)
                {
                    frame = 0;
                    ShouldStartStandingUp = false;
                }

                NPC.frameCounter = 0;
            }

            NPC.frame.X = frame / Main.npcFrameCount[NPC.type] * NPC.frame.Width;
            NPC.frame.Y = frame % Main.npcFrameCount[NPC.type] * frameHeight;

            // Handle framing for the projector
            ProjFrameChangeCounter++;
            if (ProjFrameChangeCounter >= 6)
            {
                ProjFrameCounter++;
                ProjFrameChangeCounter = 0;
            }
            if (ProjFrameCounter > 3)
            {
                ProjFrameCounter = 0;
            }
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment) => NPC.lifeMax = 16000;

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life > 0)
                return;

            if (Main.netMode != NetmodeID.Server && !HasBeenKilled)
            {
                for (int i = 1; i <= 4; i++)
                {
                    Vector2 goreSpawnOffset = Main.rand.NextVector2Circular(12f, 12f);
                    Vector2 draedonPieceVelocity = Main.rand.NextVector2CircularEdge(7f, 7f) - Vector2.UnitY * 8f;
                    Vector2 chairPieceVelocity = Vector2.UnitY.RotatedByRandom(0.13f) * Main.rand.NextFloat(4.45f, 5.4f);
                    Gore.NewGoreDirect(NPC.GetSource_Death(), NPC.Center + goreSpawnOffset, draedonPieceVelocity, Mod.Find<ModGore>($"Draedon{i}").Type);

                    goreSpawnOffset = Main.rand.NextVector2Circular(18f, 18f);
                    Gore.NewGoreDirect(NPC.GetSource_Death(), NPC.Center + goreSpawnOffset, chairPieceVelocity, Mod.Find<ModGore>($"Chair{i}").Type);
                }
            }
        }

        public override bool CheckDead()
        {
            if (BossRushEvent.BossRushActive || exoMechdusa)
                return true;

            if (!HasBeenKilled)
            {
                HologramEffectTimer = 0f;
                KillReappearDelay = 160f;
                NPC.dontTakeDamage = true;
                HasBeenKilled = true;
                NPC.life = NPC.lifeMax;
                NPC.active = true;
                NPC.netUpdate = true;
            }
            return false;
        }

        // Always instantly kill Draedon when he's vulnerable
        public override void ModifyIncomingHit(ref NPC.HitModifiers modifiers)
        {
            modifiers.SourceDamage *= 56f;
            modifiers.SourceDamage.Flat += NPC.lifeMax + Main.rand.NextFloat(50f, 750f);
            modifiers.SetCrit();
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (!NPC.IsABestiaryIconDummy)
                spriteBatch.EnterShaderRegion();
            bool holo = HasBeenKilled && KillReappearDelay <= 0;
            bool leaving = HasBeenKilled && DefeatTimer > DelayBeforeDefeatStandup + TalkDelay * 8f + 200f;
            Texture2D texture = HasBeenKilled && KillReappearDelay <= 0f ? HoloTexture.Value : TextureAssets.Npc[NPC.type].Value;
            Texture2D glowmask = Texture_Glow.Value;
            Texture2D projector = ProjectorTexture.Value;
            Texture2D projectorglow = ProjectorTexture_Glow.Value;
            Texture2D gun = TextureAssets.Item[ModContent.ItemType<PulseRifle>()].Value;
            Rectangle frame = NPC.frame;

            Vector2 drawPosition = NPC.Center - screenPos - Vector2.UnitY * 38f;
            Vector2 gunDrawPosition = NPC.Center - screenPos - Vector2.UnitY * 56f - Vector2.UnitX * 30 * NPC.spriteDirection;
            Vector2 origin = frame.Size() * 0.5f;
            Vector2 projorigin = new Vector2(projector.Size().X, projector.Size().Y / 4) * 0.5f;
            Vector2 gunorigin = new Vector2(gun.Size().X, gun.Size().Y / 4) * 0.5f;
            Color color = NPC.GetAlpha(drawColor);
            Color holoColor = new Color(color.R, color.B, color.G, 0);
            SpriteEffects direction = NPC.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            SpriteEffects gunDirection = NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            float hoveroffset = 0;
            if (HasBeenKilled)
                hoveroffset = (float)Math.Sin(Main.GlobalTimeWrappedHourly * 2.5f) * 5f;

            Vector2 playerToDrae = PlayerToFollow.Center - NPC.Center;
            playerToDrae.Normalize();
            float extraRotation = PlayerToFollow.Center.X - NPC.Center.X < 0 ? -MathHelper.PiOver4 * 4 : 0;
            float gunRotation = playerToDrae.ToRotation() + extraRotation;

            if (!NPC.IsABestiaryIconDummy)
            {
                GameShaders.Misc["CalamityMod:TeleportDisplacement"].UseOpacity(MathHelper.Clamp(1f - HologramEffectTimer / HologramFadeinTime, 0f, 1f) * 0.38f);
                GameShaders.Misc["CalamityMod:TeleportDisplacement"].UseSecondaryColor(color);
                GameShaders.Misc["CalamityMod:TeleportDisplacement"].UseSaturation(color.A / 255f);
                GameShaders.Misc["CalamityMod:TeleportDisplacement"].Shader.Parameters["frameCount"].SetValue(new Vector2(16f, Main.npcFrameCount[NPC.type]));
                GameShaders.Misc["CalamityMod:TeleportDisplacement"].Apply();
            }

            if (!leaving)
                spriteBatch.Draw(texture, new Vector2(drawPosition.X, drawPosition.Y + hoveroffset), holo ? null : frame, holo ? holoColor : (drawColor * NPC.Opacity), NPC.rotation, origin, NPC.scale, direction, 0f);

            if (!NPC.IsABestiaryIconDummy)
                spriteBatch.ExitShaderRegion();

            if (HologramEffectTimer >= HologramFadeinTime || NPC.IsABestiaryIconDummy)
            {
                spriteBatch.Draw(glowmask, drawPosition, frame, Color.White * NPC.Opacity, NPC.rotation, origin, NPC.scale, direction, 0f);
            }

            if (Main.zenithWorld && !HasBeenKilled && HologramEffectTimer >= HologramFadeinTime && !exoMechdusa)
            {
                CalamityUtils.EnterShaderRegion(spriteBatch);
                Color outlineColor = Color.Lerp(Color.Magenta, Color.White, 0.4f);
                Vector3 outlineHSL = Main.rgbToHsl(outlineColor); //BasicTint uses the opposite hue i guess? or smth is fucked with the way shaders get their colors. anyways, we invert it
                float outlineThickness = MathHelper.Clamp(2f, 0f, 3f);

                GameShaders.Misc["CalamityMod:BasicTint"].UseOpacity(1f);
                GameShaders.Misc["CalamityMod:BasicTint"].UseColor(Main.hslToRgb(1 - outlineHSL.X, outlineHSL.Y, outlineHSL.Z));
                GameShaders.Misc["CalamityMod:BasicTint"].Apply();

                for (float i = 0; i < 1; i += 0.125f)
                {
                    spriteBatch.Draw(gun, gunDrawPosition + (i * MathHelper.TwoPi + gunRotation).ToRotationVector2() * outlineThickness, null, outlineColor, gunRotation, gunorigin, NPC.scale, gunDirection, 0f);
                }
                CalamityUtils.ExitShaderRegion(spriteBatch);
                spriteBatch.Draw(gun, gunDrawPosition, null, Color.White * NPC.Opacity, gunRotation, gunorigin, NPC.scale, gunDirection, 0f);
            }

            // Projector and beam
            if (HasBeenKilled)
            {
                int beamoffset = 6;
                int projHeight = (int)ProjFrameCounter * (projector.Height / 4);

                Rectangle projRectangle = new Rectangle(0, projHeight, projector.Width, projector.Height / 4);

                drawPosition.Y += ProjectorOffset - beamoffset;

                // Beam
                if (KillReappearDelay <= 0f && !leaving)
                {
                    Effect effect = Terraria.Graphics.Effects.Filters.Scene["CalamityMod:SpreadTelegraph"].GetShader().Shader;
                    effect.Parameters["centerOpacity"].SetValue(0.7f + (float)Math.Sin(Main.GlobalTimeWrappedHourly * 2f) * 0.05f);
                    effect.Parameters["mainOpacity"].SetValue(1f + (float)Math.Sin(Main.GlobalTimeWrappedHourly * 2f) * 0.05f);
                    effect.Parameters["halfSpreadAngle"].SetValue(MathHelper.PiOver4);
                    effect.Parameters["edgeColor"].SetValue(Color.DarkCyan.ToVector3());
                    effect.Parameters["centerColor"].SetValue(Color.Cyan.ToVector3());
                    effect.Parameters["edgeBlendLength"].SetValue(0.07f);
                    effect.Parameters["edgeBlendStrength"].SetValue(8f);

                    Main.spriteBatch.End();
                    Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, effect, Main.GameViewMatrix.TransformationMatrix);

                    Texture2D invis = ModContent.Request<Texture2D>("CalamityMod/Projectiles/InvisibleProj").Value;

                    Main.EntitySpriteDraw(invis, drawPosition, null, Color.White, -MathHelper.PiOver2, new Vector2(invis.Width / 2f, invis.Height / 2f), 500f, 0, 0);

                    Main.spriteBatch.End();
                    Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                }

                drawPosition.Y += beamoffset;

                // Draw the projector
                if (KillReappearDelay <= 60f)
                {
                    spriteBatch.Draw(projector, drawPosition, projRectangle, Color.White * Lighting.GetColor((int)NPC.position.X / 16, (int)(NPC.position.Y / 16 + ProjectorOffset)).A, NPC.rotation, projorigin, NPC.scale, direction, 0f);
                    spriteBatch.Draw(projectorglow, drawPosition, projRectangle, Color.White, NPC.rotation, projorigin, NPC.scale, direction, 0f);
                }

            }

            return false;
        }
    }
}
