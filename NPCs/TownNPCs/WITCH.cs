using CalamityMod.Dusts;
using CalamityMod.Events;
using CalamityMod.Items;
using CalamityMod.Projectiles.Magic;
using CalamityMod.UI.CalamitasEnchants;
using CalamityMod.World;
using Terraria;
using Terraria.GameContent.Events;
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
            DisplayName.SetDefault("Brimstone Witch");

            Main.npcFrameCount[NPC.type] = 27;
            NPCID.Sets.ExtraFramesCount[NPC.type] = 9;
            NPCID.Sets.AttackFrameCount[NPC.type] = 4;
            NPCID.Sets.DangerDetectRange[NPC.type] = 700;
            NPCID.Sets.AttackType[NPC.type] = 1;
            NPCID.Sets.AttackTime[NPC.type] = 30;
            NPCID.Sets.AttackAverageChance[NPC.type] = 5;
        }

        public override void SetDefaults()
        {
            NPC.townNPC = true;
            NPC.friendly = true;
            NPC.lavaImmune = true;
            NPC.width = 18;
            NPC.height = 40;
            NPC.aiStyle = 7;
            NPC.damage = 10;

            // You should not be able to kill SCal under any typical circumstances.
            NPC.lifeMax = 1000000;

            NPC.defense = 120;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath6;
            NPC.knockBackResist = 0.8f;
            animationType = NPCID.Wizard;
        }

        public override bool CanTownNPCSpawn(int numTownNPCs, int money) => DownedBossSystem.downedSCal && !NPC.AnyNPCs(NPCType<SCalBoss>());

        public override string TownNPCName() => "Calamitas";

        // The way this works is by having an RNG based on weights.
        // With certain conditions (such as if a blood moon is happening) you can add possibilities
        // to the RNG via textSelector.Add("text", weight);
        // Text can always appear assuming the weight is greater than 0 and there's no if condition deciding whether it can.
        // The higher the weight is, the more likely it is to be selected from all the choices.
        // To give an example of this, assume you have two possibilities:
        // "a" with a weight of 1, and "b" with a weight of 5. The chance of "a" being displayed would be
        // 1/6, while "b" wold have a 5/6 chance of being displayed.
        // If only one possibility exists it will be the only thing that is displayed, regardless of weight.
        public override string GetChat()
        {
            WeightedRandom<string> textSelector = new WeightedRandom<string>(Main.rand);

            if (NPC.homeless)
            {
                textSelector.Add("I'm considering moving back to that old cave of mine.");
                textSelector.Add("I certainly can't return to the Tyrant's old dwellings now, have you got any places to stay?");
            }
            else
            {
                textSelector.Add("I can't pay rent, but if you've got any dead relative you want me to try and... what? You don't?");
                textSelector.Add("One of these days, I was thinking of starting a garden with the flowers from the old capitol of hell." +
                    " I love the smell of brimstone in the morning.");
                textSelector.Add("I think I've settled comfortably, thank you very much.");
                textSelector.Add("Many seasons have gone by since I first met with the Tyrant, and only now did I break free." +
                    " I wish I'd been stronger...");
                textSelector.Add("If you've got any curses you want dispelled... well I'm not your person.");

                if (!Main.dayTime)
                {
                    if (Main.bloodMoon)
                    {
                        textSelector.Add("Such an unnatural shade of red. Nothing like my brimstone flames.", 5.15);
                        textSelector.Add("I can't work with nights like these. The stars seem to have shrunk away in fear.", 5.15);
                    }
                    else
                    {
                        textSelector.Add("These undead are horrific, I can't stand to look at them. How could anyone be satisfied" +
                            " with such amateur work?", 2.8);
                        textSelector.Add("I don't think it's a stretch to say that astrology is utter nonsense... but it was a hobby" +
                            " of mine once.", 2.8);
                    }
                }

                if (BirthdayParty.PartyIsUp)
                    textSelector.Add("If another person asks me if I can dance or not, I will light their hat on fire.", 5.5);

                if (NPC.AnyNPCs(NPCType<SEAHOE>()))
                {
                    textSelector.Add("I cannot understand the Sea King. He does not seem to want me dead. That amount of compassion" +
                        " I just can't understand.", 1.45);
                }
                if (NPC.AnyNPCs(NPCType<DILF>()))
                {
                    textSelector.Add("That frosty old man... even if you ignore our brands of magic and our old alliances, I doubt I'd ever" +
                        " get along with him.", 1.45);
                }

                int fab = NPC.FindFirstNPC(NPCType<FAP>());
                if (fab != -1)
                {
                    textSelector.Add("I wonder if " + Main.npc[fab].GivenName + " ever feels cold given how revealing her dress is." +
                        " Perhaps she should cover up a bit more.", 1.45);
                }
            }

            // Select a possibility from the RNG and choose it as the thing that should be said.
            string thingToSay = textSelector.Get();

            // Have a flat chance (1/4444) to simply ignore the above selection and say something humorous instead.
            if (Main.rand.NextBool(4444))
                thingToSay = "Mrrp is cringe.";

            return thingToSay;
        }

        public override void SetChatButtons(ref string button, ref string button2) => button = "Enchant";

        public override void OnChatButtonClicked(bool firstButton, ref bool shop)
        {
            if (firstButton)
            {
                Main.playerInventory = true;
                Main.LocalPlayer.Calamity().newCalamitasInventory = false;
                CalamitasEnchantUI.NPCIndex = NPC.whoAmI;
                CalamitasEnchantUI.CurrentlyViewing = true;

                if (!Main.LocalPlayer.Calamity().GivenBrimstoneLocus)
                {
                    DropHelper.DropItem(NPC, ItemType<BrimstoneLocus>());
                    Main.LocalPlayer.Calamity().GivenBrimstoneLocus = true;
                }

                shop = false;
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
        public override void HitEffect(int hitDirection, double damage)
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
                    if (Main.rand.NextBool(2))
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
