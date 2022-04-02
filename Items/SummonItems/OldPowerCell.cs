using CalamityMod.Events;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.SummonItems
{
    public class OldPowerCell : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Old Power Cell");
            Tooltip.SetDefault("Summons the Golem when used in the temple\n" +
                "Not consumable");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 18;
            item.rare = ItemRarityID.Lime;
            item.useAnimation = 45;
            item.useTime = 45;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.consumable = false;
        }

        public override bool CanUseItem(Player player)
        {
            bool canSummon = false;
            if (player.Center.Y > Main.worldSurface * 16.0)
            {
                int num = (int)player.Center.X / 16;
                int num2 = (int)player.Center.Y / 16;
                Tile tile = Framing.GetTileSafely(num, num2);
                if (tile.wall == 87)
                    canSummon = true;
            }
            return canSummon && !NPC.AnyNPCs(NPCID.Golem) && !BossRushEvent.BossRushActive;
        }

        public override bool UseItem(Player player)
        {
            Main.PlaySound(SoundID.Roar, player.position, 0);
            if (Main.netMode != NetmodeID.MultiplayerClient)
                NPC.SpawnOnPlayer(player.whoAmI, NPCID.Golem);
            else
                NetMessage.SendData(MessageID.SpawnBoss, -1, -1, null, player.whoAmI, NPCID.Golem);

            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.LunarTabletFragment, 20);
            recipe.AddIngredient(ModContent.ItemType<EssenceofCinder>(), 10);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
