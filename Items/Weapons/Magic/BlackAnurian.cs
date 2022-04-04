using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class BlackAnurian : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Black Anurian");
            Tooltip.SetDefault("Spews bubbles and homing plankton");
        }

        public override void SetDefaults()
        {
            Item.damage = 21;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 10;
            Item.width = 58;
            Item.height = 38;
            Item.useTime = 14;
            Item.useAnimation = 14;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2.75f;
            Item.value = Item.buyPrice(0, 4, 0, 0);
            Item.rare = ItemRarityID.Orange;
            Item.UseSound = SoundID.Item111;
            Item.autoReuse = true;
            Item.shootSpeed = 8f;
            Item.shoot = ModContent.ProjectileType<BlackAnurianBubble>();
        }

        public override Vector2? HoldoutOffset() => new Vector2(-5, 0);

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int planktonAmt = 2;
            for (int index = 0; index < planktonAmt; ++index)
            {
                float SpeedX = velocity.X + (float)Main.rand.Next(-25, 26) * 0.05f;
                float SpeedY = velocity.Y + (float)Main.rand.Next(-25, 26) * 0.05f;
                Projectile.NewProjectile(source, position.X, position.Y, SpeedX, SpeedY, ModContent.ProjectileType<BlackAnurianPlankton>(), damage, knockback, player.whoAmI);
            }
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            return false;
        }
    }
}
