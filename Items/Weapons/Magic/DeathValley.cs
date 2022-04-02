using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class DeathValley : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Death Valley Duster");
            Tooltip.SetDefault("Casts a large blast of dust");
        }

        public override void SetDefaults()
        {
            item.damage = 110;
            item.magic = true;
            item.mana = 9;
            item.width = 36;
            item.height = 40;
            item.useTime = 25;
            item.useAnimation = 25;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 5f;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.rare = ItemRarityID.Pink;
            item.UseSound = SoundID.Item20;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<DustProjectile>();
            item.shootSpeed = 5f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<Tradewinds>());
            recipe.AddRecipeGroup("AnyAdamantiteBar", 5); // This is here to keep the Forbidden Fragment stuff on the same tier.
            recipe.AddIngredient(ItemID.AncientBattleArmorMaterial);
            recipe.AddIngredient(ItemID.FossilOre, 25);
            recipe.AddIngredient(ModContent.ItemType<DesertFeather>(), 5);
            recipe.AddTile(TileID.Bookcases);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
