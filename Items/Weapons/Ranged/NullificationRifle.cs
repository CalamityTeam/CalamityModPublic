using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class NullificationRifle : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Nullification Pistol");
            Tooltip.SetDefault("Is it nullable or not? Let's find out!\n" +
                "Fires a fast null bullet that distorts NPC stats");
        }

        public override void SetDefaults()
        {
            item.damage = 135;
            item.ranged = true;
            item.width = 52;
            item.height = 28;
            item.useTime = 20;
            item.useAnimation = 20;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 7f;
            item.value = Item.buyPrice(0, 80, 0, 0);
            item.rare = ItemRarityID.Yellow;
            item.UseSound = SoundID.Item33;
            item.autoReuse = true;
            item.shootSpeed = 25f;
            item.shoot = ModContent.ProjectileType<NullShot>();
            item.Calamity().canFirePointBlankShots = true;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-5, 0);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ModContent.ProjectileType<NullShot>(), damage, knockBack, player.whoAmI);
            return false;
        }
    }
}
