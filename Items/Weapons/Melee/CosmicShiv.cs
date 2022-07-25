using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee.Shortswords;
using CalamityMod.Tiles.Furniture.CraftingStations;


namespace CalamityMod.Items.Weapons.Melee
{
    public class CosmicShiv : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cosmic Shiv");
            Tooltip.SetDefault("Fires a cosmic beam that homes in on enemies\n" +
                "Upon hitting an enemy, a barrage of offscreen objects home in on the enemy as well as raining stars");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.useAnimation = 18;
            Item.useTime = 18;
            Item.width = 44;
            Item.height = 44;

            Item.damage = 218;

            Item.DamageType = DamageClass.Melee;
            Item.knockBack = 9f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.shoot = ModContent.ProjectileType<CosmicShivProj>();
            Item.shootSpeed = 2.4f;

            Item.value = CalamityGlobalItem.Rarity14BuyPrice;
            Item.Calamity().customRarity = CalamityRarity.DarkBlue;
            Item.Calamity().donorItem = true; //Yatagarasu#0001
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<ElementalShiv>().
                AddIngredient<CosmiliteBar>(8).
                AddTile(ModContent.TileType<CosmicAnvil>()).
                Register();
        }
    }
}
