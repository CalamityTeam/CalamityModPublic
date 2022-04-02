using CalamityMod.Events;
using CalamityMod.Items.Materials;
using CalamityMod.NPCs.ProfanedGuardians;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.SummonItems
{
    public class ProfanedShard : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Profaned Shard");
            Tooltip.SetDefault("A shard of the unholy flame\n" +
                "Summons the Profaned Guardians when used in the hallow or underworld during daytime\n" +
                "Not consumable");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.useAnimation = 45;
            item.useTime = 45;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.consumable = false;
            item.rare = ItemRarityID.Purple;
        }

        public override bool CanUseItem(Player player)
        {
            return !NPC.AnyNPCs(ModContent.NPCType<ProfanedGuardianBoss>()) && Main.dayTime && (player.ZoneHoly || player.ZoneUnderworldHeight) && !BossRushEvent.BossRushActive;
        }

        public override bool UseItem(Player player)
        {
            Main.PlaySound(SoundID.Roar, player.position, 0);
            if (Main.netMode != NetmodeID.MultiplayerClient)
                NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<ProfanedGuardianBoss>());
            else
                NetMessage.SendData(MessageID.SpawnBoss, -1, -1, null, player.whoAmI, ModContent.NPCType<ProfanedGuardianBoss>());

            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<UnholyEssence>(), 25);
            recipe.AddIngredient(ItemID.LunarBar, 5);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
