using CalamityMod.Items.DraedonMisc;
using CalamityMod.TileEntities;
using CalamityMod.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.Audio;
using ReLogic.Content;
using CalamityMod.UI.DraedonSummoning;

namespace CalamityMod.Tiles.DraedonSummoner
{
    public class CodebreakerTile : ModTile
    {
        public const int Width = 5;
        public const int Height = 8;
        public const int OriginOffsetX = 2;
        public const int OriginOffsetY = 7;
        public const int SheetSquare = 18;
        public static Texture2D TileTexture;
        public static Texture2D ComputerTexture;
        public static Texture2D SensorTexture;
        public static Texture2D DisplayTexture;
        public static Texture2D VoltageRegulatorTexture;
        public static Texture2D VoltageRegulatorTexture2;
        public static Texture2D CoolingCellTexture;

        public override void SetStaticDefaults()
        {
            if (!Main.dedServ)
            {
                TileTexture = ModContent.Request<Texture2D>("CalamityMod/Tiles/DraedonSummoner/CodebreakerTile", AssetRequestMode.ImmediateLoad).Value;
                ComputerTexture = ModContent.Request<Texture2D>("CalamityMod/Tiles/DraedonSummoner/CodebreakerDecryptionComputer", AssetRequestMode.ImmediateLoad).Value;
                SensorTexture = ModContent.Request<Texture2D>("CalamityMod/Tiles/DraedonSummoner/CodebreakerLongRangedSensorArray", AssetRequestMode.ImmediateLoad).Value;
                DisplayTexture = ModContent.Request<Texture2D>("CalamityMod/Tiles/DraedonSummoner/CodebreakerAdvancedDisplay", AssetRequestMode.ImmediateLoad).Value;
                VoltageRegulatorTexture = ModContent.Request<Texture2D>("CalamityMod/Tiles/DraedonSummoner/CodebreakerVoltageRegulationSystem", AssetRequestMode.ImmediateLoad).Value;
                VoltageRegulatorTexture2 = ModContent.Request<Texture2D>("CalamityMod/Tiles/DraedonSummoner/CodebreakerVoltageRegulationSystem2", AssetRequestMode.ImmediateLoad).Value;
                CoolingCellTexture = ModContent.Request<Texture2D>("CalamityMod/Tiles/DraedonSummoner/CodebreakerAuricQuantumCoolingCell", AssetRequestMode.ImmediateLoad).Value;
            }

            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = false;
            Main.tileWaterDeath[Type] = false;

            // Various data sets to protect this tile from unintentional death
            TileID.Sets.PreventsTileRemovalIfOnTopOfIt[Type] = true;
            //TileID.Sets.PreventsTileReplaceIfOnTopOfIt[Type] = true; Since this is a furniture item this may be unnecessary?
            TileID.Sets.PreventsSandfall[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
            TileObjectData.newTile.Width = Width;
            TileObjectData.newTile.Height = Height;
            TileObjectData.newTile.Origin = new Point16(OriginOffsetX, OriginOffsetY);
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.CoordinateHeights = new int[Height];

            // Initialize the coordinate heights of each frame based on the sheet area. The padding is shaved off.
            for (int i = 0; i < Height; i++)
                TileObjectData.newTile.CoordinateHeights[i] = SheetSquare - 2;

            TileObjectData.newTile.LavaDeath = false;

            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(ModContent.GetInstance<TECodebreaker>().Hook_AfterPlacement, -1, 0, true);

            TileObjectData.addTile(Type);
            AddMapEntry(new Color(92, 107, 112), CreateMapEntryName());
            AnimationFrameHeight = 144;
        }

        public override bool CanExplode(int i, int j) => false;

        // Prevent the tile from being destroyed while it's busy decrypting.
        // If it's destroyed the tile entity would be too and the resources used on decryption would be lost for nothing.
        public override bool CanKillTile(int i, int j, ref bool blockDamaged)
        {
            TECodebreaker codebreakerTileEntity = CalamityUtils.FindTileEntity<TECodebreaker>(i, j, Width, Height, SheetSquare);
            if (codebreakerTileEntity is null)
                return true;
            if (codebreakerTileEntity.DecryptionCountdown > 0)
                return false;
            return true;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.18f;
            g = 0.8f;
            b = 0.9f;
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, 182);
            return false;
        }

        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Tile t = Main.tile[i, j];
            int left = i - t.TileFrameX % (Width * SheetSquare) / SheetSquare;
            int top = j - t.TileFrameY % (Height * SheetSquare) / SheetSquare;

