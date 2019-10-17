using Microsoft.Xna.Framework;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader; using CalamityMod.Dusts;
using Terraria.ModLoader; using CalamityMod.Dusts; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;
using Terraria.ObjectData;

namespace CalamityMod.Tiles
{
    public class BossTrophy : ModTile
    {
        public override void SetDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3Wall);
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.StyleWrapLimit = 36;
            TileObjectData.addTile(Type);
            dustType = 7;
            disableSmartCursor = true;
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Trophy");
            AddMapEntry(new Color(120, 85, 60), name);
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            int item = 0;
            switch (frameX / 54)
            {
                case 0:
                    item = ModContent.ItemType<Items.DesertScourgeTrophy>();
                    break;
                case 1:
                    item = ModContent.ItemType<Items.PerforatorTrophy>();
                    break;
                case 2:
                    item = ModContent.ItemType<Items.SlimeGodTrophy>();
                    break;
                case 3:
                    item = ModContent.ItemType<Items.CryogenTrophy>();
                    break;
                case 4:
                    item = ModContent.ItemType<Items.PlaguebringerGoliathTrophy>();
                    break;
                case 5:
                    item = ModContent.ItemType<Items.LeviathanTrophy>();
                    break;
                case 6:
                    item = ModContent.ItemType<Items.ProvidenceTrophy>();
                    break;
                case 7:
                    item = ModContent.ItemType<Items.CalamitasTrophy>();
                    break;
                case 8:
                    item = ModContent.ItemType<Items.HiveMindTrophy>();
                    break;
                case 9:
                    item = ModContent.ItemType<Items.CrabulonTrophy>();
                    break;
                case 10:
                    item = ModContent.ItemType<Items.YharonTrophy>();
                    break;
                case 11:
                    item = ModContent.ItemType<Items.SignusTrophy>();
                    break;
                case 12:
                    item = ModContent.ItemType<Items.WeaverTrophy>();
                    break;
                case 13:
                    item = ModContent.ItemType<Items.CeaselessVoidTrophy>();
                    break;
                case 14:
                    item = ModContent.ItemType<Items.DevourerofGodsTrophy>();
                    break;
                case 15:
                    item = ModContent.ItemType<Items.CatastropheTrophy>();
                    break;
                case 16:
                    item = ModContent.ItemType<Items.CataclysmTrophy>();
                    break;
                case 17:
                    item = ModContent.ItemType<Items.PolterghastTrophy>();
                    break;
                case 18:
                    item = ModContent.ItemType<Items.BumblebirbTrophy>();
                    break;
                case 19:
                    item = ModContent.ItemType<Items.AstrageldonTrophy>();
                    break;
                case 20:
                    item = ModContent.ItemType<Items.AstrumDeusTrophy>();
                    break;
                case 21:
                    item = ModContent.ItemType<Items.BrimstoneElementalTrophy>();
                    break;
                case 22:
                    item = ModContent.ItemType<Items.RavagerTrophy>();
                    break;
            }
            if (item > 0)
            {
                Item.NewItem(i * 16, j * 16, 48, 48, item);
            }
        }
    }
}
