using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Tools
{
    public class WulfrumHammer : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Wulfrum Hammer");
        }

        public override void SetDefaults()
        {
            Item.damage = 7;
            Item.knockBack = 5.5f;
            Item.useTime = 11;
            Item.useAnimation = 29;
            Item.hammer = 45;
            Item.tileBoost += 1;

            Item.DamageType = DamageClass.Melee;
            Item.width = 28;
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
            CreateRecipe().
                AddIngredient<WulfrumMetalScrap>(16).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
