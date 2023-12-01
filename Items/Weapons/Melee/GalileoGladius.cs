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
            Item.width = 44;
            Item.height = 44;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.damage = 92;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = Item.useTime = 12;
            Item.shoot = ModContent.ProjectileType<GalileoGladiusProj>();
            Item.shootSpeed = 0.9f;
            Item.knockBack = 10f;
            Item.UseSound = SoundID.Item1;
            Item.value = CalamityGlobalItem.Rarity13BuyPrice;
            Item.rare = ModContent.RarityType<PureGreen>();
        }

        public override bool MeleePrefix() => true;

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
