using System;
using System.Collections.Generic;
using System.IO;
using CalamityMod.Events;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Accessories.Vanity;
using CalamityMod.Items.Ammo;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.Dyes;
using CalamityMod.Items.Dyes.HairDye;
using CalamityMod.Items.Pets;
using CalamityMod.Items.Placeables.Astral;
using CalamityMod.Items.Placeables.Crags;
using CalamityMod.Items.Placeables.Furniture;
using CalamityMod.Items.Placeables.Furniture.Fountains;
using CalamityMod.Items.Placeables.Furniture.Paintings;
using CalamityMod.Items.Potions;
using CalamityMod.Items.Potions.Alcohol;
using CalamityMod.Items.SummonItems.Invasion;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.NPCs.TownNPCs;
using CalamityMod.Projectiles.Rogue;
using CalamityMod.Systems.Collections;
using CalamityMod.Tiles.Furniture;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.WorldBuilding;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.NPCs
{
    public class CalamityGlobalTownNPC : GlobalNPC
    {
        public static float TaxYieldFactor
        {
            get
            {
                // 10 silver per NPC, max of 5 platinum.
                if (DownedBossSystem.downedDoG)
                    return 10f;

                // 8 silver per NPC, max of 4 platinum.
                if (NPC.downedMoonlord)
                    return 8f;

                // 2 silver per NPC, max of 1 platinum.
                if (NPC.downedPlantBoss)
                    return 2f;

                return 1f;
            }
        }

        // Vanilla: 50 copper
        public static int TotalTaxesPerNPC => (int)(Item.buyPrice(silver: 1) * TaxYieldFactor);

        // Vanilla: 25 gold
        public static int TaxesToCollectLimit => (int)(Item.buyPrice(gold: 50) * TaxYieldFactor);


        /// <summary> Used for allowing Patreon names for Town NPCs. </summary>
        public bool setNewName = true;
        /// <summary> Used to control the animation of the Town NPC Shop Alert icon, if the respective config is enabled. </summary>
        public int shopAlertAnimTimer = 0;
        /// <summary> <inheritdoc cref="shopAlertAnimTimer"/> </summary>
        public int shopAlertAnimFrame = 0;
        /// <summary>
        /// Controls how this Town NPC is being affected by The Gift.<br/>
        /// If true, happiness is overriden with an extremely high value. If false, happiness is overriden with an extremely low value. If null, uses vanilla happiness.
        /// </summary>
        public bool? TheGiftStatus = null;
        /// <summary>
        /// Timer to track when to reset the effects of The Gift, which occurs after 24 hours.<br/>
        /// When The Gift is applied, this is set to 0 then starts counting up.
        /// </summary>
        public double TheGiftReset = -1.0;

        public bool AffectedByTheMonument = false;

        public override bool InstancePerEntity => true;

        public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
        {
            return entity.isLikeATownNPC;
        }

        public override GlobalNPC Clone(NPC npc, NPC npcClone)
        {
            CalamityGlobalTownNPC myClone = (CalamityGlobalTownNPC)base.Clone(npc, npcClone);

            myClone.setNewName = setNewName;
            myClone.shopAlertAnimTimer = shopAlertAnimTimer;
            myClone.shopAlertAnimFrame = shopAlertAnimFrame;
            myClone.TheGiftStatus = TheGiftStatus;
            myClone.TheGiftReset = TheGiftReset;
            myClone.AffectedByTheMonument = AffectedByTheMonument;

            return myClone;
        }

        // TODO: [The Gift] Move out of this godforsaken class
        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            bitWriter.WriteBit(TheGiftStatus.HasValue);
            if (TheGiftStatus.HasValue)
                bitWriter.WriteBit(TheGiftStatus.Value);

            binaryWriter.Write(TheGiftReset);
        }

        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            TheGiftStatus = bitReader.ReadBit() ? bitReader.ReadBit() : null;
            TheGiftReset = binaryReader.ReadDouble();
        }

        #region Town NPC Patreon Name Sets
        private static readonly string[] AnglerNames =
        [
            "Dazren",
            "Johnny Test", // <@!589966747977777197> (konorango)
            "Bling Bling Boy", // <@!522970788203069442> (phallguy)
            "RICE", // <@!400107830889152524> (rice_xd.)
            "Quest Boy", // <@!770103186093637663> (justanothersoldiermain)
        ];
        private static readonly string[] ArmsDealerNames =
        [
            "Finchi",
            "Heniek", // <@!363404700445442050> (kazurgundu)
            "Fire", // <@!354362326947856384> (ultimatefirewaster)
            "Barney Calhoun", // <@!634462901431697410> (potatostego)
            "XiaoEn0426", // <@!440448864772816896> (xiaoen0426)
            "Jeffred", // <@!295362230038560768> (paladinsamuel)
            "The Cooler Arthur", // <@!568263512523014154> (gokuartillery)
            "Shark", // <@!874464051697172492> (congratsistrash)
            "Sagi", // <@!508233115781693441> (sagittariod)
        ];
        private static readonly string[] ClothierNames =
        [
            "Joeseph Jostar",
            "Storm Havik", // <@!1013452363178197072> (fishnotduck)
            "Magorfis Splunt the Greater Finklejim", // <@!147490809334333440> (eidolbyssus)
            "Perrin", // <@!253764551139393537> (easyperrin)
            "Spud McGee", // <@!727111000658018326> (starchman)
        ];
        private static readonly string[] CyborgNames =
        [
            "Sylux", // <@!331812782183809025> (gonkachino)
            "Nemesis", // <@!1104036024063107082> (yashimayamanata)
            "Univerze", // <@!1256376346536448150> (unizumi02)
            "Hunk", // <@!447597886260248578> (bagel_san)
        ];
        private static readonly string[] DemolitionistNames =
        [
            "Tavish DeGroot", // <@!442447226992721930> (magicoal)
            "Fimmy", // <@!407348617079160832> (darkmega5)
            "John Helldiver", // <@!614126424751603714> (exellent.)
        ];
        private static readonly string[] DryadNames =
        [
            "Rythmi",
            "Izuna",
            "Jasmine", // <@!430532867479699456> (phantasmagoria.)
            "Cybil", // <@!486507232666845185> (Captain Doofus#????)
            "Ruth", // <@!1001307586068492388> (briny_coffee)
            "Kanna", // <@!730203712898859018> (cosmoredeathwish)
            "Elliada", // <@!865554691345874954> (cti971)
        ];
        private static readonly string[] DyeTraderNames = null;
        private static readonly string[] GoblinTinkererNames =
        [
            "Verth",
            "Gormer", // <@!287651204924833795> (picassosbean2819 -- RIP)
            "TingFlarg", // <@!185605031716847616> (smugggo)
            "Driser", // <@!121996994406252544> (driser)
            "Eddie Spaghetti", // <@!466397267407011841> (nathansfriend)
            "G'tok", // <@!335192200956608535> (gtoktas)
            "Katto", // <@!175972165504466944> (katto_kat)
            "Him", // <@!931019614958256139> (himtheguy1)
            "Tooshiboots", // <@!333532730593771522> (ulmod)
            "Neesh", // <@!175803493464932352> (xjetty)
            "Bars Boldia", // <@!332989575708540939> (careless_imp)
            "Basel Raiden John Clive Fantasy 16", // <@!529392083136413696> (raiden_ii)
            "Gobby, Destroyer of Wallets", // <@!429024941296582658> (bwlstorm)
            "Potential Man", // <@!320787277007552512> (veruze)
            "Donny G", // <@!308736236523225088> (donnyg66)
        ];
        private static readonly string[] GolferNames = null;
        private static readonly string[] GuideNames =
        [
            "Lapp",
            "Ben Shapiro",
            "Streakist", // used to be "StreakistYT". couldn't find the youtube channel, and decided to remove the ad.
            "Necroplasmic",
            "Devin",
            "Woffle", // <@!185980979427540992> (chipbeam)
            "Cameron", // <@!340401981711712258> (cammywammy.)
            "Wilbur", // <@!295171926324805634> (all_imperfect_chaos)
            "Good Game Design", // <@!564267767042277385> (dominickarma)
            "Danmaku", // <@!756259562268524555> (danmaku0)
            "Grylken", // <@!299970404435361802> (grylken)
            "Outlaw", // <@!918311619480657922> (thechosenoutlaw)
            "Alfred Rend", // <@!606301806481375255> (deadsqurp300)
            "Leeman", // <@!281999243168841728> (tweee)
            "Mihai", // <@!373941893467209730> (cmihaii.)
            "Dinkleberg", // <@!581993958037520404> (hyperionzx)
            "Wamy", // Fab added this name with no Discord ID. May be a donor who has no Discord account.
            "Baggute", // <@!535140564174110720> (thebaggutegamer)
            "Jacob Bryson", // <@!879794107218817026> (.melongun)
            "ImaShoe", // <@!926957605056819261> (imashoe.)
        ];
        private static readonly string[] MechanicNames =
        [
            "Lilly",
            "Daawn", // <@!206162323541458944> (daawnily)
            "Robin", // <@!654737510030639112> (altzeus)
            "Curly", // <@!673092101780668416> (curly4830)
            "Cobalt", // <@!132962828922388481> (cobalt_44)
            "Dizzetriya", // <@!719818245665980517> (dizzykbity)
            "Vodka", // <@!1172497470438248565> (ehhhhh0981)
        ];
        private static readonly string[] MerchantNames =
        [
            "Morshu", // <@!194931581826236416> (uberransy)
            "Spamton G. Spamton", // <@!497146350438318101> (j.u.n.e.s)
            "Goldluna", // <@!442449102857568257> (goldluna)
        ];
        private static readonly string[] NurseNames =
        [
            "Farsni",
            "Fanny", // <@!799749125720637460> (zombiewolf511)
            "Mausi", // <@!194156349347594241> (sadouken)
            "Fiona", // <@!475216964168450048> (thatgayguy69)
            "Tikoh", // <@!618149904224616458> (r.fractal)
            "Vivienne", // <@!1043188879185952848> (thanat.oshi)
        ];
        private static readonly string[] PainterNames =
        [
            "Picasso", // <@!353316526306361347> (sconicboom -- for the late picassosbean2819)
            "Bew", // <@!232291351167893505> (dmshi)
        ];
        private static readonly string[] PartyGirlNames =
        [
            "Arin", // <@!268169458302976012> (kiyotu)
            "Typhäne", // <@!222064016107896832> (typhane.)
            "Charlotte Linlin", // <@!563406464522125323> (vixcalibur)
            "Shmizzle Dizzle", // <@!1070551501283528724> (shmizzledizzle)
        ];
        private static readonly string[] PirateNames =
        [
            "Tyler Van Hook",
            "Cap'n Deek", // "Alex N" on Patreon (No discord account)
            "Captain Billy Bones", // <@!699589229507772416> (djackv)
            "Captain J. Crackers", // <@!233232602994049024> (qyuuno)
            "Gol D. Roger", // <@!256228859110752257> (xtra3678)
            "Yarrim", // <@!290061123137306624> (borb9834)
            "Hector Barbossa", // <@!615704209303797790> (thatrockisfullamagic)
            "Blunderbeard", // <@!1039460813490102293> (parmiigianoreggiano)
            "Vergil Cyrus", // <@!732350101619605584> (cyver1)
        ];
        private static readonly string[] PrincessNames =
        [
            "Nyapano", // <@!120976656826368003> (nyapano)
            "Jade", // <@!187395834625785869> (verymasterninja)
            "Nyavi Aceso", // <@!270260920888852480> (navigator.)
            "everquartz", // <@!451343554451865611> (everquartz)
            "Gwynevere", // <@!142752927348424704> (nuclearchaosazathoth)
            "Hael", // <@!641747280944431156> (kalebtull)
            "Yumesaki Mirrin", // <@!100235144744415232> (milinen)
            "Vela", // <@!208719047146209281> (nyxxynightstar)
            "Misako Drevis", // <@!1103067115386323065> (threadsofmemory)
        ];
        private static readonly string[] SantaClausNames =
        [
            "Jank", // <@!339950757472239616> (jankle_)
            "Aoi Kurashiki", // <@!358411687885537291> (nothinpurrsonal)
        ];
        private static readonly string[] SkeletonMerchantNames =
        [
            "Sans Undertale", // <@!534770496038895616> (done_22_)
            "Papyrus Undertale", // <@!262663471189983242> (nycro)
            "Mr. Bones", // <@!359215912856977408> (jaybones.)
            "Freakbob", // <@!377863128140087296> (jevilamv)
            "Bone Cold Steve Austin", // <@!282704860992897024> (raendrag_of_rovan)
            "Them Bones", // <@!322208584534589450> (dogvtf)
            "Deep-Vein Thrombonesis", // <@!557473830457704458> (thessyll)
        ];
        private static readonly string[] SteampunkerNames =
        [
            "Vorbis",
            "Angel",
            "Mòrag Ladair", // <@!161893929485074432> (jalapeno9)
            "Linn", // <@!277983612383526913> (duckycolors)
            "Eira", // <@!1166136068408623234> (taela_gemetha)
            "Kreutz", // <@!553445849149997056> (red_r_kreutz)
            "Cathlyn", // <@!156672312425316352> (xaqult)
            "Eunice", // <@!358376627400605699> (srmg267)
            "Zera", // <@!543914166969172106> (zer0_0_0_0)
        ];
        private static readonly string[] StylistNames =
        [
            "Amber", // <@!114677116473180169> (mishirousui)
            "Faith", // <@!509050283871961123> (toasty1007)
            "Xsiana", // <@!625780237489143839> (lokistic)
            "Lain", // <@!655201622863118337> (literallyadeerfr)
            "Brio Scarlet", // <@!358576903701004289> (brio_scarlet)
            "Vanessa", // <@!638901548591611945> (mediocreking)
            "Melanie", // <@!356115964800139267> (schwarzerhumor)
        ];
        private static readonly string[] TavernkeepNames =
        [
            "Tim Lockwood", // <@!605839945483026434> (pomvoid)
            "Sir Samuel Winchester Jenkins Kester II", // <@!107659695749070848> (ryaegos)
            "Brutus", // <@!591889650692521984> (.brutus._)
            "Sloth", // <@!486265327387279391> (bossypunch)
        ];
        private static readonly string[] TaxCollectorNames =
        [
            "Emmett",
            "Bagman", // <@!701831892990820383> (supportcrispy)
            "Old Man Scrooge", // <@!1392141158255427655> (vortexgaming18)
            "Jerry Atric", // <@!181545975901454337> (halleyvetica)
            "22 Platinum Coin Guy", // <@!921440724766040064> (tsukarin_)
        ];
        private static readonly string[] TravelingMerchantNames =
        [
            "Stan Pines",
            "Intergaze", // <@!923504188615450654> (intergaze)
            "Borgus", // <@!539127427482255376> (therealmeepman)
            "Postman Hiss", // <@!454638106122125312> (karinthefairy)
            "Cosmoec", // <@!793660591449309204> (cosmoecark)
            "Junorism", // <@!740625002596008036> (hewhoshallnotbebaned)
            "Koral", // <@!1354908256681590845> (koral12244_)
            "Phantom", // <@!1360706992506667241> (phantomz980)
        ];
        private static readonly string[] TruffleNames =
        [
            "Aldrimil", // <@!413719640238194689> (Thorioum#2475)
            "Wonton", // <@!1198092982923043040> (imonthatgudkush)
            "Mad Lad", // <@!215269032360804352> (crimsoncb)
        ];
        private static readonly string[] WitchDoctorNames =
        [
            "Sok'ar",
            "Aeroni", // <@!348174404984766465> (aeroni) (previously: toxin)
            "Mixcoatl", // <@!284775927294984203> (.sharzz)
            "Amnesia Wapers", // <@!326821498323075073> (retardedadvicefromaretard)
            "Tequila", // <@!889175547744239677> (thecrispistofnuggets)
            "Bee Movie Script", // <@!407949998173454341> (literally_jesuschrist)
            "Which Doctor", // <@!746103484017016832> (sepulchre0001)
        ];
        private static readonly string[] WizardNames =
        [
            "Inorim, son of Ivukey",
            "Jensen",
            "Merasmus", // <@!288066987819663360> (spiderprovidence)
            "Habolo", // <@!163028025494077441> (hellgoat2)
            "Ortho", // <@!264984390910738432> (worcuus)
            "Chris Tallballs", // <@!770211589076418571> (vysterx) (previously: bewearium)
            "Syethas", // <@!325413275066171393> (cosmicstariight)
            "Nextdoor Psycho", // <@!173261518572486656> (nextdoorpsycho)
            "Mike Cyclops", // <@!702327497475227741> (seichoseicho)
            "Derin", // <@!466703979695308820> (god_15)
            "Umbara", // <@!450062421294579712> (umbaraeclipse)
            "Reyloth Grey", // <@!255043013116428298> (pantyslack)
        ];
        private static readonly string[] ZoologistNames =
        [
            "Kiriku", // <@!395312478160027668> (rulosss)
            "Lacuna", // <@!790746689211203604> (_lacuna_)
            "Mae Borowski", //<@!219158690433990656> (justakkolite)
            "Fera", // <@!195850711567826945> (juneark_)
            "Gwenhwyvar", // <@!291342874497515531> (diamondnife)
            "Daxie", // <@!465438861103988737> (daxie626)
            "Zora", // <@!752687500656640030> (oxytoxy365)
            "Summer", // <@!608455754093035521> (haefer)
            "Foxy", // <@!602954046332207164> (squid_san)
        ];
        // Town Slimes
        private static readonly string[] ClumsySlimeNames = null;
        private static readonly string[] CoolSlimeNames = null;
        private static readonly string[] DivaSlimeNames =
        [
            "Rise Kujikawa", // <@!630100236689342475> (roald27)
        ];
        private static readonly string[] ElderSlimeNames = null;
        private static readonly string[] MysticSlimeNames = null;
        private static readonly string[] NerdySlimeNames =
        [
            "Big Blungus", // <@!272759434282008577> (schmoov)
            "Rimuru Tempest", // <@!806463201398358036> (c0d3_404)
        ];
        private static readonly string[] SquireSlimeNames = null;
        private static readonly string[] SurlySlimeNames = null;

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
        [
            "Ozymandias", // <@!146333264871686145> (ozzatron)
            "Miss Throws a Lot", // <@!799345607847182400> (oakhamsam)
            "Brikwilla", // <@!543803736909414438> (lavendercobra)
            "Melody", // <@!1030635650963214446> (weisslerren)
        ];
        private static readonly string[] TownDogLabradorNames =
        [
            "Riley", // <@!260875558592708619> (potionpal)
            "Silvie", // <@!979862425211912242> (goldsockz2)
            "Madison", // <@!338315261352476682> (tyeski)
        ];
        private static readonly string[] TownDogPitBullNames =
        [
            "Splinter", // <@!320320801213775873> (kaimonick)
            "Mack", // <@!427765391662514185> (dorkblaze01)
        ];
        private static readonly string[] TownDogBeagleNames =
        [
            "Kendra", // <@!237247188005158912> (lordmetarex)
            "Libby", // <@!338315261352476682> (tyeski)
            "Myles", // <@!658760860722004017> (apotofkoolaid)
            "Luna", // <@!534132902095749120> (mizzultraviolet)
        ];
        private static readonly string[] TownDogCorgiNames = null;
        private static readonly string[] TownDogDalmatianNames = null;
        private static readonly string[] TownDogHuskyNames =
        [
            "Yoshi", // <@!541127291426832384> (gregthespinarak)
            "Franklin", // <@!338315261352476682> (tyeski)
            "Rocco", // <@!682411821067796480> (little_one777)
        ];

        private const int TownCatSiameseVanillaNames = 12;
        private const int TownCatBlackVanillaNames = 23;
        private const int TownCatOrangeTabbyVanillaNames = 18;
        private const int TownCatRussianBlueVanillaNames = 16;
        private const int TownCatSilverVanillaNames = 17;
        private const int TownCatWhiteVanillaNames = 15;
        private static readonly string[] TownCatNames =
        [
            "Smoogle", // <@!709968379334623274> (smooglin)
            "The Meowurer of Gods", // <@!385949114271268864> (thatgp)
            "Katsafaros", // <@!190595401328492544> (gr_mm)
            "Lucerne", // <@!271954788676141066> (lord_lucerne)
            "Milo", // <@!401849201597874179> (maskedmilo)
            "Octo", // <@!796112889353994281> (octolinggrimm)
            "Kreska", // <@!130037366852157440> (nuclearnecro)
            "Meokei", // <@!230839680076218378> (azurlia)
        ];
        private static readonly string[] TownCatSiameseNames =
        [
            "Conductor", // <@!555512087711973390> (grayaeternum)
            "Vivian", // <@!338315261352476682> (TYESKI)
            "Pudum", // <@!731141759484297226> (trianglepixel)
            "Snickers", // <@!658760860722004017> (apotofkoolaid)
            "Mr. Kitten", // <@!658760860722004017> (apotofkoolaid)
        ];
        private static readonly string[] TownCatBlackNames =
        [
            "Bear", // <@!183424826407518208> (lilac_vrt_olligoci)
            "Storm", // <@!620383533516718085> (airwaveslr)
            "Hognar", // <@!766511001356468237> (xzier_tengal)
            "Saffie", // <@!319753595161411584> (CDMusic)
            "Willow", // <@!319753595161411584> (CDMusic)
            "Maine", // <@!731141759484297226> (trianglepixel)
            "Pluey", // <@!706732954079985745> (violet.prime)
        ];
        private static readonly string[] TownCatOrangeTabbyNames =
        [
            "Felix", // <@!183424826407518208> (lilac_vrt_olligoci)
            "Tardo", // <@!739343546867384391> (midnight295)
            "Dali", // <@!460238880436781061> (darthlego)
            "Kiba", // <@!852348657072340992> (jollydragonslayer)
            "Monkey", // <@!338315261352476682> (TYESKI)
            "Percy", // <@!658760860722004017> (apotofkoolaid)
        ];
        private static readonly string[] TownCatRussianBlueNames = null;
        private static readonly string[] TownCatSilverNames =
        [
            "Archie", // <@!303022375191183360> (jackshiz)
        ];
        private static readonly string[] TownCatWhiteNames = null;

        private const int TownBunnyWhiteVanillaNames = 14;
        private const int TownBunnyAngoraVanillaNames = 10;
        private const int TownBunnyDutchVanillaNames = 11;
        private const int TownBunnyFlemishVanillaNames = 12;
        private const int TownBunnyLopVanillaNames = 13;
        private const int TownBunnySilverVanillaNames = 13;
        private static readonly string[] TownBunnyNames =
        [
            "Poco", // <@!1192261996146593872> (tostitomuncher33)
            "Puffer", // <@!181103507711983616> (piky)
        ];
        private static readonly string[] TownBunnyWhiteNames = null;
        private static readonly string[] TownBunnyAngoraNames = null;
        private static readonly string[] TownBunnyDutchNames =
        [
            "windy", // lower case intended ~ <@!498414879502368768> (altixal)
        ];
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
            if (alreadySet)
            {
                alreadySet = true;
                return currentName;
            }
            alreadySet = true;
            // PatreonNames can be null, so can global names, it short circuits in the next step if so
            int combinedLength = (patreonNames?.Length ?? 0) + (globalNames?.Length ?? 0);
            int index = Main.rand.Next(numVanillaNames + combinedLength);

            // If the roll isn't low enough, then a "vanilla name" was picked, meaning we change nothing.
            if (index >= combinedLength)
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
                                npc.GivenName = ChooseName(ref CalamityWorld.dogName, npc.GivenName, TownDogLabradorVanillaNames, TownDogLabradorNames, TownDogNames);
                                break;
                            case 1:
                                npc.GivenName = ChooseName(ref CalamityWorld.dogName, npc.GivenName, TownDogPitBullVanillaNames, TownDogPitBullNames, TownDogNames);
                                break;
                            case 2:
                                npc.GivenName = ChooseName(ref CalamityWorld.dogName, npc.GivenName, TownDogBeagleVanillaNames, TownDogBeagleNames, TownDogNames);
                                break;
                            case 3:
                                npc.GivenName = ChooseName(ref CalamityWorld.dogName, npc.GivenName, TownDogCorgiVanillaNames, TownDogCorgiNames, TownDogNames);
                                break;
                            case 4:
                                npc.GivenName = ChooseName(ref CalamityWorld.dogName, npc.GivenName, TownDogDalmatianVanillaNames, TownDogDalmatianNames, TownDogNames);
                                break;
                            case 5:
                                npc.GivenName = ChooseName(ref CalamityWorld.dogName, npc.GivenName, TownDogHuskyVanillaNames, TownDogHuskyNames, TownDogNames);
                                break;
                            default:
                                break;
                        }
                        break;
                    case NPCID.TownBunny:
                        switch (npc.townNpcVariationIndex)
                        {
                            case 0:
                                npc.GivenName = ChooseName(ref CalamityWorld.bunnyName, npc.GivenName, TownBunnyWhiteVanillaNames, TownBunnyWhiteNames, TownBunnyNames);
                                break;
                            case 1:
                                npc.GivenName = ChooseName(ref CalamityWorld.bunnyName, npc.GivenName, TownBunnyAngoraVanillaNames, TownBunnyAngoraNames, TownBunnyNames);
                                break;
                            case 2:
                                npc.GivenName = ChooseName(ref CalamityWorld.bunnyName, npc.GivenName, TownBunnyDutchVanillaNames, TownBunnyDutchNames, TownBunnyNames);
                                break;
                            case 3:
                                npc.GivenName = ChooseName(ref CalamityWorld.bunnyName, npc.GivenName, TownBunnyFlemishVanillaNames, TownBunnyFlemishNames, TownBunnyNames);
                                break;
                            case 4:
                                npc.GivenName = ChooseName(ref CalamityWorld.bunnyName, npc.GivenName, TownBunnyLopVanillaNames, TownBunnyLopNames, TownBunnyNames);
                                break;
                            case 5:
                                npc.GivenName = ChooseName(ref CalamityWorld.bunnyName, npc.GivenName, TownBunnySilverVanillaNames, TownBunnySilverNames, TownBunnyNames);
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
                case NPCID.SantaClaus:
                    AddNewNames(nameList, SantaClausNames);
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

                // Town Slimes
                case NPCID.TownSlimePurple: // Clumsy Slime
                    AddNewNames(nameList, ClumsySlimeNames);
                    break;
                case NPCID.TownSlimeGreen: // Cool Slime
                    AddNewNames(nameList, CoolSlimeNames);
                    break;
                case NPCID.TownSlimeRainbow: // Diva Slime
                    AddNewNames(nameList, DivaSlimeNames);
                    break;
                case NPCID.TownSlimeOld: // Elder Slime
                    AddNewNames(nameList, ElderSlimeNames);
                    break;
                case NPCID.TownSlimeYellow: // Mystic Slime
                    AddNewNames(nameList, MysticSlimeNames);
                    break;
                case NPCID.TownSlimeBlue: // Nerdy Slime
                    AddNewNames(nameList, NerdySlimeNames);
                    break;
                case NPCID.TownSlimeCopper: // Squire Slime
                    AddNewNames(nameList, SquireSlimeNames);
                    break;
                case NPCID.TownSlimeRed: // Surly Slime
                    AddNewNames(nameList, SurlySlimeNames);
                    break;

                // This function doesn't work with Town Pets currently
                case NPCID.TownCat:
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
                    break;

                default:
                    break;
            }
        }
        #endregion

        #region SetDefaults
        public override void SetDefaults(NPC npc)
        {
            BoundNPCSafety(Mod, npc);
        }
        #endregion

        #region PreAI
        public override bool PreAI(NPC npc)
        {
            SetPatreonTownNPCName(npc, Mod);

            // Reset The Gift after 24 hours
            if (TheGiftReset >= 0.0)
            {
                TheGiftReset += Main.dayRate;
                if (TheGiftReset >= Main.dayLength + Main.nightLength)
                {
                    TheGiftReset = -1.0;
                    TheGiftStatus = null;
                }
            }

            // Search for The Monument, for the purposes of assigning higher taxes
            AffectedByTheMonument = false;
            SearchForTheMonument(npc);

            return true;
        }

        public bool SearchForTheMonument(NPC npc)
        {
            Point tileCenter = npc.Center.ToTileCoordinates();
            Rectangle searchArea = new((int)(tileCenter.X - Main.buffScanAreaWidth / 2), (int)(tileCenter.Y - Main.buffScanAreaHeight / 2), Main.buffScanAreaWidth, Main.buffScanAreaHeight);
            searchArea = WorldUtils.ClampToWorld(searchArea);
            for (int i = searchArea.Left; i < searchArea.Right; i++)
            {
                for (int j = searchArea.Top; j < searchArea.Bottom; j++)
                {
                    if (!searchArea.Contains(i, j))
                        continue;

                    Tile tile = Main.tile[i, j];
                    if (tile == null)
                        continue;
                    if (!tile.HasTile)
                        continue;

                    if (tile.TileType == TileType<TheMonumentTile>())
                    {
                        AffectedByTheMonument = true;
                        return true;
                    }
                }
            }
            return false;
        }
        #endregion

        #region PreDraw
        public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            TownNPCAlertSystem(npc, Mod, spriteBatch);
            return true;
        }
        #endregion

        #region NPC New Shop Alert

        public static List<(int, Predicate<Player>, Action<Player, bool>)> npcAlertList = new List<(int, Predicate<Player>, Action<Player, bool>)>()
        {
            (NPCID.Merchant, (Player player) => player.Calamity().newMerchantInventory, (Player player, bool enabled) =>{ player.Calamity().newMerchantInventory = enabled; }),
            (NPCID.Painter, (Player player) => player.Calamity().newPainterInventory,(Player player, bool enabled) =>{ player.Calamity().newPainterInventory = enabled; }),
            (NPCID.Golfer, (Player player) => player.Calamity().newGolferInventory, (Player player, bool enabled) =>{ player.Calamity().newGolferInventory = enabled; }),
            (NPCID.BestiaryGirl, (Player player) => player.Calamity().newZoologistInventory,(Player player, bool enabled) =>{ player.Calamity().newZoologistInventory = enabled; }),
            (NPCID.DyeTrader, (Player player) => player.Calamity().newDyeTraderInventory, (Player player, bool enabled) =>{ player.Calamity().newDyeTraderInventory = enabled; }),
            (NPCID.PartyGirl, (Player player) => player.Calamity().newPartyGirlInventory,(Player player, bool enabled) =>{ player.Calamity().newPartyGirlInventory = enabled; }),
            (NPCID.Stylist, (Player player) => player.Calamity().newStylistInventory, (Player player, bool enabled) =>{ player.Calamity().newStylistInventory = enabled; }),
            (NPCID.Demolitionist, (Player player) => player.Calamity().newDemolitionistInventory, (Player player, bool enabled) =>{ player.Calamity().newDemolitionistInventory = enabled; }),
            (NPCID.Dryad, (Player player) => player.Calamity().newDryadInventory, (Player player, bool enabled) =>{ player.Calamity().newDryadInventory = enabled; }),
            (NPCID.DD2Bartender, (Player player) => player.Calamity().newTavernkeepInventory, (Player player, bool enabled) =>{ player.Calamity().newTavernkeepInventory = enabled; }),
            (NPCID.ArmsDealer, (Player player) => player.Calamity().newArmsDealerInventory, (Player player, bool enabled) =>{ player.Calamity().newArmsDealerInventory = enabled; }),
            (NPCID.GoblinTinkerer, (Player player) => player.Calamity().newGoblinTinkererInventory,(Player player, bool enabled) =>{ player.Calamity().newGoblinTinkererInventory = enabled; }),
            (NPCID.WitchDoctor, (Player player) => player.Calamity().newWitchDoctorInventory, (Player player, bool enabled) =>{ player.Calamity().newWitchDoctorInventory = enabled; }),
            (NPCID.Clothier, (Player player) => player.Calamity().newClothierInventory, (Player player, bool enabled) =>{ player.Calamity().newClothierInventory = enabled; }),
            (NPCID.Mechanic, (Player player) => player.Calamity().newMechanicInventory, (Player player, bool enabled) =>{ player.Calamity().newMechanicInventory = enabled; }),
            (NPCID.Pirate, (Player player) => player.Calamity().newPirateInventory, (Player player, bool enabled) =>{ player.Calamity().newPirateInventory = enabled; }),
            (NPCID.Truffle, (Player player) => player.Calamity().newTruffleInventory,(Player player, bool enabled) =>{ player.Calamity().newTruffleInventory = enabled; }),
            (NPCID.Wizard, (Player player) => player.Calamity().newWizardInventory, (Player player, bool enabled) =>{ player.Calamity().newWizardInventory = enabled; }),
            (NPCID.Steampunker, (Player player) => player.Calamity().newSteampunkerInventory, (Player player, bool enabled) =>{ player.Calamity().newSteampunkerInventory = enabled; }),
            (NPCID.Cyborg,(Player player) => player.Calamity().newCyborgInventory, (Player player, bool enabled) =>{ player.Calamity().newCyborgInventory = enabled; }),
            (NPCID.Princess, (Player player) => player.Calamity().newPrincessInventory,(Player player, bool enabled) =>{ player.Calamity().newPrincessInventory = enabled; }),
            (NPCID.SkeletonMerchant, (Player player) => player.Calamity().newSkeletonMerchantInventory, (Player player, bool enabled) =>{ player.Calamity().newSkeletonMerchantInventory = enabled; }),
            (NPCType<SeaKing>(), (Player player) => player.Calamity().newAmidiasInventory,(Player player, bool enabled) =>{ player.Calamity().newAmidiasInventory = enabled; }),
            (NPCType<Bandit>(), (Player player) => player.Calamity().newBanditInventory,(Player player, bool enabled) =>{ player.Calamity().newBanditInventory = enabled; }),
            (NPCType<Archmage>(), (Player player) => player.Calamity().newPermafrostInventory,(Player player, bool enabled) =>{ player.Calamity().newPermafrostInventory = enabled; }),
            (NPCType<BrimstoneWitch>(), (Player player) => player.Calamity().newCalamitasInventory,(Player player, bool enabled) =>{ player.Calamity().newCalamitasInventory = enabled; }) // lol
        };

        public void TownNPCAlertSystem(NPC npc, Mod mod, SpriteBatch spriteBatch)
        {
            if (CalamityClientConfig.Instance.ShopNewAlert && npc.townNPC)
            {
                for (int i = 0; i < npcAlertList.Count; i++)
                {
                    if (npc.type == npcAlertList[i].Item1 && npcAlertList[i].Item2(Main.LocalPlayer))
                    {
                        DrawNewInventoryAlert(npc);
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
            for (int i = 0; i < npcAlertList.Count; i++)
            {
                if (npc.type == npcAlertList[i].Item1)
                {
                    npcAlertList[i].Item3(Main.LocalPlayer, false);
                }
            }
        }

        public static void SetNewShopVariable(int[] types, bool alreadySet)
        {
            string npcname = ContentSamples.NpcsByNetId[types[0]].FullName;
            if (!alreadySet)
            {
                for (int i = 0; i < types.Length; i++)
                {
                    for (int n = 0; n < npcAlertList.Count; n++)
                    {
                        if (types[i] == npcAlertList[n].Item1)
                        {
                            npcAlertList[n].Item3(Main.LocalPlayer, true);
                        }
                    }
                }
            }
        }
        #endregion

        #region NPC Chat
        public override void GetChat(NPC npc, ref string chat)
        {
            int permafrost = NPC.FindFirstNPC(NPCType<Archmage>());
            int seahorse = NPC.FindFirstNPC(NPCType<SeaKing>());
            int thief = NPC.FindFirstNPC(NPCType<Bandit>());
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
                        chat = CalamityUtils.GetTextValue("Vanilla.CyborgChat.MoonLordDefeated");
                    else if (Main.rand.NextBool(10) && !DownedBossSystem.downedPlaguebringer && NPC.downedGolemBoss)
                        chat = CalamityUtils.GetTextValue("Vanilla.CyborgChat.MentionPlague");
                    else if (Main.rand.NextBool(10) && Main.raining)
                        chat = CalamityUtils.GetTextValue("Vanilla.CyborgChat.Rain");
                    break;

                case NPCID.Demolitionist:
                    if (Main.rand.NextBool(5) && DownedBossSystem.downedDoG)
                        chat = CalamityUtils.GetTextValue("Vanilla.DemolitionistChat.DoGDefeated");
                    else if (Main.rand.NextBool(10))
                        chat = CalamityUtils.GetTextValue("Vanilla.DemolitionistChat.MentionSkynamite");
                    break;

                case NPCID.Dryad:
                    if (Main.rand.NextBool(5) && DownedBossSystem.downedDoG && Main.eclipse)
                        chat = CalamityUtils.GetTextValue("Vanilla.DryadChat.DarksunEclipse");
                    else if (Main.rand.NextBool(5) && Main.LocalPlayer.Calamity().ZoneSulphur)
                        chat = CalamityUtils.GetTextValue("Vanilla.DryadChat.SulphurSea");
                    else if (Main.rand.NextBool(5) && Main.hardMode)
                        chat = CalamityUtils.GetTextValue("Vanilla.DryadChat.Hardmode");
                    break;

                case NPCID.DyeTrader:
                    if (Main.rand.NextBool(5) && permafrost != -1)
                        chat = CalamityUtils.GetText("Vanilla.DyeTraderChat.Archmage").Format(Main.npc[permafrost].GivenName);
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
                    if (Main.rand.NextBool(5) && Main.LocalPlayer.InventoryHas(ItemID.PortalGun))
                        chat = CalamityUtils.GetTextValue("Vanilla.MechanicChat.HasPortalGun");
                    else if (Main.rand.NextBool(5) && NPC.downedMoonlord)
                        chat = CalamityUtils.GetTextValue("Vanilla.MechanicChat.MoonLordDefeated");
                    else if (Main.rand.NextBool(5) && Main.eclipse)
                        chat = CalamityUtils.GetTextValue("Vanilla.MechanicChat.Eclipse");
                    else if (Main.rand.NextBool(5) && AcidRainEvent.AcidRainEventIsOngoing)
                        chat = CalamityUtils.GetTextValue("Vanilla.MechanicChat.AcidRain");
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

                case NPCID.Nurse:
                    if (Main.rand.NextBool(4) && NPC.downedPlantBoss && thief != -1)
                        chat = CalamityUtils.GetTextValue("Vanilla.NurseChat.PlanteraDefeatedAndBanditPresent");
                    else if (Main.rand.NextBool(4) && thief != -1)
                        chat = CalamityUtils.GetTextValue("Vanilla.NurseChat.Bandit");

                    break;

                case NPCID.Painter:
                    if (Main.rand.NextBool(4) && Main.LocalPlayer.ZoneCorrupt)
                        chat = CalamityUtils.GetTextValue("Vanilla.PainterChat.Corruption");
                    if (Main.rand.NextBool(4) && Main.LocalPlayer.ZoneCrimson)
                        chat = CalamityUtils.GetTextValue("Vanilla.PainterChat.Crimson");
                    if (Main.rand.NextBool(4) && Main.LocalPlayer.ZoneSnow)
                    {
                        if (Main.rand.NextBool() && permafrost != -1)
                            chat = CalamityUtils.GetText("Vanilla.PainterChat.Archmage").Format(Main.npc[permafrost].GivenName);
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
                    break;

                case NPCID.Pirate:
                    if (Main.rand.NextBool(5) && !DownedBossSystem.downedLeviathan)
                        chat = CalamityUtils.GetTextValue("Vanilla.PirateChat.PreLeviathan");
                    else if (Main.rand.NextBool(5) && DownedBossSystem.downedAquaticScourge)
                        chat = CalamityUtils.GetTextValue("Vanilla.PirateChat.WetScourgeDefeated");
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
                    if (Main.rand.NextBool(5) && NPC.downedMoonlord)
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
                    if ((Main.rand.NextBool(npc.GivenName == "Amber" ? 10 : 15)) && Main.LocalPlayer.Calamity().pSoulArtifact)
                    {
                        if (Main.LocalPlayer.Calamity().profanedCrystalBuffs)
                            chat = CalamityUtils.GetTextValue("Vanilla.StylistChat.ProfanedSoulCrystal" + Main.rand.Next(1, 2 + 1));
                        else if (Main.LocalPlayer.Calamity().pSoulGuardians)
                            chat = CalamityUtils.GetTextValue("Vanilla.StylistChat.ProfanedDonuts");
                    }
                    break;

                case NPCID.DD2Bartender:
                    if (Main.rand.NextBool(5) && !Main.dayTime && Main.moonPhase == 0)
                        chat = CalamityUtils.GetTextValue("Vanilla.TavernkeepChat.FullMoon");
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

                case NPCID.Truffle:
                    if (Main.rand.NextBool(8))
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
            if (CalamityNPCSets.BoundTownNPC[npc.type])
                npc.dontTakeDamageFromHostiles = true;
        }

        // Does not affect Dryad's Bane
        // See CalamityGlobalNPC: UpdateLifeRegen
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

        public override bool? CanBeHitByProjectile(NPC npc, Projectile projectile)
        {
            // Not an axe but close enough
            if (npc.type == NPCID.TaxCollector && projectile.type == ProjectileType<SlickCaneProjectile>())
                return true;
            return base.CanBeHitByProjectile(npc, projectile);
        }
        #endregion

        #region Shop Stuff
        public override void ModifyShop(NPCShop shop)
        {
            int type = shop.NpcType;

            Condition spelunkerGlowCondition = new(Language.GetText("Conditions.NightDayFullMoon"), () => !Main.dayTime || Main.GetMoonPhase() == MoonPhase.Full); // Identical to the one in NPCShopDatabase
            Condition hasFlareGunUpgrade = new(CalamityUtils.GetText("Condition.HasFlareGun"), () => (Main.LocalPlayer.HasItem(ItemType<FirestormCannon>()) || Main.LocalPlayer.HasItem(ItemType<SpectralstormCannon>())) && !Main.LocalPlayer.HasItem(ItemID.FlareGun));
            Condition bestiaryProgressLacewing = new(CalamityUtils.GetText("Condition.LacewingBestiary"), () => Main.GetBestiaryProgressReport().CompletionPercent >= 0.4f);
            Condition crescentMoons = new(CalamityUtils.GetText("Condition.CrescentMoons"), () => Main.GetMoonPhase() == MoonPhase.QuarterAtLeft || Main.GetMoonPhase() == MoonPhase.QuarterAtRight); // for Craw Carapace
            Condition gibbousMoons = new(CalamityUtils.GetText("Condition.GibbousMoons"), () => Main.GetMoonPhase() == MoonPhase.ThreeQuartersAtLeft || Main.GetMoonPhase() == MoonPhase.ThreeQuartersAtRight); // for Giant Shell

            if (type == NPCID.Merchant)
            {
                shop.InsertAfter(ItemID.ManaPotion, ItemID.WormholePotion, Condition.HappyEnoughToSellPylons)
                .InsertAfter(ItemID.Safe, ItemID.MusicBox)
                .InsertAfter(ItemID.Flare, ItemID.Flare, hasFlareGunUpgrade)
                .InsertAfter(ItemID.BlueFlare, ItemID.BlueFlare, hasFlareGunUpgrade)
                .AddWithCustomValue(ItemID.AngelStatue, Item.buyPrice(gold: 5), Condition.NpcIsPresent(NPCType<Bandit>()));
            }

            if (type == NPCID.DyeTrader)
            {
                shop.Add<DefiledFlameDye>(Condition.Hardmode)
                .AddWithCustomValue(ItemID.DyeTradersScimitar, Item.buyPrice(gold: 15));
            }

            if (type == NPCID.Demolitionist)
            {
                shop.Add<DeepcoreGK2>(Condition.DownedMechBossAny);
            }

            if (type == NPCID.ArmsDealer)
            {
                shop.Add<M1Garand>(Condition.DownedSkeletron)
                .Add<P90>(Condition.Hardmode)
                .AddWithCustomValue(ItemID.Boomstick, Item.buyPrice(gold: 25), Condition.DownedQueenBee)
                .AddWithCustomValue(ItemID.Uzi, Item.buyPrice(gold: 50), Condition.DownedPlantera);
            }

            if (type == NPCID.Stylist)
            {
                shop.Add<StealthHairDye>(CalamityConditions.PlayerHasRogueArmor)
                .Add<WingTimeHairDye>(CalamityConditions.PlayerHasWings)
                .Add<AdrenalineHairDye>(CalamityConditions.InRevengeanceMode)
                .Add<RageHairDye>(CalamityConditions.InRevengeanceMode)
                .AddWithCustomValue(ItemID.StylistKilLaKillScissorsIWish, Item.buyPrice(gold: 15));
            }

            if (type == NPCID.Cyborg)
            {
                shop.Add<MartianDistressRemote>(Condition.DownedGolem)
                .Add<LionHeart>(CalamityConditions.DownedPolterghast);
            }

            if (type == NPCID.Dryad)
            {
                shop.InsertAfter(ItemID.AshGrassSeeds, ItemType<CinderBlossomSeeds>(), Condition.DownedSkeletron)
                // Vanilla sells these in Hardmode, we just make them available at all times
                // Fun fact: Corrupt and Crimson Seeds are sold twice, in different positions!
                // This position is placed over the Graveyard one, and not the Blood Moon one (which is what happens if you insert after Corrupt Seeds). Totally awesome shop database. - Iris
                .InsertAfter(ItemID.GrassWall, ItemID.CorruptSeeds, Condition.CrimsonWorld, Condition.InGraveyard, Condition.PreHardmode)
                .InsertAfter(ItemID.GrassWall, ItemID.CrimsonSeeds, Condition.CorruptWorld, Condition.InGraveyard, Condition.PreHardmode)
                .InsertAfter(ItemID.HallowedGrassEcho, ItemType<AstralGrassSeeds>(), Condition.NotBloodMoon, Condition.Hardmode)
                .AddWithCustomValue(ItemID.JungleRose, Item.buyPrice(gold: 3))
                .AddWithCustomValue(ItemID.NaturesGift, Item.buyPrice(gold: 15))
                .Add<RomajedaOrchid>();
            }

            if (type == NPCID.GoblinTinkerer)
            {
                shop.Add<StatMeter>()
                .Add(ItemID.Toolbox, Condition.NpcIsPresent(NPCID.Mechanic));
            }

            if (type == NPCID.Mechanic)
            {
                shop.AddWithCustomValue(ItemID.BuilderPotion, Item.buyPrice(gold: 2), Condition.HappyEnoughToSellPylons)
                .AddWithCustomValue(ItemID.CombatWrench, Item.buyPrice(gold: 15));
            }

            if (type == NPCID.Clothier)
            {
                shop.Add<CounterScarf>()
                .AddWithCustomValue(ItemID.GoldenKey, Item.buyPrice(gold: 15), Condition.Hardmode)
                .Add<GodSlayerHornedHelm>(CalamityConditions.DownedDevourerOfGods)
                .Add<GodSlayerVisage>(CalamityConditions.DownedDevourerOfGods)
                .Add<SilvaHelm>(CalamityConditions.DownedDevourerOfGods)
                .Add<SilvaHornedHelm>(CalamityConditions.DownedDevourerOfGods)
                .Add<SilvaMask>(CalamityConditions.DownedDevourerOfGods);
            }

            if (type == NPCID.Painter)
            {
                shop.AddWithCustomValue(ItemID.PainterPaintballGun, Item.buyPrice(gold: 15))
                .Add(ItemType<CalamityCanvas2023>())
                .Add(ItemType<CalamityCanvas2024>());
            }

            if (type == NPCID.Steampunker)
            {
                shop.InsertAfter(ItemID.BlueSolution, ItemType<AstralSolution>(), Condition.NotRemixWorld)
                .InsertAfter(ItemID.PurpleSolution, ItemID.PurpleSolution, Condition.InGraveyard, Condition.CrimsonWorld, Condition.NotRemixWorld)
                .InsertAfter(ItemID.RedSolution, ItemID.RedSolution, Condition.InGraveyard, Condition.CorruptWorld, Condition.NotRemixWorld)
                .Add<LucisHairstyle>()
                .Add<LucisMilitaryUniform>()
                .Add<LucisBoots>()
                .Add<LucisSight>();
            }

            if (type == NPCID.Wizard)
            {
                shop.Add<HowlsHeart>()
                .AddWithCustomValue(ItemID.MagicMissile, Item.buyPrice(gold: 25))
                .Add<ResilientCandle>()
                .Add<SpitefulCandle>()
                .Add<VigorousCandle>()
                .Add<WeightlessCandle>();
            }

            if (type == NPCID.WitchDoctor)
            {
                shop.InsertAfter(ItemID.OasisFountain, ItemType<SunkenSeaFountain>())
                .InsertAfter(ItemID.OasisFountain, ItemType<SulphurousFountainItem>())
                .InsertAfter(ItemID.OasisFountain, ItemType<AbyssFountainItem>())
                .InsertAfter(ItemID.OasisFountain, ItemType<AstralFountainItem>())
                .InsertAfter(ItemID.OasisFountain, ItemType<BrimstoneLavaFountainItem>());
            }

            if (type == NPCID.PartyGirl)
            {
                shop.Add(ItemID.GenderChangePotion, Condition.HappyEnoughToSellPylons);
            }

            if (type == NPCID.Princess)
            {
                Mod musicMod = ExternalMods.musicMod;
                musicMod.TryFind("Interlude1MusicBox", out ModItem interlude1Box);
                musicMod.TryFind("Interlude2MusicBox", out ModItem interlude2Box);
                musicMod.TryFind("Interlude3MusicBox", out ModItem interlude3Box);
                musicMod.TryFind("DevourerofGodsEulogyMusicBox", out ModItem eulogyBox);

                shop.InsertAfter(ItemID.MusicBoxCredits, interlude1Box.Type, CalamityConditions.DownedCalamitasClone)
                .InsertAfter(ItemID.MusicBoxCredits, interlude2Box.Type, Condition.DownedMoonLord)
                .InsertAfter(ItemID.MusicBoxCredits, interlude3Box.Type, CalamityConditions.DownedYharon)
                .InsertAfter(ItemID.MusicBoxCredits, eulogyBox.Type, CalamityConditions.DownedDevourerOfGods)
                .AddWithCustomValue(ItemID.PrincessWeapon, Item.buyPrice(platinum: 1), Condition.Hardmode)
                .AddWithCustomValue(ItemType<ForgivenessPainting>(), Item.buyPrice(gold: 15), Condition.NpcIsPresent(NPCType<BrimstoneWitch>()))
                .Add<LanternCenter>();
            }

            if (type == NPCID.SkeletonMerchant)
            {
                shop.InsertAfter(ItemID.HealingPotion, ItemType<CalciumPotion>(), Condition.MoonPhasesHalf0)
                .InsertAfter(ItemID.SpelunkerFlare, ItemID.SpelunkerFlare, spelunkerGlowCondition, hasFlareGunUpgrade)
                .AddWithCustomValue(ItemID.Marrow, Item.buyPrice(gold: 25), Condition.Hardmode, Condition.MoonPhases26) // 26 = half moons
                .AddWithCustomValue<GiantShell>(Item.buyPrice(gold: 15), gibbousMoons)
                .AddWithCustomValue<CrawCarapace>(Item.buyPrice(gold: 15), crescentMoons);
            }

            if (type == NPCID.BestiaryGirl)
            {
                shop.Add(ItemID.EmpressButterfly, bestiaryProgressLacewing);
            }

            if (type == NPCID.Truffle)
            {
                shop.Add<OddMushroom>();
            }
        }
        #endregion
    }
}
