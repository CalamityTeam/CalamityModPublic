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
            item.damage = 40;
            item.ranged = true;
            item.width = 64;
            item.height = 32;
            item.useTime = 15;
            item.useAnimation = 15;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 4.5f;
            item.value = CalamityGlobalItem.Rarity10BuyPrice;
            item.rare = ItemRarityID.Red;
            item.UseSound = SoundID.Item36;
            item.autoReuse = true;
            item.shoot = ProjectileID.BlackBolt;
            item.shootSpeed = 24f;
            item.useAmmo = AmmoID.Bullet;
            item.Calamity().canFirePointBlankShots = true;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-5, 0);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Vector2 velocity = new Vector2(speedX, speedY);

            // Fire the Onyx Shard that is characteristic of the Onyx Blaster
            // The shard deals 250% damage and double knockback
            int shardDamage = (int)(2.5f * damage);
            float shardKB = 2f * knockBack;
            Vector2 offset = new Vector2(Main.rand.Next(-25, 26) * 0.05f, Main.rand.Next(-25, 26) * 0.05f);
            Projectile shard = Projectile.NewProjectileDirect(position, velocity + offset, ProjectileID.BlackBolt, shardDamage, shardKB, player.whoAmI, 0f, 0f);
            shard.timeLeft = (int)(shard.timeLeft * 1.25f);

            for (int i = 0; i < 4; i++)
            {
                offset = new Vector2(Main.rand.Next(-45, 46) * 0.05f, Main.rand.Next(-45, 46) * 0.05f);
                Projectile.NewProjectile(position, velocity + offset, type, damage, knockBack, player.whoAmI, 0f, 0f);
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
