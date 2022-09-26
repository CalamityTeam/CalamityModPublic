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
			"Hold SHIFT to see a list of past and current devs and CTRL to see past and current testers");
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
			if (!Main.keyState.IsKeyDown(LeftShift) && !Main.keyState.IsKeyDown(LeftControl))
				return;

			bool tester = false;
			if (Main.keyState.IsKeyDown(LeftControl))
				tester = true;

			string tooltip = "";

			if (!tester)
			{
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
			}
			else
			{
				int namesPerLine = 5;
				for (int i = 0; i < testerList.Count; i++)
				{
					tooltip += testerList[i];

					if (i == testerList.Count - 1)
						break;

					if (i % namesPerLine == namesPerLine - 1)
						tooltip += "\n";
					else
						tooltip += ", ";
				}
			}

			TooltipLine line = tooltips.FirstOrDefault(x => x.Mod == "Terraria" && x.Name == "Tooltip2");
			if (line != null)
				line.Text = tooltip;
		}

        public IList<string> devList = new List<string>()
		{
			"Fabsol, the mod's founder and owner", // Fabsol gets a line to himself
			"Alterra",
			"Ben-TK",
			"Bravioli",
			"CDMusic",
			"Cobalion",
			"Cooper",
			"Daim",
			"Dominic Karma",
			"Enreden",
			"GramOfSalt",
			"IbanPlay",
			"Inanis",
			"Lilac",
			"LordMetarex",
			"MarieArk",
			"Merkalto",
			"Minecat",
			"Mrrp",
			"Nycro",
			"Ozzatron",
			"Piky",
			"Phupperbat",
			"Popo",
			"RoverdriveX",
			"Runefield",
			"Skeletony",
			"Sok",
			"spooktacular",
			"Tinymanx",
			"Tomat",
			"Uncle Danny",
			"YuH",

			"ApidemDragon",
			"Amadis",
			"Aleksh",
			"AstroKnight",
			"Blockaroz",
			"Boffin",
			"DarkTiny",
			"DM Dokuro",
			"Earth",
			"EchoDuck",
			"ENNWAY",
			"Frous",
			"Gahtao",
			"Gamagamer64",
			"Graydee",
			"Grox the Great",
			"Huggles",
			"JaceDaDorito",
			"Jenosis",
			"jontchua",
			"Khaelis",
			"L0st",
			"Leviathan",
			"MishiroUsui",
			"Nao",
			"Nincity",
			"Niorin",
			"Nitro",
			"NyctoDarkMatter",
			"PaleoStar",
			"Pbtopacio",
			"Phantasmal Deathray",
			"PhoenixBlade",
			"Pinkie Poss",
			"pixlgray",
			"Poly",
			"President Waluigi",
			"Puff",
			"Purple Necromancer",
			"sentri",
			"Shucks",
			"SixteenInMono",
			"Spoopyro",
			"Svante",
			"Terry N. Muse",
			"Trivaxy",
			"Turquoise",
			"Vaikyia",
			"Vladimier",
			"Yuyutsu",
			"Zach",
			"Ziggums",
		};

        public IList<string> testerList = new List<string>()
		{
			"Afzofa",
			"Altix",
			"AquaSG",
			"Atalya",
			"Blast",
			"CrabBar",
			"Demik",
			"Ein",
			"Epsilon",
			"Fargowilta",
			"Ian-1KV",
			"Leon",
			"Lompl Allimath",
			"Memes",
			"Shayy",
			"StipulateVenus",
			"Storm2103",
			"That Blasterd Basterd",
			"TikiWiki",
			"Uberransy",
			"Spider Prov",
			"Xyk",

			"Akeeli",
			"Alphi",
			"Chetto",
			"Chill Dude",
			"Doog",
			"drh",
			"Hectique",
			"Lauren",
			"Sargassum",
			"ThousandFields",
			"Teragat",
			"Yatagarasu",
		};

	}
}
