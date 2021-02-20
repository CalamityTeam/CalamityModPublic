using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

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
            item.damage = 175;
            item.ranged = true;
            item.width = 50;
            item.height = 30;
            item.useTime = 40;
            item.useAnimation = 40;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 7f;
            item.value = Item.buyPrice(0, 60, 0, 0);
            item.rare = ItemRarityID.Lime;
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
