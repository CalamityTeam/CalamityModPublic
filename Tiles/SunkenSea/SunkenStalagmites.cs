using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Enums;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace CalamityMod.Tiles.SunkenSea
{
	public class SunkenStalagmites : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2);
            TileObjectData.addTile(Type);
            DustType = 253;
            ModTranslation name = CreateMapEntryName();
            AddMapEntry(new Color(31, 92, 114));
            MineResist = 3f;

            base.SetStaticDefaults();
		}
	}
}