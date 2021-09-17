using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class Phangasm : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Phangasm");
            Tooltip.SetDefault("Fires a spread of 5 arrows and emits phantom arrows on enemy hits");
        }

        public override void SetDefaults()
        {
            item.damage = 110;
            item.width = 20;
            item.height = 12;
            item.useTime = 12;
            item.useAnimation = 12;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.knockBack = 3f;
            item.value = Item.buyPrice(1, 80, 0, 0);
            item.rare = ItemRarityID.Red;
            item.UseSound = SoundID.Item5;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.ranged = true;
            item.channel = true;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<PhangasmBow>();
            item.shootSpeed = 20f;
            item.useAmmo = AmmoID.Arrow;
            item.Calamity().customRarity = CalamityRarity.DarkBlue;
            item.Calamity().canFirePointBlankShots = true;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[item.shoot] <= 0;

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ModContent.ProjectileType<PhangasmBow>(), damage, knockBack, player.whoAmI);
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Phantasm);
            recipe.AddIngredient(ModContent.ItemType<CosmiliteBar>(), 8);
            recipe.AddIngredient(ModContent.ItemType<Phantoplasm>(), 20);
            recipe.AddIngredient(ModContent.ItemType<NightmareFuel>(), 20);
            recipe.AddTile(ModContent.TileType<CosmicAnvil>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
