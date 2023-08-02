using System.Collections.Generic;
using CalamityMod.Events;
using CalamityMod.Items.Materials;
using CalamityMod.NPCs.CalClone;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
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
            ItemID.Sets.SortingPriorityBossSpawns[Type] = 10; // Pirate Map
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
            return !Main.dayTime && !NPC.AnyNPCs(ModContent.NPCType<CalamitasClone>()) && !BossRushEvent.BossRushActive;
        }

        public override bool? UseItem(Player player)
        {
            SoundEngine.PlaySound(SoundID.Roar, player.Center);
            if (Main.netMode != NetmodeID.MultiplayerClient)
                NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<CalamitasClone>());
            else
                NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, -1, -1, null, player.whoAmI, ModContent.NPCType<CalamitasClone>());

            if (Main.netMode != NetmodeID.MultiplayerClient && Main.zenithWorld)
            {
                safeBox.X = spawnX = spawnXReset = (int)(player.Center.X - 1250f);
                spawnX2 = spawnXReset2 = (int)(player.Center.X + 1250f);
                safeBox.Y = spawnY = spawnYReset = (int)(player.Center.Y - 1250f);
                safeBox.Width = 2500;
                safeBox.Height = 2500;
                spawnYAdd = 125;

                int num52 = (int)(safeBox.X + (float)(safeBox.Width / 2)) / 16;
                int num53 = (int)(safeBox.Y + (float)(safeBox.Height / 2)) / 16;
                int num54 = safeBox.Width / 2 / 16 + 1;
                for (int num55 = num52 - num54; num55 <= num52 + num54; num55++)
                {
                    for (int num56 = num53 - num54; num56 <= num53 + num54; num56++)
                    {
                        if (!WorldGen.InWorld(num55, num56, 2))
                            continue;

                        int xoffset = 0;
                        int yoffset = 0;

                        if ((num55 == num52 - num54 || num55 == num52 + num54 || num56 == num53 - num54 || num56 == num53 + num54) && !Main.tile[num55 + xoffset, num56 + yoffset].HasTile)
                        {
                            Main.tile[num55 + xoffset, num56 + yoffset].TileType = (ushort)ModContent.TileType<Tiles.ArenaTile>();
                            Main.tile[num55 + xoffset, num56 + yoffset].Get<TileWallWireStateData>().HasTile = true;
                        }
                        if (Main.netMode == NetmodeID.Server)
                        {
                            NetMessage.SendTileSquare(-1, num55 + xoffset, num56 + yoffset, 1, TileChangeType.None);
                        }
                        else
                        {
                            WorldGen.SquareTileFrame(num55 + xoffset, num56 + yoffset, true);
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
