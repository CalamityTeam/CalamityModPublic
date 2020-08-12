using CalamityMod.Items.Placeables;
using CalamityMod.TileEntities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace CalamityMod.Tiles
{
    public class DraedonTurretTile : ModTile
    {
        public const int Width = 3;
        public const int Height = 2;
        public override void SetDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = true;
            Main.tileWaterDeath[Type] = false;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(ModContent.GetInstance<TEDraedonTurret>().Hook_AfterPlacement, -1, 0, true);
            TileObjectData.addTile(Type);
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Turret");
            AddMapEntry(new Color(67, 72, 81), name);
            soundType = SoundID.Item;
            soundStyle = 14;
            minPick = 55;
        }

        public override bool CanExplode(int i, int j) => false;

        public TEDraedonTurret RetrieveTileEntity(int i, int j)
        {
            int left = i - Main.tile[i, j].frameX % (Width * 18) / 18;
            int top = j - Main.tile[i, j].frameY % (Height * 18) / 18;
            int determinedID = ModContent.GetInstance<TEDraedonTurret>().Find(left, top);
            if (determinedID == -1 || !(TileEntity.ByID[determinedID] is TEDraedonTurret))
            {
                ModTileEntity modTileEntity = ModTileEntity.ConstructFromType(ModContent.GetInstance<TEDraedonTurret>().type);
                modTileEntity.Position = new Point16(left, top);
                modTileEntity.ID = TileEntity.AssignNewID();
                modTileEntity.type = ModContent.GetInstance<TEDraedonTurret>().type;
                TileEntity.ByID[modTileEntity.ID] = modTileEntity;
                TileEntity.ByPosition[modTileEntity.Position] = modTileEntity;
                determinedID = modTileEntity.ID;
            }
            return (TEDraedonTurret)TileEntity.ByID[determinedID];
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
            int left = i - Main.tile[i, j].frameX % (Width * 18) / 18;
            int top = j - Main.tile[i, j].frameY % (Height * 18) / 18;

            RetrieveTileEntity(i, j).Kill(left, top);
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile trackTile = Main.tile[i, j];
            if (trackTile.frameX != 36 || trackTile.frameY != 0)
                return;

            // This is done so that the turret has priority over, say, trees when drawing.
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Texture, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.Default, RasterizerState.CullNone);

            TEDraedonTurret charger = RetrieveTileEntity(i, j);
            Color drawColor = Lighting.GetColor(i, j);

            Texture2D drawTexture = ModContent.GetTexture("CalamityMod/Projectiles/DraedonsArsenal/PulseTurret");
            Vector2 screenOffset = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
            Vector2 drawOffset = new Vector2(i * 16 - Main.screenPosition.X, j * 16 - Main.screenPosition.Y) + screenOffset;
            drawOffset.Y -= 2f;
            drawOffset.X += charger.Direction == -1 ? -10 : 2;
            float adjustedAngle = charger.Rotation;
            spriteBatch.Draw(drawTexture, drawOffset, null, drawColor, adjustedAngle, drawTexture.Size() * 0.5f, 1f, charger.Direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0.0f);
        }
    }
}
