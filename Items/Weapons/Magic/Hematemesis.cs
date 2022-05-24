using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class Hematemesis : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hematemesis");
            Tooltip.SetDefault("Casts a barrage of blood geysers from below");
            Item.staff[Item.type] = true;
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 75;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 14;
            Item.rare = ItemRarityID.Yellow;
            Item.width = 48;
            Item.height = 54;
            Item.useTime = 22;
            Item.useAnimation = 22;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 3.75f;
            Item.value = Item.buyPrice(0, 80, 0, 0);
            Item.UseSound = SoundID.Item21;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<BloodBlast>();
            Item.shootSpeed = 10f;
        }

        public override Vector2? HoldoutOrigin() => new Vector2(15, 15);

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            position = Main.MouseWorld;
            for (int x = 0; x < 10; x++)
            {
                Projectile.NewProjectile(source, position.X + (float)Main.rand.Next(-150, 150), position.Y + 600f, 0f, -10f, type, damage, knockback, player.whoAmI, 0f, 0f);
            }
            return false;
        }
    }
}
