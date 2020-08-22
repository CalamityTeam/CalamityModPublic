using CalamityMod.Items.Placeables;
using CalamityMod.TileEntities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace CalamityMod.Tiles
{
    public class DraedonFuelFactory : ModTile
    {
        public const int Width = 4;
        public const int Height = 4;
        public const int CellCreationDelay = 600;
        public const int TotalFrames = 45;
        public override void SetDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = false;
            Main.tileWaterDeath[Type] = false;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
            TileObjectData.newTile.Width = 4;
            TileObjectData.newTile.Height = 4;
            TileObjectData.newTile.Origin = new Point16(0, 3);
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16, 16 };
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(ModContent.GetInstance<TEDraedonFuelFactory>().Hook_AfterPlacement, -1, 0, true);
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.addTile(Type);
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Fuel Factory");
            AddMapEntry(new Color(67, 72, 81), name);
            animationFrameHeight = 68;
        }

        public override bool CanExplode(int i, int j) => false;

        public TEDraedonFuelFactory RetrieveTileEntity(int i, int j)
        {
            // This is very fucking important. ByID and ByPostion can apparently be different and as a result using both together is fucking unreliable.
            int left = i - Main.tile[i, j].frameX % (Width * 18) / 18;
            int top = j - Main.tile[i, j].frameY % (Height * 18) / 18;
            if (!TileEntity.ByID.Any(tileEntity => tileEntity.Value.Position == new Point16(left, top)))
            {
                var factory = ModTileEntity.ConstructFromType(ModContent.TileEntityType<TEDraedonFuelFactory>());
                factory.Position = new Point16(left, top);
                TileEntity.ByID[TileEntity.ByID.Count] = factory;
            }
            return (TEDraedonFuelFactory)TileEntity.ByID.Where(tileEntity => tileEntity.Value.Position == new Point16(left, top)).First().Value;
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, 226);
            return false;
        }

        public override bool HasSmartInteract() => true;

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Item.NewItem(i * 16, j * 16, 32, 32, ModContent.ItemType<DraedonsFuelFactoryItem>());
            int left = i - Main.tile[i, j].frameX % (Width * 18) / 18;
            int top = j - Main.tile[i, j].frameY % (Height * 18) / 18;

            TEDraedonFuelFactory factory = RetrieveTileEntity(i, j);
            if (factory.HeldItem.stack > 0)
            {
                Item.NewItem(new Vector2(i, j) * 16f, factory.HeldItem.type, factory.HeldItem.stack);
            }
            factory.Kill(left, top);
        }

        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            frameCounter++;

            // After CellCreationDelay frames, start the fuel cell generation animation, and make one at the end.
            // Otherwise, just continue waiting.
            if (frameCounter % (CellCreationDelay + TotalFrames * 5 + 5) >= CellCreationDelay)
            {
                if (frameCounter % 5 == 4)
                {
                    frame++;
                    if (frame >= TotalFrames)
                    {
                        frame = 0;
                    }
                }
            }
            else
            {
                frame = TotalFrames - 1;
            }
        }

        public override bool NewRightClick(int i, int j)
        {
            int left = i - Main.tile[i, j].frameX % (Width * 18) / 18;
            int top = j - Main.tile[i, j].frameY % (Height * 18) / 18;

            Player player = Main.LocalPlayer;
            TEDraedonFuelFactory factory = RetrieveTileEntity(i, j);
            Main.mouseRightRelease = false;

            if (player.sign >= 0)
            {
                Main.PlaySound(SoundID.MenuClose);
                player.sign = -1;
                Main.editSign = false;
                Main.npcChatText = "";
            }
            if (Main.editChest)
            {
                Main.PlaySound(SoundID.MenuTick);
                Main.editChest = false;
                Main.npcChatText = "";
            }
            if (player.Calamity().CurrentlyViewedFactory == factory)
            {
                player.Calamity().CurrentlyViewedFactory = null;
                player.Calamity().CurrentlyViewedFactoryX = player.Calamity().CurrentlyViewedFactoryY = -1;
                Main.PlaySound(SoundID.MenuClose);
            }
            else
            {
                // Ensure that the UI position is always centered and above the tile.
                player.Calamity().CurrentlyViewedFactoryX = left * 16 + 16;
                player.Calamity().CurrentlyViewedFactoryY = top * 16;

                player.Calamity().CurrentlyViewedFactory = factory;

                Main.playerInventory = true;
                Main.recBigList = false;
                Main.PlaySound(player.Calamity().CurrentlyViewedFactory == null ? SoundID.MenuOpen : SoundID.MenuTick);
            }

            Recipe.FindRecipes();
            return true;
        }
        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile trackTile = Main.tile[i, j];
            int xPos = Main.tile[i, j].frameX;
            int yPos = Main.tile[i, j].frameY;
            xPos += Main.tileFrame[trackTile.type] / 15 * 72;
            yPos += Main.tileFrame[trackTile.type] % 15 * 72;
            Texture2D glowmask = ModContent.GetTexture("CalamityMod/Tiles/DraedonFuelFactory");
            Vector2 offset = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
            Vector2 drawOffset = new Vector2(i * 16 - Main.screenPosition.X, j * 16 - Main.screenPosition.Y) + offset;
            Color drawColor = Lighting.GetColor(i, j);

            if (!trackTile.halfBrick() && trackTile.slope() == 0)
                spriteBatch.Draw(glowmask, drawOffset, new Rectangle(xPos, yPos, 16, 16), drawColor, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
            else if (trackTile.halfBrick())
                spriteBatch.Draw(glowmask, drawOffset + Vector2.UnitY * 8f, new Rectangle(xPos, yPos, 16, 16), drawColor, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
            return false;
        }
    }
}
