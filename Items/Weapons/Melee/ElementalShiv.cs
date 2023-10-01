using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee.Shortswords;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    [LegacyName("ElementalShortsword")]
    public class ElementalShiv : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";

        public override void SetDefaults()
        {
            Item.width = 44;
            Item.height = 44;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.damage = 190;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = Item.useTime = 13;
            Item.shoot = ModContent.ProjectileType<ElementalShivProj>();
            Item.shootSpeed = 2.4f;
            Item.knockBack = 8.5f;
            Item.UseSound = SoundID.Item1;
            Item.value = CalamityGlobalItem.Rarity11BuyPrice;
            Item.rare = ItemRarityID.Purple;
        }

        public override bool MeleePrefix() => true;

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.PiercingStarlight).
                AddIngredient(ItemID.LunarBar, 5).
                AddIngredient<LifeAlloy>(5).
                AddIngredient<GalacticaSingularity>(5).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
