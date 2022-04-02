using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee.Spears;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class StarnightLance : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Starnight Lance");
            Tooltip.SetDefault("Shoots a starnight beam");
        }

        public override void SetDefaults()
        {
            item.width = 72;
            item.damage = 110;
            item.melee = true;
            item.noMelee = true;
            item.useTurn = true;
            item.noUseGraphic = true;
            item.useAnimation = 23;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.useTime = 23;
            item.knockBack = 6;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 72;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.rare = ItemRarityID.Pink;
            item.shoot = ModContent.ProjectileType<StarnightLanceProjectile>();
            item.shootSpeed = 6f;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[item.shoot] <= 0;

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<VerstaltiteBar>(), 12);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
