using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    [LegacyName("Fabstaff")]
    public class Sylvestaff : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";

        /// <summary>
        ///     The amount by which the staff recoils after firing.
        /// </summary>
        public static float StaffRecoilForce => 0.13f;

        /// <summary>
        ///     The rate at which rays from the staff can release bolts.
        /// </summary>
        public static int RayBoltShootRate => CalamityUtils.SecondsToFrames(0.065f);

        /// <summary>
        ///     The distance range that targets need to be within relative to a ray's evaluation points in order to shoot bolts.
        /// </summary>
        public static float RayBoltTargetingRange => 272f;

        public override void SetStaticDefaults()
        {
            Item.staff[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 84;
            Item.height = 84;
            Item.damage = 125;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 50;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.channel = true;
            Item.noUseGraphic = true;
            Item.knockBack = 5f;

            Item.value = CalamityGlobalItem.RarityHotPinkBuyPrice;
            Item.rare = ModContent.RarityType<HotPink>();
            Item.Calamity().devItem = true;

            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<SylvestaffHoldout>();
            Item.shootSpeed = 13.5f;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.RainbowRod).
                AddIngredient(ItemID.GenderChangePotion).
                AddIngredient<Necroplasm>(10).
                AddIngredient<ShadowspecBar>(5).
                AddTile<DraedonsForge>().
                Register();
        }
    }
}
