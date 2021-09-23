using CalamityMod.NPCs.ExoMechs.Ares;
using CalamityMod.NPCs.ExoMechs.Thanatos;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

using ApolloBoss = CalamityMod.NPCs.ExoMechs.Apollo.Apollo;
using ArtemisBoss = CalamityMod.NPCs.ExoMechs.Artemis.Artemis;

namespace CalamityMod.NPCs.ExoMechs
{
    public class Draedon : ModNPC
    {
        public float DefeatTimer;
        public bool ShouldStartStandingUp;
        public Vector2 HoverDestinationOffset
        {
            get => new Vector2(npc.ai[1], npc.ai[2]);
            set
            {
                npc.ai[1] = value.X;
                npc.ai[2] = value.Y;
            }
        }
        public Player PlayerToFollow => Main.player[npc.target];
        public ref float TalkTimer => ref npc.ai[0];
        public ref float GeneralTimer => ref npc.ai[3];
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
        public static readonly Color TextColor = new Color(155, 255, 255);
        public static readonly Color TextColorEdgy = new Color(213, 4, 11);
        public const int TeleportFadeinTime = 45;
        public const int TalkDelay = 150;
        public const int DelayPerDialogLine = 130;
        public const int ExoMechChooseDelay = TalkDelay + DelayPerDialogLine * 4 + 10;
        public const int ExoMechShakeTime = 100;
        public const int DelayBeforeDefeatStandup = 30;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Draedon");
            Main.npcFrameCount[npc.type] = 15;
        }

        public override void SetDefaults()
        {
            npc.damage = 0;
            npc.width = npc.height = 34;
            npc.defense = 100;
            npc.lifeMax = 16000;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.dontTakeDamage = true;
            npc.aiStyle = aiType = -1;
            npc.knockBackResist = 0f;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(DefeatTimer);
            writer.Write(ShouldStartStandingUp);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            DefeatTimer = reader.ReadSingle();
            ShouldStartStandingUp = reader.ReadBoolean();
        }

        public override void AI()
        {
            // Set the whoAmI variable.
            CalamityGlobalNPC.draedon = npc.whoAmI;

            // Prevent stupid natural despawns.
            npc.timeLeft = 3600;

            // Decide an initial target and play a teleport sound on the first frame.
            if (TalkTimer == 0f)
            {
                npc.TargetClosest(false);
                Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/DraedonTeleport"), PlayerToFollow.Center);
            }

            // Pick someone else to pay attention to if the old target is gone.
            if (PlayerToFollow.dead || !PlayerToFollow.active)
            {
                npc.TargetClosest(false);

                // Fuck off if no living target exists.
                if (PlayerToFollow.dead || !PlayerToFollow.active)
                {
                    npc.life = 0;
                    npc.checkDead();
                    npc.active = false;
                    return;
                }
            }

            // Stay within the world.
            npc.position.Y = MathHelper.Clamp(npc.position.Y, 150f, Main.maxTilesY * 16f - 150f);

            npc.Opacity = Utils.InverseLerp(0f, 8f, TalkTimer, true);
            npc.spriteDirection = (PlayerToFollow.Center.X < npc.Center.X).ToDirectionInt();

            // Play the stand up animation after teleportation.
            if (TalkTimer == TeleportFadeinTime + 5f)
                ShouldStartStandingUp = true;

            if (Main.netMode != NetmodeID.MultiplayerClient && TalkTimer == TalkDelay)
            {
                CalamityUtils.DisplayLocalizedText("Mods.CalamityMod.DraedonIntroductionText1", TextColor);
                npc.netUpdate = true;
            }

            if (Main.netMode != NetmodeID.MultiplayerClient && TalkTimer == TalkDelay + DelayPerDialogLine)
            {
                CalamityUtils.DisplayLocalizedText("Mods.CalamityMod.DraedonIntroductionText2", TextColor);
                npc.netUpdate = true;
            }

            if (Main.netMode != NetmodeID.MultiplayerClient && TalkTimer == TalkDelay + DelayPerDialogLine * 2f)
            {
                CalamityUtils.DisplayLocalizedText("Mods.CalamityMod.DraedonIntroductionText3", TextColor);
                npc.netUpdate = true;
            }

            if (Main.netMode != NetmodeID.MultiplayerClient && TalkTimer == TalkDelay + DelayPerDialogLine * 3f)
            {
                CalamityUtils.DisplayLocalizedText("Mods.CalamityMod.DraedonIntroductionText4", TextColor);
                npc.netUpdate = true;
            }

            // Inform the player who summoned draedon they may choose the first mech and cause a selection UI to appear over their head.
            if (Main.netMode != NetmodeID.MultiplayerClient && TalkTimer == TalkDelay + DelayPerDialogLine * 4f)
            {
                CalamityUtils.DisplayLocalizedText("Mods.CalamityMod.DraedonIntroductionText5", TextColorEdgy);
                npc.netUpdate = true;
            }

            // Wait for the player to select an exo mech.
            if (TalkTimer >= ExoMechChooseDelay && TalkTimer < ExoMechChooseDelay + 8f && CalamityWorld.DraedonMechToSummon == ExoMech.None)
            {
                PlayerToFollow.Calamity().AbleToSelectExoMech = true;
                TalkTimer = ExoMechChooseDelay;
            }

            // Disable music before talking.
            if (TalkTimer <= 50f)
                Main.LocalPlayer.Calamity().MusicMuffleFactor = 1f;

            // Fly around once the exo mechs have been spawned.
            if (ExoMechIsPresent || DefeatTimer > 0f)
            {
                FlyAroundInGamerChair();
                GeneralTimer++;
            }

            // Make the screen rumble and summon the exo mechs.
            if (TalkTimer > ExoMechChooseDelay + 8f && TalkTimer < ExoMechChooseDelay + ExoMechShakeTime)
            {
                Main.LocalPlayer.Calamity().GeneralScreenShakePower = Utils.InverseLerp(4200f, 1400f, Main.LocalPlayer.Distance(PlayerToFollow.Center), true) * 18f;
                Main.LocalPlayer.Calamity().GeneralScreenShakePower *= Utils.InverseLerp(ExoMechChooseDelay + 5f, ExoMechChooseDelay + ExoMechShakeTime, TalkTimer, true);
            }

            // Summon the selected exo mech.
            if (TalkTimer == ExoMechChooseDelay + 10f)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    SummonExoMech();

                if (Main.netMode != NetmodeID.Server)
                {
                    var sound = Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/FlareSound"), PlayerToFollow.Center);

                    if (sound != null)
                        sound.Volume = MathHelper.Clamp(sound.Volume * 1.55f, 0f, 1f);
                }
            }

