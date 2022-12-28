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
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 175;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 50;
            Item.height = 30;
            Item.useTime = 40;
            Item.useAnimation = 40;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 7f;
            Item.value = CalamityGlobalItem.Rarity7BuyPrice;
            Item.rare = ItemRarityID.Lime;
            Item.UseSound = SoundID.Item92;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<AstralCannonProjectile>();
            Item.shootSpeed = 2f;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-5, 0);
        }
    }
}
