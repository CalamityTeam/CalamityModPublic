using CalamityMod.World;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Tools.ClimateChange
{
    public class AridArtifact : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Arid Artifact");
            Tooltip.SetDefault("Summons a sandstorm\n" +
                               "The sandstorm will happen shortly after the item is used\n" +
                               "Not consumable");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.rare = ItemRarityID.Pink;
            item.useAnimation = 20;
            item.useTime = 20;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.UseSound = SoundID.Item66;
        }

        public override bool CanUseItem(Player player)
        {
            return CalamityWorld.downedDesertScourge;
        }

        // this is extremely ugly and has to be fully qualified because we add an item called Sandstorm
        public override bool UseItem(Player player)
        {
            if (Terraria.GameContent.Events.Sandstorm.Happening)
                CalamityUtils.StopSandstorm();
            else
                CalamityUtils.StartSandstorm();
            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.SandBlock, 50);
            recipe.anySand = true;
            recipe.AddRecipeGroup("AnyAdamantiteBar", 10);
            recipe.AddIngredient(ItemID.AncientCloth, 5);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
