using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class MagnaStriker : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Magna Striker");
            Tooltip.SetDefault("Fires a string of opal and magna strikes");
        }

        public override void SetDefaults()
        {
            item.damage = 35;
            item.ranged = true;
            item.width = 72;
            item.height = 38;
            item.useTime = 5;
            item.reuseDelay = 6;
            item.useAnimation = 20;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 2.25f;
            item.value = Item.buyPrice(0, 80, 0, 0);
            item.rare = ItemRarityID.Yellow;
            item.UseSound = mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/OpalStrike");
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<OpalStrike>();
            item.shootSpeed = 15f;
            item.useAmmo = AmmoID.Bullet;
            item.Calamity().canFirePointBlankShots = true;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-5, 0);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            int randomProj = Main.rand.Next(2);
            if (randomProj == 0)
            {
                Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ModContent.ProjectileType<OpalStrike>(), (int)(damage * 0.75), knockBack, player.whoAmI, 0f, 0f);
            }
            else
            {
                Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ModContent.ProjectileType<MagnaStrike>(), damage, knockBack, player.whoAmI, 0f, 0f);
            }
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<OpalStriker>());
            recipe.AddIngredient(ModContent.ItemType<MagnaCannon>());
            recipe.AddRecipeGroup("AnyAdamantiteBar", 6);
            recipe.AddIngredient(ItemID.Ectoplasm, 5);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
