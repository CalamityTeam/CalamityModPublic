using CalamityMod.BiomeManagers;
using CalamityMod.Dusts;
using CalamityMod.Events;
using CalamityMod.Items;
using CalamityMod.Projectiles.Magic;
using CalamityMod.UI.CalamitasEnchants;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.Events;
using Terraria.GameContent.Personalities;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;
using static Terraria.ModLoader.ModContent;
using SCalBoss = CalamityMod.NPCs.SupremeCalamitas.SupremeCalamitas;

namespace CalamityMod.NPCs.TownNPCs
{
    [AutoloadHead]
    public class WITCH : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 27;
            NPCID.Sets.ExtraFramesCount[NPC.type] = 9;
            NPCID.Sets.AttackFrameCount[NPC.type] = 4;
            NPCID.Sets.DangerDetectRange[NPC.type] = 700;
            NPCID.Sets.AttackType[NPC.type] = 1;
            NPCID.Sets.AttackTime[NPC.type] = 30;
            NPCID.Sets.AttackAverageChance[NPC.type] = 5;
            NPCID.Sets.ShimmerTownTransform[Type] = false;
            NPC.Happiness
                .SetBiomeAffection<ForestBiome>(AffectionLevel.Like)
                .SetBiomeAffection<BrimstoneCragsBiome>(AffectionLevel.Dislike)
                .SetNPCAffection(NPCID.Clothier, AffectionLevel.Like)
                .SetNPCAffection(NPCID.PartyGirl, AffectionLevel.Dislike);
			NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers(0) {
				Velocity = 1f // Draws the NPC in the bestiary as if its walking +1 tiles in the x direction
			};
			NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, drawModifiers);
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

            // You should not be able to kill SCal under any typical circumstances.
            NPC.lifeMax = 960000;

            NPC.defense = 120;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath6;
            NPC.knockBackResist = 0.8f;
            AnimationType = NPCID.Wizard;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] 
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,        
				new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.WITCH")
            });
        }

        public override bool CanTownNPCSpawn(int numTownNPCs) => DownedBossSystem.downedCalamitas && !NPC.AnyNPCs(NPCType<SCalBoss>());

		public override List<string> SetNPCNameList() => new List<string>() { this.GetLocalizedValue("Name.Calamitas") };

        // The way this works is by having an RNG based on weights.
        // With certain conditions (such as if a blood moon is happening) you can add possibilities
        // to the RNG via dialogue.Add("text", weight);
        // Text can always appear assuming the weight is greater than 0 and there's no if condition deciding whether it can.
        // The higher the weight is, the more likely it is to be selected from all the choices.
        // To give an example of this, assume you have two possibilities:
        // "a" with a weight of 1, and "b" with a weight of 5. The chance of "a" being displayed would be
        // 1/6, while "b" wold have a 5/6 chance of being displayed.
        // If only one possibility exists it will be the only thing that is displayed, regardless of weight.
        public override string GetChat()
        {
            WeightedRandom<string> dialogue = new WeightedRandom<string>();

            // Have a flat chance (1/4444) to simply ignore the below selection and say something humorous instead.
            if (Main.rand.NextBool(4444))
                return this.GetLocalizedValue("Chat.EasterEgg");

            if (NPC.homeless)
                return this.GetLocalizedValue("Chat.Homeless" + Main.rand.Next(1, 2 + 1));

            dialogue.Add(this.GetLocalizedValue("Chat.Normal1"));
            dialogue.Add(this.GetLocalizedValue("Chat.Normal2"));
            dialogue.Add(this.GetLocalizedValue("Chat.Normal3"));
            dialogue.Add(this.GetLocalizedValue("Chat.Normal4"));
            dialogue.Add(this.GetLocalizedValue("Chat.Normal5"));

            if (!Main.dayTime)
            {
                if (Main.bloodMoon)
                {
                    dialogue.Add(this.GetLocalizedValue("Chat.BloodMoon1"), 5.15);
                    dialogue.Add(this.GetLocalizedValue("Chat.BloodMoon2"), 5.15);
                }
                else
                {
                    dialogue.Add(this.GetLocalizedValue("Chat.Night1"), 2.8);
                    dialogue.Add(this.GetLocalizedValue("Chat.Night2"), 2.8);
                }
            }

            int fab = NPC.FindFirstNPC(NPCType<FAP>());
            if (fab != -1 && ChildSafety.Disabled)
                dialogue.Add(this.GetLocalization("Chat.DrunkPrincess").Format(Main.npc[fab].GivenName), 1.45);

            if (NPC.AnyNPCs(NPCType<SEAHOE>()))
                dialogue.Add(this.GetLocalizedValue("Chat.SeaKing"), 1.45);

            if (BirthdayParty.PartyIsUp)
                dialogue.Add(this.GetLocalizedValue("Chat.Party"), 5.5);

            return dialogue;
        }

        public override void SetChatButtons(ref string button, ref string button2) => button = this.GetLocalizedValue("EnchantButton");

        public override void OnChatButtonClicked(bool firstButton, ref string shopName)
        {
            if (firstButton)
            {
                Main.playerInventory = true;
                CalamitasEnchantUI.NPCIndex = NPC.whoAmI;
                CalamitasEnchantUI.CurrentlyViewing = true;

                if (!Main.LocalPlayer.Calamity().GivenBrimstoneLocus)
                {
                    Item.NewItem(NPC.GetSource_Loot(), NPC.Hitbox, ItemType<BrimstoneLocus>());
                    Main.LocalPlayer.Calamity().GivenBrimstoneLocus = true;
                }
            }
        }

        // Make this Town NPC teleport to the Queen statue when triggered.
        public override bool CanGoToStatue(bool toKingStatue) => !toKingStatue;

        public override void TownNPCAttackStrength(ref int damage, ref float knockback)
        {
            damage = 50;
            knockback = 10f;
        }

        public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown)
        {
            cooldown = 10;
            randExtraCooldown = 50;
        }

        public override bool PreAI()
        {
            // Disappear if the SCal boss is active. She's supposed to be the boss.
            // However, this doesn't happen in Boss Rush; the SCal there is a silent puppet created by Xeroc, not SCal herself.
            if (NPC.AnyNPCs(NPCType<SCalBoss>()) && !BossRushEvent.BossRushActive)
            {
                NPC.active = false;
                NPC.netUpdate = true;
                return false;
            }
            return true;
        }

        //public override void TownNPCAttackMagic(ref float auraLightMultiplier)

        public override void TownNPCAttackProj(ref int projType, ref int attackDelay)
        {
            projType = ProjectileType<SeethingDischargeBrimstoneHellblast>();
            attackDelay = 1;
        }

        public override void TownNPCAttackProjSpeed(ref float multiplier, ref float gravityCorrection, ref float randomOffset)
        {
            multiplier = 2f;
        }

        // Explode into red dust on death.
        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                NPC.position = NPC.Center;
                NPC.width = NPC.height = 50;
                NPC.position.X -= NPC.width / 2;
                NPC.position.Y -= NPC.height / 2;
                for (int i = 0; i < 5; i++)
                {
                    int brimstone = Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.Brimstone, 0f, 0f, 100, default, 2f);
                    Main.dust[brimstone].velocity *= 3f;
                    if (Main.rand.NextBool())
                    {
                        Main.dust[brimstone].scale = 0.5f;
                        Main.dust[brimstone].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                    }
                }

                for (int i = 0; i < 10; i++)
                {
                    int fire = Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.Brimstone, 0f, 0f, 100, default, 3f);
                    Main.dust[fire].noGravity = true;
                    Main.dust[fire].velocity *= 5f;

                    fire = Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.Brimstone, 0f, 0f, 100, default, 2f);
                    Main.dust[fire].velocity *= 2f;
                }
            }
        }
    }
}
