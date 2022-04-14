using Terraria.DataStructures;
using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class ClockGatlignum : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Clock Gatlignum");
            Tooltip.SetDefault("33% chance to not consume ammo\n" +
                "Converts musket balls into strings of 3 high velocity bullets");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 55;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 66;
            Item.height = 34;
            Item.useTime = 3;
            Item.reuseDelay = 12;
            Item.useAnimation = 9;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 3.75f;
            Item.value = Item.buyPrice(0, 80, 0, 0);
            Item.rare = ItemRarityID.Yellow;
            Item.UseSound = SoundID.Item31;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 20f;
            Item.useAmmo = AmmoID.Bullet;
            Item.Calamity().canFirePointBlankShots = true;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-5, 0);

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float SpeedX = velocity.X + Main.rand.Next(-15, 16) * 0.05f;
            float SpeedY = velocity.Y + Main.rand.Next(-15, 16) * 0.05f;

            if (type == ProjectileID.Bullet)
                Projectile.NewProjectile(source, position.X, position.Y, SpeedX, SpeedY, ProjectileID.BulletHighVelocity, damage, knockback, player.whoAmI);
            else
                Projectile.NewProjectile(source, position.X, position.Y, SpeedX, SpeedY, type, damage, knockback, player.whoAmI);

            return false;
        }

        public override bool CanConsumeAmmo(Player player)
        {
            if (Main.rand.Next(0, 100) < 33)
                return false;
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ItemID.Gatligator).AddIngredient(ItemID.VenusMagnum).AddIngredient(ItemID.ClockworkAssaultRifle).AddIngredient(ModContent.ItemType<BarofLife>(), 3).AddIngredient(ItemID.Ectoplasm, 5).AddTile(TileID.MythrilAnvil).Register();
        }
    }
}
