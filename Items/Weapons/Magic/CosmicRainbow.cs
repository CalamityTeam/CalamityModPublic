using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class CosmicRainbow : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cosmic Rainbow");
            Tooltip.SetDefault("Launches a barrage of rainbows");
        }

        public override void SetDefaults()
        {
            item.damage = 105;
            item.magic = true;
            item.mana = 30;
            item.width = 26;
            item.height = 64;
            item.useTime = 35;
            item.useAnimation = 35;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 0f;
            item.value = Item.buyPrice(0, 95, 0, 0);
            item.rare = 9;
            item.UseSound = SoundID.Item67;
            item.autoReuse = true;
            item.shoot = ProjectileID.RainbowFront;
            item.shootSpeed = 18f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.RainbowGun);
            recipe.AddIngredient(ItemID.PearlwoodBow);
            recipe.AddIngredient(ModContent.ItemType<MeldiateBar>(), 5);
            recipe.AddIngredient(ModContent.ItemType<CoreofCalamity>());
            recipe.AddIngredient(ModContent.ItemType<BarofLife>(), 5);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Vector2 source = player.RotatedRelativePoint(player.MountedCenter, true);
            float piOverTen = MathHelper.Pi * 0.1f;
            int projAmt = 3;
            Vector2 velocity = new Vector2(speedX, speedY);
            velocity.Normalize();
            velocity *= 60f;
            bool canHit = Collision.CanHit(source, 0, 0, source + velocity, 0, 0);
            for (int i = 0; i < projAmt; i++)
            {
                float offsetAmt = (float)i - ((float)projAmt - 1f) / 2f;
                Vector2 offset = velocity.RotatedBy((double)(piOverTen * offsetAmt), default);
                if (!canHit)
                {
                    offset -= velocity;
                }
                Projectile.NewProjectile(source.X + offset.X, source.Y + offset.Y, speedX, speedY, type, damage, knockBack, player.whoAmI, 0f, 0f);
            }

			CalamityUtils.ProjectileToMouse(player, 2, item.shootSpeed, 0f, 15f, type, damage, knockBack, player.whoAmI, true);
            return false;
        }
    }
}
