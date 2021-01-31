using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class SomaPrime : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Soma Prime");
            Tooltip.SetDefault("Shoots extremely powerful high velocity rounds that inflict a powerful bleed debuff");
        }

        public override void SetDefaults()
        {
            item.damage = 1000;
            item.ranged = true;
            item.width = 94;
            item.height = 34;
            item.useTime = 2;
            item.useAnimation = 2;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 2f;
            item.UseSound = SoundID.Item40;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<SlashRound>();
            item.shootSpeed = 30f;
            item.useAmmo = AmmoID.Bullet;

            item.value = CalamityGlobalItem.Rarity16BuyPrice;
            item.Calamity().customRarity = CalamityRarity.ItemSpecific;
            item.Calamity().devItem = true;
        }

        // Terraria seems to really dislike high crit values in SetDefaults
        public override void GetWeaponCrit(Player player, ref int crit) => crit += 40;

        public override Vector2? HoldoutOffset() => new Vector2(-25, 0);

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<ShadowspecBar>(), 5);
            recipe.AddIngredient(ModContent.ItemType<P90>());
            recipe.AddIngredient(ModContent.ItemType<Minigun>());
            recipe.AddTile(ModContent.TileType<DraedonsForge>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            for (int i = 0; i < 2; i++)
            {
                float SpeedX = speedX + (float)Main.rand.Next(-10, 11) * 0.05f;
                float SpeedY = speedY + (float)Main.rand.Next(-10, 11) * 0.05f;
                Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, ModContent.ProjectileType<SlashRound>(), damage, knockBack, player.whoAmI, 0f, 0f);
            }
            return false;
        }

        public override bool ConsumeAmmo(Player player)
        {
            if (Main.rand.Next(0, 100) < 50)
                return false;
            return true;
        }
    }
}
