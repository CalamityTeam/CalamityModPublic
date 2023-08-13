using CalamityMod.Events;
using CalamityMod.Items;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Ammo;
using CalamityMod.Items.Armor;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.Dyes;
using CalamityMod.Items.Dyes.HairDye;
using CalamityMod.Items.Pets;
using CalamityMod.Items.Placeables.Furniture;
using CalamityMod.Items.Placeables.Furniture.Fountains;
using CalamityMod.Items.Potions;
using CalamityMod.Items.SummonItems.Invasion;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.NPCs.TownNPCs;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using CalamityMod.Items.Placeables.Walls;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.NPCs
{
    public partial class CalamityGlobalNPC : GlobalNPC
    {
        public static float TaxYieldFactor
        {
            get
            {
                // Max of 20 platinum.
                if (DownedBossSystem.downedYharon)
                    return 40f;

                // Max of 10 platinum.
                if (DownedBossSystem.downedDoG)
                    return 20f;

                // Max of 5 platinum.
                if (NPC.downedMoonlord)
                    return 10f;

                // Max of 2 platinum.
                if (NPC.downedPlantBoss)
                    return 4f;

                return 1f;
            }
        }

        public static int TotalTaxesPerNPC => (int)(Item.buyPrice(0, 0, 1, 50) * TaxYieldFactor);

        public static int TaxesToCollectLimit => (int)(Item.buyPrice(0, 50, 0, 0) * TaxYieldFactor);

        // TODO -- Update all Discord tags here to new Discord usernames.
        #region Town NPC Patreon Name Sets
        private static readonly string[] AnglerNames =
        {
            "Dazren",
            "Johnny Test", // <@!589966747977777197> (konorango)
        };
        private static readonly string[] ArmsDealerNames =
        {
            "Drifter",
            "Finchi",
            "Heniek", // <@!363404700445442050> (Kazurgundu#3791)
            "Fire", // <@!354362326947856384> (fire#0692)
            "Barney Calhoun", // <@!634462901431697410> (Potato Power#6578)
            "XiaoEn0426", // <@!440448864772816896> (XiaoEn0426#9157)
            "Jeffred", // <@!295362230038560768> (Knight Solaire#0873)
            "The Cooler Arthur", // <@!568263512523014154> (better artilery#0001)
            "Markie", // <@!291141964039061504> (Markie#6969)
        };
        private static readonly string[] ClothierNames =
        {
            "Joeseph Jostar",
        };
        private static readonly string[] CyborgNames =
        {
            "Sylux", // <@!331812782183809025> (Gonk#2451)
        };
        private static readonly string[] DemolitionistNames =
        {
            "Tavish DeGroot", // <@!442447226992721930> (Magicoal#2655)
        };
        private static readonly string[] DryadNames =
        {
            "Rythmi",
            "Izuna",
            "Jasmine", // <@!430532867479699456> (phantasmagoria#6777)
            "Cybil", // <@!486507232666845185> (Captain Doofus#????)
            "Ruth", // <@!1001307586068492388> (Briny_Coffee#4393)
        };
        private static readonly string[] DyeTraderNames = null;
        private static readonly string[] GoblinTinkererNames =
        {
            "Verth",
            "Gormer", // <@!287651204924833795> (Picasso's Bean#2819 -- RIP)
            "TingFlarg", // <@!185605031716847616> (Smug#7160)
            "Driser", // <@!121996994406252544> (Driser#8630)
            "Eddie Spaghetti", // <@!466397267407011841> (Eddie Spaghetti#0002)
            "G'tok", // <@!335192200956608535> (gtoktas#7589)
            "Katto", // <@!175972165504466944> (Katto#2858)
            "Him", // <@!931019614958256139> (@himtheguy1)
            "Tooshiboots", // <@!333532730593771522> (ulmod)
        };
        private static readonly string[] GolferNames = null;
        private static readonly string[] GuideNames =
        {
            "Lapp",
            "Ben Shapiro",
            "Streakist", // used to be "StreakistYT". couldn't find the youtube channel, and decided to remove the ad.
            "Necroplasmic",
            "Devin",
            "Woffle", // <@!185980979427540992> (Chipbeam#2268)
            "Cameron", // <@!340401981711712258> (CammyWammy#8634)
            "Wilbur", // <@!295171926324805634> (ChaosChaos#5979)
            "Good Game Design", // <@!564267767042277385> (Dominic Karma#7777)
            "Danmaku", // <@!756259562268524555> (Danmaku#2659)
            "Grylken", // <@!299970404435361802> (Grylken#1569)
            "Outlaw", // <@!918311619480657922> (TheChosenOutlaw#8746)
            "Alfred Rend", // <@!606301806481375255> (Deadsqurp300#0907)
            "Leeman", // <@!281999243168841728> (Tweee#5831)
        };
        private static readonly string[] MechanicNames =
        {
            "Lilly",
            "Daawn", // <@!206162323541458944> (Daawnily#3859)
            "Robin", // <@!654737510030639112> (Altzeus#8687)
            "Curly", // <@!673092101780668416> (Curly~Brace#4830)
        };
        private static readonly string[] MerchantNames =
        {
            "Morshu", // <@!194931581826236416> (Uberransy#6969)
        };
        private static readonly string[] NurseNames =
        {
            "Farsni",
            "Fanny", // <@!799749125720637460> (zombiewolf511#4581)
            "Mausi", // <@!194156349347594241> (Sadouken | Daniel#3996)
        };
        private static readonly string[] PainterNames =
        {
            "Picasso", // <@!353316526306361347> (SCONICBOOM#2164 -- for the late Picasso's Bean#2819)
        };
        private static readonly string[] PartyGirlNames =
        {
            "Arin", // <@!268169458302976012> (Kiyotu#0006)
        };
        private static readonly string[] PirateNames =
        {
            "Tyler Van Hook",
            "Cap'n Deek", // "Alex N" on Patreon
            "Captain Billy Bones", // <@!699589229507772416> (DjackV#2882)
            "Captain J. Crackers", // <@!233232602994049024> (Qyuuno#3031)
        };
        private static readonly string[] PrincessNames =
        {
            "Catalyst", // <@!156672312425316352> (xAqult#1122)
            "Nyapano", // <@!120976656826368003> (Emi - Nyapano She/Her#4040)
            "Jade", // <@!187395834625785869> (VeryMasterNinja#7728)
            "Nyavi Aceso", // <@!270260920888852480> (Navigator#8739)
        };
        private static readonly string[] SkeletonMerchantNames =
        {
            "Sans Undertale", // <@!145379091648872450> (Shayy#5257)
            "Papyrus Undertale", // <@!262663471189983242> (Nycro#0001)
        };
        private static readonly string[] SteampunkerNames =
        {
            "Vorbis",
            "Angel",
        };
        private static readonly string[] StylistNames =
        {
            "Amber", // <@!114677116473180169> (Mishiro Usui#1295)
            "Faith", // <@!509050283871961123> (Toasty#1007)
            "Xsiana", // <@!625780237489143839> (xiana.#0015)
        };
        private static readonly string[] TavernkeepNames =
        {
            "Tim Lockwood", // <@!605839945483026434> (Deimelo#0001)
            "Sir Samuel Winchester Jenkins Kester II", // <@!107659695749070848> (Ryaegos#1661)
            "Brutus", // <@!591889650692521984> (Brutus#4337)
            "Sloth", // <@!486265327387279391> (BossyPunch#4983)
        };
        private static readonly string[] TaxCollectorNames =
        {
            "Emmett",
        };
        private static readonly string[] TravelingMerchantNames =
        {
            "Stan Pines",
            "Slap Battles", // <@!923504188615450654> (Conquestor#9009)
        };
        private static readonly string[] TruffleNames =
        {
            "Aldrimil", // <@!413719640238194689> (Thorioum#2475)
        };
        private static readonly string[] WitchDoctorNames =
        {
            "Sok'ar",
            "Toxin", // <@!348174404984766465> (Toxin#9598),
            "Mixcoatl", // <@!284775927294984203> (SharZz#7777)
            "Khatunz", // <@!303022375191183360> (jackshiz#7839)
            "Amnesia Wapers", // <@!326821498323075073> (Retarded Advice from a Retard#6969)
        };
        private static readonly string[] WizardNames =
        {
            "Mage One-Trick", // <@!340996105460514816> (pixlgray) - going of off memory on this one
            "Inorim, son of Ivukey",
            "Jensen",
            "Merasmus", // <@!288066987819663360> (Spider pee pee#3328)
            "Habolo", // <@!163028025494077441> (ChristmasGoat#7810)
            "Ortho", // <@!264984390910738432> (Worcuus#5225)
            "Chris Tallballs", // <@!770211589076418571> (Bewearium#1111)
            "Syethas", // <@!325413275066171393> (CosmicStarIight#4430)
            "Nextdoor Psycho", // <@!173261518572486656> (⋈-NextdoorPsycho-⋈#0001)
        };
        private static readonly string[] ZoologistNames =
        {
            "Kiriku", // <@!395312478160027668> (rulosss#6814)
            "Lacuna", // <@!790746689211203604> (Lacuna#8629)
            "Mae Borowski", //<@!219158690433990656> (justakkolite)
        };

        // The following sets are for the 1.4 Town Pets: Town Dogs, Cats and Bunnies.
        // All three pet types come in numerous breeds. Each breed has its own name pool.
        // Donator pet names should be appended to all breeds' name pools equally.

        private const int TownDogLabradorVanillaNames = 17;
        private const int TownDogPitBullVanillaNames = 14;
        private const int TownDogBeagleVanillaNames = 12;
        private const int TownDogCorgiVanillaNames = 14;
        private const int TownDogDalmatianVanillaNames = 13;
        private const int TownDogHuskyVanillaNames = 16;
        private static readonly string[] TownDogNames =
        {
            "Ozymandias", // <@!146333264871686145> (Ozzatron#0001)
        };
        private static readonly string[] TownDogLabradorNames =
        {
            "Riley", // <@!260875558592708619> (potion pal#9979)
        };
        private static readonly string[] TownDogPitBullNames =
        {
            "Splinter", // <@!320320801213775873> (Kaimonick#1738)
        };
        private static readonly string[] TownDogBeagleNames =
        {
            "Kendra", // <@!237247188005158912> (LordMetarex#6407)
        };
        private static readonly string[] TownDogCorgiNames = null;
        private static readonly string[] TownDogDalmatianNames = null;
        private static readonly string[] TownDogHuskyNames =
        {
            "Yoshi", // <@!541127291426832384> (GregTheSpinarak#6643)
        };

        private const int TownCatSiameseVanillaNames = 12;
        private const int TownCatBlackVanillaNames = 23;
        private const int TownCatOrangeTabbyVanillaNames = 18;
        private const int TownCatRussianBlueVanillaNames = 16;
        private const int TownCatSilverVanillaNames = 17;
        private const int TownCatWhiteVanillaNames = 15;
        private static readonly string[] TownCatNames =
        {
            "Smoogle", // <@!709968379334623274> (smoogle#5672)
            "The Meowurer of Gods", // <@!385949114271268864> (GP#7876)
            "Katsafaros", // <@!190595401328492544> (NavyGuy#2650)
            "Lucerne", // <@!271954788676141066> (lord_lucerne)
            "Milo", // <@!401849201597874179> (maskedmilo)
			"Octo", // <@!796112889353994281> (OctolingGrimm#8888)
        };
        private static readonly string[] TownCatSiameseNames = null;
        private static readonly string[] TownCatBlackNames =
        {
            "Bear", // <@!183424826407518208> (Lilac Vrt Olligoci#5585)
        };
        private static readonly string[] TownCatOrangeTabbyNames =
        {
            "Felix" // <@!183424826407518208> (Lilac Vrt Olligoci#5585)
        };
        private static readonly string[] TownCatRussianBlueNames = null;
        private static readonly string[] TownCatSilverNames = null;
        private static readonly string[] TownCatWhiteNames = null;

        private const int TownBunnyWhiteVanillaNames = 14;
        private const int TownBunnyAngoraVanillaNames = 10;
        private const int TownBunnyDutchVanillaNames = 11;
        private const int TownBunnyFlemishVanillaNames = 12;
        private const int TownBunnyLopVanillaNames = 13;
        private const int TownBunnySilverVanillaNames = 13;
        private static readonly string[] TownBunnyNames = null;
        private static readonly string[] TownBunnyWhiteNames = null;
        private static readonly string[] TownBunnyAngoraNames = null;
        private static readonly string[] TownBunnyDutchNames = null;
        private static readonly string[] TownBunnyFlemishNames = null;
        private static readonly string[] TownBunnyLopNames = null;
        private static readonly string[] TownBunnySilverNames = null;
        #endregion

        #region Town NPC Names
        #region Pets
        public static void ResetTownNPCNameBools()
        {
            void ResetName(int npcID, ref bool nameBool)
            {
                if (NPC.FindFirstNPC(npcID) == -1)
                    nameBool = false;
            }

            ResetName(NPCID.TownCat, ref CalamityWorld.catName);
            ResetName(NPCID.TownDog, ref CalamityWorld.dogName);
            ResetName(NPCID.TownBunny, ref CalamityWorld.bunnyName);
        }
        // Annoyingly, because npc.GivenName is a property, it can't be passed as a ref parameter.
        private string ChooseName(ref bool alreadySet, string currentName, int numVanillaNames, string[] patreonNames, string[] globalNames)
        {
            if (alreadySet || patreonNames is null || patreonNames.Length == 0)
            {
                alreadySet = true;
                return currentName;
            }
            alreadySet = true;
            int index = Main.rand.Next(numVanillaNames + patreonNames.Length + globalNames.Length);

            // If the roll isn't low enough, then a "vanilla name" was picked, meaning we change nothing.
            if (index >= patreonNames.Length + globalNames.Length)
                return currentName;

            // Change the name to be a randomly selected Patreon name if the roll is low enough.
            if (index >= globalNames.Length)
                return patreonNames[index - globalNames.Length];
            return globalNames[index];
        }

        public void SetPatreonTownNPCName(NPC npc, Mod mod)
        {
            if (setNewName)
            {
                setNewName = false;
                switch (npc.type)
                {
                    case NPCID.TownCat:
                        switch (npc.townNpcVariationIndex)
                        {
                            case 0:
                                npc.GivenName = ChooseName(ref CalamityWorld.catName, npc.GivenName, TownCatSiameseVanillaNames, TownCatSiameseNames, TownCatNames);
                                break;
                            case 1:
                                npc.GivenName = ChooseName(ref CalamityWorld.catName, npc.GivenName, TownCatBlackVanillaNames, TownCatBlackNames, TownCatNames);
                                break;
                            case 2:
                                npc.GivenName = ChooseName(ref CalamityWorld.catName, npc.GivenName, TownCatOrangeTabbyVanillaNames, TownCatOrangeTabbyNames, TownCatNames);
                                break;
                            case 3:
                                npc.GivenName = ChooseName(ref CalamityWorld.catName, npc.GivenName, TownCatRussianBlueVanillaNames, TownCatRussianBlueNames, TownCatNames);
                                break;
                            case 4:
                                npc.GivenName = ChooseName(ref CalamityWorld.catName, npc.GivenName, TownCatSilverVanillaNames, TownCatSilverNames, TownCatNames);
                                break;
                            case 5:
                                npc.GivenName = ChooseName(ref CalamityWorld.catName, npc.GivenName, TownCatWhiteVanillaNames, TownCatWhiteNames, TownCatNames);
                                break;
                            default:
                                break;
                        }
                        break;
                    case NPCID.TownDog:
                        switch (npc.townNpcVariationIndex)
                        {
                            case 0:
                                npc.GivenName = ChooseName(ref CalamityWorld.catName, npc.GivenName, TownDogLabradorVanillaNames, TownDogLabradorNames, TownDogNames);
                                break;
                            case 1:
                                npc.GivenName = ChooseName(ref CalamityWorld.catName, npc.GivenName, TownDogPitBullVanillaNames, TownDogPitBullNames, TownDogNames);
                                break;
                            case 2:
                                npc.GivenName = ChooseName(ref CalamityWorld.catName, npc.GivenName, TownDogBeagleVanillaNames, TownDogBeagleNames, TownDogNames);
                                break;
                            case 3:
                                npc.GivenName = ChooseName(ref CalamityWorld.catName, npc.GivenName, TownDogCorgiVanillaNames, TownDogCorgiNames, TownDogNames);
                                break;
                            case 4:
                                npc.GivenName = ChooseName(ref CalamityWorld.catName, npc.GivenName, TownDogDalmatianVanillaNames, TownDogDalmatianNames, TownDogNames);
                                break;
                            case 5:
                                npc.GivenName = ChooseName(ref CalamityWorld.catName, npc.GivenName, TownDogHuskyVanillaNames, TownDogHuskyNames, TownDogNames);
                                break;
                            default:
                                break;
                        }
                        break;
                    case NPCID.TownBunny:
                        switch (npc.townNpcVariationIndex)
                        {
                            case 0:
                                npc.GivenName = ChooseName(ref CalamityWorld.catName, npc.GivenName, TownBunnyWhiteVanillaNames, TownBunnyWhiteNames, TownBunnyNames);
                                break;
                            case 1:
                                npc.GivenName = ChooseName(ref CalamityWorld.catName, npc.GivenName, TownBunnyAngoraVanillaNames, TownBunnyAngoraNames, TownBunnyNames);
                                break;
                            case 2:
                                npc.GivenName = ChooseName(ref CalamityWorld.catName, npc.GivenName, TownBunnyDutchVanillaNames, TownBunnyDutchNames, TownBunnyNames);
                                break;
                            case 3:
                                npc.GivenName = ChooseName(ref CalamityWorld.catName, npc.GivenName, TownBunnyFlemishVanillaNames, TownBunnyFlemishNames, TownBunnyNames);
                                break;
                            case 4:
                                npc.GivenName = ChooseName(ref CalamityWorld.catName, npc.GivenName, TownBunnyLopVanillaNames, TownBunnyLopNames, TownBunnyNames);
                                break;
                            case 5:
                                npc.GivenName = ChooseName(ref CalamityWorld.catName, npc.GivenName, TownBunnySilverVanillaNames, TownBunnySilverNames, TownBunnyNames);
                                break;
                            default:
                                break;
                        }
                        break;
                    default:
                        break;
                }
            }
        }
        #endregion

        private void AddNewNames(List<string> nameList, string[] patreonNames)
        {
            if (patreonNames is null || patreonNames.Length == 0)
            {
                return;
            }
            for (int i = 0; i < patreonNames.Length; i++)
            {
                nameList.Add(patreonNames[i]);
            }
        }

        public override void ModifyNPCNameList(NPC npc, List<string> nameList)
        {
            switch (npc.type)
            {
                case NPCID.Angler:
                    AddNewNames(nameList, AnglerNames);
                    break;
                case NPCID.ArmsDealer:
                    AddNewNames(nameList, ArmsDealerNames);
                    break;
                case NPCID.Clothier:
                    AddNewNames(nameList, ClothierNames);
                    break;
                case NPCID.Cyborg:
                    AddNewNames(nameList, CyborgNames);
                    break;
                case NPCID.Demolitionist:
                    AddNewNames(nameList, DemolitionistNames);
                    break;
                case NPCID.Dryad:
                    AddNewNames(nameList, DryadNames);
                    break;
                case NPCID.DyeTrader:
                    AddNewNames(nameList, DyeTraderNames);
                    break;
                case NPCID.GoblinTinkerer:
                    AddNewNames(nameList, GoblinTinkererNames);
                    break;
                case NPCID.Golfer:
                    AddNewNames(nameList, GolferNames);
                    break;
                case NPCID.Guide:
                    AddNewNames(nameList, GuideNames);
                    break;
                case NPCID.Mechanic:
                    AddNewNames(nameList, MechanicNames);
                    break;
                case NPCID.Merchant:
                    AddNewNames(nameList, MerchantNames);
                    break;
                case NPCID.Nurse:
                    AddNewNames(nameList, NurseNames);
                    break;
                case NPCID.Painter:
                    AddNewNames(nameList, PainterNames);
                    break;
                case NPCID.PartyGirl:
                    AddNewNames(nameList, PartyGirlNames);
                    break;
                case NPCID.Pirate:
                    AddNewNames(nameList, PirateNames);
                    break;
                case NPCID.Princess:
                    AddNewNames(nameList, PrincessNames);
                    break;
                case NPCID.SkeletonMerchant:
                    AddNewNames(nameList, SkeletonMerchantNames);
                    break;
                case NPCID.Steampunker:
                    AddNewNames(nameList, SteampunkerNames);
                    break;
                case NPCID.Stylist:
                    AddNewNames(nameList, StylistNames);
                    break;
                case NPCID.DD2Bartender: // Tavernkeep
                    AddNewNames(nameList, TavernkeepNames);
                    break;
                case NPCID.TaxCollector:
                    AddNewNames(nameList, TaxCollectorNames);
                    break;
                case NPCID.TravellingMerchant:
                    AddNewNames(nameList, TravelingMerchantNames);
                    break;
                case NPCID.Truffle:
                    AddNewNames(nameList, TruffleNames);
                    break;
                case NPCID.WitchDoctor:
                    AddNewNames(nameList, WitchDoctorNames);
                    break;
                case NPCID.Wizard:
                    AddNewNames(nameList, WizardNames);
                    break;
                case NPCID.BestiaryGirl: // Zoologist
                    AddNewNames(nameList, ZoologistNames);
                    break;

                // This function doesn't work with Town Pets currently
                /*case NPCID.TownCat:
                    AddNewNames(nameList, TownCatNames);
                    switch (npc.townNpcVariationIndex)
                    {
                        case 0:
                            AddNewNames(nameList, TownCatSiameseNames);
                            break;
                        case 1:
                            AddNewNames(nameList, TownCatBlackNames);
                            break;
                        case 2:
                            AddNewNames(nameList, TownCatOrangeTabbyNames);
                            break;
                        case 3:
                            AddNewNames(nameList, TownCatRussianBlueNames);
                            break;
                        case 4:
                            AddNewNames(nameList, TownCatSilverNames);
                            break;
                        case 5:
                            AddNewNames(nameList, TownCatWhiteNames);
                            break;
                        default:
                            break;
                    }
                    break;
                case NPCID.TownDog:
                    AddNewNames(nameList, TownDogNames);
                    switch (npc.townNpcVariationIndex)
                    {
                        case 0:
                            AddNewNames(nameList, TownDogLabradorNames);
                            break;
                        case 1:
                            AddNewNames(nameList, TownDogPitBullNames);
                            break;
                        case 2:
                            AddNewNames(nameList, TownDogBeagleNames);
                            break;
                        case 3:
                            AddNewNames(nameList, TownDogCorgiNames);
                            break;
                        case 4:
                            AddNewNames(nameList, TownDogDalmatianNames);
                            break;
                        case 5:
                            AddNewNames(nameList, TownDogHuskyNames);
                            break;
                        default:
                            break;
                    }
                    break;
                case NPCID.TownBunny:
                    AddNewNames(nameList, TownBunnyNames);
                    switch (npc.townNpcVariationIndex)
                    {
                        case 0:
                            AddNewNames(nameList, TownBunnyWhiteNames);
                            break;
                        case 1:
                            AddNewNames(nameList, TownBunnyAngoraNames);
                            break;
                        case 2:
                            AddNewNames(nameList, TownBunnyDutchNames);
                            break;
                        case 3:
                            AddNewNames(nameList, TownBunnyFlemishNames);
                            break;
                        case 4:
                            AddNewNames(nameList, TownBunnyLopNames);
                            break;
                        case 5:
                            AddNewNames(nameList, TownBunnySilverNames);
                            break;
                        default:
                            break;
                    }
                    break;*/

                default:
                    break;
            }
        }
        #endregion

        #region NPC New Shop Alert
        public void TownNPCAlertSystem(NPC npc, Mod mod, SpriteBatch spriteBatch)
        {
            if (CalamityConfig.Instance.ShopNewAlert && npc.townNPC)
            {
                if (npc.type == NPCType<DILF>() && Main.LocalPlayer.Calamity().newPermafrostInventory)
                {
                    DrawNewInventoryAlert(npc);
                }
                else if (npc.type == NPCType<FAP>() && Main.LocalPlayer.Calamity().newCirrusInventory)
                {
                    DrawNewInventoryAlert(npc);
                }
                else if (npc.type == NPCType<SEAHOE>() && Main.LocalPlayer.Calamity().newAmidiasInventory)
                {
                    DrawNewInventoryAlert(npc);
                }
                else if (npc.type == NPCType<THIEF>() && Main.LocalPlayer.Calamity().newBanditInventory)
                {
                    DrawNewInventoryAlert(npc);
                }
                else
                {
                    switch (npc.type)
                    {
                        case NPCID.Merchant:
                            if (Main.LocalPlayer.Calamity().newMerchantInventory)
                                DrawNewInventoryAlert(npc);
                            break;
                        case NPCID.Painter:
                            if (Main.LocalPlayer.Calamity().newPainterInventory)
                                DrawNewInventoryAlert(npc);
                            break;
                        case NPCID.Golfer:
                            if (Main.LocalPlayer.Calamity().newGolferInventory)
                                DrawNewInventoryAlert(npc);
                            break;
                        case NPCID.BestiaryGirl:
                            if (Main.LocalPlayer.Calamity().newZoologistInventory)
                                DrawNewInventoryAlert(npc);
                            break;
                        case NPCID.DyeTrader:
                            if (Main.LocalPlayer.Calamity().newDyeTraderInventory)
                                DrawNewInventoryAlert(npc);
                            break;
                        case NPCID.PartyGirl:
                            if (Main.LocalPlayer.Calamity().newPartyGirlInventory)
                                DrawNewInventoryAlert(npc);
                            break;
                        case NPCID.Stylist:
                            if (Main.LocalPlayer.Calamity().newStylistInventory)
                                DrawNewInventoryAlert(npc);
                            break;
                        case NPCID.Demolitionist:
                            if (Main.LocalPlayer.Calamity().newDemolitionistInventory)
                                DrawNewInventoryAlert(npc);
                            break;
                        case NPCID.Dryad:
                            if (Main.LocalPlayer.Calamity().newDryadInventory)
                                DrawNewInventoryAlert(npc);
                            break;
                        case NPCID.DD2Bartender:
                            if (Main.LocalPlayer.Calamity().newTavernkeepInventory)
                                DrawNewInventoryAlert(npc);
                            break;
                        case NPCID.ArmsDealer:
                            if (Main.LocalPlayer.Calamity().newArmsDealerInventory)
                                DrawNewInventoryAlert(npc);
                            break;
                        case NPCID.GoblinTinkerer:
                            if (Main.LocalPlayer.Calamity().newGoblinTinkererInventory)
                                DrawNewInventoryAlert(npc);
                            break;
                        case NPCID.WitchDoctor:
                            if (Main.LocalPlayer.Calamity().newWitchDoctorInventory)
                                DrawNewInventoryAlert(npc);
                            break;
                        case NPCID.Clothier:
                            if (Main.LocalPlayer.Calamity().newClothierInventory)
                                DrawNewInventoryAlert(npc);
                            break;
                        case NPCID.Mechanic:
                            if (Main.LocalPlayer.Calamity().newMechanicInventory)
                                DrawNewInventoryAlert(npc);
                            break;
                        case NPCID.Pirate:
                            if (Main.LocalPlayer.Calamity().newPirateInventory)
                                DrawNewInventoryAlert(npc);
                            break;
                        case NPCID.Truffle:
                            if (Main.LocalPlayer.Calamity().newTruffleInventory)
                                DrawNewInventoryAlert(npc);
                            break;
                        case NPCID.Wizard:
                            if (Main.LocalPlayer.Calamity().newWizardInventory)
                                DrawNewInventoryAlert(npc);
                            break;
                        case NPCID.Steampunker:
                            if (Main.LocalPlayer.Calamity().newSteampunkerInventory)
                                DrawNewInventoryAlert(npc);
                            break;
                        case NPCID.Cyborg:
                            if (Main.LocalPlayer.Calamity().newCyborgInventory)
                                DrawNewInventoryAlert(npc);
                            break;
                        case NPCID.Princess:
                            if (Main.LocalPlayer.Calamity().newPrincessInventory)
                                DrawNewInventoryAlert(npc);
                            break;
                        case NPCID.SkeletonMerchant:
                            if (Main.LocalPlayer.Calamity().newSkeletonMerchantInventory)
                                DrawNewInventoryAlert(npc);
                            break;
                        default:
                            break;
                    }
                }

                void DrawNewInventoryAlert(NPC npc2)
                {
                    // The position where the display is drawn
                    Vector2 drawPos = npc2.Center - Main.screenPosition;

                    // The height of a single frame of the npc
                    float npcHeight = (float)(TextureAssets.Npc[npc2.type].Value.Height / Main.npcFrameCount[npc2.type] / 2) * npc2.scale;

                    // Offset the debuff display based on the npc's graphical offset, and 16 units, to create some space between the sprite and the display
                    float drawPosY = npcHeight + npc.gfxOffY + 36f;

                    // Texture animation variables
                    Texture2D texture = Request<Texture2D>("CalamityMod/UI/MiscTextures/NPCAlertDisplay").Value;
                    shopAlertAnimTimer++;
                    if (shopAlertAnimTimer >= 6)
                    {
                        shopAlertAnimTimer = 0;

                        shopAlertAnimFrame++;
                        if (shopAlertAnimFrame > 4)
                            shopAlertAnimFrame = 0;
                    }
                    int frameHeight = texture.Height / 5;
                    Rectangle animRect = new Rectangle(0, frameHeight * shopAlertAnimFrame, texture.Width, frameHeight);

                    spriteBatch.Draw(texture, drawPos - new Vector2(5f, drawPosY), animRect, Color.White, 0f, default, 1f, SpriteEffects.None, 0f);
                }
            }
        }

        public override void OnChatButtonClicked(NPC npc, bool firstButton)
        {
            if (npc.townNPC)
            {
                switch (npc.type)
                {
                    case NPCID.Merchant:
                        Main.LocalPlayer.Calamity().newMerchantInventory = false;
                        break;
                    case NPCID.Painter:
                        Main.LocalPlayer.Calamity().newPainterInventory = false;
                        break;
                    case NPCID.Golfer:
                        Main.LocalPlayer.Calamity().newGolferInventory = false;
                        break;
                    case NPCID.BestiaryGirl:
                        Main.LocalPlayer.Calamity().newZoologistInventory = false;
                        break;
                    case NPCID.DyeTrader:
                        Main.LocalPlayer.Calamity().newDyeTraderInventory = false;
                        break;
                    case NPCID.PartyGirl:
                        Main.LocalPlayer.Calamity().newPartyGirlInventory = false;
                        break;
                    case NPCID.Stylist:
                        Main.LocalPlayer.Calamity().newStylistInventory = false;
                        break;
                    case NPCID.Demolitionist:
                        Main.LocalPlayer.Calamity().newDemolitionistInventory = false;
                        break;
                    case NPCID.Dryad:
                        Main.LocalPlayer.Calamity().newDryadInventory = false;
                        break;
                    case NPCID.DD2Bartender:
                        Main.LocalPlayer.Calamity().newTavernkeepInventory = false;
                        break;
                    case NPCID.ArmsDealer:
                        Main.LocalPlayer.Calamity().newArmsDealerInventory = false;
                        break;
                    case NPCID.GoblinTinkerer:
                        Main.LocalPlayer.Calamity().newGoblinTinkererInventory = false;
                        break;
                    case NPCID.WitchDoctor:
                        Main.LocalPlayer.Calamity().newWitchDoctorInventory = false;
                        break;
                    case NPCID.Clothier:
                        Main.LocalPlayer.Calamity().newClothierInventory = false;
                        break;
                    case NPCID.Mechanic:
                        Main.LocalPlayer.Calamity().newMechanicInventory = false;
                        break;
                    case NPCID.Pirate:
                        Main.LocalPlayer.Calamity().newPirateInventory = false;
                        break;
                    case NPCID.Truffle:
                        Main.LocalPlayer.Calamity().newTruffleInventory = false;
                        break;
                    case NPCID.Wizard:
                        Main.LocalPlayer.Calamity().newWizardInventory = false;
                        break;
                    case NPCID.Steampunker:
                        Main.LocalPlayer.Calamity().newSteampunkerInventory = false;
                        break;
                    case NPCID.Cyborg:
                        Main.LocalPlayer.Calamity().newCyborgInventory = false;
                        break;
                    case NPCID.Princess:
                        Main.LocalPlayer.Calamity().newPrincessInventory = false;
                        break;
                    case NPCID.SkeletonMerchant:
                        Main.LocalPlayer.Calamity().newSkeletonMerchantInventory = false;
                        break;
                    default:
                        break;
                }
            }
        }

        public static void SetNewShopVariable(int[] types, bool alreadySet)
        {
            if (!alreadySet)
            {
                for (int i = 0; i < types.Length; i++)
                {
                    if (types[i] == NPCType<DILF>())
                    {
                        Main.LocalPlayer.Calamity().newPermafrostInventory = true;
                    }
                    else if (types[i] == NPCType<FAP>())
                    {
                        Main.LocalPlayer.Calamity().newCirrusInventory = true;
                    }
                    else if (types[i] == NPCType<SEAHOE>())
                    {
                        Main.LocalPlayer.Calamity().newAmidiasInventory = true;
                    }
                    else if (types[i] == NPCType<THIEF>())
                    {
                        Main.LocalPlayer.Calamity().newBanditInventory = true;
                    }
                    else
                    {
                        switch (types[i])
                        {
                            case NPCID.Merchant:
                                Main.LocalPlayer.Calamity().newMerchantInventory = true;
                                break;
                            case NPCID.Painter:
                                Main.LocalPlayer.Calamity().newPainterInventory = true;
                                break;
                            case NPCID.Golfer:
                                Main.LocalPlayer.Calamity().newGolferInventory = true;
                                break;
                            case NPCID.BestiaryGirl:
                                Main.LocalPlayer.Calamity().newZoologistInventory = true;
                                break;
                            case NPCID.DyeTrader:
                                Main.LocalPlayer.Calamity().newDyeTraderInventory = true;
                                break;
                            case NPCID.PartyGirl:
                                Main.LocalPlayer.Calamity().newPartyGirlInventory = true;
                                break;
                            case NPCID.Stylist:
                                Main.LocalPlayer.Calamity().newStylistInventory = true;
                                break;
                            case NPCID.Demolitionist:
                                Main.LocalPlayer.Calamity().newDemolitionistInventory = true;
                                break;
                            case NPCID.Dryad:
                                Main.LocalPlayer.Calamity().newDryadInventory = true;
                                break;
                            case NPCID.DD2Bartender:
                                Main.LocalPlayer.Calamity().newTavernkeepInventory = true;
                                break;
                            case NPCID.ArmsDealer:
                                Main.LocalPlayer.Calamity().newArmsDealerInventory = true;
                                break;
                            case NPCID.GoblinTinkerer:
                                Main.LocalPlayer.Calamity().newGoblinTinkererInventory = true;
                                break;
                            case NPCID.WitchDoctor:
                                Main.LocalPlayer.Calamity().newWitchDoctorInventory = true;
                                break;
                            case NPCID.Clothier:
                                Main.LocalPlayer.Calamity().newClothierInventory = true;
                                break;
                            case NPCID.Mechanic:
                                Main.LocalPlayer.Calamity().newMechanicInventory = true;
                                break;
                            case NPCID.Pirate:
                                Main.LocalPlayer.Calamity().newPirateInventory = true;
                                break;
                            case NPCID.Truffle:
                                Main.LocalPlayer.Calamity().newTruffleInventory = true;
                                break;
                            case NPCID.Wizard:
                                Main.LocalPlayer.Calamity().newWizardInventory = true;
                                break;
                            case NPCID.Steampunker:
                                Main.LocalPlayer.Calamity().newSteampunkerInventory = true;
                                break;
                            case NPCID.Cyborg:
                                Main.LocalPlayer.Calamity().newCyborgInventory = true;
                                break;
                            case NPCID.Princess:
                                Main.LocalPlayer.Calamity().newPrincessInventory = true;
                                break;
                            case NPCID.SkeletonMerchant:
                                Main.LocalPlayer.Calamity().newSkeletonMerchantInventory = true;
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }
        #endregion

        #region NPC Chat
        public override void GetChat(NPC npc, ref string chat)
        {
            int fapsol = NPC.FindFirstNPC(NPCType<FAP>());
            int permadong = NPC.FindFirstNPC(NPCType<DILF>());
            int seahorse = NPC.FindFirstNPC(NPCType<SEAHOE>());
            int thief = NPC.FindFirstNPC(NPCType<THIEF>());
            int angelstatue = NPC.FindFirstNPC(NPCID.Merchant);

            switch (npc.type)
            {
                case NPCID.Angler:
                    if (Main.rand.NextBool(5) && seahorse != -1)
                        chat = CalamityUtils.GetText("Vanilla.AnglerChat.SeaKing").Format(Main.npc[seahorse].GivenName);
                    break;

                case NPCID.ArmsDealer:
                    // If you've beaten Skeletron and don't have Quad-Barrel Shotgun, drop a hint
                    // This is rarer in hardmode since the weapon is irrelevant by then
                    if (Main.rand.NextBool(Main.hardMode ? 20 : 4) && NPC.downedBoss3 && !Main.LocalPlayer.InventoryHas(ItemID.QuadBarrelShotgun) && !Main.LocalPlayer.ZoneGraveyard)
                        chat = CalamityUtils.GetTextValue("Vanilla.ArmsDealerChat.MentionQuadBarrel");
                    else if (Main.rand.NextBool(5) && Main.LocalPlayer.InventoryHas(ItemID.QuadBarrelShotgun))
                        chat = CalamityUtils.GetTextValue("Vanilla.ArmsDealerChat.HasQuadBarrel");
                    else if (Main.rand.NextBool(10) && DownedBossSystem.downedDoG)
                        chat = CalamityUtils.GetTextValue("Vanilla.ArmsDealerChat.DoGDefeated");
                    else if (Main.rand.NextBool(5) && Main.eclipse)
                        chat = CalamityUtils.GetTextValue("Vanilla.ArmsDealerChat.Eclipse");
                    break;

                case NPCID.Clothier:
                    if (Main.rand.NextBool(10) && DownedBossSystem.downedPolterghast)
                        chat = CalamityUtils.GetTextValue("Vanilla.ClothierChat.PolterghastDefeated");
                    if (Main.rand.NextBool(5) && NPC.downedMoonlord)
                        chat = CalamityUtils.GetTextValue("Vanilla.ClothierChat.MoonLordDefeated" + Main.rand.Next(1, 3 + 1));
                    if (Main.rand.NextBool(5) && NPC.AnyNPCs(NPCID.MoonLordCore))
                        chat = CalamityUtils.GetTextValue("Vanilla.ClothierChat.MoonLordPresent");
                    break;

                case NPCID.Cyborg:
                    if (Main.rand.NextBool(5) && NPC.downedMoonlord)
                        chat = CalamityUtils.GetTextValue("Vanilla.CyborgChat.MoonLordDefeated" + Main.rand.Next(1, 2 + 1));
                    else if (Main.rand.NextBool(10) && !DownedBossSystem.downedPlaguebringer && NPC.downedGolemBoss)
                        chat = CalamityUtils.GetTextValue("Vanilla.CyborgChat.MentionPlague");
                    else if (Main.rand.NextBool(10) && Main.raining)
                        chat = CalamityUtils.GetTextValue("Vanilla.CyborgChat.Rain");
                    break;

                case NPCID.Demolitionist:
                    if (Main.rand.NextBool(5) && DownedBossSystem.downedDoG)
                        chat = CalamityUtils.GetTextValue("Vanilla.DemolitionistChat.DoGDefeated");
                    break;

                case NPCID.Dryad:
                    if (Main.rand.NextBool(5) && DownedBossSystem.downedDoG && Main.eclipse)
                        chat = CalamityUtils.GetTextValue("Vanilla.DryadChat.DarksunEclipse");
                    else if (Main.rand.NextBool(5) && Main.LocalPlayer.ZoneGlowshroom)
                    {
                        if (Main.rand.NextBool(2) && fapsol != -1)
                            chat = CalamityUtils.GetText("Vanilla.DryadChat.DrunkPrincessShroom").Format(Main.npc[fapsol].GivenName);
                        else
                            chat = CalamityUtils.GetTextValue("Vanilla.DryadChat.Mushroom");
                    }
                    else if (Main.rand.NextBool(5) && Main.LocalPlayer.Calamity().ZoneSulphur)
                        chat = CalamityUtils.GetTextValue("Vanilla.DryadChat.SulphurSea");
                    else if (Main.rand.NextBool(5) && Main.hardMode)
                        chat = CalamityUtils.GetTextValue("Vanilla.DryadChat.Hardmode");
                    break;

                case NPCID.DyeTrader:
                    if (Main.rand.NextBool(5) && permadong != -1)
                        chat = CalamityUtils.GetText("Vanilla.PainterChat.Archmage").Format(Main.npc[permadong].GivenName);
                    else if (Main.rand.NextBool(5))
                        chat = CalamityUtils.GetTextValue("Vanilla.DyeTraderChat.Normal");
                    break;

                case NPCID.GoblinTinkerer:
                    if (Main.rand.NextBool(10) && NPC.downedMoonlord)
                        chat = CalamityUtils.GetTextValue("Vanilla.GoblinTinkererChat.MoonLordDefeated");
                    else if (Main.rand.NextBool(3) && thief != -1 && CalamityWorld.Reforges >= 1)
                        chat = CalamityUtils.GetText("Vanilla.GoblinTinkererChat.Bandit").Format(Main.npc[thief].GivenName);
                    break;

                case NPCID.Guide:
                    if (Main.rand.NextBool(10) && DownedBossSystem.downedProvidence)
                        chat = CalamityUtils.GetTextValue("Vanilla.GuideChat.ProvidenceDefeated" + Main.rand.Next(1, 2 + 1));
                    else if (Main.rand.NextBool(20) && NPC.downedMoonlord)
                        chat = CalamityUtils.GetTextValue("Vanilla.GuideChat.MoonLordDefeated");
                    else if (Main.rand.NextBool(10) && Main.hardMode)
                        chat = CalamityUtils.GetTextValue("Vanilla.GuideChat.Hardmode" + Main.rand.Next(1, 2 + 1));
                    break;

                case NPCID.Mechanic:
                    if (Main.rand.NextBool(5) && NPC.downedMoonlord)
                        chat = CalamityUtils.GetTextValue("Vanilla.MechanicChat.MoonLordDefeated");
                    else if (Main.rand.NextBool(5) && Main.eclipse)
                        chat = CalamityUtils.GetTextValue("Vanilla.MechanicChat.Eclipse");
                    else if (Main.rand.NextBool(5) && AcidRainEvent.AcidRainEventIsOngoing)
                        chat = CalamityUtils.GetTextValue("Vanilla.MechanicChat.AcidRain");
                    else if (Main.rand.NextBool(5) && fapsol != -1)
                        chat = CalamityUtils.GetText("Vanilla.MechanicChat.DrunkPrincess").Format(Main.npc[fapsol].GivenName);
                    break;

                case NPCID.Merchant:
                    if (Main.rand.NextBool(5) && NPC.downedMoonlord)
                        chat = CalamityUtils.GetTextValue("Vanilla.MerchantChat.MoonLordDefeated");
                    else if (Main.rand.NextBool(5) && Main.eclipse)
                        chat = CalamityUtils.GetTextValue("Vanilla.MerchantChat.Eclipse");
                    else if (Main.rand.NextBool(5) && AcidRainEvent.AcidRainEventIsOngoing)
                        chat = CalamityUtils.GetTextValue("Vanilla.MerchantChat.AcidRain");
                    else if (Main.rand.NextBool(7) && thief != -1)
                        chat = CalamityUtils.GetTextValue("Vanilla.MerchantChat.Bandit");
                    break;

                case NPCID.Painter:
                    if (Main.rand.NextBool(4) && Main.LocalPlayer.ZoneCorrupt)
                        chat = CalamityUtils.GetTextValue("Vanilla.PainterChat.Corruption");
                    if (Main.rand.NextBool(4) && Main.LocalPlayer.ZoneCrimson)
                        chat = CalamityUtils.GetTextValue("Vanilla.PainterChat.Crimson");
                    if (Main.rand.NextBool(4) && Main.LocalPlayer.ZoneSnow)
                    {
                        if (Main.rand.NextBool(2) && permadong != -1)
                            chat = CalamityUtils.GetText("Vanilla.PainterChat.Archmage").Format(Main.npc[permadong].GivenName);
                        else
                            chat = CalamityUtils.GetTextValue("Vanilla.PainterChat.Tundra");
                    }
                    if (Main.rand.NextBool(4) && Main.LocalPlayer.ZoneDesert)
                        chat = CalamityUtils.GetTextValue("Vanilla.PainterChat.Desert");
                    if (Main.rand.NextBool(4) && Main.LocalPlayer.ZoneHallow)
                        chat = CalamityUtils.GetTextValue("Vanilla.PainterChat.Hallow");
                    if (Main.rand.NextBool(4) && Main.LocalPlayer.ZoneSkyHeight)
                        chat = CalamityUtils.GetTextValue("Vanilla.PainterChat.Space");
                    if (Main.rand.NextBool(4) && Main.LocalPlayer.ZoneJungle)
                        chat = CalamityUtils.GetTextValue("Vanilla.PainterChat.Jungle");
                    if (Main.rand.NextBool(4) && Main.LocalPlayer.Calamity().ZoneAstral)
                        chat = CalamityUtils.GetTextValue("Vanilla.PainterChat.Astral");
                    if (Main.rand.NextBool(4) && Main.LocalPlayer.ZoneUnderworldHeight)
                        chat = CalamityUtils.GetTextValue("Vanilla.PainterChat.Underworld" + Main.rand.Next(1, 2 + 1));
                    if (Main.rand.NextBool(4) && Main.LocalPlayer.Calamity().ZoneCalamity)
                        chat = CalamityUtils.GetTextValue("Vanilla.PainterChat.Crags");
                    if (Main.rand.NextBool(4) && Main.LocalPlayer.Calamity().ZoneSulphur)
                        chat = CalamityUtils.GetTextValue("Vanilla.PainterChat.SulphurSea");
                    if (Main.rand.NextBool(4) && Main.LocalPlayer.Calamity().ZoneAbyss)
                        chat = CalamityUtils.GetTextValue("Vanilla.PainterChat.Abyss");
                    break;

                case NPCID.PartyGirl:
                    if (Main.rand.NextBool(4) && Main.eclipse)
                        chat = CalamityUtils.GetTextValue("Vanilla.PartyGirlChat.Eclipse" + Main.rand.Next(1, 2 + 1));
                    else if (Main.rand.NextBool(10) && fapsol != -1)
                        chat = CalamityUtils.GetText("Vanilla.PartyGirlChat.DrunkPrincess").Format(Main.npc[fapsol].GivenName);
                    break;

                case NPCID.Pirate:
                    if (Main.rand.NextBool(5) && !DownedBossSystem.downedLeviathan)
                        chat = CalamityUtils.GetTextValue("Vanilla.PirateChat.PreLeviathan");
                    else if (Main.rand.NextBool(5) && DownedBossSystem.downedAquaticScourge)
                        chat = CalamityUtils.GetTextValue("Vanilla.PirateChat.WetScourgeDefeated");
                    else if (Main.rand.NextBool(5) && fapsol != -1)
                        chat = CalamityUtils.GetTextValue("Vanilla.PirateChat.DrunkPrincess");
                    else if (Main.rand.NextBool(5) && seahorse != -1)
                        chat = CalamityUtils.GetText("Vanilla.PirateChat.SeaKing").Format(Main.npc[seahorse].GivenName);
                    else if (Main.rand.NextBool(5) && Main.LocalPlayer.Center.ToTileCoordinates().X < 380 && !Main.LocalPlayer.Calamity().ZoneSulphur)
                        chat = CalamityUtils.GetTextValue("Vanilla.PirateChat.Ocean");
                    else if (Main.rand.NextBool(5) && Main.LocalPlayer.Calamity().ZoneSulphur)
                        chat = CalamityUtils.GetTextValue("Vanilla.PirateChat.SulphurSea" + Main.rand.Next(1, 2 + 1));
                    break;

                case NPCID.SkeletonMerchant:
                    if (Main.rand.NextBool(5))
                        chat = CalamityUtils.GetTextValue("Vanilla.SkeletonMerchantChat.Normal");
                    break;

                case NPCID.Steampunker:
                    if (Main.rand.NextBool(5) && Main.LocalPlayer.InventoryHas(ItemID.PortalGun))
                        chat = CalamityUtils.GetTextValue("Vanilla.SteampunkerChat.HasPortalGun");
                    else if (Main.rand.NextBool(5) && NPC.downedMoonlord)
                        chat = CalamityUtils.GetTextValue("Vanilla.SteampunkerChat.MoonLordDefeated");
                    else if (Main.rand.NextBool(5) && Main.LocalPlayer.Calamity().ZoneAstral)
                        chat = CalamityUtils.GetTextValue("Vanilla.SteampunkerChat.Astral");
                    else if (Main.rand.NextBool(5) && Main.LocalPlayer.ZoneHallow)
                        chat = CalamityUtils.GetTextValue("Vanilla.SteampunkerChat.Hallow");
                    break;

                case NPCID.Stylist:
                    string worldEvil = Language.GetTextValue("LegacyMisc." + (WorldGen.crimson ? 102 : 101));
                    if (Main.rand.NextBool(15) && Main.hardMode)
                        chat = CalamityUtils.GetText("Vanilla.StylistChat.Hardmode").Format(worldEvil);
                    if (Main.rand.NextBool(15) && fapsol != -1)
                        chat = CalamityUtils.GetText("Vanilla.StylistChat.DrunkPrincess" + (ChildSafety.Disabled ? Main.rand.Next(1, 2 + 1) : 1)).Format(Main.npc[fapsol].GivenName);
                    if ((Main.rand.NextBool(npc.GivenName == "Amber" ? 10 : 15)) && Main.LocalPlayer.Calamity().pArtifact)
                    {
                        if (Main.LocalPlayer.Calamity().profanedCrystalBuffs)
                            chat = CalamityUtils.GetTextValue("Vanilla.StylistChat.ProfanedSoulCrystal" + Main.rand.Next(1, 2 + 1));
                        else if (Main.LocalPlayer.Calamity().donutBabs)
                            chat = CalamityUtils.GetTextValue("Vanilla.StylistChat.ProfanedDonuts");
                    }
                    break;

                case NPCID.DD2Bartender:
                    if (Main.rand.NextBool(5) && !Main.dayTime && Main.moonPhase == 0)
                        chat = CalamityUtils.GetTextValue("Vanilla.TavernkeepChat.FullMoon");
                    else if (Main.rand.NextBool(10) && fapsol != -1)
                        chat = CalamityUtils.GetText("Vanilla.TavernkeepChat.DrunkPrincess").Format(Main.npc[fapsol].GivenName);
                    break;

                case NPCID.TaxCollector:
                    int platinumCoins = 0;
                    Player player = Main.LocalPlayer;
                    if (player.active)
                    {
                        for (int j = 0; j < player.inventory.Length; j++)
                        {
                            if (player.inventory[j].type == ItemID.PlatinumCoin)
                            {
                                platinumCoins += player.inventory[j].stack;
                            }
                        }
                    }

                    if (Main.rand.NextBool(10) && DownedBossSystem.downedDoG)
                        chat = CalamityUtils.GetTextValue("Vanilla.TaxCollectorChat.DoGDefeated");
                    else if (Main.rand.NextBool(5) && !DownedBossSystem.downedBrimstoneElemental)
                        chat = CalamityUtils.GetTextValue("Vanilla.TaxCollectorChat.PreBrimmy");
                    else if (Main.rand.NextBool(10) && CalamityUtils.InventoryHas(Main.LocalPlayer, ItemType<SlickCane>()))
                        chat = CalamityUtils.GetTextValue("Vanilla.TaxCollectorChat.HasSlickCane");
                    else if (Main.rand.NextBool(5) && platinumCoins >= 500)
                        chat = CalamityUtils.GetTextValue("Vanilla.TaxCollectorChat.Has500Plat");
                    else if (Main.rand.NextBool(5) && platinumCoins >= 100)
                        chat = CalamityUtils.GetTextValue("Vanilla.TaxCollectorChat.Has100Plat");
                    break;

                case NPCID.TravellingMerchant:
                    if (Main.rand.NextBool(5) && fapsol != -1 && angelstatue != -1)
                        chat = CalamityUtils.GetText("Vanilla.TravellingMerchantChat.DrunkPrincessMerchant").Format(Main.npc[fapsol].GivenName, Main.npc[angelstatue].GivenName);
                    break;

                case NPCID.Truffle:
                    if (Main.rand.NextBool(4) && fapsol != -1)
                        chat = CalamityUtils.GetText("Vanilla.TruffleChat.DrunkPrincess").Format(Main.npc[fapsol].GivenName);
                    else if (Main.rand.NextBool(8))
                        chat = CalamityUtils.GetTextValue("Vanilla.TruffleChat.Normal");
                    break;

                case NPCID.WitchDoctor:
                    if (Main.rand.NextBool(8) && Main.bloodMoon)
                        chat = CalamityUtils.GetTextValue("Vanilla.WitchDoctorChat.BloodMoon");
                    else if (Main.rand.NextBool(8) && Main.hardMode && !NPC.downedPlantBoss)
                        chat = CalamityUtils.GetTextValue("Vanilla.WitchDoctorChat.PrePlantera");
                    else if (Main.rand.NextBool(8) && Main.LocalPlayer.ZoneJungle)
                        chat = CalamityUtils.GetTextValue("Vanilla.WitchDoctorChat.Jungle");
                    break;

                case NPCID.Wizard:
                    if (Main.rand.NextBool(6) && !Main.LocalPlayer.InventoryHas(ItemID.RodofDiscord) && !Main.LocalPlayer.InventoryHas(ModContent.ItemType<NormalityRelocator>()) && !Main.LocalPlayer.ZoneHallow)
                        chat = CalamityUtils.GetTextValue("Vanilla.WizardChat.MentionRoD");
                    if (Main.rand.NextBool(10) && Main.hardMode)
                        chat = CalamityUtils.GetTextValue("Vanilla.WizardChat.Hardmode");
                    break;

                default:
                    break;
            }
        }
        #endregion

        #region NPC Stat Changes
        public void BoundNPCSafety(Mod mod, NPC npc)
        {
            // Make Bound Town NPCs take no damage
            if (CalamityLists.BoundNPCIDs.Contains(npc.type))
            {
                npc.dontTakeDamageFromHostiles = true;
            }
        }

        public void MakeTownNPCsTakeMoreDamage(NPC npc, Projectile projectile, Mod mod, ref NPC.HitModifiers modifiers)
        {
            if (npc.townNPC && projectile.hostile)
                modifiers.SourceDamage *= 2f;
        }

        public override void BuffTownNPC(ref float damageMult, ref int defense)
        {
            if (NPC.downedMoonlord)
            {
                damageMult += 0.6f;
                defense += 20;
            }
            if (DownedBossSystem.downedProvidence)
            {
                damageMult += 0.2f;
                defense += 12;
            }
            if (DownedBossSystem.downedPolterghast)
            {
                damageMult += 0.2f;
                defense += 12;
            }
            if (DownedBossSystem.downedDoG)
            {
                damageMult += 0.2f;
                defense += 12;
            }
            if (DownedBossSystem.downedYharon)
            {
                damageMult += 0.2f;
                defense += 12;
            }
            if (DownedBossSystem.downedExoMechs)
            {
                damageMult += 0.6f;
                defense += 20;
            }
            if (DownedBossSystem.downedCalamitas)
            {
                damageMult += 0.6f;
                defense += 20;
            }
        }
        #endregion

        #region Shop Stuff
         public override void ModifyShop(NPCShop shop)
        {
            int type = shop.NpcType;
            int goldCost = NPC.downedMoonlord ? 16 : Main.hardMode ? 8 : 4;

            bool happy = Main.LocalPlayer.currentShoppingSettings.PriceAdjustment <= 0.9;

            Condition potionSells = new(CalamityUtils.GetText("Condition.PotionConfig"), () => CalamityConfig.Instance.PotionSelling);
            Condition hasFlareGunUpgrade = new(CalamityUtils.GetText("Condition.HasFlareGun"), () => (Main.LocalPlayer.HasItem(ItemType<FirestormCannon>()) || Main.LocalPlayer.HasItem(ItemType<SpectralstormCannon>())) && !Main.LocalPlayer.HasItem(ItemID.FlareGun));
            Condition roguePlayer = new(CalamityUtils.GetText("Condition.HasRogueArmor"), () => Main.LocalPlayer.Calamity().rogueStealthMax > 0f && Main.LocalPlayer.Calamity().wearingRogueArmor);
            Condition wingedPlayer = new(CalamityUtils.GetText("Condition.HasWings"), () => Main.LocalPlayer.wingTimeMax > 0);
            Condition revengeance = new(CalamityUtils.GetText("Condition.InRev"), () => CalamityWorld.revenge);
            Condition drunk = new(CalamityUtils.GetText("Condition.AlcoholPoison"), () => Main.LocalPlayer.Calamity().alcoholPoisoning);
            Condition downedPolterghast = new(CalamityUtils.GetText("Condition.PostPolter"), () => DownedBossSystem.downedPolterghast);
            Condition downedDoG = new(CalamityUtils.GetText("Condition.PostDoG"), () => DownedBossSystem.downedDoG);

            if (type == NPCID.Merchant)
            {
                shop.AddWithCustomValue(ItemID.Bottle, Item.buyPrice(copper: 20), potionSells, Condition.HappyEnough)
                .AddWithCustomValue(ItemID.WormholePotion, Item.buyPrice(copper: 25), potionSells, Condition.HappyEnough);
                shop.Add(ItemID.HealingPotion, potionSells, Condition.HappyEnough, Condition.DownedEowOrBoc)
                .Add(ItemID.ManaPotion, potionSells, Condition.HappyEnough, Condition.DownedEowOrBoc)
                .Add(ItemID.Flare, hasFlareGunUpgrade)
                .Add(ItemID.BlueFlare, hasFlareGunUpgrade)
                .Add(ItemID.ApprenticeBait, Condition.DownedEyeOfCthulhu)
                .Add(ItemID.JourneymanBait, Condition.DownedEowOrBoc)
                .Add(ItemID.MasterBait, Condition.DownedSkeletron)
                .AddWithCustomValue(ItemID.AngelStatue, Item.buyPrice(gold: 5), Condition.NpcIsPresent(NPCType<THIEF>()))
                .AddWithCustomValue(ItemID.Burger, Item.buyPrice(gold: 5), Condition.HappyEnough, Condition.DownedSkeletron)
                .AddWithCustomValue(ItemID.Hotdog, Item.buyPrice(gold: 5), Condition.HappyEnough, Condition.DownedSkeletron)
                .AddWithCustomValue(ItemID.CoffeeCup, Item.buyPrice(gold: 2), Condition.HappyEnough);
            }

            if (type == NPCID.DyeTrader)
            {
                shop.AddWithCustomValue(ItemType<DefiledFlameDye>(), Item.buyPrice(gold: 10), Condition.Hardmode)
                .AddWithCustomValue(ItemID.DyeTradersScimitar, Item.buyPrice(gold: 15));
            }

            if (type == NPCID.Demolitionist)
            {
                shop.Add(ModContent.ItemType<DeepcoreGK2>(), Condition.DownedMechBossAny);
            }

            if (type == NPCID.ArmsDealer)
            {

                shop.AddWithCustomValue(ItemType<P90>(), Item.buyPrice(gold: 25), Condition.Hardmode)
                .AddWithCustomValue(ItemID.Boomstick, Item.buyPrice(gold: 20), Condition.DownedQueenBee)
                .AddWithCustomValue(ItemID.AmmoBox, Item.buyPrice(gold: 25), Condition.Hardmode)
                .AddWithCustomValue(ItemID.Uzi, Item.buyPrice(gold: 45), Condition.DownedPlantera)
                .AddWithCustomValue(ItemID.TacticalShotgun, Item.buyPrice(gold: 60), Condition.DownedGolem)
                .AddWithCustomValue(ItemID.SniperRifle, Item.buyPrice(gold: 60), Condition.DownedGolem)
                .AddWithCustomValue(ItemID.RifleScope, Item.buyPrice(gold: 60), Condition.DownedGolem);
            }

            if (type == NPCID.Stylist)
            {
                shop.Add(ItemType<StealthHairDye>(), roguePlayer)
                .Add(ItemType<WingTimeHairDye>(), wingedPlayer)
                .Add(ItemType<AdrenalineHairDye>(), revengeance)
                .Add(ItemType<RageHairDye>(), revengeance)
                .AddWithCustomValue(ItemID.StylistKilLaKillScissorsIWish, Item.buyPrice(gold: 15))
                .Add(ItemType<CirrusDress>(), Condition.NpcIsPresent(NPCType<FAP>()), drunk)
                .AddWithCustomValue(ItemID.ChocolateChipCookie, Item.buyPrice(gold: 3), Condition.HappyEnough, Condition.NpcIsPresent(NPCType<FAP>()));
            }

            if (type == NPCID.Cyborg)
            {
                shop.AddWithCustomValue(ItemID.RocketLauncher, Item.buyPrice(gold: 25), Condition.DownedGolem)
                .AddWithCustomValue(ItemType<MartianDistressRemote>(), Item.buyPrice(gold: 50), Condition.DownedGolem)
                .Add(ItemType<LionHeart>(), downedPolterghast);
            }

            if (type == NPCID.Dryad)
            {
                shop.AddWithCustomValue(ItemID.JungleRose, Item.buyPrice(gold: 2))
                .AddWithCustomValue(ItemID.NaturesGift, Item.buyPrice(gold: 10))
                .Add(ItemType<RomajedaOrchid>())
                .AddWithCustomValue(ItemID.Grapes, Item.buyPrice(gold: 2, silver: 50), Condition.HappyEnough, Condition.DownedSkeletron);
            }

            if (type == NPCID.GoblinTinkerer)
            {
                shop.AddWithCustomValue(ItemID.StinkPotion, Item.buyPrice(silver: 25), potionSells, Condition.HappyEnough)
                .Add(ItemType<StatMeter>())
                .AddWithCustomValue(ItemID.Spaghetti, Item.buyPrice(gold: 5), Condition.HappyEnough, Condition.DownedSkeletron);
            }

            if (type == NPCID.Mechanic)
            {
                shop.AddWithCustomValue(ItemID.BuilderPotion, Item.buyPrice(gold: 4), potionSells, Condition.HappyEnough)
                .AddWithCustomValue(ItemID.CombatWrench, Item.buyPrice(gold: 10));
            }

            if (type == NPCID.Clothier)
            {
                shop.AddWithCustomValue(ItemType<CounterScarf>(), Item.buyPrice(gold: 10))
                .AddWithCustomValue(ItemID.GoldenKey, Item.buyPrice(gold: 5), Condition.Hardmode)
                .AddWithCustomValue(ItemType<GodSlayerHornedHelm>(), Item.buyPrice(gold: 8), downedDoG)
                .AddWithCustomValue(ItemType<GodSlayerVisage>(), Item.buyPrice(gold: 8), downedDoG)
                .AddWithCustomValue(ItemType<SilvaHelm>(), Item.buyPrice(gold: 8), downedDoG)
                .AddWithCustomValue(ItemType<SilvaHornedHelm>(), Item.buyPrice(gold: 8), downedDoG)
                .AddWithCustomValue(ItemType<SilvaMask>(), Item.buyPrice(gold: 8), downedDoG);
            }

            if (type == NPCID.Painter)
            {
                shop.AddWithCustomValue(ItemID.PainterPaintballGun, Item.buyPrice(gold: 15));
            }

            if (type == NPCID.Steampunker)
            {
                shop.Add(ItemType<AstralSolution>(), Condition.NotRemixWorld)
                .Add(ItemID.PurpleSolution, Condition.InGraveyard, Condition.CrimsonWorld, Condition.NotRemixWorld)
                .Add(ItemID.RedSolution, Condition.InGraveyard, Condition.CorruptWorld, Condition.NotRemixWorld);
            }

            if (type == NPCID.Wizard)
            {
                shop.Add(ItemType<HowlsHeart>())
                .AddWithCustomValue(ItemID.MagicMissile, Item.buyPrice(gold: 5))
                .AddWithCustomValue(ItemID.RodofDiscord, Item.buyPrice(gold: 10), Condition.Hardmode, Condition.InHallow)
                .AddWithCustomValue(ItemID.SpectreStaff, Item.buyPrice(gold: 25), Condition.DownedGolem)
                .AddWithCustomValue(ItemID.InfernoFork, Item.buyPrice(gold: 25), Condition.DownedGolem)
                .AddWithCustomValue(ItemID.ShadowbeamStaff, Item.buyPrice(gold: 25), Condition.DownedGolem)
                .AddWithCustomValue(ItemID.MagnetSphere, Item.buyPrice(gold: 25), Condition.DownedGolem);
            }

            if (type == NPCID.WitchDoctor)
            {
                shop.Add(ItemType<SunkenSeaFountain>())
                .Add(ItemType<SulphurousFountainItem>())
                .Add(ItemType<AbyssFountainItem>(), Condition.Hardmode)
                .Add(ItemType<AstralFountainItem>(), Condition.Hardmode)
                .AddWithCustomValue(ItemID.ButterflyDust, Item.buyPrice(gold: 10), Condition.DownedGolem)
                .AddWithCustomValue(ItemID.FriedEgg, Item.buyPrice(gold: 2, silver: 50), Condition.HappyEnough);
            }

            if (type == NPCID.PartyGirl)
            {
                shop.AddWithCustomValue(ItemID.GenderChangePotion, Item.buyPrice(silver: 25), potionSells, Condition.HappyEnough)
                .AddWithCustomValue(ItemID.Pizza, Item.buyPrice(gold: 5), Condition.HappyEnough, Condition.DownedSkeletron)
                .AddWithCustomValue(ItemID.CreamSoda, Item.buyPrice(gold: 2, silver: 50), Condition.HappyEnough);
            }

            if (type == NPCID.Princess)
            {
                shop.AddWithCustomValue(ItemID.PrincessWeapon, Item.buyPrice(gold: 50))
                .Add(ItemType<LanternCenter>())
                .Add(ItemID.AppleJuice, Condition.NpcIsPresent(NPCType<FAP>()))
                .Add(ItemID.FruitJuice, Condition.NpcIsPresent(NPCType<FAP>()))
                .Add(ItemID.Lemonade, Condition.NpcIsPresent(NPCType<FAP>()))
                .Add(ItemID.PrismaticPunch, Condition.NpcIsPresent(NPCType<FAP>()))
                .Add(ItemID.SmoothieofDarkness, Condition.NpcIsPresent(NPCType<FAP>()))
                .Add(ItemID.TropicalSmoothie, Condition.NpcIsPresent(NPCType<FAP>()));
            }

            if (type == NPCID.SkeletonMerchant)
            {
                shop.AddWithCustomValue(ItemType<CalciumPotion>(), Item.buyPrice(silver: 25), potionSells)
                .Add(ItemID.MilkCarton)
                .AddWithCustomValue(ItemID.Marrow, Item.buyPrice(gold: 25), Condition.Hardmode)
                .AddWithCustomValue(ItemType<GiantShell>(), Item.buyPrice(gold: 12))
                .AddWithCustomValue(ItemType<CrawCarapace>(), Item.buyPrice(gold: 12));
            }

            if (type == NPCID.Golfer)
            {
                shop.AddWithCustomValue(ItemID.PotatoChips, Item.buyPrice(gold: 1), Condition.HappyEnough);
            }

            if (type == NPCID.BestiaryGirl)
            {
                shop.AddWithCustomValue(ItemID.Steak, Item.buyPrice(gold: 5), Condition.HappyEnough, Condition.Hardmode);
            }
        }

        public override void SetupTravelShop(int[] shop, ref int nextSlot)
        {
            if (Main.moonPhase == 0)
            {
                shop[nextSlot] = ItemType<FrostBarrier>();
                nextSlot++;
            }
        }
        #endregion
    }
}
