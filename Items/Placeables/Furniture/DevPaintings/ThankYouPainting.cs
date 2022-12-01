using CalamityMod.Tiles.Furniture.DevPaintings;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Microsoft.Xna.Framework.Input.Keys;

namespace CalamityMod.Items.Placeables.Furniture.DevPaintings
{
	public class ThankYouPainting : ModItem
	{
		public const int DropInt = 100;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Thank You");
			Tooltip.SetDefault("Thanks to the entire team, everyone who supported, and those who all play the mod and keep it alive!\n" +
			"The confines of this painting is not enough to fit the entire team\n" +
			"Hold SHIFT to see a list of past and current contributors");
            SacrificeTotal = 1;
		}

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
			for (int i = 0; i < contribList.Count; i++)
			{
				tooltip += contribList[i];

				if (i == contribList.Count - 1)
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

        public static IList<string> contribList = new List<string>()
		{
			"Fabsol, the mod's founder and owner", // Fabsol gets a line to himself
			"Altix",
			"AquaSG",
			"Atalya",
			"Ben-TK",
			"Bravioli",
			"CDMusic",
			"Cobalion",
			"Cooper",
			"CrabBar",
			"Daim",
			"Dominic Karma",
			"Ein",
			"Enreden",
			"GramOfSalt",
			"Ian-1KV",
			"IbanPlay",
			"Inanis",
			"Lilac",
			"LordMetarex",
			"MarieArk",
			"Memes",
			"Merkalto",
			"Minecat",
			"Moonburn",
			"Mrrp",
			"Nycro",
			"Ozzatron",
			"Piky",
			"Phupperbat",
			"Popo",
			"RoverdriveX",
			"Runefield",
			"Shade",
			"Shayy",
			"Skeletony",
			"Sok",
			"Spider Prov",
			"spooktacular",
			"StipulateVenus",
			"That Blasterd Basterd",
			"Tinymanx",
			"Tomat",
			"Uberransy",
			"Uncle Danny",
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
			"Blockaroz",
			"Boffin",
			"Cei",
			"Chetto",
			"Chill Dude",
			"DarkTiny",
			"Demik",
			"DM Dokuro",
			"Doog",
			"drh",
			"Earth",
			"EchoDuck",
			"ENNWAY",
			"Epsilon",
			"Fargowilta",
			"Frous",
			"Gahtao",
			"Gamagamer64",
			"Graydee",
			"Grox the Great",
			"Hectique",
			"Huggles",
			"JaceDaDorito",
			"Jenosis",
			"jontchua",
			"Khaelis",
			"KnightyKnight",
			"L0st",
			"Leon",
			"Leviathan",
			"Lompl Allimath",
			"MishiroUsui",
			"Nao",
			"Neverglide",
			"Nincity",
			"Niorin",
			"Nitro",
			"NyctoDarkMatter",
			"PaleoStar",
			"Pbtopacio",
			"Phantasmal Deathray",
			"Pinkie Poss",
			"pixlgray",
			"Poly",
			"President Waluigi",
			"Puff",
			"Purple Necromancer",
			"Sargassum",
			"sentri",
			"Shucks",
			"Silver-Lord of Ash",
			"SixteenInMono",
			"Spoopyro",
			"Svante",
			"Teragat",
			"Terry N. Muse",
			"ThousandFields",
			"TikiWiki",
			"Trivaxy",
			"Vaikyia",
			"Vladimier",
			"Yatagarasu",
			"Yuyutsu",
			"Zach",
			"Ziggums",
		};
	}
}
