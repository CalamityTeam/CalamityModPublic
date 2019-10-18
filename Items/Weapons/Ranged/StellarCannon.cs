using Microsoft.Xna.Framework;
using Terraria;
using CalamityMod.Projectiles;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class StellarCannon : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Stellar Cannon");
            Tooltip.SetDefault("Launches an explosive astral crystal");
        }

        public override void SetDefaults()
        {
            item.damage = 250;
            item.ranged = true;
            item.width = 50;
            item.height = 30;
            item.useTime = 30;
            item.useAnimation = 30;
            item.useStyle = 5;
            item.noMelee = true;
            item.knockBack = 7f;
            item.value = Item.buyPrice(0, 60, 0, 0);
            item.rare = 7;
            item.UseSound = SoundID.Item92;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<AstralCannonProjectile>();
            item.shootSpeed = 2f;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-5, 0);
        }
    }
}
