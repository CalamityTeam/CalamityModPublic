using CalamityMod.NPCs.GreatSandShark;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Items.SummonItems
{
    public class SandstormsCore : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Sandstorm's Core");
            Tooltip.SetDefault("Summons the Great Sand Shark when used in the desert\n" +
                "Not consumable");
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Lime;
            Item.useAnimation = 10;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = false;
        }

        public override bool CanUseItem(Player player)
        {
            return player.ZoneDesert && !NPC.AnyNPCs(ModContent.NPCType<GreatSandShark>());
        }

        public override bool? UseItem(Player player)
        {
            SoundEngine.PlaySound(SoundID.Roar, player.position, 0);
            if (Main.netMode != NetmodeID.MultiplayerClient)
                NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<GreatSandShark>());
            else
                NetMessage.SendData(MessageID.SpawnBoss, -1, -1, null, player.whoAmI, ModContent.NPCType<GreatSandShark>());

            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Ectoplasm, 10).
                AddIngredient(ItemID.HallowedBar, 5).
                AddIngredient(ItemID.AncientBattleArmorMaterial, 3).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
