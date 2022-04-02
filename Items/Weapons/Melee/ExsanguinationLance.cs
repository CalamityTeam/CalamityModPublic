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
            item.width = 44;
            item.damage = 90;
            item.melee = true;
            item.noMelee = true;
            item.useTurn = true;
            item.noUseGraphic = true;
            item.useAnimation = 22;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.useTime = 22;
            item.knockBack = 6.75f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 44;
            item.value = Item.buyPrice(0, 80, 0, 0);
            item.rare = ItemRarityID.Yellow;
            item.shoot = ModContent.ProjectileType<ExsanguinationLanceProjectile>();
            item.shootSpeed = 10f;
            item.Calamity().trueMelee = true;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[item.shoot] <= 0;

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<CruptixBar>(), 10);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
