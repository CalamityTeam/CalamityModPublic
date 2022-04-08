using CalamityMod.NPCs.ExoMechs.Ares;
using CalamityMod.NPCs.ExoMechs.Thanatos;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

using ApolloBoss = CalamityMod.NPCs.ExoMechs.Apollo.Apollo;
using ArtemisBoss = CalamityMod.NPCs.ExoMechs.Artemis.Artemis;
using Terraria.Audio;

namespace CalamityMod.NPCs.ExoMechs
{
    public class Draedon : ModNPC
    {
        public float DefeatTimer;
        public bool ShouldStartStandingUp;
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
        public static readonly Color TextColor = new(155, 255, 255);
        public static readonly Color TextColorEdgy = new(213, 4, 11);
        public const int HologramFadeinTime = 45;
        public const int TalkDelay = 150;
        public const int DelayPerDialogLine = 130;
        public const int ExoMechChooseDelay = TalkDelay + DelayPerDialogLine * 4 + 10;
        public const int ExoMechShakeTime = 100;
        public const int ExoMechPhaseDialogueTime = ExoMechChooseDelay + ExoMechShakeTime;
        public const int DelayBeforeDefeatStandup = 30;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Draedon");
            Main.npcFrameCount[NPC.type] = 12;
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
            NPC.Calamity().DoesNotGenerateRage = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(DialogueType);
            writer.Write(DefeatTimer);
            writer.Write(HologramEffectTimer);
            writer.Write(KillReappearDelay);
            writer.Write(ShouldStartStandingUp);
            writer.Write(HasBeenKilled);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            DialogueType = reader.ReadSingle();
            DefeatTimer = reader.ReadSingle();
            HologramEffectTimer = reader.ReadSingle();
            KillReappearDelay = reader.ReadSingle();
            ShouldStartStandingUp = reader.ReadBoolean();
            HasBeenKilled = reader.ReadBoolean();
        }

