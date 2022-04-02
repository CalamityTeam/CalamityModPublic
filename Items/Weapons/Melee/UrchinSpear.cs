using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee.Spears;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class UrchinSpear : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Urchin Spear");
            Tooltip.SetDefault("Poisons enemies and fires short-range stingers");
        }

        public override void SetDefaults()
        {
            item.width = 56;
            item.damage = 28;
            item.melee = true;
            item.noMelee = true;
            item.useTurn = true;
            item.noUseGraphic = true;
            item.useAnimation = 25;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.useTime = 25;
            item.knockBack = 5f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 56;
            item.value = Item.buyPrice(0, 2, 0, 0);
            item.rare = ItemRarityID.Green;
            item.shoot = ModContent.ProjectileType<UrchinSpearProjectile>();
            item.shootSpeed = 4f;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[item.shoot] <= 0;

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<VictideBar>(), 4);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
