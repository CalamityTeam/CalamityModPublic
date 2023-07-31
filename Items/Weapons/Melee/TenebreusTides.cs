using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Projectiles.Melee.Spears;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class TenebreusTides : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public override void SetStaticDefaults()
        {
            ItemID.Sets.Spears[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 80;
            Item.knockBack = 4.5f;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = Item.useTime = 14;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<TenebreusTidesProjectile>();
            Item.shootSpeed = 12f;

            Item.value = CalamityGlobalItem.Rarity9BuyPrice;
            Item.rare = ItemRarityID.Cyan;
            Item.Calamity().donorItem = true;

            Item.width = Item.height = 68;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundID.Item1;
            Item.noMelee = true;
            Item.useTurn = true;
            Item.noUseGraphic = true;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0;

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<AmidiasTrident>().
                AddIngredient<Atlantis>().
                AddIngredient(ItemID.InfluxWaver).
                AddIngredient<SeaPrism>(20).
                AddIngredient<PlantyMush>(25).
                AddIngredient<Lumenyl>(50).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
