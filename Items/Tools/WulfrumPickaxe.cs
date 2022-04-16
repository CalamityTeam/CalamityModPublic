using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Tools
{
    public class WulfrumPickaxe : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            DisplayName.SetDefault("Wulfrum Pickaxe");
        }

        public override void SetDefaults()
        {
            Item.damage = 5;
            Item.knockBack = 2f;
            Item.useTime = 8;
            Item.useAnimation = 16;
            Item.pick = 40;
            Item.tileBoost += 1;

            Item.DamageType = DamageClass.Melee;
            Item.width = 46;
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
                AddIngredient<WulfrumShard>(12).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