        public override void AI()
        {
            // Set the whoAmI variable.
            CalamityGlobalNPC.draedon = NPC.whoAmI;

            // Prevent stupid natural despawns.
            NPC.timeLeft = 3600;

            // Decide an initial target and play a teleport sound on the first frame.
            if (TalkTimer == 0f)
            {
                NPC.TargetClosest(false);
                SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Sounds/Custom/DraedonTeleport"), PlayerToFollow.Center);
            }

            // Pick someone else to pay attention to if the old target is gone.
            if (PlayerToFollow.dead || !PlayerToFollow.active)
            {
                NPC.TargetClosest(false);

                // Fuck off if no living target exists.
                if (PlayerToFollow.dead || !PlayerToFollow.active)
                {
                    NPC.life = 0;
                    NPC.active = false;
                    return;
                }
            }

            // Stay within the world.
            NPC.position.Y = MathHelper.Clamp(NPC.position.Y, 150f, Main.maxTilesY * 16f - 150f);

            NPC.spriteDirection = (PlayerToFollow.Center.X < NPC.Center.X).ToDirectionInt();

            // Handle delays when re-appearing after being killed.
            if (KillReappearDelay > 0f)
            {
                NPC.Opacity = 0f;
                KillReappearDelay--;
                if (KillReappearDelay <= 0f)
                    CalamityUtils.DisplayLocalizedText("Mods.CalamityMod.DraedonEndKillAttemptText", TextColor);
                return;
            }

            // Synchronize the hologram effect and talk timer at the beginning.
            // Also calculate opacity.
            if (TalkTimer <= HologramFadeinTime)
            {
                HologramEffectTimer = TalkTimer;
                NPC.Opacity = Utils.GetLerpValue(0f, 8f, TalkTimer, true);
            }

            // Play the stand up animation after teleportation.
            if (TalkTimer == HologramFadeinTime + 5f)
                ShouldStartStandingUp = true;

            // Gloss over the arbitrary details and just get to the Exo Mech selection if Draedon has already been talked to.
            if (CalamityWorld.TalkedToDraedon && TalkTimer > 70 && TalkTimer < TalkDelay * 4f - 25f)
            {
                TalkTimer = TalkDelay * 4f - 25f;
                NPC.netUpdate = true;
            }

            if (Main.netMode != NetmodeID.MultiplayerClient && TalkTimer == TalkDelay)
            {
                CalamityUtils.DisplayLocalizedText("Mods.CalamityMod.DraedonIntroductionText1", TextColor);
                NPC.netUpdate = true;
            }

            if (Main.netMode != NetmodeID.MultiplayerClient && TalkTimer == TalkDelay + DelayPerDialogLine)
            {
                CalamityUtils.DisplayLocalizedText("Mods.CalamityMod.DraedonIntroductionText2", TextColor);
                NPC.netUpdate = true;
            }

            if (Main.netMode != NetmodeID.MultiplayerClient && TalkTimer == TalkDelay + DelayPerDialogLine * 2f)
            {
                CalamityUtils.DisplayLocalizedText("Mods.CalamityMod.DraedonIntroductionText3", TextColor);
                NPC.netUpdate = true;
            }

            if (Main.netMode != NetmodeID.MultiplayerClient && TalkTimer == TalkDelay + DelayPerDialogLine * 3f)
            {
                CalamityUtils.DisplayLocalizedText("Mods.CalamityMod.DraedonIntroductionText4", TextColor);
                NPC.netUpdate = true;
            }

            // Inform the player who summoned draedon they may choose the first mech and cause a selection UI to appear over their head.
            if (Main.netMode != NetmodeID.MultiplayerClient && TalkTimer == TalkDelay + DelayPerDialogLine * 4f)
            {
                if (CalamityWorld.TalkedToDraedon)
                    CalamityUtils.DisplayLocalizedText("Mods.CalamityMod.DraedonResummonText", TextColorEdgy);
                else
                    CalamityUtils.DisplayLocalizedText("Mods.CalamityMod.DraedonIntroductionText5", TextColorEdgy);

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
            if (TalkTimer == ExoMechChooseDelay + 10f)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    SummonExoMech();

                if (Main.netMode != NetmodeID.Server)
                {
                    var sound = SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Sounds/Item/FlareSound"), PlayerToFollow.Center);
                    CalamityUtils.SafeVolumeChange(ref sound, 1.55f);
                }
            }

            // Dialogue lines depending on what phase the exo mechs are at.
            switch ((int)DialogueType)
            {
                case 1:

                    if (Main.netMode != NetmodeID.MultiplayerClient && TalkTimer == ExoMechPhaseDialogueTime)
                    {
                        CalamityUtils.DisplayLocalizedText("Mods.CalamityMod.DraedonExoPhase1Text1", TextColor);
                        NPC.netUpdate = true;
                    }

                    if (Main.netMode != NetmodeID.MultiplayerClient && TalkTimer == ExoMechPhaseDialogueTime + DelayPerDialogLine)
                    {
                        CalamityUtils.DisplayLocalizedText("Mods.CalamityMod.DraedonExoPhase1Text2", TextColor);
                        NPC.netUpdate = true;
                    }

                    break;

                case 2:

                    if (TalkTimer == ExoMechPhaseDialogueTime)
                    {
                        SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Sounds/Custom/DraedonLaugh"), PlayerToFollow.Center);
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            CalamityUtils.DisplayLocalizedText("Mods.CalamityMod.DraedonExoPhase2Text1", TextColor);
                            NPC.netUpdate = true;
                        }
                    }

                    if (Main.netMode != NetmodeID.MultiplayerClient && TalkTimer == ExoMechPhaseDialogueTime + DelayPerDialogLine)
                    {
                        CalamityUtils.DisplayLocalizedText("Mods.CalamityMod.DraedonExoPhase2Text2", TextColor);
                        NPC.netUpdate = true;
                    }

                    break;

                case 3:

                    if (Main.netMode != NetmodeID.MultiplayerClient && TalkTimer == ExoMechPhaseDialogueTime)
                    {
                        CalamityUtils.DisplayLocalizedText("Mods.CalamityMod.DraedonExoPhase3Text1", TextColor);
                        NPC.netUpdate = true;
                    }

                    if (TalkTimer == ExoMechPhaseDialogueTime + DelayPerDialogLine)
                    {
                        SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Sounds/Custom/DraedonLaugh"), PlayerToFollow.Center);
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            CalamityUtils.DisplayLocalizedText("Mods.CalamityMod.DraedonExoPhase3Text2", TextColor);
                            NPC.netUpdate = true;
                        }
                    }

                    break;

                case 4:

                    if (Main.netMode != NetmodeID.MultiplayerClient && TalkTimer == ExoMechPhaseDialogueTime)
                    {
                        CalamityUtils.DisplayLocalizedText("Mods.CalamityMod.DraedonExoPhase4Text1", TextColor);
                        NPC.netUpdate = true;
                    }

                    if (Main.netMode != NetmodeID.MultiplayerClient && TalkTimer == ExoMechPhaseDialogueTime + DelayPerDialogLine)
                    {
                        CalamityUtils.DisplayLocalizedText("Mods.CalamityMod.DraedonExoPhase4Text2", TextColor);
                        NPC.netUpdate = true;
                    }

                    break;

                case 5:

                    if (Main.netMode != NetmodeID.MultiplayerClient && TalkTimer == ExoMechPhaseDialogueTime)
                    {
                        CalamityUtils.DisplayLocalizedText("Mods.CalamityMod.DraedonExoPhase5Text1", TextColor);
                        NPC.netUpdate = true;
                    }

