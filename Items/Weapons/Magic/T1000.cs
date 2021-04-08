using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class T1000 : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("T1000");
            Tooltip.SetDefault("Fires a spread of rainbow lasers");
        }

        public override void SetDefaults()
        {
            item.damage = 150;
            item.magic = true;
            item.mana = 10;
            item.width = 20;
            item.height = 12;
            item.useTime = 12;
            item.useAnimation = 12;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.channel = true;
            item.knockBack = 4f;
            item.value = Item.buyPrice(1, 80, 0, 0);
            item.rare = ItemRarityID.Red;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<T1000Proj>();
            item.shootSpeed = 24f;
            item.Calamity().customRarity = CalamityRarity.DarkBlue;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[item.shoot] <= 0;

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<Purge>());
            recipe.AddIngredient(ModContent.ItemType<CosmiliteBar>(), 5);
            recipe.AddIngredient(ModContent.ItemType<AscendantSpiritEssence>());
            recipe.AddTile(ModContent.TileType<DraedonsForge>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ModContent.ProjectileType<T1000Proj>(), damage, knockBack, player.whoAmI, 0.0f, 0.0f);
            return false;
        }
    }
}
