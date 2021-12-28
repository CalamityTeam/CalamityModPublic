using CalamityMod.Items.Placeables;
using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class Effervescence : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Effervescence");
            Tooltip.SetDefault("Shoots a massive spread of bubbles");
        }

        public override void SetDefaults()
        {
            item.damage = 49;
            item.magic = true;
            item.mana = 17;
            item.width = 56;
            item.height = 26;
            item.useTime = 15;
            item.useAnimation = 15;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 3.75f;
            item.value = CalamityGlobalItem.Rarity10BuyPrice;
            item.rare = ItemRarityID.Red;
            item.UseSound = SoundID.Item95;
            item.autoReuse = true;
            item.shootSpeed = 13f;
            item.shoot = ModContent.ProjectileType<UberBubble>();
        }

        public override Vector2? HoldoutOffset() => new Vector2(-5, 0);

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            for (int randomBullets = 0; randomBullets < 4; randomBullets++)
            {
                float SpeedX = speedX + Main.rand.Next(-25, 26) * 0.05f;
                float SpeedY = speedY + Main.rand.Next(-25, 26) * 0.05f;
                Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, type, damage, knockBack, player.whoAmI, 0f, 0f);
            }
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.BubbleGun);
            recipe.AddIngredient(ItemID.Xenopopper);
            recipe.AddIngredient(ItemID.LunarBar, 5);
            recipe.AddIngredient(ModContent.ItemType<SeaPrism>(), 15);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
