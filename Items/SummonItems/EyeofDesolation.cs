using System.Collections.Generic;
using System.Linq;
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
    public class EyeofDesolation : ModItem
    {
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
            SacrificeTotal = 1;
            DisplayName.SetDefault("Eye of Desolation");
            Tooltip.SetDefault("Tonight is going to be a horrific night...\n" +
                "Summons Calamitas when used during nighttime\n" +
                "Enrages during the day\n" +
                "Not consumable");
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
            SoundEngine.PlaySound(SoundID.Roar, player.position);
            if (Main.netMode != NetmodeID.MultiplayerClient)
                NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<CalamitasClone>());
            else
                NetMessage.SendData(MessageID.SpawnBoss, -1, -1, null, player.whoAmI, ModContent.NPCType<CalamitasClone>());

            if (Main.netMode != NetmodeID.MultiplayerClient && CalamityWorld.getFixedBoi)
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

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            Player player = Main.LocalPlayer;
            TooltipLine line3 = list.FirstOrDefault(x => x.Mod == "Terraria" && x.Name == "Tooltip2");

            if (CalamityWorld.getFixedBoi)
            {
                line3.Text = "Creates a square arena of blocks, with you at its center\nEnrages during the day";
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.HellstoneBar, 10).
                AddIngredient<EssenceofChaos>(7).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
