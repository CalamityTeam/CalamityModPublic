using CalamityMod.Events;
using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.NPCs.BrimstoneElemental;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.SummonItems
{
    public class CharredIdol : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Charred Idol");
            Tooltip.SetDefault("Use at your own risk\n" +
               "Summons the Brimstone Elemental when used in the brimstone crags\n" +
               "Not consumable");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 18;
            item.rare = ItemRarityID.Pink;
            item.useAnimation = 45;
            item.useTime = 45;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.consumable = false;
        }

        public override bool CanUseItem(Player player)
        {
            CalamityPlayer modPlayer = player.Calamity();
            return modPlayer.ZoneCalamity && !NPC.AnyNPCs(ModContent.NPCType<BrimstoneElemental>()) && !BossRushEvent.BossRushActive;
        }

        public override bool UseItem(Player player)
        {
            Main.PlaySound(SoundID.Roar, player.position, 0);
            if (Main.netMode != NetmodeID.MultiplayerClient)
                NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<BrimstoneElemental>());
            else
                NetMessage.SendData(MessageID.SpawnBoss, -1, -1, null, player.whoAmI, ModContent.NPCType<BrimstoneElemental>());

            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.SoulofNight, 5);
            recipe.AddIngredient(ModContent.ItemType<EssenceofChaos>(), 7);
            recipe.AddIngredient(ModContent.ItemType<UnholyCore>(), 2);
            recipe.AddTile(TileID.Hellforge);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
