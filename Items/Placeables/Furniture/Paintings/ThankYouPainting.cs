using System.Collections.Generic;
using System.Linq;
using CalamityMod.Tiles.Furniture.Paintings;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables.Furniture.Paintings
{
    public class ThankYouPainting : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Placeables";
        public const int DropInt = 100;
        public static bool holdShift = true;
        public static bool showingFormerDevs = true;

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<ThankYouPaintingTile>());
            Item.width = 96;
            Item.height = 64;
            Item.value = Item.sellPrice(silver: 40);
            Item.Calamity().donorItem = true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                if (!holdShift)
                {
                    holdShift = true;
                    showingFormerDevs = !showingFormerDevs;
                }

                string tooltip = "--------\n";
                int namesPerLine = 7;
                IList<string> listToPullFrom = showingFormerDevs ? formerDevList : currentDevList;

                for (int i = 1; i <= listToPullFrom.Count; i++)
                {
                    tooltip += listToPullFrom[i - 1];

                    if (i == listToPullFrom.Count)
                        break;

                    if (i % namesPerLine == 0)
                        tooltip += "\n";
                    else
                        tooltip += ", ";
                }

                TooltipLine line = tooltips.FirstOrDefault(x => x.Mod == "Terraria" && x.Name == "Tooltip2");
                if (line != null)
                    line.Text = tooltip;
            }
            else
                holdShift = false;
        }

        public static IList<string> currentDevList = new List<string>()
        {
            "Altixal",
            "apotofkoolaid",
            "ArchonSystem",
            "Atalya",
            "Ben-TK",
            "Big E",
            "carnymassacre",
            "CDMusic",
            "CongratsIsTrash",
            "Cooper",
            "CosmaticMango",
            "CrabBar",
            "Critaquil",
            "Dandy",
            "Done",
            "dozezoze",
            "Flowaria",
            "Fluffy",
            "fryzahh",
            "Gpscorpion",
            "Hugekraken",
            "jasper",
            "LordMetarex",
            "Moonburn",
            "Ozzatron",
            "PokerFace",
            "Raesh",
            "Sagittariod",
            "sixtydegrees",
            "StipulateVenus",
            "Sunny",
            "_tofu",
            "Tomat",
            "Triangle",
            "TYESKI",
            "Xyk",
            "YuH",
        };

        public static IList<string> formerDevList = new List<string>()
        {
            "Afzofa",
            "AdipemDragon",
            "Akeeli",
            "Aleksh",
            "Alphi",
            "Altalyra",
            "Amadis",
            "AquaSG",
            "AstroKnight",
            "Blast",
            "Blastitle",
            "Blockaroz",
            "Boffin",
            "Bravioli",
            "Cei",
            "Chetto",
            "Chill Dude",
            "Cobalion",
            "Daim",
            "DarkTiny",
            "Demik",
            "Dia",
            "DM Dokuro",
            "Doog",
            "drh",
            "dwshin",
            "DylanDoe21",
            "Earth",
            "EchoDuck",
            "Eddie Spaghetti",
            "Ein",
            "enamoured",
            "Enreden",
            "ENNWAY",
            "Epsilon",
            "Fargowilta",
            "Frous",
            "Gahtao",
            "Gamagamer64",
            "GramOfSalt",
            "Graydee",
            "Grox the Great",
            "Heart Plus Up!",
            "Hectique",
            "Huggles",
            "Ian-1KV",
            "IbanPlay",
            "Inanis",
            "JaceDaDorito",
            "Jenosis",
            "jontchua",
            "Khaelis",
            "KnightyKnight",
            "L0st",
            "Leon",
            "Leviathan",
            "Lilac Olligoci",
            "Lompl Allimath",
            "Lucille Karma",
            "MarieArk",
            "math2",
            "Mercutio 'Merkalto' Takle",
            "Mihaii",
            "Minecat",
            "Mishiro Usui",
            "Mrrp",
            "Nao",
            "Neverglide",
            "Nincity",
            "Niorin",
            "Nitro",
            "Nycro",
            "NyctoDarkMatter",
            "PaleoStar",
            "Pbtopacio",
            "Phantasmal Deathray",
            "Phupperbat",
            "Piky",
            "Pinkie Poss",
            "Poly",
            "Popo",
            "Poroboros",
            "President Waluigi",
            "Puff",
            "Purple Necromancer",
            "RoverdriveX",
            "Runefield",
            "Sargassum",
            "sentri",
            "Shade",
            "SharZz",
            "Shucks",
            "Silver-Lord of Ash",
            "SixteenInMono",
            "Skeletony",
            "Sok",
            "Spider Prov",
            "spooktacular",
            "Spoopyro",
            "Svante",
            "Sylvium",
            "Teragat",
            "Terry N. Muse",
            "ThousandFields",
            "TikiWiki",
            "Tinymanx",
            "Trivaxy",
            "Tobias",
            "Uberransy",
            "Uncle Danny",
            "Vaikyia",
            "Vladimier",
            "Yatagarasu",
            "Yuyutsu",
            "Zach",
            "Ziggums",
        };
    }
}
