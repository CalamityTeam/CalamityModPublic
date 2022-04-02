using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class Spyker : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spyker");
            Tooltip.SetDefault("Converts musket balls into spikes that stick to enemies, tiles and explode into shrapnel");
        }

        public override void SetDefaults()
        {
            item.damage = 150;
            item.ranged = true;
            item.width = 44;
            item.height = 26;
            item.useTime = 13;
            item.useAnimation = 13;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 6f;
            item.value = CalamityGlobalItem.Rarity12BuyPrice;
            item.rare = ItemRarityID.Purple;
            item.Calamity().customRarity = CalamityRarity.Turquoise;
            item.UseSound = SoundID.Item108;
            item.autoReuse = true;
            item.shootSpeed = 9f;
            item.shoot = ModContent.ProjectileType<SpykerProj>();
            item.useAmmo = AmmoID.Bullet;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-5, 0);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (type == ProjectileID.Bullet)
                Projectile.NewProjectile(position, new Vector2(speedX, speedY), ModContent.ProjectileType<SpykerProj>(), damage, knockBack, player.whoAmI);
            else
                Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI);

            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<Needler>());
            recipe.AddIngredient(ItemID.Stynger);
            recipe.AddIngredient(ModContent.ItemType<UeliaceBar>(), 5);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
