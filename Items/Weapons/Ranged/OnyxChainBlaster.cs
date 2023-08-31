using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class OnyxChainBlaster : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        public override void SetDefaults()
        {
            Item.damage = 51;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 64;
            Item.height = 32;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 4.5f;
            Item.value = CalamityGlobalItem.Rarity10BuyPrice;
            Item.rare = ItemRarityID.Red;
            Item.UseSound = SoundID.Item36;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.BlackBolt;
            Item.shootSpeed = 24f;
            Item.useAmmo = AmmoID.Bullet;
            Item.Calamity().canFirePointBlankShots = true;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-5, 0);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            // Fire the Onyx Shard that is characteristic of the Onyx Blaster
            // The shard deals 250% damage and double knockback
            int shardDamage = (int)(2.5f * damage);
            float shardKB = 2f * knockback;
            Vector2 offset = new Vector2(Main.rand.Next(-25, 26) * 0.05f, Main.rand.Next(-25, 26) * 0.05f);
            Projectile shard = Projectile.NewProjectileDirect(source, position, velocity + offset, ProjectileID.BlackBolt, shardDamage, shardKB, player.whoAmI, 0f, 0f);
            shard.timeLeft = (int)(shard.timeLeft * 1.25f);

            for (int i = 0; i < 4; i++)
            {
                offset = new Vector2(Main.rand.Next(-45, 46) * 0.05f, Main.rand.Next(-45, 46) * 0.05f);
                Projectile.NewProjectile(source, position, velocity + offset, type, damage, knockback, player.whoAmI, 0f, 0f);
            }
            return false;
        }

        public override bool CanConsumeAmmo(Item ammo, Player player)
        {
            if (Main.rand.Next(0, 100) < 50)
                return false;
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.OnyxBlaster).
                AddIngredient(ItemID.ChainGun).
                AddIngredient(ItemID.LunarBar, 5).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