            if (TalkTimer > ExoMechChooseDelay + 10f && !ExoMechIsPresent)
            {
                HandleDefeatStuff();
                DefeatTimer++;
            }

            if (!ExoMechIsPresent && DefeatTimer <= 0f)
                music = mod.GetSoundSlot(SoundType.Music, "Sounds/Music/DraedonAmbience");
            if (ExoMechIsPresent)
                music = /*CalamityMod.Instance.GetMusicFromMusicMod("AdultEidolonWyrm") ??*/ MusicID.Boss3;

            TalkTimer++;
        }

        // TODO - Make this work in conjunction with exo mech transitions. This requires that the exo mech AIs be finished.
        public void FlyAroundInGamerChair()
        {
            // Define a hover destination offset if one hasn't been decided yet.
            if (Main.netMode != NetmodeID.MultiplayerClient && HoverDestinationOffset == Vector2.Zero)
            {
                HoverDestinationOffset = -Vector2.UnitY * 700f;
                npc.netUpdate = true;
            }

            // Switch hover destinations from time to time.
            if (Main.netMode != NetmodeID.MultiplayerClient && GeneralTimer % 480f == 479f)
            {
                Vector2 offsetDirection;
                Vector2 directionToTarget = npc.SafeDirectionTo(PlayerToFollow.Center);

                // Reroll the offset direction if its direction noticably contrasts the current direction from the target.
                // This is done to prevent Draedon from selecting a position to zoom to that would make him have to move
                // close to the player.
                do
                    offsetDirection = Main.rand.NextVector2Unit();
                while (Vector2.Dot(directionToTarget, offsetDirection) > 0.2f);

                HoverDestinationOffset = offsetDirection * Main.rand.NextFloat(750f, 1100f);
                npc.netUpdate = true;
            }

            // Always hover to the side of the target when defeated.
            if (DefeatTimer > 5f)
                HoverDestinationOffset = Vector2.UnitX * (PlayerToFollow.Center.X < npc.Center.X).ToDirectionInt() * 325f;

            Vector2 hoverDestination = PlayerToFollow.Center + HoverDestinationOffset;

            // Decide sprite direction based on movement if not close enough to the desination.
            // Not deciding this here results in Draedon using the default of looking at the target he's following.
            if (npc.WithinRange(hoverDestination, 300f))
            {
                npc.velocity *= 0.96f;

                float moveSpeed = MathHelper.Lerp(2f, 8f, Utils.InverseLerp(45f, 275f, npc.Distance(hoverDestination), true));
                npc.Center = npc.Center.MoveTowards(hoverDestination, moveSpeed);
            }
            else
            {
                if (DefeatTimer < DelayBeforeDefeatStandup)
                    npc.spriteDirection = (npc.velocity.X < 0f).ToDirectionInt();

                float flySpeed = DefeatTimer > 5f ? 14f : 32f;
                Vector2 idealVelocity = npc.SafeDirectionTo(hoverDestination) * flySpeed;
                npc.SimpleFlyMovement(idealVelocity, flySpeed / 400f);
                npc.velocity = Vector2.Lerp(npc.velocity, idealVelocity, 0.045f);
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
            npc.dontTakeDamage = DefeatTimer < 250f;
            npc.Calamity().CanHaveBossHealthBar = !npc.dontTakeDamage;

            // Stand up in awe after a small amount of time has passed.
            if (DefeatTimer > DelayBeforeDefeatStandup)
                ShouldStartStandingUp = true;
            
            // TODO - This needs to be changed.
            if (DefeatTimer == DelayBeforeDefeatStandup + 10f)
                Main.NewText("Wait, what? How?", TextColor);

            if (DefeatTimer == DelayBeforeDefeatStandup + 80f)
                Main.NewText("No", TextColor);
        }

        // Draedon should not manually despawn.
        public override bool CheckActive() => false;

        public override Color? GetAlpha(Color drawColor)
        {
            float teleportFade = Utils.InverseLerp(0f, TeleportFadeinTime, TalkTimer, true);
            Color color = Color.Lerp(drawColor, Color.Cyan, 1f - (float)Math.Pow(teleportFade, 3D));
            color.A = (byte)(int)(teleportFade * 255f);

            return color * npc.Opacity;
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frame.Width = 102;

            int xFrame = npc.frame.X / npc.frame.Width;
            int yFrame = npc.frame.Y / frameHeight;
            int frame = xFrame * Main.npcFrameCount[npc.type] + yFrame;

            // Prepare to stand up if called for and not already doing so.
            if (ShouldStartStandingUp && frame < 23)
                frame = 23;

            int frameChangeDelay = 7;
            bool shouldNotSitDown = DefeatTimer > DelayBeforeDefeatStandup;

            npc.frameCounter++;
            if (npc.frameCounter >= frameChangeDelay)
            {
                frame++;

                if (!ShouldStartStandingUp && frame >= 23)
                    frame = 0;

                // Do the sit animation infinitely if Draedon should not sit down again.
                if (shouldNotSitDown && frame >= 47)
                    frame = 36;

                if (frame >= 59)
                {
                    frame = 0;
                    ShouldStartStandingUp = false;
                }

                npc.frameCounter = 0;
            }

            npc.frame.X = frame / Main.npcFrameCount[npc.type] * npc.frame.Width;
            npc.frame.Y = frame % Main.npcFrameCount[npc.type] * frameHeight;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = 16000;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (npc.life > 0)
                return;

            for (int i = 1; i <= 4; i++)
            {
                Vector2 goreSpawnOffset = Main.rand.NextVector2Circular(12f, 12f);
                Vector2 draedonPieceVelocity = Main.rand.NextVector2CircularEdge(7f, 7f) - Vector2.UnitY * 8f;
                Vector2 chairPieceVelocity = Vector2.UnitY.RotatedByRandom(0.13f) * Main.rand.NextFloat(4.45f, 5.4f);
                Gore.NewGoreDirect(npc.Center + goreSpawnOffset, draedonPieceVelocity, mod.GetGoreSlot($"Gores/Draedon/Draedon{i}"));

                goreSpawnOffset = Main.rand.NextVector2Circular(18f, 18f);
                Gore.NewGoreDirect(npc.Center + goreSpawnOffset, chairPieceVelocity, mod.GetGoreSlot($"Gores/Draedon/Chair{i}"));
            }
        }

        // Always instantly kill Draedon when he's vulnerable
        public override bool StrikeNPC(ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
        {
            damage *= 56D;
            if (damage < npc.lifeMax)
                damage = npc.lifeMax + Main.rand.NextFloat(50f, 750f);
            crit = true;
            return true;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            spriteBatch.EnterShaderRegion();

            Texture2D texture = Main.npcTexture[npc.type];
            Texture2D glowmask = ModContent.GetTexture("CalamityMod/NPCs/ExoMechs/DraedonGlowmask");
            Rectangle frame = npc.frame;

            Vector2 drawPosition = npc.Center - Main.screenPosition - Vector2.UnitY * 38f;
            Vector2 origin = frame.Size() * 0.5f;
            Color color = npc.GetAlpha(drawColor);
            SpriteEffects direction = npc.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            GameShaders.Misc["CalamityMod:TeleportDisplacement"].UseOpacity(MathHelper.Clamp(1f - TalkTimer / TeleportFadeinTime, 0f, 1f) * 0.38f);
            GameShaders.Misc["CalamityMod:TeleportDisplacement"].UseSecondaryColor(color);
            GameShaders.Misc["CalamityMod:TeleportDisplacement"].UseSaturation(color.A / 255f);
            GameShaders.Misc["CalamityMod:TeleportDisplacement"].Shader.Parameters["frameCount"].SetValue(new Vector2(2f, Main.npcFrameCount[npc.type]));
            GameShaders.Misc["CalamityMod:TeleportDisplacement"].Apply();

            spriteBatch.Draw(texture, drawPosition, frame, drawColor * npc.Opacity, npc.rotation, origin, npc.scale, direction, 0f);

            spriteBatch.ExitShaderRegion();

            if (TalkTimer > TeleportFadeinTime)
                spriteBatch.Draw(glowmask, drawPosition, frame, Color.White * npc.Opacity, npc.rotation, origin, npc.scale, direction, 0f);

            return false;
        }
    }
}
