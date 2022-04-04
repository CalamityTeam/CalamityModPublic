using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Tools
{
    public class WulfrumAxe : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Wulfrum Axe");
        }

        public override void SetDefaults()
        {
            Item.damage = 5;
            Item.knockBack = 4.5f;
            Item.useTime = 15;
            Item.useAnimation = 26;
            Item.axe = 60 / 5;
            Item.tileBoost += 1;

            Item.DamageType = DamageClass.Melee;
            Item.width = 30;
            Item.height = 38;
            Item.useTurn = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.value = Item.buyPrice(0, 1, 0, 0);
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<WulfrumShard>(14)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
