using Terraria.DataStructures;
using Terraria.DataStructures;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class CosmicBolter : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cosmic Bolter");
            Tooltip.SetDefault("Fires three arrows at once\n" +
                "Converts wooden arrows into sliding energy bolts");
        }

        public override void SetDefaults()
        {
            Item.damage = 39;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 40;
            Item.height = 76;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2.75f;
            Item.value = Item.buyPrice(0, 80, 0, 0);
            Item.rare = ItemRarityID.Yellow;
            Item.UseSound = SoundID.Item75;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<LunarBolt2>();
            Item.shootSpeed = 10f;
            Item.useAmmo = AmmoID.Arrow;
            Item.Calamity().canFirePointBlankShots = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 source = player.RotatedRelativePoint(player.MountedCenter, true);
            float piOver10 = MathHelper.Pi * 0.1f;
            int projAmt = 3;

            velocity.Normalize();
            velocity *= 30f;
            bool canHit = Collision.CanHit(source, 0, 0, source + velocity, 0, 0);
            for (int i = 0; i < projAmt; i++)
            {
                float offsetAmt = i - (projAmt - 1f) / 2f;
                Vector2 offset = velocity.RotatedBy(piOver10 * offsetAmt, default);
                if (!canHit)
                    offset -= velocity;

                if (type == ProjectileID.WoodenArrowFriendly)
                    Projectile.NewProjectile(source + offset, velocity, ModContent.ProjectileType<LunarBolt2>(), damage, knockback, player.whoAmI);
                else
                {
                    int proj = Projectile.NewProjectile(source + offset, velocity, type, damage, knockback, player.whoAmI);
                    Main.projectile[proj].noDropItem = true;
                }
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ModContent.ItemType<LunarianBow>()).AddIngredient(ModContent.ItemType<LivingShard>(), 5).AddIngredient(ItemID.HallowedBar, 5).AddIngredient(ItemID.SoulofSight, 5).AddTile(TileID.MythrilAnvil).Register();
        }
    }
}
