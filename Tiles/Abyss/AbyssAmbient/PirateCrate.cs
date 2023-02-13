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
    public class PirateCrate1 : ModTile
    {
        public static readonly SoundStyle MineSound = new("CalamityMod/Sounds/Custom/CrateBreak", 3) { Volume = 0.8f };
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = true;
            Main.tileWaterDeath[Type] = false;
            Main.tileNoAttach[Type] = true;
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.addTile(Type);
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Abandoned Crate");
            AddMapEntry(new Color(97, 69, 52), name);
            DustType = DustID.WoodFurniture;
            HitSound = MineSound;

            base.SetStaticDefaults();
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY) 
		{
			Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 32, 32, ItemID.GoldCoin, Main.rand.Next(1, 2));
            Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 32, 32, ItemID.SilverCoin, Main.rand.Next(45, 75));
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 2;
        }
    }

    public class PirateCrate2 : PirateCrate1
    {
    }

    public class PirateCrate3 : PirateCrate1
    {
    }

    public class PirateCrate4 : PirateCrate1
    {
    }

    public class PirateCrate5 : PirateCrate1
    {
    }

    public class PirateCrate6 : PirateCrate1
    {
    }
}
