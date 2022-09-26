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
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Thank You");
			Tooltip.SetDefault("Thanks to the entire team, everyone who supported, and those who all play the mod and keep it alive!\n" +
			"The confines of this painting is not enough to fit the entire team\n" +
			"Hold SHIFT to see a list of past and current devs");
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
			Item.value = Item.buyPrice(0, 5, 0, 0);;
			Item.rare = ItemRarityID.Blue;
			Item.createTile = ModContent.TileType<ThankYouPaintingTile>();
		}

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
			if (!Main.keyState.IsKeyDown(LeftShift))
				return;

			string devString = "";
			for (int i = 0; i < devList.Count; i++)
			{
				devString += devList[i];

				if (i == devList.Count - 1)
					break;

				if (i % 5 == 0)
					devString += "\n";
				else
					devString += ", ";
			}

			TooltipLine line = tooltips.FirstOrDefault(x => x.Mod == "Terraria" && x.Name == "Tooltip2");
			if (line != null)
				line.Text = devString;
		}

        public IList<string> devList = new List<string>()
		{
			"Fabsol",
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
			"Blockaroz",
			"Boffin",
			"DarkTiny",
			"DM Dokuro",
			"Earth",
			"EchoDuck",
			"Frous",
			"Gahtao",
			"Gamagamer64",
			"President Waluigi",
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
			"Puff",
			"Purple Necromancer",
			"sentri",
			"Shadow",
			"Shucks",
			"SixteenInMono",
			"Spoopyro",
			"Svante",
			"Terry N. Muse",
			"Triactis",
			"Trivaxy",
			"Turquoise",
			"Vaikyia",
			"Vladimier",
			"Yuyutsu",
			"Zach",
			"Ziggums"
		};

	}
}
