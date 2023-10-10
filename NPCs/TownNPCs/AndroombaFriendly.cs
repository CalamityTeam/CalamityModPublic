using CalamityMod.BiomeManagers;
using CalamityMod.Items.Critters;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.TownNPCs
{
    public class AndroombaFriendly : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 9;
            Main.npcCatchable[NPC.type] = true;
            NPCID.Sets.NoTownNPCHappiness[Type] = true;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0);
            value.Position.Y += 16;
            value.PortraitPositionYOverride = 36f;
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;
            NPCID.Sets.ShimmerTownTransform[Type] = false;
        }

        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.damage = 0;
            NPC.width = 36;
            NPC.height = 16;
            NPC.lifeMax = 80;
            NPC.friendly = true;
            NPC.townNPC = true;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = false;
            NPC.chaseable = false;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath44;
            NPC.catchItem = (short)ModContent.ItemType<AndroombaItem>();
            SpawnModBiomes = new int[1] { ModContent.GetInstance<ArsenalLabBiome>().Type };
            NPC.dontTakeDamage = true;
        }

        public override bool CanChat() => false;

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] 
            {
                new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.AndroombaFriendly")
            });
        }

        public override void AI()
        {
            // Gravity
            NPC.velocity.Y = MathHelper.Clamp(NPC.velocity.Y + 0.4f, -15f, 15f);
            NPC.spriteDirection = (int)NPC.ai[2];
            switch (NPC.ai[0])
            {
                case 0:
                    {
                        if (NPC.ai[1] == 0)
                        {
                            Player closest = Main.player[Player.FindClosest(NPC.position, 9999, 9999)];
                            NPC.ai[2] = (closest.position.X <= NPC.position.X) ? 1 : -1;
                        }
                    }
                    break;
                // Main
                case 1:
                    {
                        NPC.ai[1]++;
                        NPC.velocity.X = NPC.ai[2] * 2;
                        if (!Collision.CanHit(NPC.Center - Vector2.UnitX * NPC.ai[2] * 8f, 2, 2, NPC.Center + Vector2.UnitX * NPC.ai[2] * 32f, 8, 8))
                        {
                            ChangeAIHook(2);
                        }
                        Convert((int)NPC.ai[3]);
                    }
                    break;
                // Turn
                case 2:
                    {
                        NPC.velocity.X = 0;
                        Convert((int)NPC.ai[3]);
                    }
                    break;
            }

            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player player = Main.player[i];
                if (player is null || !player.active)
                    continue;

                if (NPC.Hitbox.Intersects(player.HitboxForBestiaryNearbyCheck))
                {
                    Main.BestiaryTracker.Chats.RegisterChatStartWith(NPC);
                    break;
                }
            }
        }

        public void Convert(int conversionType)
        {
            int x = (int)(NPC.Center.X / 16f);
            int y = (int)(NPC.Center.Y / 16f);
            if (conversionType <= 7)
            {
                ConvertType type = ConvertType.Pure;
                switch (conversionType)
                {
                    case 1:
                        type = ConvertType.Corrupt;
                        break;
                    case 2:
                        type = ConvertType.Hallow;
                        break;
                    case 4:
                        type = ConvertType.Crimson;
                        break;
                }
                AstralBiome.ConvertFromAstral(x - 2, x + 2, y - 2, y + 2, type);
                WorldGen.Convert(x, y, conversionType, 2);
            }
            else
            {
                AstralBiome.ConvertToAstral(x - 2, x + 2, y - 2, y + 2);
            }
        }

        public void ChangeAIHook(int phase)
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                ChangeAI(NPC.whoAmI, phase);
            }
            else
            {
                var netMessage = Mod.GetPacket();
                netMessage.Write((byte)CalamityModMessageType.SyncAndroombaAI);
                netMessage.Write(NPC.whoAmI);
                netMessage.Write(phase);
                netMessage.Send();
            }
        }

        public static void ChangeAI(int index, int phase)
        {
            NPC npc = Main.npc[index];

            if (npc is null || !npc.active)
                return;

            npc.ai[0] = phase;
            npc.ai[1] = 0;
            npc.netUpdate = true;
            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, index);
        }

        public static void SwapSolution(int index, int solutionType)
        {
            NPC npc = Main.npc[index];

            if (npc is null || !npc.active)
                return;

            npc.ai[3] = solutionType;
            npc.netUpdate = true;
            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, index);
        }

        public override bool? CanBeHitByItem(Player player, Item item) => null;

        public override bool? CanBeHitByProjectile(Projectile projectile) => null;

        public override void FindFrame(int frameHeight)
        {
            /*
            -Frame 0: Asleep
            -Frames 1-4: Moving
            -Frames 5-9: Turning
            */
            NPC.frameCounter += 1.0;
            if (NPC.frameCounter > 6.0)
            {
                NPC.frameCounter = 0.0;
                NPC.frame.Y += frameHeight;
            }

            // Idle
            if (NPC.ai[0] == 1 || NPC.IsABestiaryIconDummy)
            {
                if (NPC.frame.Y > frameHeight * 4)
                {
                    NPC.frameCounter = 0.0;
                    NPC.frame.Y = frameHeight;
                }
            }
            // Turnaround
            else if (NPC.ai[0] == 2)
            {
                if (NPC.frame.Y < frameHeight * 5)
                {
                    NPC.frameCounter = 0.0;
                    NPC.frame.Y = frameHeight * 5;
                }
                if (NPC.frame.Y > frameHeight * 8)
                {
                    NPC.frame.Y = frameHeight;
                    NPC.ai[2] *= -1;
                    ChangeAIHook(1);
                }
            }
            // Sleep
            else
            {
                NPC.frame.Y = 0;
            }
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int i = 0; i < 6; i++)
                Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, 226);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D critterTexture = TextureAssets.Npc[NPC.type].Value;
            string pathextenstion = "Pure";
            switch (NPC.ai[3])
            {
                case 0:
                    pathextenstion = "Pure";
                    break;
                case 1:
                    pathextenstion = "Corruption";
                    break;
                case 2:
                    pathextenstion = "Hallow";
                    break;
                case 3:
                    pathextenstion = "Mushroom";
                    break;
                case 4:
                    pathextenstion = "Crimson";
                    break;
                case 5:
                    pathextenstion = "Desert";
                    break;
                case 6:
                    pathextenstion = "Snow";
                    break;
                case 7:
                    pathextenstion = "Forest";
                    break;
                case 8:
                    pathextenstion = "Astral";
                    break;
            }
            Texture2D glowmask = ModContent.Request<Texture2D>("CalamityMod/NPCs/TownNPCs/AndroombaFriendly_" + pathextenstion).Value;
            Vector2 drawPosition = NPC.Center - screenPos + Vector2.UnitY * NPC.gfxOffY;
            drawPosition.Y += DrawOffsetY;
            SpriteEffects direction = NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            spriteBatch.Draw(critterTexture, drawPosition, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() * 0.5f, NPC.scale, direction, 0f);
            spriteBatch.Draw(glowmask, drawPosition, NPC.frame, NPC.GetAlpha(Color.White), NPC.rotation, NPC.frame.Size() * 0.5f, NPC.scale, direction, 0f);
            return false;
        }
    }
}
