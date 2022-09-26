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
            Item.damage = 40;
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
            Item.value = Item.buyPrice(0, 2, 0, 0);
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.TungstenAxe).
                AddIngredient(ItemID.TungstenBar, 10).
                AddIngredient(ItemID.Wood, 15).
                AddTile(TileID.Anvils).
                Register();

            CreateRecipe().
                AddIngredient(ItemID.SilverAxe).
                AddIngredient(ItemID.SilverBar, 10).
                AddIngredient(ItemID.Wood, 15).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
