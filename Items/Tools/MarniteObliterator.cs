using CalamityMod.Projectiles.Melee;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;

namespace CalamityMod.Items.Tools
{
    public class MarniteObliterator : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Marnite Obliterator");
        }

        public override void SetDefaults()
        {
            Item.damage = 13;
            Item.knockBack = 1f;
            Item.useTime = 6;
            Item.useAnimation = 25;
            Item.pick = 45;

            Item.DamageType = TrueMeleeNoSpeedDamageClass.Instance;
            Item.width = 36;
            Item.height = 18;
            Item.channel = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.value = CalamityGlobalItem.Rarity1BuyPrice;
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = SoundID.Item23;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<MarniteObliteratorProj>();
            Item.shootSpeed = 40f;
            Item.tileBoost = 7;
        }

        public override void HoldItem(Player player)
        {
            player.Calamity().mouseWorldListener = true;
        }


        public override void AddRecipes()
        {
            CreateRecipe().
                AddRecipeGroup("AnyGoldBar", 3).
                AddIngredient(ItemID.Granite, 5).
                AddIngredient(ItemID.Marble, 5).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
