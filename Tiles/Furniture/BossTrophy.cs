using CalamityMod.Items.Placeables.Furniture.Trophies;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace CalamityMod.Tiles.Furniture
{
    public class BossTrophy : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3Wall);
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.StyleWrapLimit = 36;
            TileObjectData.addTile(Type);
            DustType = 7;
            TileID.Sets.DisableSmartCursor[Type] = true;
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Trophy");
            AddMapEntry(new Color(120, 85, 60), name);
            TileID.Sets.FramesOnKillWall[Type] = true;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            int item = 0;
            switch (frameX / 54)
            {
                case 0:
                    item = ModContent.ItemType<DesertScourgeTrophy>();
                    break;
                case 1:
                    item = ModContent.ItemType<PerforatorTrophy>();
                    break;
                case 2:
                    item = ModContent.ItemType<SlimeGodTrophy>();
                    break;
                case 3:
                    item = ModContent.ItemType<CryogenTrophy>();
                    break;
                case 4:
                    item = ModContent.ItemType<PlaguebringerGoliathTrophy>();
                    break;
                case 5:
                    item = ModContent.ItemType<LeviathanTrophy>();
                    break;
                case 6:
                    item = ModContent.ItemType<ProvidenceTrophy>();
                    break;
                case 7:
                    item = ModContent.ItemType<CalamitasCloneTrophy>();
                    break;
                case 8:
                    item = ModContent.ItemType<HiveMindTrophy>();
                    break;
                case 9:
                    item = ModContent.ItemType<CrabulonTrophy>();
                    break;
                case 10:
                    item = ModContent.ItemType<YharonTrophy>();
                    break;
                case 11:
                    item = ModContent.ItemType<SignusTrophy>();
                    break;
                case 12:
                    item = ModContent.ItemType<WeaverTrophy>();
                    break;
                case 13:
                    item = ModContent.ItemType<CeaselessVoidTrophy>();
                    break;
                case 14:
                    item = ModContent.ItemType<DevourerofGodsTrophy>();
                    break;
                case 15:
                    item = ModContent.ItemType<CatastropheTrophy>();
                    break;
                case 16:
                    item = ModContent.ItemType<CataclysmTrophy>();
                    break;
                case 17:
                    item = ModContent.ItemType<PolterghastTrophy>();
                    break;
                case 18:
                    item = ModContent.ItemType<DragonfollyTrophy>();
                    break;
                case 19:
                    item = ModContent.ItemType<AstrumAureusTrophy>();
                    break;
                case 20:
                    item = ModContent.ItemType<AstrumDeusTrophy>();
                    break;
                case 21:
                    item = ModContent.ItemType<BrimstoneElementalTrophy>();
                    break;
                case 22:
                    item = ModContent.ItemType<RavagerTrophy>();
                    break;
                case 23:
                    item = ModContent.ItemType<AquaticScourgeTrophy>();
                    break;
                case 24:
                    item = ModContent.ItemType<OldDukeTrophy>();
                    break;
                case 25:
                    item = ModContent.ItemType<ProfanedGuardianTrophy>();
                    break;
                case 26:
                    item = ModContent.ItemType<SupremeCalamitasTrophy>();
                    break;
                case 27:
                    item = ModContent.ItemType<AnahitaTrophy>();
                    break;
                case 28:
                    item = ModContent.ItemType<ApolloTrophy>();
                    break;
                case 29:
                    item = ModContent.ItemType<ArtemisTrophy>();
                    break;
                case 30:
                    item = ModContent.ItemType<AresTrophy>();
                    break;
                case 31:
                    item = ModContent.ItemType<ThanatosTrophy>();
                    break;
                case 32:
                    item = ModContent.ItemType<SupremeCatastropheTrophy>();
                    break;
                case 33:
                    item = ModContent.ItemType<SupremeCataclysmTrophy>();
                    break;
            }
            if (item > 0)
            {
                Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 48, 48, item);
            }
        }
    }
}
