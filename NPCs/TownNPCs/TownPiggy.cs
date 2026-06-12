using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace CalamityMod.NPCs.TownNPCs
{
    [AutoloadHead]
    public class TownPiggy : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 13;
            NPCID.Sets.ExtraFramesCount[Type] = 0;
            NPCID.Sets.AttackFrameCount[Type] = 0;
            NPCID.Sets.DangerDetectRange[Type] = 250;
            NPCID.Sets.HatOffsetY[Type] = -4;
            NPCID.Sets.ShimmerTownTransform[Type] = false;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Shimmer] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
            NPCID.Sets.NPCFramingGroup[Type] = 8;

            NPCID.Sets.IsTownPet[Type] = true;
            NPCID.Sets.CannotSitOnFurniture[Type] = false;
            NPCID.Sets.TownNPCBestiaryPriority.Add(Type);
            NPCID.Sets.PlayerDistanceWhilePetting[Type] = 32;
            NPCID.Sets.IsPetSmallForPetting[Type] = true;

            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new()
            {
                Velocity = 0.25f,
            };

            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
        }

        public override void SetDefaults()
        {
            NPC.townNPC = true;
            NPC.friendly = true;
            NPC.width = 20;
            NPC.height = 20;
            NPC.aiStyle = NPCAIStyleID.Passive;
            AIType = NPCID.TownBunny;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.lifeMax = 250;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0.5f;
            NPC.housingCategory = 1;
            DrawOffsetY = -4;
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(
            [
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.TownPiggy")
            ]);
        }

        public override bool CanTownNPCSpawn(int numTownNPCs)
        {
            if (CalamityWorld.unlockedTownPig && !NPC.AnyNPCs(ModContent.NPCType<ShadySalesman>()))
            {
                return true;
            }
            return false;
        }

        public override void AI()
        {
            NPC.spriteDirection = NPC.direction;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            // Put a gold crown on Techno
            if (NPC.GivenName == this.GetLocalizedValue("Name.Techno") && !BirthdayParty.PartyIsUp)
            {
                int equipID = ArmorIDs.Head.GoldCrown;
                Main.instance.LoadArmorHead(equipID);
                Texture2D crown = TextureAssets.ArmorHead[equipID].Value;
                Rectangle crownFrame = crown.Frame(1, 20, 0, 0);
                int frameHeight = TextureAssets.Npc[Type].Value.Height / Main.npcFrameCount[Type];
                int curFrame = NPC.frame.Y / frameHeight;
                int sheetOff = curFrame switch
                {
                    0 => 0,
                    1 => -2,
                    2 => -6,
                    3 => -6,
                    4 => -2,
                    5 => -4,
                    6 => -6,
                    7 => -2,
                    _ => 0
                };
                spriteBatch.Draw(crown, NPC.Center - screenPos + new Vector2(8 * NPC.spriteDirection, -2 + NPC.gfxOffY + sheetOff).RotatedBy(NPC.rotation), crownFrame, NPC.GetAlpha(drawColor), NPC.rotation, new Vector2(crown.Width / 2, crown.Height / 40), NPC.scale, NPC.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
            }
        }

        public override void FindFrame(int frameHeight)
        {
            // Sleep animation
            if (NPC.ai[0] >= 20f && NPC.ai[0] <= 22f)
            {
                if (NPC.velocity.Y == 0)
                {
                    if (NPC.ai[1] > 30f && (NPC.frame.Y < frameHeight * 8))
                    {
                        NPC.frame.Y = frameHeight * 8;
                    }
                    if (NPC.frame.Y > 0)
                    {
                        NPC.frameCounter++;
                    }
                    if (NPC.frameCounter > 6)
                    {
                        NPC.frameCounter = 0;
                        NPC.frame.Y += frameHeight;
                    }
                    if (NPC.frame.Y > 12 * frameHeight)
                    {
                        NPC.frame.Y = frameHeight * 8;
                    }
                }
            }
            else if (NPC.velocity.X == 0)
            {
                NPC.frame.Y = 0;
            }
            else
            {
                if (NPC.velocity.Y == 0)
                {
                    if (NPC.frameCounter++ % 6 == 0)
                    {
                        NPC.frame.Y += frameHeight;
                    }
                }
                if (NPC.frame.Y < frameHeight)
                    NPC.frame.Y = frameHeight;
                if (NPC.frame.Y > frameHeight * 7)
                    NPC.frame.Y = frameHeight;
            }
        }

        public override List<string> SetNPCNameList() => new List<string>()
        {
            // Original names
            this.GetLocalizedValue("Name.Curly"),

            // Reference names
            this.GetLocalizedValue("Name.Oolong"), // Dragon Ball
            this.GetLocalizedValue("Name.Napoleon"), // Animal Farm
            this.GetLocalizedValue("Name.Waddles"), // Gravity Falls
            this.GetLocalizedValue("Name.Crenando"), // Ganondorf
            this.GetLocalizedValue("Name.Olivia"), // Olivia
            this.GetLocalizedValue("Name.Wilbur"), // Charlotte's Web
            this.GetLocalizedValue("Name.Pumbaa"), // The Lion King
            this.GetLocalizedValue("Name.Peppa"), // Peppa Pig
            this.GetLocalizedValue("Name.Conan"), // Conan the mighty pig
            this.GetLocalizedValue("Name.Reuben"), // Minecraft: Story Mode
            this.GetLocalizedValue("Name.Porky"), // Looney Tunes
            this.GetLocalizedValue("Name.Hamm"), // Toy Story
            this.GetLocalizedValue("Name.Runt"), // Chicken Little
            this.GetLocalizedValue("Name.Roko"), // Roko's Basilisk
            this.GetLocalizedValue("Name.RichardHam"), // Pig from Clarkson's Farm named after Richard Hammond
            this.GetLocalizedValue("Name.Techno"), // Technoblade
            this.GetLocalizedValue("Name.JohnPork"), // John Pork
            this.GetLocalizedValue("Name.Piglet"), // Winnie-the-Pooh
            
            // Dedicated names
        };

        public override string GetChat()
        {
            WeightedRandom<string> chat = new();
            
            for (int i = 0; i <= 2; i++)
            {
                chat.Add(CalamityUtils.GetText("NPCs.TownPiggy.Chat." + i).Value);
            }

            return chat;
        }

        public override void SetChatButtons(ref string button, ref string button2)
        {
            button = Language.GetTextValue("UI.PetTheAnimal");
        }
    }
}
