using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class TriactisTruePaladinianMageHammerofMightMelee : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Triactis' True Paladinian Mage-Hammer of Might");
            Tooltip.SetDefault("Explodes on enemy hits");
        }

        public override void SetDefaults()
        {
            item.width = 160;
            item.height = 160;
            item.damage = 2000;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.autoReuse = true;
            item.useAnimation = 10;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 10;
            item.knockBack = 50f;
            item.UseSound = SoundID.Item1;
            item.melee = true;
            item.shoot = ModContent.ProjectileType<TriactisHammerProj>();
            item.shootSpeed = 25f;

            item.value = CalamityGlobalItem.Rarity16BuyPrice;
            item.Calamity().customRarity = CalamityRarity.HotPink;
            item.Calamity().devItem = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<GalaxySmasherMelee>());
            recipe.AddIngredient(ItemID.SoulofMight, 30);
            recipe.AddIngredient(ModContent.ItemType<ShadowspecBar>(), 5);
            recipe.AddTile(ModContent.TileType<DraedonsForge>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
