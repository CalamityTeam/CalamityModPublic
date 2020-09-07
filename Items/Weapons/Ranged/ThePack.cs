using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class ThePack : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Pack");
            Tooltip.SetDefault("Fires large homing rockets that explode into more homing mini rockets when in proximity to an enemy");
        }

        public override void SetDefaults()
        {
            item.damage = 2500;
            item.crit += 8;
            item.ranged = true;
            item.width = 96;
            item.height = 40;
            item.useTime = 52;
            item.useAnimation = 52;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 7.5f;
            item.value = Item.buyPrice(1, 80, 0, 0);
            item.rare = 10;
            item.Calamity().customRarity = CalamityRarity.Dedicated;
            item.UseSound = SoundID.Item11;
            item.autoReuse = true;
            item.shootSpeed = 24f;
            item.shoot = ModContent.ProjectileType<ThePackMissile>();
            item.useAmmo = AmmoID.Rocket;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-40, 0);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ModContent.ProjectileType<ThePackMissile>(), damage, knockBack, player.whoAmI, 0.0f, 0.0f);
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<Scorpion>());
            recipe.AddIngredient(ItemID.MarbleBlock, 50);
            recipe.AddIngredient(ModContent.ItemType<CosmiliteBar>(), 10);
            recipe.AddIngredient(ModContent.ItemType<ArmoredShell>(), 4);
            recipe.AddTile(ModContent.TileType<DraedonsForge>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
