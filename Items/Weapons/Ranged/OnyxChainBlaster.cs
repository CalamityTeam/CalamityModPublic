using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class OnyxChainBlaster : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Onyx Chain Blaster");
            Tooltip.SetDefault("50% chance to not consume ammo\n" +
                "Fires a spread of bullets and an onyx shard");
        }

        public override void SetDefaults()
        {
            item.damage = 42;
            item.ranged = true;
            item.width = 64;
            item.height = 32;
            item.useTime = 15;
            item.useAnimation = 15;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 4.5f;
			item.value = CalamityGlobalItem.Rarity11BuyPrice;
			item.rare = ItemRarityID.Purple;
			item.UseSound = SoundID.Item36;
            item.autoReuse = true;
            item.shoot = ProjectileID.PurificationPowder;
            item.shootSpeed = 24f;
            item.useAmmo = AmmoID.Bullet;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-5, 0);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            float SpeedX = speedX + Main.rand.Next(-25, 26) * 0.05f;
            float SpeedY = speedY + Main.rand.Next(-25, 26) * 0.05f;
            Projectile.NewProjectile(position.X, position.Y, SpeedX * 0.9f, SpeedY * 0.9f, ProjectileID.BlackBolt, damage, knockBack, player.whoAmI, 0f, 0f);
            for (int i = 0; i < 4; i++)
            {
                float SpeedNewX = speedX + Main.rand.Next(-45, 46) * 0.05f;
                float SpeedNewY = speedY + Main.rand.Next(-45, 46) * 0.05f;
                Projectile.NewProjectile(position.X, position.Y, SpeedNewX, SpeedNewY, type, (int)(damage * 1.25f), knockBack, player.whoAmI, 0f, 0f);
            }
            return false;
        }

        public override bool ConsumeAmmo(Player player)
        {
            if (Main.rand.Next(0, 100) < 50)
                return false;
            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.OnyxBlaster);
            recipe.AddIngredient(ItemID.ChainGun);
            recipe.AddIngredient(ItemID.LunarBar, 5);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
