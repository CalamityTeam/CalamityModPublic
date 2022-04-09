using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee.Spears;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class ExsanguinationLance : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Vulcanite Lance");
            Tooltip.SetDefault("Explodes on enemy hits and summons homing flares on critical hits");
        }

        public override void SetDefaults()
        {
            Item.width = 44;
            Item.damage = 90;
            Item.DamageType = DamageClass.Melee;
            Item.noMelee = true;
            Item.useTurn = true;
            Item.noUseGraphic = true;
            Item.useAnimation = 22;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTime = 22;
            Item.knockBack = 6.75f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.height = 44;
            Item.value = Item.buyPrice(0, 80, 0, 0);
            Item.rare = ItemRarityID.Yellow;
            Item.shoot = ModContent.ProjectileType<ExsanguinationLanceProjectile>();
            Item.shootSpeed = 10f;
            Item.Calamity().trueMelee = true;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0;

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<CruptixBar>(10).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
