using System;
using System.Collections.Generic;
using CalamityMod.Events;
using CalamityMod.Items.Mounts;
using CalamityMod.Items.Placeables.Furniture;
using CalamityMod.Items.Potions.Alcohol;
using CalamityMod.NPCs.SupremeCalamitas;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Projectiles.Summon;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.Events;
using Terraria.GameContent.Personalities;
using Terraria.GameContent.UI;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace CalamityMod.NPCs.TownNPCs
{
    [AutoloadHead]
    public class FAP : ModNPC
    {
        public static Asset<Texture2D> AltTexture;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 27;
            NPCID.Sets.ExtraFramesCount[NPC.type] = 9;
            NPCID.Sets.AttackFrameCount[NPC.type] = 4;
            NPCID.Sets.DangerDetectRange[NPC.type] = 400;
            NPCID.Sets.AttackType[NPC.type] = 0;
            NPCID.Sets.AttackTime[NPC.type] = 60;
            NPCID.Sets.AttackAverageChance[NPC.type] = 15;
            NPCID.Sets.ShimmerTownTransform[Type] = false;
            NPC.Happiness
                .SetBiomeAffection<HallowBiome>(AffectionLevel.Love)
                .SetBiomeAffection<OceanBiome>(AffectionLevel.Like)
                .SetBiomeAffection<DesertBiome>(AffectionLevel.Dislike)
                .SetBiomeAffection<UndergroundBiome>(AffectionLevel.Hate)
                .SetNPCAffection(NPCID.Stylist, AffectionLevel.Love)
                .SetNPCAffection(NPCID.BestiaryGirl, AffectionLevel.Love)
                .SetNPCAffection(NPCID.Truffle, AffectionLevel.Like)
                .SetNPCAffection(NPCID.PartyGirl, AffectionLevel.Like)
                .SetNPCAffection(NPCID.DD2Bartender, AffectionLevel.Dislike)
                .SetNPCAffection(NPCID.TaxCollector, AffectionLevel.Dislike)
                .SetNPCAffection(NPCID.GoblinTinkerer, AffectionLevel.Hate)
                .SetNPCAffection(NPCID.Angler, AffectionLevel.Hate);
            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Velocity = 1f // Draws the NPC in the bestiary as if its walking +1 tiles in the x direction
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, drawModifiers);
            if (!Main.dedServ)
            {
                AltTexture = ModContent.Request<Texture2D>(Texture + "Alt", AssetRequestMode.AsyncLoad);
            }
        }

        public override void SetDefaults()
        {
            NPC.townNPC = true;
            NPC.friendly = true;
            NPC.lavaImmune = true;
            NPC.width = 18;
            NPC.height = 40;
            NPC.aiStyle = NPCAIStyleID.Passive;
            NPC.damage = 10;
            NPC.defense = 15;
            NPC.lifeMax = 20000;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath6;
            NPC.knockBackResist = 0.5f;
            //AnimationType = NPCID.Guide;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheHallow,
                new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.FAP")
            });
        }

        public override void FindFrame(int frameHeight)
        {
            int extraFrameAmt = (NPC.isLikeATownNPC ? NPCID.Sets.ExtraFramesCount[NPC.type] : 0);
            /*if (false && !Main.dedServ && TownNPCProfiles.Instance.GetProfile(this, out var profile))
            {
                Asset<Texture2D> textureNPCShouldUse = profile.GetTextureNPCShouldUse(this);
                if (textureNPCShouldUse.IsLoaded)
                {
                    num = textureNPCShouldUse.Height() / Main.npcFrameCount[type];
                    frame.Width = textureNPCShouldUse.Width();
                    frame.Height = num;
                }
            }*/

            if (NPC.velocity.Y == 0f)
            {
                if (NPC.direction == 1)
                    NPC.spriteDirection = 1;

                if (NPC.direction == -1)
                    NPC.spriteDirection = -1;

                int nonAttackFrames = Main.npcFrameCount[NPC.type] - NPCID.Sets.AttackFrameCount[NPC.type];
                if (NPC.ai[0] == 23f)
                {
                    NPC.frameCounter += 1D;
                    int currentFrameHeight = NPC.frame.Y / frameHeight;
                    int currentFrame = nonAttackFrames - currentFrameHeight;
                    if ((uint)(currentFrame - 1) > 1u && (uint)(currentFrame - 4) > 1u && currentFrameHeight != 0)
                    {
                        NPC.frame.Y = 0;
                        NPC.frameCounter = 0D;
                    }

                    int num239 = ((!(NPC.frameCounter < 6D)) ? (nonAttackFrames - 4) : (nonAttackFrames - 5));
                    if (NPC.ai[1] < 6f)
                        num239 = nonAttackFrames - 5;

                    NPC.frame.Y = frameHeight * num239;
                }
                else if (NPC.ai[0] >= 20f && NPC.ai[0] <= 22f)
                {
                    int num240 = NPC.frame.Y / frameHeight;
                    switch ((int)NPC.ai[0])
                    {
                        case 20:
                        case 21:
                        case 22:
                            break;
                    }

                    NPC.frame.Y = num240 * frameHeight;
                }
                else if (NPC.ai[0] == 2f)
                {
                    NPC.frameCounter += 1D;
                    if (NPC.frame.Y / frameHeight == nonAttackFrames - 1 && NPC.frameCounter >= 5D)
                    {
                        NPC.frame.Y = 0;
                        NPC.frameCounter = 0D;
                    }
                    else if (NPC.frame.Y / frameHeight == 0 && NPC.frameCounter >= 40D)
                    {
                        NPC.frame.Y = frameHeight * (nonAttackFrames - 1);
                        NPC.frameCounter = 0D;
                    }
                    else if (NPC.frame.Y != 0 && NPC.frame.Y != frameHeight * (nonAttackFrames - 1))
                    {
                        NPC.frame.Y = 0;
                        NPC.frameCounter = 0D;
                    }
                }
                else if (NPC.ai[0] == 5f) // Sitting
                {
                    NPC.frame.Y = frameHeight * (nonAttackFrames - 3);
                    NPC.frameCounter = 0D;
                }
                else if (NPC.ai[0] == 6f) // Throwing confetti
                {
                    NPC.frameCounter += 1D;
                    int confettiFrameHeight = NPC.frame.Y / frameHeight;
                    int currentFrame = nonAttackFrames - confettiFrameHeight;
                    if ((uint)(currentFrame - 1) > 1u && (uint)(currentFrame - 4) > 1u && confettiFrameHeight != 0)
                    {
                        NPC.frame.Y = 0;
                        NPC.frameCounter = 0D;
                    }

                    int confettiFrame = ((!(NPC.frameCounter < 10D)) ?
                        ((NPC.frameCounter < 16D) ?
                        (nonAttackFrames - 5) : ((NPC.frameCounter < 46D) ?
                        (nonAttackFrames - 4) : ((NPC.frameCounter < 60D) ?
                        (nonAttackFrames - 5) : ((!(NPC.frameCounter < 66D)) ?
                        ((NPC.frameCounter < 72D) ?
                        (nonAttackFrames - 5) : ((NPC.frameCounter < 102D) ?
                        (nonAttackFrames - 4) : ((NPC.frameCounter < 108D) ?
                        (nonAttackFrames - 5) : ((!(NPC.frameCounter < 114D)) ?
                        ((NPC.frameCounter < 120D) ?
                        (nonAttackFrames - 5) : ((NPC.frameCounter < 150D) ?
                        (nonAttackFrames - 4) : ((NPC.frameCounter < 156D) ?
                        (nonAttackFrames - 5) : ((!(NPC.frameCounter < 162D)) ?
                        ((NPC.frameCounter < 168D) ?
                        (nonAttackFrames - 5) : ((NPC.frameCounter < 198D) ?
                        (nonAttackFrames - 4) : ((NPC.frameCounter < 204D) ?
                        (nonAttackFrames - 5) : ((!(NPC.frameCounter < 210D)) ?
                        ((NPC.frameCounter < 216D) ?
                        (nonAttackFrames - 5) : ((NPC.frameCounter < 246D) ?
                        (nonAttackFrames - 4) : ((NPC.frameCounter < 252D) ?
                        (nonAttackFrames - 5) : ((!(NPC.frameCounter < 258D)) ?
                        ((NPC.frameCounter < 264D) ?
                        (nonAttackFrames - 5) : ((NPC.frameCounter < 294D) ?
                        (nonAttackFrames - 4) : ((NPC.frameCounter < 300D) ?
                        (nonAttackFrames - 5) : 0))) : 0)))) : 0)))) : 0)))) : 0)))) : 0)))) : 0);

                    if (confettiFrame == nonAttackFrames - 4 && confettiFrameHeight == nonAttackFrames - 5)
                    {
                        Vector2 vector4 = NPC.Center + new Vector2(10 * NPC.direction, -4f);
                        for (int n = 0; n < 8; n++)
                        {
                            int confettiDust = Main.rand.Next(139, 143);
                            int partyTime = Dust.NewDust(vector4, 0, 0, confettiDust, NPC.velocity.X + (float)NPC.direction, NPC.velocity.Y - 2.5f, 0, default(Color), 1.2f);
                            Main.dust[partyTime].velocity.X += (float)NPC.direction * 1.5f;
                            Dust dust = Main.dust[partyTime];
                            dust.position -= new Vector2(4f);
                            dust = Main.dust[partyTime];
                            dust.velocity *= 2f;
                            Main.dust[partyTime].scale = 0.7f + Main.rand.NextFloat() * 0.3f;
                        }
                    }

                    NPC.frame.Y = frameHeight * confettiFrame;
                    if (NPC.frameCounter >= 300D)
                        NPC.frameCounter = 0D;
                }
                else if (NPC.ai[0] == 7f || NPC.ai[0] == 19f) // Talking to the player
                {
                    NPC.frameCounter += 1D;
                    int playerTalkFrameHeight = NPC.frame.Y / frameHeight;
                    int currentFrame = nonAttackFrames - playerTalkFrameHeight;
                    if ((uint)(currentFrame - 1) > 1u && (uint)(currentFrame - 4) > 1u && playerTalkFrameHeight != 0)
                    {
                        NPC.frame.Y = 0;
                        NPC.frameCounter = 0D;
                    }

                    int playerTalkFrame = 0;
                    if (NPC.frameCounter < 16D)
                        playerTalkFrame = 0;
                    else if (NPC.frameCounter == 16D)
                        EmoteBubble.NewBubbleNPC(new WorldUIAnchor(NPC), 112);
                    else if (NPC.frameCounter < 128D)
                        playerTalkFrame = ((NPC.frameCounter % 16D < 8D) ? (nonAttackFrames - 2) : 0);
                    else if (NPC.frameCounter < 160D)
                        playerTalkFrame = 0;
                    else if (NPC.frameCounter != 160D)
                        playerTalkFrame = ((NPC.frameCounter < 220D) ? ((NPC.frameCounter % 12D < 6D) ? (nonAttackFrames - 2) : 0) : 0);
                    else
                        EmoteBubble.NewBubbleNPC(new WorldUIAnchor(NPC), 60);

                    NPC.frame.Y = frameHeight * playerTalkFrame;
                    if (NPC.frameCounter >= 220D)
                        NPC.frameCounter = 0D;
                }
                else if (NPC.ai[0] == 9f)
                {
                    NPC.frameCounter += 1D;
                    int num251 = NPC.frame.Y / frameHeight;
                    int currentFrame = nonAttackFrames - num251;
                    if ((uint)(currentFrame - 1) > 1u && (uint)(currentFrame - 4) > 1u && num251 != 0)
                    {
                        NPC.frame.Y = 0;
                        NPC.frameCounter = 0D;
                    }

                    int num252 = ((!(NPC.frameCounter < 10D)) ? ((!(NPC.frameCounter < 16D)) ? (nonAttackFrames - 4) : (nonAttackFrames - 5)) : 0);
                    if (NPC.ai[1] < 16f)
                        num252 = nonAttackFrames - 5;

                    if (NPC.ai[1] < 10f)
                        num252 = 0;

                    NPC.frame.Y = frameHeight * num252;
                }
                else if (NPC.ai[0] == 18f)
                {
                    NPC.frameCounter += 1D;
                    int num253 = NPC.frame.Y / frameHeight;
                    int currentFrame = nonAttackFrames - num253;
                    if ((uint)(currentFrame - 1) > 1u && (uint)(currentFrame - 4) > 1u && num253 != 0)
                    {
                        NPC.frame.Y = 0;
                        NPC.frameCounter = 0D;
                    }

                    int num254 = 0;
                    if (NPC.frameCounter < 10D)
                        num254 = 0;
                    else if (NPC.frameCounter < 16D)
                        num254 = nonAttackFrames - 1;
                    else
                        num254 = nonAttackFrames - 2;

                    if (NPC.ai[1] < 16f)
                        num254 = nonAttackFrames - 1;

                    if (NPC.ai[1] < 10f)
                        num254 = 0;

                    num254 = Main.npcFrameCount[NPC.type] - 2;
                    NPC.frame.Y = frameHeight * num254;
                }
                else if (NPC.ai[0] == 10f || NPC.ai[0] == 13f) // Attacking
                {
                    NPC.frameCounter += 1D;
                    int attackFrameHeight = NPC.frame.Y / frameHeight;
                    int currentFrame = attackFrameHeight - nonAttackFrames;
                    if ((uint)currentFrame > 3u && attackFrameHeight != 0)
                    {
                        NPC.frame.Y = 0;
                        NPC.frameCounter = 0D;
                    }

                    int attackTimingStart = 10;
                    int attackFrameTiming = 6;
                    int attackFrame = ((!(NPC.frameCounter < (double)attackTimingStart)) ?
                        ((NPC.frameCounter < (double)(attackTimingStart + attackFrameTiming)) ?
                        nonAttackFrames : ((NPC.frameCounter < (double)(attackTimingStart + attackFrameTiming * 2)) ?
                        (nonAttackFrames + 1) : ((NPC.frameCounter < (double)(attackTimingStart + attackFrameTiming * 3)) ?
                        (nonAttackFrames + 2) : ((NPC.frameCounter < (double)(attackTimingStart + attackFrameTiming * 4)) ?
                        (nonAttackFrames + 3) : 0)))) : 0);

                    NPC.frame.Y = frameHeight * attackFrame;
                }
                else if (NPC.ai[0] == 15f)
                {
                    NPC.frameCounter += 1D;
                    int num259 = NPC.frame.Y / frameHeight;
                    int currentFrame = num259 - nonAttackFrames;
                    if ((uint)currentFrame > 3u && num259 != 0)
                    {
                        NPC.frame.Y = 0;
                        NPC.frameCounter = 0D;
                    }

                    float num260 = NPC.ai[1] / (float)NPCID.Sets.AttackTime[NPC.type];
                    int num261 = 0;
                    num261 = ((num260 > 0.65f) ?
                        nonAttackFrames : ((num260 > 0.5f) ?
                        (nonAttackFrames + 1) : ((num260 > 0.35f) ?
                        (nonAttackFrames + 2) : ((num260 > 0f) ?
                        (nonAttackFrames + 3) : 0))));

                    NPC.frame.Y = frameHeight * num261;
                }
                else if (NPC.ai[0] == 25f)
                {
                    NPC.frame.Y = frameHeight;
                }
                else if (NPC.ai[0] == 12f)
                {
                    NPC.frameCounter += 1D;
                    int num262 = NPC.frame.Y / frameHeight;
                    int currentFrame = num262 - nonAttackFrames;
                    if ((uint)currentFrame > 4u && num262 != 0)
                    {
                        NPC.frame.Y = 0;
                        NPC.frameCounter = 0D;
                    }

                    int num263 = nonAttackFrames + NPC.GetShootingFrame(NPC.ai[2]);
                    NPC.frame.Y = frameHeight * num263;
                }
                else if (NPC.ai[0] == 14f || NPC.ai[0] == 24f)
                {
                    NPC.frameCounter += 1D;
                    int num264 = NPC.frame.Y / frameHeight;
                    int currentFrame = num264 - nonAttackFrames;
                    if ((uint)currentFrame > 1u && num264 != 0)
                    {
                        NPC.frame.Y = 0;
                        NPC.frameCounter = 0D;
                    }

                    int num265 = 12;
                    int num266 = ((NPC.frameCounter % (double)num265 * 2D < (double)num265) ? nonAttackFrames : (nonAttackFrames + 1));
                    NPC.frame.Y = frameHeight * num266;
                    if (NPC.ai[0] == 24f)
                    {
                        if (NPC.frameCounter == 60D)
                            EmoteBubble.NewBubble(EmoteID.EmoteConfused, new WorldUIAnchor(NPC), 60);

                        if (NPC.frameCounter == 150D)
                            EmoteBubble.NewBubble(EmoteID.EmotionAlert, new WorldUIAnchor(NPC), 90);

                        if (NPC.frameCounter >= 240D)
                            NPC.frame.Y = 0;
                    }
                }
                else if (NPC.ai[0] == 1001f)
                {
                    NPC.frame.Y = frameHeight * (nonAttackFrames - 1);
                    NPC.frameCounter = 0D;
                }
                else if (NPC.CanTalk && (NPC.ai[0] == 3f || NPC.ai[0] == 4f)) // Talking to another NPC
                {
                    NPC.frameCounter += 1D;
                    int npcTalkFrameHeight = NPC.frame.Y / frameHeight;
                    int currentFrame = nonAttackFrames - npcTalkFrameHeight;
                    if ((uint)(currentFrame - 1) > 1u && (uint)(currentFrame - 4) > 1u && npcTalkFrameHeight != 0)
                    {
                        NPC.frame.Y = 0;
                        NPC.frameCounter = 0D;
                    }

                    bool displayEmote = NPC.ai[0] == 3f;
                    int npcTalkFrame = 0;
                    int npcTalkHandFrame = 0;
                    int emoteDisplayTime = -1;
                    int emoteDisplayTime2 = -1;
                    if (NPC.frameCounter < 10D)
                        npcTalkFrame = 0;
                    else if (NPC.frameCounter < 16D)
                        npcTalkFrame = nonAttackFrames - 5;
                    else if (NPC.frameCounter < 46D)
                        npcTalkFrame = nonAttackFrames - 4;
                    else if (NPC.frameCounter < 60D)
                        npcTalkFrame = nonAttackFrames - 5;
                    else if (NPC.frameCounter < 216D)
                        npcTalkFrame = 0;
                    else if (NPC.frameCounter == 216D && Main.netMode != NetmodeID.MultiplayerClient)
                        emoteDisplayTime = 70;
                    else if (NPC.frameCounter < 286D)
                        npcTalkFrame = ((NPC.frameCounter % 12D < 6D) ? (nonAttackFrames - 2) : 0);
                    else if (NPC.frameCounter < 320D)
                        npcTalkFrame = 0;
                    else if (NPC.frameCounter != 320D || Main.netMode == NetmodeID.MultiplayerClient)
                        npcTalkFrame = ((NPC.frameCounter < 420D) ? ((NPC.frameCounter % 16D < 8D) ? (nonAttackFrames - 2) : 0) : 0);
                    else
                        emoteDisplayTime = 100;

                    if (NPC.frameCounter < 70D)
                    {
                        npcTalkHandFrame = 0;
                    }
                    else if (NPC.frameCounter != 70D || Main.netMode == NetmodeID.MultiplayerClient)
                    {
                        npcTalkHandFrame = ((NPC.frameCounter < 160D) ?
                            ((NPC.frameCounter % 16D < 8D) ?
                            (nonAttackFrames - 2) : 0) : ((NPC.frameCounter < 166D) ?
                            (nonAttackFrames - 5) : ((NPC.frameCounter < 186D) ?
                            (nonAttackFrames - 4) : ((NPC.frameCounter < 200D) ?
                            (nonAttackFrames - 5) : ((!(NPC.frameCounter < 320D)) ?
                            ((NPC.frameCounter < 326D) ?
                            (nonAttackFrames - 1) : 0) : 0)))));
                    }
                    else
                        emoteDisplayTime2 = 90;

                    if (displayEmote)
                    {
                        NPC nPC = Main.npc[(int)NPC.ai[2]];
                        if (emoteDisplayTime != -1)
                            EmoteBubble.NewBubbleNPC(new WorldUIAnchor(NPC), emoteDisplayTime, new WorldUIAnchor(nPC));

                        if (emoteDisplayTime2 != -1 && nPC.CanTalk)
                            EmoteBubble.NewBubbleNPC(new WorldUIAnchor(nPC), emoteDisplayTime2, new WorldUIAnchor(NPC));
                    }

                    NPC.frame.Y = frameHeight * (displayEmote ? npcTalkFrame : npcTalkHandFrame);
                    if (NPC.frameCounter >= 420D)
                        NPC.frameCounter = 0D;
                }
                else if (NPC.CanTalk && (NPC.ai[0] == 16f || NPC.ai[0] == 17f)) // Rock Paper Scissors
                {
                    NPC.frameCounter += 1D;
                    int rpsFrameHeight = NPC.frame.Y / frameHeight;
                    int currentFrame = nonAttackFrames - rpsFrameHeight;
                    if ((uint)(currentFrame - 1) > 1u && (uint)(currentFrame - 4) > 1u && rpsFrameHeight != 0)
                    {
                        NPC.frame.Y = 0;
                        NPC.frameCounter = 0D;
                    }

                    bool controlsRPS = NPC.ai[0] == 16f;
                    int rpsFrame = 0;
                    int emoteDisplayTime = -1;
                    if (NPC.frameCounter < 10D)
                        rpsFrame = 0;
                    else if (NPC.frameCounter < 16D)
                        rpsFrame = nonAttackFrames - 5;
                    else if (NPC.frameCounter < 22D)
                        rpsFrame = nonAttackFrames - 4;
                    else if (NPC.frameCounter < 28D)
                        rpsFrame = nonAttackFrames - 5;
                    else if (NPC.frameCounter < 34D)
                        rpsFrame = nonAttackFrames - 4;
                    else if (NPC.frameCounter < 40D)
                        rpsFrame = nonAttackFrames - 5;
                    else if (NPC.frameCounter == 40D && Main.netMode != NetmodeID.MultiplayerClient)
                        emoteDisplayTime = 45;
                    else if (NPC.frameCounter < 70D)
                        rpsFrame = nonAttackFrames - 4;
                    else if (NPC.frameCounter < 76D)
                        rpsFrame = nonAttackFrames - 5;
                    else if (NPC.frameCounter < 82D)
                        rpsFrame = nonAttackFrames - 4;
                    else if (NPC.frameCounter < 88D)
                        rpsFrame = nonAttackFrames - 5;
                    else if (NPC.frameCounter < 94D)
                        rpsFrame = nonAttackFrames - 4;
                    else if (NPC.frameCounter < 100D)
                        rpsFrame = nonAttackFrames - 5;
                    else if (NPC.frameCounter == 100D && Main.netMode != NetmodeID.MultiplayerClient)
                        emoteDisplayTime = 45;
                    else if (NPC.frameCounter < 130D)
                        rpsFrame = nonAttackFrames - 4;
                    else if (NPC.frameCounter < 136D)
                        rpsFrame = nonAttackFrames - 5;
                    else if (NPC.frameCounter < 142D)
                        rpsFrame = nonAttackFrames - 4;
                    else if (NPC.frameCounter < 148D)
                        rpsFrame = nonAttackFrames - 5;
                    else if (NPC.frameCounter < 154D)
                        rpsFrame = nonAttackFrames - 4;
                    else if (NPC.frameCounter < 160D)
                        rpsFrame = nonAttackFrames - 5;
                    else if (NPC.frameCounter != 160D || Main.netMode == NetmodeID.MultiplayerClient)
                        rpsFrame = ((NPC.frameCounter < 220D) ? (nonAttackFrames - 4) : ((NPC.frameCounter < 226D) ? (nonAttackFrames - 5) : 0));
                    else
                        emoteDisplayTime = 75;

                    if (controlsRPS && emoteDisplayTime != -1)
                    {
                        int npcPick = (int)NPC.localAI[2];
                        int npcWins = (int)NPC.localAI[3];
                        int opponentWins = (int)Main.npc[(int)NPC.ai[2]].localAI[3];
                        int opponentPick = (int)Main.npc[(int)NPC.ai[2]].localAI[2];
                        int rpsGameEnder = 3 - npcPick - npcWins;
                        int numGamesPlayed = 0;
                        if (NPC.frameCounter == 40D)
                            numGamesPlayed = 1;

                        if (NPC.frameCounter == 100D)
                            numGamesPlayed = 2;

                        if (NPC.frameCounter == 160D)
                            numGamesPlayed = 3;

                        int gameCountdown = 3 - numGamesPlayed;
                        int rockPaperScissorsResultType = -1;
                        int gameFrameTimer = 0;
                        while (rockPaperScissorsResultType < 0)
                        {
                            currentFrame = gameFrameTimer + 1;
                            gameFrameTimer = currentFrame;
                            if (currentFrame >= 100)
                                break;

                            rockPaperScissorsResultType = Main.rand.Next(2);
                            if (rockPaperScissorsResultType == 0 && opponentPick >= npcWins)
                                rockPaperScissorsResultType = -1;

                            if (rockPaperScissorsResultType == 1 && opponentWins >= npcPick)
                                rockPaperScissorsResultType = -1;

                            if (rockPaperScissorsResultType == -1 && gameCountdown <= rpsGameEnder)
                                rockPaperScissorsResultType = 2;
                        }

                        if (rockPaperScissorsResultType == 0)
                        {
                            Main.npc[(int)NPC.ai[2]].localAI[3] += 1f;
                            opponentWins++;
                        }

                        if (rockPaperScissorsResultType == 1)
                        {
                            Main.npc[(int)NPC.ai[2]].localAI[2] += 1f;
                            opponentPick++;
                        }

                        int emoteType = Utils.SelectRandom<int>(Main.rand, EmoteID.RPSPaper, EmoteID.RPSRock, EmoteID.RPSScissors);
                        int emoteType2 = emoteType;
                        switch (rockPaperScissorsResultType)
                        {
                            case 0:
                                switch (emoteType)
                                {
                                    case EmoteID.RPSPaper:
                                        emoteType2 = EmoteID.RPSRock;
                                        break;
                                    case EmoteID.RPSRock:
                                        emoteType2 = EmoteID.RPSScissors;
                                        break;
                                    case EmoteID.RPSScissors:
                                        emoteType2 = EmoteID.RPSPaper;
                                        break;
                                }
                                break;
                            case 1:
                                switch (emoteType)
                                {
                                    case EmoteID.RPSPaper:
                                        emoteType2 = EmoteID.RPSScissors;
                                        break;
                                    case EmoteID.RPSRock:
                                        emoteType2 = EmoteID.RPSPaper;
                                        break;
                                    case EmoteID.RPSScissors:
                                        emoteType2 = EmoteID.RPSRock;
                                        break;
                                }
                                break;
                        }

                        if (gameCountdown == 0)
                        {
                            if (opponentWins >= 2)
                                emoteType -= 3;

                            if (opponentPick >= 2)
                                emoteType2 -= 3;
                        }

                        EmoteBubble.NewBubble(emoteType, new WorldUIAnchor(NPC), emoteDisplayTime);
                        EmoteBubble.NewBubble(emoteType2, new WorldUIAnchor(Main.npc[(int)NPC.ai[2]]), emoteDisplayTime);
                    }

                    NPC.frame.Y = frameHeight * (controlsRPS ? rpsFrame : rpsFrame);
                    if (NPC.frameCounter >= 420D)
                        NPC.frameCounter = 0D;
                }
                else if (NPC.velocity.X == 0f)
                {
                    NPC.frame.Y = 0;
                    NPC.frameCounter = 0D;
                }
                else // Walking
                {
                    NPC.frameCounter += Math.Abs(NPC.velocity.X) * 2f;
                    NPC.frameCounter += 1D;

                    int walkFrameHeightLimit = frameHeight * 2;
                    if (NPC.frame.Y < walkFrameHeightLimit)
                        NPC.frame.Y = walkFrameHeightLimit;

                    int walkFrameTimer = 6;
                    if (NPC.frameCounter > (double)walkFrameTimer)
                    {
                        NPC.frame.Y += frameHeight;
                        NPC.frameCounter = 0D;
                    }

                    if (NPC.frame.Y / frameHeight >= Main.npcFrameCount[NPC.type] - extraFrameAmt)
                        NPC.frame.Y = walkFrameHeightLimit;
                }

                return;
            }

            NPC.frameCounter = 0D;
            NPC.frame.Y = frameHeight;
        }

        public override void AI()
        {
            if (!CalamityWorld.spawnedCirrus)
                CalamityWorld.spawnedCirrus = true;
        }

        public override bool CanTownNPCSpawn(int numTownNPCs)
        {
            if (NPC.AnyNPCs(ModContent.NPCType<SupremeCalamitas.SupremeCalamitas>()) && Main.zenithWorld)
                return false;

            if (CalamityWorld.spawnedCirrus)
                return true;

            foreach (Player player in Main.ActivePlayers)
            {
                bool hasVodka = player.InventoryHas(ModContent.ItemType<FabsolsVodka>()) || player.PortableStorageHas(ModContent.ItemType<FabsolsVodka>());
                if (hasVodka)
                    return Main.hardMode;
            }
            return false;
        }

        public override List<string> SetNPCNameList() => new List<string>() { this.GetLocalizedValue("Name.Cirrus") };

        public override string GetChat()
        {
            Player player = Main.player[Main.myPlayer];
            if (Main.zenithWorld)
            {
                player.Hurt(PlayerDeathReason.ByCustomReason(CalamityUtils.GetText("Status.Death.CirrusSlap" + Main.rand.Next(1, 2 + 1)).Format(player.name)), player.statLife / 2, -player.direction, false, false, -1, false);
                SoundEngine.PlaySound(CnidarianJellyfishOnTheString.SlapSound, player.Center);
            }

            if (CalamityUtils.AnyBossNPCS())
                return this.GetLocalizedValue("Chat.BossAlive");

            if (NPC.homeless)
                return this.GetLocalizedValue("Chat.Homeless" + Main.rand.Next(1, 2 + 1));

            int wife = NPC.FindFirstNPC(NPCID.Stylist);
            bool wifeIsAround = wife != -1;
            bool beLessDrunk = wifeIsAround && NPC.downedMoonlord;

            if (Main.bloodMoon)
            {
                if (Main.rand.NextBool(4))
                {
                    player.Hurt(PlayerDeathReason.ByCustomReason(CalamityUtils.GetText("Status.Death.CirrusSlap" + Main.rand.Next(1, 2 + 1)).Format(player.name)), player.statLife / 2, -player.direction, false, false, -1, false); ;
                    SoundEngine.PlaySound(CnidarianJellyfishOnTheString.SlapSound, player.Center);
                    return this.GetLocalizedValue("Chat.BloodMoonSlap");
                }
                return this.GetLocalizedValue("Chat.BloodMoon" + Main.rand.Next(1, 3 + 1));
            }

            WeightedRandom<string> dialogue = new WeightedRandom<string>();

            dialogue.Add(this.GetLocalizedValue("Chat.Normal1"));
            dialogue.Add(this.GetLocalizedValue("Chat.Normal2"));
            dialogue.Add(this.GetLocalizedValue("Chat.Normal3"));
            if (ChildSafety.Disabled)
                dialogue.Add(this.GetLocalizedValue("Chat.Normal4"));

            int tavernKeep = NPC.FindFirstNPC(NPCID.DD2Bartender);
            if (tavernKeep != -1)
            {
                dialogue.Add(this.GetLocalization("Chat.Tavernkeep1").Format(Main.npc[tavernKeep].GivenName));
                dialogue.Add(this.GetLocalization("Chat.Tavernkeep2").Format(Main.npc[tavernKeep].GivenName));

                if (ChildSafety.Disabled)
                    dialogue.Add(this.GetLocalizedValue("Chat.Tavernkeep3"));
            }

            int permadong = NPC.FindFirstNPC(ModContent.NPCType<DILF>());
            if (permadong != -1)
                dialogue.Add(this.GetLocalization("Chat.Archmage").Format(Main.npc[permadong].GivenName));

            int witch = NPC.FindFirstNPC(ModContent.NPCType<WITCH>());
            if (witch != -1)
                dialogue.Add(this.GetLocalization("Chat.BrimstoneWitch").Format(Main.npc[witch].GivenName));

            if (wifeIsAround)
            {
                dialogue.Add(this.GetLocalization("Chat.Stylist1").Format(Main.npc[wife].GivenName));
                if (ChildSafety.Disabled)
                {
                    dialogue.Add(this.GetLocalization("Chat.Stylist2").Format(Main.npc[wife].GivenName));
                    dialogue.Add(this.GetLocalization("Chat.Stylist3").Format(Main.npc[wife].GivenName));
                }
            }

            if (Main.dayTime)
            {
                dialogue.Add(this.GetLocalizedValue("Chat.Day1"));
                dialogue.Add(this.GetLocalizedValue("Chat.Day2"));
                dialogue.Add(this.GetLocalizedValue("Chat.Day3"));
                dialogue.Add(this.GetLocalizedValue("Chat.Day4"));

                if (beLessDrunk)
                {
                    dialogue.Add(this.GetLocalization("Chat.DayStylist1").Format(Main.npc[wife].GivenName));
                    dialogue.Add(this.GetLocalization("Chat.DayStylist2").Format(Main.npc[wife].GivenName));
                }
                else
                {
                    dialogue.Add(this.GetLocalizedValue("Chat.DayDrunk1"));
                    dialogue.Add(this.GetLocalizedValue("Chat.DayDrunk2"));
                }
            }
            else
            {
                dialogue.Add(this.GetLocalizedValue("Chat.Night1"));
                dialogue.Add(this.GetLocalizedValue("Chat.Night2"));
                dialogue.Add(this.GetLocalizedValue("Chat.Night3"));
                dialogue.Add(this.GetLocalizedValue("Chat.Night4"));
                dialogue.Add(this.GetLocalizedValue("Chat.Night5"));

                if (wifeIsAround)
                    dialogue.Add(this.GetLocalization("Chat.NightStylist").Format(Main.npc[wife].GivenName));
            }

            if (BirthdayParty.PartyIsUp)
                dialogue.Add(this.GetLocalizedValue("Chat.Party"));

            if (AcidRainEvent.AcidRainEventIsOngoing)
                dialogue.Add(this.GetLocalizedValue("Chat.AcidRain"));

            if (Main.invasionType == InvasionID.MartianMadness)
                dialogue.Add(this.GetLocalizedValue("Chat.Martians"));

            if (DownedBossSystem.downedCryogen && ChildSafety.Disabled)
                dialogue.Add(this.GetLocalizedValue("Chat.CryogenDefeated"));

            if (DownedBossSystem.downedLeviathan)
                dialogue.Add(this.GetLocalizedValue("Chat.LeviathanDefeated"));

            if (NPC.downedMoonlord)
                dialogue.Add(this.GetLocalizedValue("Chat.MoonLordDefeated"));

            if (DownedBossSystem.downedPolterghast)
                dialogue.Add(this.GetLocalizedValue("Chat.PolterghastDefeated"));

            if (DownedBossSystem.downedDoG)
                dialogue.Add(this.GetLocalizedValue("Chat.DoGDefeated"));

            if (player.Calamity().chibii)
                dialogue.Add(this.GetLocalizedValue("Chat.HasChibii"));

            if (player.Calamity().aquaticHeart && !player.Calamity().aquaticHeartHide && ChildSafety.Disabled)
                dialogue.Add(this.GetLocalizedValue("Chat.HasAnahitaTrans"));

            if (player.Calamity().fabsolVodka)
                dialogue.Add(this.GetLocalizedValue("Chat.HasVodka"));

            if (player.HasItem(ModContent.ItemType<Fabsol>()))
            {
                dialogue.Add(this.GetLocalizedValue("Chat.HasAlicorn1"));
                dialogue.Add(this.GetLocalizedValue("Chat.HasAlicorn2"));
                if (ChildSafety.Disabled)
                    dialogue.Add(this.GetLocalizedValue("Chat.HasAlicorn3"));
            }

            return dialogue;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (Main.LocalPlayer.Calamity().trippy)
                return false;

            var something = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            spriteBatch.Draw(BirthdayParty.PartyIsUp ? AltTexture.Value : TextureAssets.Npc[NPC.type].Value, NPC.Center - screenPos + new Vector2(0, NPC.gfxOffY) - new Vector2(0f, 6f), NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, something, 0);
            return false;
        }

        public string Death()
        {
            int deaths = Main.player[Main.myPlayer].numberOfDeathsPVE;

            string text = this.GetLocalization("DeathCount").Format(deaths);

            if (deaths > 10000)
                text += " " + this.GetLocalizedValue("Death10000");
            else if (deaths > 5000)
                text += " " + this.GetLocalizedValue("Death5000");
            else if (deaths > 2500)
                text += " " + this.GetLocalizedValue("Death2500");
            else if (deaths > 1000)
                text += " " + this.GetLocalizedValue("Death1000");
            else if (deaths > 500)
                text += " " + this.GetLocalizedValue("Death500");
            else if (deaths > 250)
                text += " " + this.GetLocalizedValue("Death250");
            else if (deaths > 100)
                text += " " + this.GetLocalizedValue("Death100");

            IList<string> donorList = new List<string>(CalamityLists.donatorList);
            int maxDonorsListed = 25;
            string[] donors = new string[maxDonorsListed];
            for (int i = 0; i < maxDonorsListed; i++)
            {
                donors[i] = donorList[Main.rand.Next(donorList.Count)];
                donorList.Remove(donors[i]);
            }

            text += ("\n\n" + this.GetLocalization("DonorShoutout").Format(donors));

            return text;
        }

        public override void SetChatButtons(ref string button, ref string button2)
        {
            button = Language.GetTextValue("LegacyInterface.28");
            button2 = this.GetLocalizedValue("DeathCountButton");
        }

        public override void OnChatButtonClicked(bool firstButton, ref string shopName)
        {
            if (firstButton)
            {
                shopName = "Shop";
            }
            else
            {
                Main.npcChatText = Death();
            }
        }

        public override void AddShops()
        {
            Mod musicMod = CalamityMod.Instance.musicMod;
            musicMod.TryFind("Interlude1MusicBox", out ModItem interlude1Box);
            musicMod.TryFind("Interlude2MusicBox", out ModItem interlude2Box);
            musicMod.TryFind("Interlude3MusicBox", out ModItem interlude3Box);
            musicMod.TryFind("DevourerofGodsEulogyMusicBox", out ModItem eulogyBox);

            NPCShop shop = new(Type);
            shop.AddWithCustomValue(ItemID.LovePotion, Item.buyPrice(silver: 25), CalamityConditions.PotionSellingConfig, Condition.HappyEnough)
                .AddWithCustomValue(ModContent.ItemType<GrapeBeer>(), Item.buyPrice(silver: 30))
                .AddWithCustomValue(ModContent.ItemType<RedWine>(), Item.buyPrice(gold: 1))
                .AddWithCustomValue(ModContent.ItemType<Whiskey>(), Item.buyPrice(gold: 2))
                .AddWithCustomValue(ModContent.ItemType<Rum>(), Item.buyPrice(gold: 2))
                .AddWithCustomValue(ModContent.ItemType<Tequila>(), Item.buyPrice(gold: 2))
                .AddWithCustomValue(ModContent.ItemType<Fireball>(), Item.buyPrice(gold: 3))
                .AddWithCustomValue(ModContent.ItemType<FabsolsVodka>(), Item.buyPrice(gold: 4))
                .AddWithCustomValue(ModContent.ItemType<Vodka>(), Item.buyPrice(gold: 2), Condition.DownedMechBossAll)
                .AddWithCustomValue(ModContent.ItemType<Screwdriver>(), Item.buyPrice(gold: 6), Condition.DownedMechBossAll)
                .AddWithCustomValue(ModContent.ItemType<WhiteWine>(), Item.buyPrice(gold: 6), Condition.DownedMechBossAll)
                .AddWithCustomValue(ModContent.ItemType<EvergreenGin>(), Item.buyPrice(gold: 8), Condition.DownedPlantera)
                .AddWithCustomValue(ModContent.ItemType<CaribbeanRum>(), Item.buyPrice(gold: 8), Condition.DownedPlantera)
                .AddWithCustomValue(ModContent.ItemType<Margarita>(), Item.buyPrice(gold: 8), Condition.DownedPlantera)
                .AddWithCustomValue(ModContent.ItemType<OldFashioned>(), Item.buyPrice(gold: 8), Condition.DownedPlantera)
                .AddWithCustomValue(ItemID.EmpressButterfly, Item.buyPrice(gold: 10), Condition.DownedPlantera)
                .AddWithCustomValue(ModContent.ItemType<Everclear>(), Item.buyPrice(gold: 3), CalamityConditions.DownedAstrumAureus)
                .AddWithCustomValue(ModContent.ItemType<BloodyMary>(), Item.buyPrice(gold: 4), CalamityConditions.DownedAstrumAureus, Condition.BloodMoon)
                .AddWithCustomValue(ModContent.ItemType<StarBeamRye>(), Item.buyPrice(gold: 6), CalamityConditions.DownedAstrumAureus, Condition.TimeNight)
                .AddWithCustomValue(ModContent.ItemType<Moonshine>(), Item.buyPrice(gold: 2), Condition.DownedGolem)
                .AddWithCustomValue(ModContent.ItemType<MoscowMule>(), Item.buyPrice(gold: 8), Condition.DownedGolem)
                .AddWithCustomValue(ModContent.ItemType<CinnamonRoll>(), Item.buyPrice(gold: 8), Condition.DownedGolem)
                .AddWithCustomValue(ModContent.ItemType<TequilaSunrise>(), Item.buyPrice(gold: 10), Condition.DownedGolem)
                .AddWithCustomValue(ItemID.BloodyMoscato, Item.buyPrice(gold: 1), Condition.DownedMoonLord, Condition.NpcIsPresent(NPCID.Stylist))
                .AddWithCustomValue(ItemID.BananaDaiquiri, Item.buyPrice(silver: 75), Condition.DownedMoonLord, Condition.NpcIsPresent(NPCID.Stylist))
                .AddWithCustomValue(ItemID.PeachSangria, Item.buyPrice(silver: 50), Condition.DownedMoonLord, Condition.NpcIsPresent(NPCID.Stylist))
                .AddWithCustomValue(ItemID.PinaColada, Item.buyPrice(gold: 1), Condition.DownedMoonLord, Condition.NpcIsPresent(NPCID.Stylist))
                .AddWithCustomValue(ModContent.ItemType<WeightlessCandle>(), Item.buyPrice(gold: 50))
                .AddWithCustomValue(ModContent.ItemType<VigorousCandle>(), Item.buyPrice(gold: 50))
                .AddWithCustomValue(ModContent.ItemType<ResilientCandle>(), Item.buyPrice(gold: 50))
                .AddWithCustomValue(ModContent.ItemType<SpitefulCandle>(), Item.buyPrice(gold: 50))
                .AddWithCustomValue(ModContent.ItemType<OddMushroom>(), Item.buyPrice(1))
                .AddWithCustomValue(interlude1Box.Type, Item.buyPrice(gold: 10), CalamityConditions.DownedCalamitasClone)
                .AddWithCustomValue(interlude2Box.Type, Item.buyPrice(gold: 10), Condition.DownedMoonLord)
                .AddWithCustomValue(interlude3Box.Type, Item.buyPrice(gold: 10), CalamityConditions.DownedYharon)
                .AddWithCustomValue(eulogyBox.Type, Item.buyPrice(gold: 10), CalamityConditions.DownedDevourerOfGods)
                .AddWithCustomValue(ItemID.UnicornHorn, Item.buyPrice(0, 2, 50), Condition.HappyEnough, Condition.InHallow)
                .AddWithCustomValue(ItemID.Milkshake, Item.buyPrice(gold: 5), Condition.HappyEnough, Condition.InHallow, Condition.NpcIsPresent(NPCID.Stylist))
                .AddWithCustomValue(ModContent.ItemType<CirrusCouch>(), Item.buyPrice(gold: 25), Condition.HappyEnough, Condition.NpcIsPresent(NPCID.Stylist), Condition.NpcIsPresent(NPCID.BestiaryGirl))
                .Register();
        }

        // Make this Town NPC teleport to the Queen statue when triggered.
        public override bool CanGoToStatue(bool toKingStatue) => !toKingStatue;

        public override void TownNPCAttackStrength(ref int damage, ref float knockback)
        {
            damage = 15;
            knockback = 2f;
        }

        public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown)
        {
            cooldown = 180;
            randExtraCooldown = 60;
        }

        public override void TownNPCAttackProj(ref int projType, ref int attackDelay)
        {
            projType = ModContent.ProjectileType<SylvRay>();
            attackDelay = 1;
        }

        public override void TownNPCAttackProjSpeed(ref float multiplier, ref float gravityCorrection, ref float randomOffset)
        {
            multiplier = 11.5f;
        }
    }
}
