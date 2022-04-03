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
            Item.damage = 49;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 17;
            Item.width = 56;
            Item.height = 26;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 3.75f;
            Item.value = CalamityGlobalItem.Rarity10BuyPrice;
            Item.rare = ItemRarityID.Red;
            Item.UseSound = SoundID.Item95;
            Item.autoReuse = true;
            Item.shootSpeed = 13f;
            Item.shoot = ModContent.ProjectileType<UberBubble>();
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
            CreateRecipe(1).AddIngredient(ItemID.BubbleGun).AddIngredient(ItemID.Xenopopper).AddIngredient(ItemID.LunarBar, 5).AddIngredient(ModContent.ItemType<SeaPrism>(), 15).AddTile(TileID.LunarCraftingStation).Register();
        }
    }
}
