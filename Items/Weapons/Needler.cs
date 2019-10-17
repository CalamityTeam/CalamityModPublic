using Microsoft.Xna.Framework;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class Needler : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Needler");
            Tooltip.SetDefault("Fires needles that stick to enemies and explode");
        }

        public override void SetDefaults()
        {
            item.damage = 38;
            item.ranged = true;
            item.width = 44;
            item.height = 26;
            item.useTime = 18;
            item.useAnimation = 18;
            item.useStyle = 5;
            item.noMelee = true;
            item.knockBack = 5.5f;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.rare = 5;
            item.UseSound = SoundID.Item108;
            item.autoReuse = true;
            item.shootSpeed = 9f;
            item.shoot = ModContent.ProjectileType<Projectiles.Needler>();
            item.useAmmo = 97;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-5, 0);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ModContent.ProjectileType<Projectiles.Needler>(), damage, knockBack, player.whoAmI, 0f, 0f);
            return false;
        }
    }
}
