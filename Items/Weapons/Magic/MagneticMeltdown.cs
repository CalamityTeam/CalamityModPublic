using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class MagneticMeltdown : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Magnetic Meltdown");
            Tooltip.SetDefault("Launches a diamond cross of supercharged magnet spheres");
            Item.staff[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.damage = 60;
            item.magic = true;
            item.mana = 40;
            item.width = 78;
            item.height = 78;
            item.useTime = 49;
            item.useAnimation = 49;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 4f;
            item.UseSound = SoundID.Item20;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<MagneticOrb>();
            item.shootSpeed = 12f;

            item.value = CalamityGlobalItem.Rarity12BuyPrice;
            item.Calamity().customRarity = CalamityRarity.Turquoise;
        }

        public override Vector2? HoldoutOrigin() => new Vector2(15, 15);

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Vector2 v = new Vector2(speedX, speedY);
            float offset = 3f;

            // Fire four orbs at once
            Projectile.NewProjectile(position, v + offset * Vector2.UnitX, type, damage, knockBack, player.whoAmI, 1f);
            Projectile.NewProjectile(position, v - offset * Vector2.UnitX, type, damage, knockBack, player.whoAmI, 1f);
            Projectile.NewProjectile(position, v + offset * Vector2.UnitY, type, damage, knockBack, player.whoAmI, 1f);
            Projectile.NewProjectile(position, v - offset * Vector2.UnitY, type, damage, knockBack, player.whoAmI, 1f);
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<DarkPlasma>(), 3);
            recipe.AddIngredient(ItemID.SpectreStaff);
            recipe.AddIngredient(ItemID.MagnetSphere);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
