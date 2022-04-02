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
            Tooltip.SetDefault("Summons a blood moon\n" +
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
            return !Main.bloodMoon && !Main.dayTime && !Main.pumpkinMoon && !Main.snowMoon;
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
            recipe.AddRecipeGroup("EvilPowder", 20);
            recipe.AddIngredient(ItemID.SoulofNight, 20);
            recipe.AddIngredient(ModContent.ItemType<UnholyCore>(), 10);
            recipe.AddIngredient(ModContent.ItemType<BloodOrb>(), 5);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
