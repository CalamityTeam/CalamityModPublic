using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Tools.ClimateChange
{
    public class BloodIdol : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blood Relic");
            Tooltip.SetDefault("Summons a blood moon");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.maxStack = 20;
            item.rare = 5;
            item.useAnimation = 20;
            item.useTime = 20;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.UseSound = SoundID.Item66;
            item.consumable = true;
        }

        public override bool CanUseItem(Player player)
        {
            return !Main.bloodMoon && !Main.dayTime;
        }

        public override bool UseItem(Player player)
        {
            Main.bloodMoon = true;
            CalamityNetcode.SyncWorld();
            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddRecipeGroup("FetidBloodletting", 2);
            recipe.AddIngredient(ModContent.ItemType<UnholyCore>(), 5);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
