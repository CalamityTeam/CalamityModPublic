using System.Collections.Generic;
using CalamityMod.Events;
using CalamityMod.Items.Materials;
using CalamityMod.NPCs.CalClone;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.SummonItems
{
    [LegacyName("BlightedEyeball")]
    public class EyeofDesolation : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.SummonItems";
        public Rectangle safeBox = default;
        public int spawnX = 0;
        public int spawnX2 = 0;
        public int spawnXReset = 0;
        public int spawnXReset2 = 0;
        public int spawnXAdd = 200;
        public int spawnY = 0;
        public int spawnYReset = 0;
        public int spawnYAdd = 0;
        public override void SetStaticDefaults()
        {
            ItemID.Sets.SortingPriorityBossSpawns[Type] = 11; // Pirate Map (1 above Mechanical Skull)
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 18;
            Item.rare = ItemRarityID.LightPurple;
            Item.useAnimation = 10;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = false;
        }

        public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
        {
            itemGroup = ContentSamples.CreativeHelper.ItemGroup.BossItem;
        }

        public override bool CanUseItem(Player player)
        {
            return !Main.IsItDay() && !NPC.AnyNPCs(ModContent.NPCType<CalamitasClone>()) && !BossRushEvent.BossRushActive;
        }

        public override bool? UseItem(Player player)
        {
            CalamityUtils.SpawnBossUsingItem<CalamitasClone>(player, SoundID.Roar);

            if (Main.netMode != NetmodeID.MultiplayerClient && Main.zenithWorld)
            {
                safeBox.X = spawnX = spawnXReset = (int)(player.Center.X - 1250f);
                spawnX2 = spawnXReset2 = (int)(player.Center.X + 1250f);
                safeBox.Y = spawnY = spawnYReset = (int)(player.Center.Y - 1250f);
                safeBox.Width = 2500;
                safeBox.Height = 2500;
                spawnYAdd = 125;

                int safeBoxTilesX = (int)(safeBox.X + (float)(safeBox.Width / 2)) / 16;
                int safeBoxTilesY = (int)(safeBox.Y + (float)(safeBox.Height / 2)) / 16;
                int safeBoxTilesWidth = safeBox.Width / 2 / 16 + 1;
                for (int i = safeBoxTilesX - safeBoxTilesWidth; i <= safeBoxTilesX + safeBoxTilesWidth; i++)
                {
                    for (int j = safeBoxTilesY - safeBoxTilesWidth; j <= safeBoxTilesY + safeBoxTilesWidth; j++)
                    {
                        if (!WorldGen.InWorld(i, j, 2))
                            continue;

                        int xoffset = 0;
                        int yoffset = 0;

                        if ((i == safeBoxTilesX - safeBoxTilesWidth || i == safeBoxTilesX + safeBoxTilesWidth || j == safeBoxTilesY - safeBoxTilesWidth || j == safeBoxTilesY + safeBoxTilesWidth) && !Main.tile[i + xoffset, j + yoffset].HasTile)
                        {
                            Main.tile[i + xoffset, j + yoffset].TileType = (ushort)ModContent.TileType<Tiles.ArenaTile>();
                            Main.tile[i + xoffset, j + yoffset].Get<TileWallWireStateData>().HasTile = true;
                        }
                        if (Main.netMode == NetmodeID.Server)
                        {
                            NetMessage.SendTileSquare(-1, i + xoffset, j + yoffset, 1, TileChangeType.None);
                        }
                        else
                        {
                            WorldGen.SquareTileFrame(i + xoffset, j + yoffset, true);
                        }
                    }
                }
            }
            return true;
        }

        public override void ModifyTooltips(List<TooltipLine> list) => list.FindAndReplace("[GFB]", Main.zenithWorld ? "\n" + this.GetLocalizedValue("GFBInfo") : string.Empty);

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.HellstoneBar, 10).
                // waffles% stipulation: you can only get 5 essences of havoc from the music box, not 7, and cal clone must be accessible
                AddIngredient<EssenceofHavoc>(5).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
