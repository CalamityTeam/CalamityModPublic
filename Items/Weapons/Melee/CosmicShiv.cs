using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee.Shortswords;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class CosmicShiv : ModItem, ILocalizedModType
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
            Item.damage = 270;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = Item.useTime = 40;
            Item.shoot = ModContent.ProjectileType<CosmicShivProj>();
            Item.shootSpeed = 2.4f;
            Item.knockBack = 9f;
            Item.UseSound = SoundID.Item1;
            Item.value = CalamityGlobalItem.RarityDarkBlueBuyPrice;
            Item.rare = ModContent.RarityType<DarkBlue>();
            Item.Calamity().donorItem = true; // revertusernamechange on Discord (Yatagarasu)
        }

        public override bool MeleePrefix() => true;

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<ElementalShiv>().
                AddIngredient<CosmiliteBar>(8).
                AddTile<CosmicAnvil>().
                Register();
        }
    }
}