using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.SummonItems
{
    public class BlightedEyeball : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Eye of Desolation");
            Tooltip.SetDefault("Tonight is going to be a horrific night...\n" +
                "Summons Calamitas\n" +
                "Not consumable");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 18;
            item.rare = 6;
            item.useAnimation = 45;
            item.useTime = 45;
            item.useStyle = 4;
            item.consumable = false;
        }

        public override bool CanUseItem(Player player)
        {
            return !Main.dayTime && !NPC.AnyNPCs(ModContent.NPCType<Calamitas>()) && !NPC.AnyNPCs(ModContent.NPCType<CalamitasRun3>());
        }

        public override bool UseItem(Player player)
        {
            NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<Calamitas>());
            Main.PlaySound(SoundID.Roar, player.position, 0);
            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.HellstoneBar, 10);
            recipe.AddIngredient(ModContent.ItemType<EssenceofChaos>(), 7);
            recipe.AddIngredient(ItemID.SoulofFright, 5);
            recipe.AddIngredient(ModContent.ItemType<BlightedLens>(), 3);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
