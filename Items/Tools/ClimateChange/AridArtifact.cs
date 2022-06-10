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
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Pink;
            Item.useAnimation = 20;
            Item.useTime = 20;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.UseSound = SoundID.Item66;
        }

        public override bool CanUseItem(Player player)
        {
            return DownedBossSystem.downedDesertScourge;
        }

        // this is extremely ugly and has to be fully qualified because we add an item called Sandstorm
        public override bool? UseItem(Player player)
        {
            if (Terraria.GameContent.Events.Sandstorm.Happening)
                CalamityUtils.StopSandstorm();
            else
                CalamityUtils.StartSandstorm();
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.SandBlock, 50).
                AddRecipeGroup("AnyAdamantiteBar", 10).
                AddIngredient(ItemID.AncientCloth, 5).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
