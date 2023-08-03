using CalamityMod.Projectiles.Environment;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ModLoader;
using Terraria.ObjectData;
using System;

namespace CalamityMod.Tiles.Abyss.AbyssAmbient
{
    public class MassiveRarePearl : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = true;
            Main.tileWaterDeath[Type] = false;
            Main.tileNoAttach[Type] = true;
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x1);
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(106, 80, 102));
            DustType = 33;

            base.SetStaticDefaults();
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY) 
		{
            if (Main.rand.NextBool(5))
			    Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 32, 32, ItemID.PinkPearl);
            else
                Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 32, 32, ItemID.WhitePearl);
        }
    }
}
