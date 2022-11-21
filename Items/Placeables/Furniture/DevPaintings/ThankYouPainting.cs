﻿using CalamityMod.Tiles.Furniture.DevPaintings;
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

        public static IList<string> devList = new List<string>()
		{
			"Fabsol, the mod's founder and owner", // Fabsol gets a line to himself
			"Altix",
			"Ben-TK",
			"Bravioli",
			"CDMusic",
			"Cobalion",
			"Cooper",
			"CrabBar",
			"Daim",
			"Dominic Karma",
			"Enreden",
			"GramOfSalt",
			"Ian-1KV",
			"IbanPlay",
			"Inanis",
			"Lilac",
			"LordMetarex",
			"MarieArk",
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
			"Shayy",
			"Skeletony",
			"Sok",
			"Spider Prov",
			"spooktacular",
			"That Blasterd Basterd",
			"Tinymanx",
			"Tomat",
			"Uncle Danny",
			"Xyk",
			"YuH",

			"Altalyra",
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
			"KnightyKnight",
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
			"Pinkie Poss",
			"pixlgray",
			"Poly",
			"President Waluigi",
			"Puff",
			"Purple Necromancer",
			"sentri",
			"Shucks",
			"Silver-Lord of Ash",
			"SixteenInMono",
			"Spoopyro",
			"Svante",
			"Terry N. Muse",
			"Trivaxy",
			"Vaikyia",
			"Vladimier",
			"Yuyutsu",
			"Zach",
			"Ziggums",
		};

        public static IList<string> testerList = new List<string>()
		{
			"AquaSG",
			"Atalya",
			"Blast",
			"Demik",
			"Ein",
			"Epsilon",
			"Fargowilta",
			"Leon",
			"Memes",
			"StipulateVenus",
			"Shade",
			"Uberransy",

			"Afzofa",
			"Akeeli",
			"Alphi",
			"Cei",
			"Chetto",
			"Chill Dude",
			"Doog",
			"drh",
			"Hectique",
			"Lompl Allimath",
			"Neverglide",
			"Sargassum",
			"ThousandFields",
			"Teragat",
			"TikiWiki",
			"Yatagarasu",
		};

	}
}
