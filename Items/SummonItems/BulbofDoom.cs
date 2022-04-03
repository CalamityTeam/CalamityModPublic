using CalamityMod.Events;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Items.SummonItems
{
    public class BulbofDoom : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Portabulb");
            Tooltip.SetDefault("Summons Plantera when used in the jungle\n" +
                "Not consumable");
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 18;
            Item.useAnimation = 45;
            Item.useTime = 45;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.rare = ItemRarityID.Lime;
            Item.consumable = false;
        }

        public override bool CanUseItem(Player player)
        {
            return player.ZoneJungle && !NPC.AnyNPCs(NPCID.Plantera) && !BossRushEvent.BossRushActive;
        }

        public override bool? UseItem(Player player)
        {
            SoundEngine.PlaySound(SoundID.Roar, player.position, 0);
            if (Main.netMode != NetmodeID.MultiplayerClient)
                NPC.SpawnOnPlayer(player.whoAmI, NPCID.Plantera);
            else
                NetMessage.SendData(MessageID.SpawnBoss, -1, -1, null, player.whoAmI, NPCID.Plantera);

            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ItemID.JungleSpores, 15).AddIngredient(ItemID.SoulofNight, 10).AddIngredient(ItemID.SoulofLight, 10).AddIngredient(ModContent.ItemType<MurkyPaste>(), 3).AddIngredient(ItemID.Vine).AddIngredient(ModContent.ItemType<TrapperBulb>()).AddTile(TileID.MythrilAnvil).Register();
        }
    }
}
