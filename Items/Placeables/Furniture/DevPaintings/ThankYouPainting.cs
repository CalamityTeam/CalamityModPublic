using System.Collections.Generic;
using System.Linq;
using CalamityMod.Tiles.Furniture.DevPaintings;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Microsoft.Xna.Framework.Input.Keys;

namespace CalamityMod.Items.Placeables.Furniture.DevPaintings
{
    public class ThankYouPainting : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Placeables";
        public const int DropInt = 100;

        public override void SetDefaults()
        {
            Item.width = 96;
            Item.height = 64;
            Item.maxStack = 9999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.value = Item.buyPrice(0, 2, 0, 0); ;
            Item.rare = ItemRarityID.White;
            Item.createTile = ModContent.TileType<ThankYouPaintingTile>();
            Item.Calamity().donorItem = true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (!Main.keyState.IsKeyDown(LeftShift))
                return;

            string tooltip = "";

            int namesPerLine = 5;
            for (int i = 0; i < devList.Count; i++)
            {
                tooltip += devList[i];

                if (i == devList.Count - 1)
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

        public static IList<string> devList = new List<string>()
        {
			"Altix",
            "apotofkoolaid",
            "AquaSG",
            "Atalya",
            "Ben-TK",
            "Big E",
            "CDMusic",
            "Cei",
            "CongratsIsTrash",
            "Cooper",
            "CosmaticMango",
            "CrabBar",
            "Dandy",
            "Dia",
            "Done",
            "dozezoze",
            "Eddie Spaghetti",
            "ENNWAY",
            "Flowaria",
            "Fluffy",
            "fryzahh",
            "HaguriHat",
            "Heart Plus Up!",
            "LordMetarex",
            "Memes",
            "Mercutio 'Merkalto' Takle",
            "Mishiro Usui",
            "Moonburn",
            "Nycro",
            "Ozzatron",
            "Piky",
            "Poroboros",
            "PokerFace",
            "Raesh",
            "Sagittariod",
            "Shade",
            "Shayy",
            "Spider Prov",
            "StipulateVenus",
            "Sunny",
            "Tobias",
            "Tomat",
            "Triangle",
            "TYESKI (Universe)",
            "Uncle Danny",
            "Xyk",
            "YuH",
            // Former devs
			"Afzofa",
            "AdipemDragon",
            "Akeeli",
            "Aleksh",
            "Alphi",
            "Altalyra",
            "Amadis",
            "AstroKnight",
            "Blast",
            "Blastitle",
            "Blockaroz",
            "Boffin",
            "Bravioli",
            "Chetto",
            "Chill Dude",
            "Cobalion",
            "Daim",
            "DarkTiny",
            "Demik",
            "DM Dokuro",
            "Doog",
            "drh",
            "dwshin",
            "DylanDoe21",
            "Earth",
            "EchoDuck",
            "Ein",
            "enamoured",
            "Enreden",
            "Epsilon",
            "Fargowilta",
            "Frous",
            "Gahtao",
            "Gamagamer64",
            "GramOfSalt",
            "Graydee",
            "Grox the Great",
            "Hectique",
            "Hugekraken",
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
            "Mihaii",
            "Minecat",
            "Mrrp",
            "Nao",
            "Neverglide",
            "Nincity",
            "Niorin",
            "Nitro",
            "NyctoDarkMatter",
            "PaleoStar",
            "Pbtopacio",
            "Phantasmal Deathray",
            "Phupperbat",
            "Pinkie Poss",
            "Poly",
            "Popo",
            "President Waluigi",
            "Puff",
            "Purple Necromancer",
            "RoverdriveX",
            "Runefield",
            "Sargassum",
            "sentri",
            "SharZz",
            "Shucks",
            "Silver-Lord of Ash",
            "SixteenInMono",
            "Skeletony",
            "Sok",
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
            "Uberransy",
            "Vaikyia",
            "Vladimier",
            "Yatagarasu",
            "Yuyutsu",
            "Zach",
            "Ziggums",
        };
    }
}
