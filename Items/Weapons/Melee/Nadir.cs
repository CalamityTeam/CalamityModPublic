using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee.Spears;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class Nadir : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public static int BaseDamage = 365;
        public static float ProjShootSpeed = 20f;
        public static int FadeoutSpeed = 20;

        public override void SetStaticDefaults()
        {
            ItemID.Sets.Spears[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 144;
            Item.height = 144;
            Item.noUseGraphic = true;
            Item.DamageType = DamageClass.Melee;
            Item.damage = BaseDamage;
            Item.knockBack = 8f;
            Item.useAnimation = 18;
            Item.useTime = 18;
            Item.autoReuse = true;
            Item.noMelee = true;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundID.Item1;

            Item.value = CalamityGlobalItem.Rarity15BuyPrice;
            Item.rare = ModContent.RarityType<Violet>();
            Item.Calamity().donorItem = true;

            Item.shoot = ModContent.ProjectileType<NadirSpear>();
            Item.shootSpeed = 12f; // This isn't the projectile's speed, it's the spear's.
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0;

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<ElementalLance>().
                AddIngredient<AuricBar>(5).
                AddIngredient<TwistingNether>(5).
                AddIngredient<DarksunFragment>(8).
                AddTile<CosmicAnvil>().
                Register();
        }
    }
}
