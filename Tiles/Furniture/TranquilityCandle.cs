using CalamityMod.Buffs.Placeables;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace CalamityMod.Tiles.Furniture
{
    public class TranquilityCandle : ModTile
    {
        public override void SetDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.StyleOnTable1x1);
            TileObjectData.addTile(Type);
            drop = ModContent.ItemType<Items.Placeables.Furniture.TranquilityCandle>();
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Tranquility Candle");
            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);
            AddMapEntry(new Color(238, 145, 105), name);
            animationFrameHeight = 20;
        }

        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            frameCounter++;
            if (frameCounter >= 4)
            {
                frame = (frame + 1) % 10;
                frameCounter = 0;
            }
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.showItemIcon = true;
            player.showItemIcon2 = ModContent.ItemType<Items.Placeables.Furniture.TranquilityCandle>();
        }

        public override void NearbyEffects(int i, int j, bool closer)
        {
            Player player = Main.LocalPlayer;
            if (player is null)
                return;
            if (!player.dead && player.active)
                player.AddBuff(ModContent.BuffType<TranquilityCandleBuff>(), 20);
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.55f;
            g = 0.055f;
            b = 0.55f;
        }

		//this doesn't work
		/*public virtual bool NewRightClick(int i, int j)
		{
            Item.NewItem(i * 16, j * 16, 8, 8, ModContent.ItemType<Items.Placeables.Furniture.TranquilityCandle>());
			if (Main.tile[i, j] != null && Main.tile[i, j].active())
			{
				WorldGen.KillTile(i, j, false, false, false);
				if (!Main.tile[i, j].active() && Main.netMode != NetmodeID.SinglePlayer)
				{
					NetMessage.SendData(17, -1, -1, null, 0, (float)i, (float)j, 0f, 0, 0, 0);
				}
			}
			return true;
		}

		public override bool HasSmartInteract()
		{
			return true;
		}*/
    }
}
