using CalamityMod.Events;
using CalamityMod.Items.Materials;
using CalamityMod.NPCs.DesertScourge;
using CalamityMod.World;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Items.SummonItems
{
    public class DriedSeafood : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Desert Medallion");
            Tooltip.SetDefault("Summons the Desert Scourge when used in the desert\n" +
                "Not consumable");
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 28;
            Item.rare = ItemRarityID.Green;
            Item.useAnimation = 45;
            Item.useTime = 45;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = false;
        }

        public override bool CanUseItem(Player player)
        {
            return player.ZoneDesert && !NPC.AnyNPCs(ModContent.NPCType<DesertScourgeHead>()) && !BossRushEvent.BossRushActive;
        }

        public override bool? UseItem(Player player)
        {
            SoundEngine.PlaySound(SoundID.Roar, player.position, 0);
            if (Main.netMode != NetmodeID.MultiplayerClient)
                NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<DesertScourgeHead>());
            else
                NetMessage.SendData(MessageID.SpawnBoss, -1, -1, null, player.whoAmI, ModContent.NPCType<DesertScourgeHead>());

            if (CalamityWorld.revenge)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<DesertNuisanceHead>());
                else
                    NetMessage.SendData(MessageID.SpawnBoss, -1, -1, null, player.whoAmI, ModContent.NPCType<DesertNuisanceHead>());

                if (Main.netMode != NetmodeID.MultiplayerClient)
                    NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<DesertNuisanceHead>());
                else
                    NetMessage.SendData(MessageID.SpawnBoss, -1, -1, null, player.whoAmI, ModContent.NPCType<DesertNuisanceHead>());
            }

            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ItemID.SandBlock, 25).AddIngredient(ItemID.Cactus, 15).AddIngredient(ItemID.AntlionMandible, 4).AddIngredient(ModContent.ItemType<StormlionMandible>(), 2).AddTile(TileID.DemonAltar).Register();
        }
    }
}