                    if (Main.netMode != NetmodeID.MultiplayerClient && TalkTimer == ExoMechPhaseDialogueTime + DelayPerDialogLine)
                    {
                        CalamityUtils.DisplayLocalizedText("Mods.CalamityMod.DraedonExoPhase5Text2", TextColor);
                        NPC.netUpdate = true;
                    }

                    break;

                case 6:

                    if (Main.netMode != NetmodeID.MultiplayerClient && TalkTimer == ExoMechPhaseDialogueTime)
                    {
                        CalamityUtils.DisplayLocalizedText("Mods.CalamityMod.DraedonExoPhase6Text1", TextColor);
                        NPC.netUpdate = true;
                    }

                    if (Main.netMode != NetmodeID.MultiplayerClient && TalkTimer == ExoMechPhaseDialogueTime + DelayPerDialogLine)
                    {
                        CalamityUtils.DisplayLocalizedText("Mods.CalamityMod.DraedonExoPhase6Text2", TextColor);
                        NPC.netUpdate = true;
                    }

                    if (TalkTimer == ExoMechPhaseDialogueTime + DelayPerDialogLine * 2f)
                    {
                        SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Sounds/Custom/DraedonLaugh"), PlayerToFollow.Center);
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            CalamityUtils.DisplayLocalizedText("Mods.CalamityMod.DraedonExoPhase6Text3", TextColor);
                            NPC.netUpdate = true;
                        }
                    }

                    break;
            }

            if (TalkTimer > ExoMechChooseDelay + 10f && !ExoMechIsPresent)
            {
                HandleDefeatStuff();
                DefeatTimer++;
            }

            if (!ExoMechIsPresent && DefeatTimer <= 0f)
                Music = SoundLoader.GetSoundSlot(Mod, "Sounds/Music/DraedonAmbience");
            if (ExoMechIsPresent)
                Music = CalamityMod.Instance.GetMusicFromMusicMod("ExoMechs") ?? MusicID.Boss3;

            TalkTimer++;
        }

        // TODO -- Make this work in conjunction with exo mech transitions. This requires that the exo mech AIs be finished.
        public void FlyAroundInGamerChair()
        {
            // Define a hover destination offset if one hasn't been decided yet.
            if (Main.netMode != NetmodeID.MultiplayerClient && HoverDestinationOffset == Vector2.Zero)
            {
                HoverDestinationOffset = -Vector2.UnitY * 700f;
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

                HoverDestinationOffset = offsetDirection * Main.rand.NextFloat(750f, 1100f);
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

        public void HandleDefeatStuff()
        {
            // Become vulnerable after being defeated after a certain point.
            NPC.dontTakeDamage = DefeatTimer < TalkDelay * 2f + 50f || HasBeenKilled;
            NPC.Calamity().CanHaveBossHealthBar = !NPC.dontTakeDamage;
            NPC.Calamity().ShouldCloseHPBar = HasBeenKilled;

            bool leaving = DefeatTimer > DelayBeforeDefeatStandup + TalkDelay * 7f + 200f;

            // Fade away and disappear when leaving.
            if (leaving)
            {
                HologramEffectTimer = MathHelper.Clamp(HologramEffectTimer - 1f, 0f, HologramFadeinTime);
                if (HologramEffectTimer <= 0f)
                    NPC.active = false;
            }

            // Fade back in as a hologram if the player tried to kill Draedon.
            else if (HasBeenKilled)
                HologramEffectTimer = MathHelper.Clamp(HologramEffectTimer + 1f, 0f, HologramFadeinTime - 5f);

            // Adjust opacity.
            NPC.Opacity = HologramEffectTimer / HologramFadeinTime;
            if (HasBeenKilled)
                NPC.Opacity *= 0.67f;

            // Stand up in awe after a small amount of time has passed.
            if (DefeatTimer > DelayBeforeDefeatStandup && DefeatTimer < TalkDelay * 2f + 50f)
                ShouldStartStandingUp = true;

            if (DefeatTimer == DelayBeforeDefeatStandup + 50f)
                CalamityUtils.DisplayLocalizedText("Mods.CalamityMod.DraedonEndText1", TextColor);

            if (DefeatTimer == DelayBeforeDefeatStandup + TalkDelay + 50f)
                CalamityUtils.DisplayLocalizedText("Mods.CalamityMod.DraedonEndText2", TextColor);

            // After this point Draedon becomes vulnerable.
            // He sits back down as well as he thinks for a bit.
            // Killing him will cause gore to appear but also for Draedon to come back as a hologram.
            if (DefeatTimer == DelayBeforeDefeatStandup + TalkDelay * 2f + 50f)
                CalamityUtils.DisplayLocalizedText("Mods.CalamityMod.DraedonEndText3", TextColor);

            if (DefeatTimer == DelayBeforeDefeatStandup + TalkDelay * 3f + 165f)
                CalamityUtils.DisplayLocalizedText("Mods.CalamityMod.DraedonEndText4", TextColor);

            if (DefeatTimer == DelayBeforeDefeatStandup + TalkDelay * 4f + 165f)
                CalamityUtils.DisplayLocalizedText("Mods.CalamityMod.DraedonEndText5", TextColor);

            if (DefeatTimer == DelayBeforeDefeatStandup + TalkDelay * 5f + 165f)
                CalamityUtils.DisplayLocalizedText("Mods.CalamityMod.DraedonEndText6", TextColor);

            if (DefeatTimer == DelayBeforeDefeatStandup + TalkDelay * 6f + 165f)
                CalamityUtils.DisplayLocalizedText("Mods.CalamityMod.DraedonEndText7", TextColor);

            if (DefeatTimer == DelayBeforeDefeatStandup + TalkDelay * 7f + 165f)
                CalamityUtils.DisplayLocalizedText("Mods.CalamityMod.DraedonEndText8", TextColor);

            if (DefeatTimer == DelayBeforeDefeatStandup + TalkDelay * 8f + 165f)
                CalamityUtils.DisplayLocalizedText("Mods.CalamityMod.DraedonEndText9", TextColor);
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
            bool shouldNotSitDown = DefeatTimer > DelayBeforeDefeatStandup && DefeatTimer < TalkDelay * 2f + 10f;

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
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            NPC.lifeMax = 16000;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.life > 0)
                return;

            if (Main.netMode != NetmodeID.Server)
            {
                for (int i = 1; i <= 4; i++)
                {
                    Vector2 goreSpawnOffset = Main.rand.NextVector2Circular(12f, 12f);
                    Vector2 draedonPieceVelocity = Main.rand.NextVector2CircularEdge(7f, 7f) - Vector2.UnitY * 8f;
                    Vector2 chairPieceVelocity = Vector2.UnitY.RotatedByRandom(0.13f) * Main.rand.NextFloat(4.45f, 5.4f);
                    Gore.NewGoreDirect(NPC.Center + goreSpawnOffset, draedonPieceVelocity, Mod.Find<ModGore>($"Gores/Draedon/Draedon{i}").Type);

                    goreSpawnOffset = Main.rand.NextVector2Circular(18f, 18f);
                    Gore.NewGoreDirect(NPC.Center + goreSpawnOffset, chairPieceVelocity, Mod.Find<ModGore>($"Gores/Draedon/Chair{i}").Type);
                }
            }
        }

        public override bool CheckDead()
        {
            if (!HasBeenKilled)
            {
                HologramEffectTimer = 0f;
                KillReappearDelay = 90f;
                NPC.dontTakeDamage = true;
                HasBeenKilled = true;
                NPC.life = NPC.lifeMax;
                NPC.active = true;
                NPC.netUpdate = true;
            }
            return false;
        }

        // Always instantly kill Draedon when he's vulnerable
        public override bool StrikeNPC(ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
        {
            damage *= 56D;
            if (damage < NPC.lifeMax)
                damage = NPC.lifeMax + Main.rand.NextFloat(50f, 750f);
            crit = true;
            return true;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            spriteBatch.EnterShaderRegion();

            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            Texture2D glowmask = ModContent.Request<Texture2D>("CalamityMod/NPCs/ExoMechs/DraedonGlowmask").Value;
            Rectangle frame = NPC.frame;

            Vector2 drawPosition = NPC.Center - screenPos - Vector2.UnitY * 38f;
            Vector2 origin = frame.Size() * 0.5f;
            Color color = NPC.GetAlpha(drawColor);
            SpriteEffects direction = NPC.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            GameShaders.Misc["CalamityMod:TeleportDisplacement"].UseOpacity(MathHelper.Clamp(1f - HologramEffectTimer / HologramFadeinTime, 0f, 1f) * 0.38f);
            GameShaders.Misc["CalamityMod:TeleportDisplacement"].UseSecondaryColor(color);
            GameShaders.Misc["CalamityMod:TeleportDisplacement"].UseSaturation(color.A / 255f);
            GameShaders.Misc["CalamityMod:TeleportDisplacement"].Shader.Parameters["frameCount"].SetValue(new Vector2(16f, Main.npcFrameCount[NPC.type]));
            GameShaders.Misc["CalamityMod:TeleportDisplacement"].Apply();

            spriteBatch.Draw(texture, drawPosition, frame, drawColor * NPC.Opacity, NPC.rotation, origin, NPC.scale, direction, 0f);

            spriteBatch.ExitShaderRegion();

            if (HologramEffectTimer >= HologramFadeinTime)
                spriteBatch.Draw(glowmask, drawPosition, frame, Color.White * NPC.Opacity, NPC.rotation, origin, NPC.scale, direction, 0f);

            return false;
        }
    }
}
