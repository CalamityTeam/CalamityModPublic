using CalamityMod.Tiles.Furniture.DevPaintings;
using System.Collections.Generic;
using System.Linq;
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
			Item.value = Item.buyPrice(0, 2, 0, 0);;
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
			"Fabsol, the mod's founder and owner", // Fabsol gets a line to himself
			"Altix",
			"Angel",
			"apotofkoolaid",
			"AquaSG",
			"Atalya",
			"Ben-TK",
			"Cei",
			"CongratsIsTrash",
			"Cooper",
			"CosmaticMango",
			"CrabBar",
			"Dia",
			"Done",
			"DylanDoe21",
			"Fluffi",
			"HaguriHat",
			"Heart Plus Up!",
			"Hugekraken",
			"Lilac Olligoci",
			"LordMetarex",
			"Memes",
			"Mercutio 'Merkalto' Takle",
			"Mishiro Usui",
			"Moonburn",
			"Mr.Small",
			"nalyddd",
			"Nycro",
			"Ozzatron",
			"Piky",
			"PokerFace",
			"Shade",
			"SharZz",
			"Shayy",
			"Spider Prov",
			"StipulateVenus",
			"Tomat",
			"Triangle",
			"Uberransy",
			"Xyk",
			"YuH",

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
			"CDMusic",
			"Chetto",
			"Chill Dude",
			"Cobalion",
			"Daim",
			"DarkTiny",
			"Demik",
			"DM Dokuro",
			"Dominic Karma",
			"Doog",
			"drh",
			"dwshin",
			"Earth",
			"EchoDuck",
			"Ein",
			"ENNWAY",
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
			"Lompl Allimath",
			"MarieArk",
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
			"pixlgray",
			"Poly",
			"Popo",
			"President Waluigi",
			"Puff",
			"Purple Necromancer",
			"RoverdriveX",
			"Runefield",
			"Sargassum",
			"sentri",
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
