using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class WulfrumBow : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Wulfrum Bow");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 11;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 27;
            Item.height = 38;
            Item.useTime = 25;
            Item.useAnimation = 25;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2.25f;
            Item.value = Item.buyPrice(0, 1, 0, 0);
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = SoundID.Item5;
            Item.autoReuse = false;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 12f;
            Item.useAmmo = AmmoID.Arrow;
            Item.Calamity().canFirePointBlankShots = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<WulfrumShard>(10).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
