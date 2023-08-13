using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Ores;
using CalamityMod.Projectiles.Melee.Shortswords;
using CalamityMod.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class GalileoGladius : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";

        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = 8;
            Item.useTime = 8;
            Item.width = 44;
            Item.height = 44;
            Item.damage = 110;
            Item.knockBack = 10f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.shoot = ModContent.ProjectileType<GalileoGladiusProj>();
            Item.shootSpeed = 0.9f;

            Item.value = CalamityGlobalItem.Rarity13BuyPrice;
            Item.rare = ModContent.RarityType<PureGreen>();
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Gladius).
                AddIngredient<Lumenyl>(8).
                AddIngredient<RuinousSoul>(5).
                AddIngredient<ExodiumCluster>(15).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
