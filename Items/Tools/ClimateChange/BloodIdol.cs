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
            return !Main.bloodMoon && !Main.dayTime && !Main.pumpkinMoon && !Main.snowMoon;
        }

        public override bool? UseItem(Player player)
        {
            Main.bloodMoon = true;
            CalamityNetcode.SyncWorld();
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddRecipeGroup("EvilPowder", 20).AddIngredient(ItemID.SoulofNight, 20).AddIngredient(ModContent.ItemType<UnholyCore>(), 10).AddIngredient(ModContent.ItemType<BloodOrb>(), 5).AddTile(TileID.MythrilAnvil).Register();
        }
    }
}
