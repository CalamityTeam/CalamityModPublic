using Terraria.DataStructures;
using Terraria.DataStructures;
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
            Item.damage = 150;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 44;
            Item.height = 26;
            Item.useTime = 13;
            Item.useAnimation = 13;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 6f;
            Item.value = CalamityGlobalItem.Rarity12BuyPrice;
            Item.rare = ItemRarityID.Purple;
            Item.Calamity().customRarity = CalamityRarity.Turquoise;
            Item.UseSound = SoundID.Item108;
            Item.autoReuse = true;
            Item.shootSpeed = 9f;
            Item.shoot = ModContent.ProjectileType<SpykerProj>();
            Item.useAmmo = AmmoID.Bullet;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-5, 0);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (type == ProjectileID.Bullet)
                Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<SpykerProj>(), damage, knockback, player.whoAmI);
            else
                Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ModContent.ItemType<Needler>()).AddIngredient(ItemID.Stynger).AddIngredient(ModContent.ItemType<UeliaceBar>(), 5).AddTile(TileID.LunarCraftingStation).Register();
        }
    }
}
