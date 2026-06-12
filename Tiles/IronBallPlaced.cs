using System;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Pets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace CalamityMod.Tiles
{
    public class IronBallPlaced : ModTile
    {
        public static Asset<Texture2D> GlowTexture = null;

        public override void Load()
        {
            GlowTexture = ModContent.Request<Texture2D>("CalamityMod/Tiles/IronBallPlaced_Glow");
        }

        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileLighted[Type] = true;
            Main.tileSpelunker[Type] = true;
            Main.tileOreFinderPriority[Type] = 770; // Same as Chillet

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.addTile(Type);

            DustType = DustID.Silver;

            AddMapEntry(new Color(108, 118, 134), CalamityUtils.GetText("Tiles.IronBall"));
            RegisterItemDrop(ModContent.ItemType<IronBall>());
            FlexibleTileWand.RubblePlacementSmall.AddVariations(ModContent.ItemType<IronBall>(), Type, 0);
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            // Dim flickering red light
            r = MathF.Sin(Main.GlobalTimeWrappedHourly * 20) * 0.05f + 0.3f;
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(GlowTexture.Value, new Vector2(i * 16 - Main.screenPosition.X, j * 16 - Main.screenPosition.Y) + CalamityUtils.TileDrawOffset, new Rectangle(Main.tile[i, j].TileFrameX, Main.tile[i, j].TileFrameY, 16, 16), Color.White);
        }
    }
}
