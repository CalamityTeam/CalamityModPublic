using CalamityMod.Dusts;
using CalamityMod.Items.TreasureBags.MiscGrabBags;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace CalamityMod.Tiles.Abyss
{
    public class SulphurousPots : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = true;
            Main.tileWaterDeath[Type] = false;
            Main.tileOreFinderPriority[Type] = (short)100;
            Main.tileSpelunker[Type] = true;
            Main.tileCut[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.DrawYOffset = 4;
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(226, 205, 101), Language.GetText("MapObject.Pot"));

            DustType = (int)CalamityDusts.SulfurousSeaAcid;
            HitSound = SoundID.Shatter;
        }


        public override IEnumerable<Item> GetItemDrops(int i, int j)
        {
            Tile tileAtPosition = CalamityUtils.ParanoidTileRetrieval(i, j);
            if (tileAtPosition.TileFrameX % 36 == 0 && tileAtPosition.TileFrameY % 36 == 0)
            {
                if (Main.netMode != NetmodeID.Server)
				{
					int goreAmt = Main.rand.Next(1, 2 + 1);
					for (int k = 0; k < goreAmt; k++)
					{
						Gore.NewGore(new EntitySource_TileBreak(i, j), new Vector2(i, j) * 16, Main.rand.NextVector2CircularEdge(3f, 3f), Mod.Find<ModGore>("SulphPotGore1").Type);
						Gore.NewGore(new EntitySource_TileBreak(i, j), new Vector2(i, j) * 16, Main.rand.NextVector2CircularEdge(3f, 3f), Mod.Find<ModGore>("SulphPotGore2").Type);
					}
				}

				// 1 in 400 for a Coin Portal
				if (Player.GetClosestRollLuck(i, j, 400) == 0f)
				{
					if (Main.netMode != NetmodeID.MultiplayerClient)
						Projectile.NewProjectile(new EntitySource_TileBreak(i, j), i * 16 + 16, j * 16 + 16, 0f, -12f, ProjectileID.CoinPortal, 0, 0f, Main.myPlayer);
				}
				// Followed by a 1 in 4 for a bomb in For The Worthy worlds
				else if (Main.getGoodWorld && Main.rand.Next(4) == 0)
					Projectile.NewProjectile(new EntitySource_TileBreak(i, j), i * 16 + 16, j * 16 + 8, (float)Main.rand.Next(-100, 101) * 0.002f, 0f, ProjectileID.Bomb, 0, 0f, Player.FindClosest(new Vector2(i * 16, j * 16), 16, 16));
				else
					yield return new Item(ModContent.ItemType<SulphuricTreasure>());
			}
		}

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }
}