            TECodebreaker codebreakerTileEntity = CalamityUtils.FindTileEntity<TECodebreaker>(i, j, Width, Height, SheetSquare);

            // Drop any any attached components.
            codebreakerTileEntity?.DropConstituents(i, j);

            // And destroy the tile entity, if it exists.
            codebreakerTileEntity?.Kill(left, top);
        }

        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;

        public override bool RightClick(int i, int j)
        {
            TECodebreaker codebreakerTileEntity = CalamityUtils.FindTileEntity<TECodebreaker>(i, j, Width, Height, SheetSquare);
            Player player = Main.LocalPlayer;
            player.CancelSignsAndChests();

            // If this is the tile the player is currently looking at, the associated tile entity doesn't really exist, or it's simply not upgraded enough, close the GUI.
            if (codebreakerTileEntity is null || codebreakerTileEntity.ID == CodebreakerUI.ViewedTileEntityID || !codebreakerTileEntity.ContainsDecryptionComputer)
            {
                // Create a warning if someone attempts to click on a codebreaker without a computer with which to open the UI.
                if (!codebreakerTileEntity.ContainsDecryptionComputer)
                    CombatText.NewText(player.Hitbox, Color.Cyan, CalamityUtils.GetTextValue("Misc.NoComputer"));

                CodebreakerUI.ViewedTileEntityID = -1;
                SoundEngine.PlaySound(SoundID.MenuClose);
            }

            // Otherwise, open the decryption interface when it exists. This can be either opening the GUI from nothing, or just opening a separate codebreaker.
            else if (codebreakerTileEntity != null)
            {
                // Play a sound depending on whether the player had another codebreaker open previously.
                SoundEngine.PlaySound(CodebreakerUI.ViewedTileEntityID == -1 ? SoundID.MenuOpen : SoundID.MenuTick);
                CodebreakerUI.ViewedTileEntityID = codebreakerTileEntity.ID;
                Main.playerInventory = true;
                Main.recBigList = false;
            }

            Recipe.FindRecipes();
            return true;
        }

        // All tile drawcode is done manually because the tile's animation is controlled by a tile entity.
        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            // These offsets start as the tile offsets, i.e. which sub-tile of the FrameImportant structure this specific location is.
            Tile t = Main.tile[i, j];
            int left = i - t.TileFrameX % (Width * SheetSquare) / SheetSquare;
            int frameXPos = t.TileFrameX;
            int frameYPos = t.TileFrameY + Height * SheetSquare * (int)((Main.GlobalTimeWrappedHourly * 12f + left) % 8);

            // Grab the tile entity because it stores the information regarding what is actually attached.
            TECodebreaker codebreakerTileEntity = CalamityUtils.FindTileEntity<TECodebreaker>(i, j, Width, Height, SheetSquare);

            Vector2 offset = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
            if (t.IsHalfBlock)
                offset.Y += 8f;

            Vector2 drawPosition = new Vector2(i * 16 - Main.screenPosition.X, j * 16 - Main.screenPosition.Y) + offset;
            Color drawColor = Lighting.GetColor(i, j);
            Rectangle frame = new Rectangle(frameXPos, frameYPos, 16, 16);

            if ((!t.IsHalfBlock && t.Slope == 0) || t.IsHalfBlock)
            {
                spriteBatch.Draw(TileTexture, drawPosition, frame, drawColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

                // Place secondary parts.
                if (codebreakerTileEntity != null)
                {
                    if (codebreakerTileEntity.ContainsDecryptionComputer)
                        spriteBatch.Draw(ComputerTexture, drawPosition, frame, drawColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                    if (codebreakerTileEntity.ContainsVoltageRegulationSystem)
                        spriteBatch.Draw(VoltageRegulatorTexture2, drawPosition, frame, drawColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                    if (codebreakerTileEntity.ContainsSensorArray)
                        spriteBatch.Draw(SensorTexture, drawPosition, frame, drawColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                    if (codebreakerTileEntity.ContainsCoolingCell)
                        spriteBatch.Draw(CoolingCellTexture, drawPosition, frame, drawColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                    if (codebreakerTileEntity.ContainsVoltageRegulationSystem)
                        spriteBatch.Draw(VoltageRegulatorTexture, drawPosition, frame, drawColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                    if (codebreakerTileEntity.ContainsAdvancedDisplay)
                        spriteBatch.Draw(DisplayTexture, drawPosition, frame, drawColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                }
            }
            return false;
        }
    }
}
