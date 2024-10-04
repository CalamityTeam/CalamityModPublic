using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace CalamityMod.Tiles.Abyss.AbyssAmbient
{
    //
    // Pirate Crate With Glow

    public class PirateCrate1 : ModTile
    {
        protected virtual string GlowAsset => "CalamityMod/Tiles/Abyss/AbyssAmbient/PirateCrate1Glow";
        internal static FramedGlowMask GlowMask;

        public static readonly SoundStyle MineSound = new("CalamityMod/Sounds/Custom/CrateBreak", 3) { Volume = 0.8f };
        public override void SetStaticDefaults()
        {
            if (!string.IsNullOrEmpty(GlowAsset))
            {
                GlowMask = new(GlowAsset, 18, 18);
            }

            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = true;
            Main.tileWaterDeath[Type] = false;
            Main.tileNoAttach[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(97, 69, 52), CalamityUtils.GetText("Tiles.PirateCrate"));
            DustType = DustID.WoodFurniture;
            HitSound = MineSound;

            base.SetStaticDefaults();
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            //explode when broken (troll!)
            //also instakill players in for the worthy
            Vector2 spawnPosition = new(i * 16f + 24f, j * 16f - 4f);
            Projectile.NewProjectile(new EntitySource_WorldEvent(), spawnPosition.X, spawnPosition.Y, 0, 0, ProjectileID.Explosives, Main.getGoodWorld ? 99999 : 130, 0f);
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 2;
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);

            if (GlowMask is not null && GlowMask.HasContentInFramePos(tile.TileFrameX, tile.TileFrameY))
            {
                Vector2 pos = new Vector2(i * 16, j * 16 + 2) - Main.screenPosition + zero;
                Rectangle frame = new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16);
                spriteBatch.Draw(GlowMask.Texture, pos, frame, Color.White);
            }
        }
    }

    public class PirateCrate2 : PirateCrate1
    {
        protected override string GlowAsset => "CalamityMod/Tiles/Abyss/AbyssAmbient/PirateCrate2Glow";
    }

    public class PirateCrate3 : PirateCrate1
    {
        protected override string GlowAsset => "CalamityMod/Tiles/Abyss/AbyssAmbient/PirateCrate3Glow";
    }

    //
    // Pirate Crate Without Glow

    public class PirateCrate4 : ModTile
    {
        public static readonly SoundStyle MineSound = new("CalamityMod/Sounds/Custom/CrateBreak", 3) { Volume = 0.8f };
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = true;
            Main.tileWaterDeath[Type] = false;
            Main.tileNoAttach[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(97, 69, 52), CalamityUtils.GetText("Tiles.PirateCrate"));
            DustType = DustID.WoodFurniture;
            HitSound = MineSound;

            base.SetStaticDefaults();
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            if (Main.rand.NextBool())
                Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 32, 32, ItemID.GoldCoin, Main.rand.Next(1, 2));

            Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 32, 32, ItemID.SilverCoin, Main.rand.Next(45, 75));
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 2;
        }
    }

    public class PirateCrate5 : PirateCrate4
    {
    }

    public class PirateCrate6 : PirateCrate4
    {
    }
}
