using CalamityMod.CalPlayer;
using CalamityMod.Events;
using CalamityMod.Items.Placeables;
using CalamityMod.NPCs.AquaticScourge;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.SummonItems
{
    public class Seafood : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Seafood");
            Tooltip.SetDefault("Summons the Aquatic Scourge when used in the sulphur sea\n" +
                "Not consumable");
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 24;
            item.rare = ItemRarityID.Pink;
            item.useAnimation = 45;
            item.useTime = 45;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.consumable = false;
        }

        public override bool CanUseItem(Player player)
        {
            CalamityPlayer modPlayer = player.Calamity();
            return modPlayer.ZoneSulphur && !NPC.AnyNPCs(ModContent.NPCType<AquaticScourgeHead>()) && !BossRushEvent.BossRushActive;
        }

        public override bool UseItem(Player player)
        {
            Main.PlaySound(SoundID.Roar, player.position, 0);
            if (Main.netMode != NetmodeID.MultiplayerClient)
                NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<AquaticScourgeHead>());
            else
                NetMessage.SendData(MessageID.SpawnBoss, -1, -1, null, player.whoAmI, ModContent.NPCType<AquaticScourgeHead>());

            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<SulphurousSand>(), 20);
            recipe.AddIngredient(ItemID.Starfish, 10);
            recipe.AddIngredient(ItemID.SharkFin, 5);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
