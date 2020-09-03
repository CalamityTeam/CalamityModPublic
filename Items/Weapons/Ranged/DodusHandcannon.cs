using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class DodusHandcannon : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dodu's Handcannon");
            Tooltip.SetDefault("The power of the nut rests in your hands");
        }

        public override void SetDefaults()
        {
            item.width = 70;
            item.height = 42;
            item.damage = 485;
            item.crit += 16;
            item.ranged = true;
            item.useTime = 30;
            item.useAnimation = 30;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 10f;

            item.value = Item.buyPrice(1, 40, 0, 0);
            item.rare = 10;
            item.Calamity().customRarity = CalamityRarity.Dedicated;

            item.UseSound = mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/LargeWeaponFire");

            item.shootSpeed = 24f;
            item.shoot = ProjectileID.BulletHighVelocity;
            item.useAmmo = AmmoID.Bullet;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-15, 5);
        }

        public override bool CanUseItem(Player player)
        {
            return CalamityGlobalItem.HasEnoughAmmo(player, item, 5);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            for (int i = 0; i < 5; i++)
                Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ProjectileID.BulletHighVelocity, damage, knockBack, player.whoAmI, 0f, 0f);

            // Consume 5 ammo per shot
            CalamityGlobalItem.ConsumeAdditionalAmmo(player, item, 5);

            return false;
        }

        // Disable vanilla ammo consumption
        public override bool ConsumeAmmo(Player player)
        {
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<Aeries>());
            recipe.AddIngredient(ModContent.ItemType<RuinousSoul>(), 5);
            recipe.AddIngredient(ItemID.LunarBar, 15);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
