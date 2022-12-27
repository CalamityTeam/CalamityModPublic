using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Tools
{
    public class FellerofEvergreens : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Feller of Evergreens");
        }

        public override void SetDefaults()
        {
            Item.damage = 18;
            Item.knockBack = 5f;
            Item.useTime = 17;
            Item.useAnimation = 25;
            Item.axe = 100 / 5;

            Item.DamageType = DamageClass.Melee;
            Item.width = 36;
            Item.height = 36;
            Item.scale = 1.5f;
            Item.useTurn = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.value = CalamityGlobalItem.Rarity2BuyPrice;
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddRecipeGroup("AnySilverBar", 18).
                AddIngredient(ItemID.Wood, 18).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
