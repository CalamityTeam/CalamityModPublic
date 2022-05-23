using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class Downpour : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Downpour");
            Tooltip.SetDefault("Fires a spray of water that drips extra trails of water");
            Item.staff[Item.type] = true;
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 43;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 10;
            Item.width = 42;
            Item.height = 42;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 3f;
            Item.value = Item.buyPrice(0, 36, 0, 0);
            Item.rare = ItemRarityID.Pink;
            Item.UseSound = SoundID.Item13;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<WaterStream>();
            Item.shootSpeed = 14f;
        }

        public override Vector2? HoldoutOrigin() => new Vector2(10, 10);

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 0.0f, 0.0f);
            return false;
        }
    }
}
