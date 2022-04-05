using Terraria.DataStructures;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class BarracudaGun : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Barracuda Gun");
            Tooltip.SetDefault("Fires two barracudas that latch onto enemies");
        }

        public override void SetDefaults()
        {
            Item.damage = 63;
            Item.channel = true;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 54;
            Item.height = 28;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 1f;
            Item.value = Item.buyPrice(0, 80, 0, 0);
            Item.rare = ItemRarityID.Yellow;
            Item.UseSound = SoundID.Item10;
            Item.autoReuse = true;
            Item.shootSpeed = 15f;
            Item.shoot = ModContent.ProjectileType<MechanicalBarracuda>();
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-10, 0);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (int index = 0; index < 2; ++index)
            {
                float SpeedX = velocity.X + (float)Main.rand.Next(-10, 11) * 0.05f;
                float SpeedY = velocity.Y + (float)Main.rand.Next(-10, 11) * 0.05f;
                Projectile.NewProjectile(source, position.X, position.Y, SpeedX, SpeedY, type, damage, knockback, player.whoAmI, 0f, 0f);
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ItemID.PiranhaGun).AddIngredient(ModContent.ItemType<CoreofCalamity>(), 2).AddIngredient(ModContent.ItemType<BarofLife>()).AddIngredient(ModContent.ItemType<Tenebris>(), 5).AddIngredient(ItemID.SharkFin, 2).AddTile(TileID.MythrilAnvil).Register();
        }
    }
}
