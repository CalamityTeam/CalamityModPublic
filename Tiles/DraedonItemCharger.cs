using CalamityMod.Items.Placeables;
using CalamityMod.TileEntities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace CalamityMod.Tiles
{
    public class DraedonItemCharger : ModTile
    {
        public const int Width = 3;
        public const int Height = 2;
        public override void SetDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = false;
            Main.tileWaterDeath[Type] = false;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(ModContent.GetInstance<TEDraedonItemCharger>().Hook_AfterPlacement, -1, 0, true);
            TileObjectData.addTile(Type);
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Item Charger");
            AddMapEntry(new Color(67, 72, 81), name);
        }

        public override bool CanExplode(int i, int j) => false;

        public TEDraedonItemCharger RetrieveTileEntity(int i, int j)
        {
            // This is very fucking important. ByID and ByPostion can apparently be different and as a result using both together is fucking unreliable.
            int left = i - Main.tile[i, j].frameX % (Width * 18) / 18;
            int top = j - Main.tile[i, j].frameY % (Height * 18) / 18;
            if (!TileEntity.ByID.Any(tileEntity => tileEntity.Value.Position == new Point16(left, top)))
            {
                var factory = ModTileEntity.ConstructFromType(ModContent.TileEntityType<TEDraedonItemCharger>());
                factory.Position = new Point16(left, top);
                TileEntity.ByID[TileEntity.ByID.Count] = factory;
            }
            return (TEDraedonItemCharger)TileEntity.ByID.Where(tileEntity => tileEntity.Value.Position == new Point16(left, top)).First().Value;
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
            Item.NewItem(i * 16, j * 16, 32, 32, ModContent.ItemType<DraedonsChargingStation>());
            int left = i - Main.tile[i, j].frameX % (Width * 18) / 18;
            int top = j - Main.tile[i, j].frameY % (Height * 18) / 18;

            TEDraedonItemCharger charger = RetrieveTileEntity(i, j);
            if (charger.FuelItem.stack > 0)
            {
                Item.NewItem(new Vector2(i, j) * 16f, charger.FuelItem.type, charger.FuelItem.stack);
            }
            if (charger.ItemBeingCharged.stack > 0)
            {
                int idx = Item.NewItem(new Vector2(i, j) * 16f, charger.ItemBeingCharged.type, charger.ItemBeingCharged.stack);
                Main.item[idx].Calamity().CurrentCharge = charger.Charge;
                Main.item[idx].prefix = charger.ItemBeingCharged.prefix;
            }
            charger.Kill(left, top);
        }

        public override bool NewRightClick(int i, int j)
        {
            int left = i - Main.tile[i, j].frameX % (Width * 18) / 18;
            int top = j - Main.tile[i, j].frameY % (Height * 18) / 18;
            Player player = Main.LocalPlayer;

            TEDraedonItemCharger charger = RetrieveTileEntity(i, j);
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
            if (player.Calamity().CurrentlyViewedCharger == charger)
            {
                player.Calamity().CurrentlyViewedCharger = null;
                player.Calamity().CurrentlyViewedChargerX = player.Calamity().CurrentlyViewedChargerY = -1;
                Main.PlaySound(SoundID.MenuClose);
            }
            else
            {
                player.Calamity().CurrentlyViewedChargerX = left * 16;
                player.Calamity().CurrentlyViewedChargerY = top * 16;

                player.Calamity().CurrentlyViewedCharger = charger;

                Main.playerInventory = true;
                Main.recBigList = false;
                Main.PlaySound(player.Calamity().CurrentlyViewedCharger == null ? SoundID.MenuOpen : SoundID.MenuTick);
            }

            Recipe.FindRecipes();
            return true;
        }
        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            TEDraedonItemCharger charger = RetrieveTileEntity(i, j);
            Color drawColor = Color.Lerp(Color.Red, Color.MediumSpringGreen, Utils.InverseLerp(0f, TEDraedonItemCharger.ActiveTimerMax, charger.ActiveTimer, true));

            int xFrame = Main.tile[i, j].frameX;
            int yFrame = Main.tile[i, j].frameY;
            Texture2D glowmask = ModContent.GetTexture("CalamityMod/Tiles/DraedonItemChargerGlow");
            Vector2 screenOffset = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
            Vector2 drawOffset = new Vector2(i * 16 - Main.screenPosition.X, j * 16 - Main.screenPosition.Y) + screenOffset;
            Tile trackTile = Main.tile[i, j];
            if (!trackTile.halfBrick() && trackTile.slope() == 0)
            {
                Main.spriteBatch.Draw(glowmask, drawOffset, new Rectangle?(new Rectangle(xFrame, yFrame, 18, 18)), drawColor, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
            }
            else if (trackTile.halfBrick())
            {
                Main.spriteBatch.Draw(glowmask, drawOffset + new Vector2(0f, 8f), new Rectangle?(new Rectangle(xFrame, yFrame, 18, 8)), drawColor, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
            }
        }
    }
}
