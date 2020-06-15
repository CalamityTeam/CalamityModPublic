using CalamityMod.TileEntities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace CalamityMod.Tiles
{
    public class DraedonFuelFactory : ModTile
    {
        public override void SetDefaults()
        {
            Main.tileSolidTop[Type] = true;
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileTable[Type] = true;
            Main.tileLavaDeath[Type] = true;
            Main.tileWaterDeath[Type] = false;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(ModContent.GetInstance<TEDraedonFuelFactory>().Hook_AfterPlacement, -1, 0, true);
            TileObjectData.addTile(Type);
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Fuel Factory");
            AddMapEntry(new Color(67, 72, 81), name);
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
            Item.NewItem(i * 16, j * 16, 32, 32, chestDrop);
            int left = i - Main.tile[i, j].frameX % 36 / 18;
            int top = j - Main.tile[i, j].frameY % 36 / 18;

            TEDraedonFuelFactory factory = (TEDraedonFuelFactory)TileEntity.ByID[ModContent.GetInstance<TEDraedonFuelFactory>().Find(left, top)];
            if (factory.HeldItem.stack > 0)
            {
                Item.NewItem(new Vector2(i, j) * 16f, factory.HeldItem.type, factory.HeldItem.stack);
            }
            factory.Kill(left, top);
        }

        public override bool NewRightClick(int i, int j)
        {
            int left = i - Main.tile[i, j].frameX % 36 / 18;
            int top = j - Main.tile[i, j].frameY % 36 / 18;

            Player player = Main.LocalPlayer;
            TEDraedonFuelFactory factory = (TEDraedonFuelFactory)TileEntity.ByID[ModContent.GetInstance<TEDraedonFuelFactory>().Find(left, top)];
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
                player.Calamity().CurrentlyViewedFactoryX = left * 16;
                player.Calamity().CurrentlyViewedFactoryY = top * 16;

                player.Calamity().CurrentlyViewedFactory = factory;

                Main.playerInventory = true;
                Main.recBigList = false;
                Main.PlaySound(player.Calamity().CurrentlyViewedFactory == null ? SoundID.MenuOpen : SoundID.MenuTick);
            }

            Recipe.FindRecipes();
            return true;
        }
    }
}
