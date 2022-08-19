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
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 160;
            Item.height = 160;
            Item.damage = 2000;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
            Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 10;
            Item.knockBack = 50f;
            Item.UseSound = SoundID.Item1;
            Item.DamageType = DamageClass.MeleeNoSpeed;
            Item.shoot = ModContent.ProjectileType<TriactisHammerProj>();
            Item.shootSpeed = 25f;

            Item.value = CalamityGlobalItem.Rarity16BuyPrice;
            Item.Calamity().customRarity = CalamityRarity.HotPink;
            Item.Calamity().devItem = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<GalaxySmasher>().
                AddIngredient(ItemID.SoulofMight, 30).
                AddIngredient<ShadowspecBar>(5).
                AddTile<DraedonsForge>().
                Register();
        }
    }
}
