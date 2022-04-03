using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class SulphuricAcidCannon : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sulphuric Acid Cannon");
            Tooltip.SetDefault("Fires an acidic shot that sticks to enemies and dissolves them");
        }

        public override void SetDefaults()
        {
            Item.damage = 144;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 90;
            Item.height = 30;
            Item.useTime = 18;
            Item.useAnimation = 18;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 6f;
            Item.autoReuse = true;
            Item.value = CalamityGlobalItem.RarityPureGreenBuyPrice;
            Item.rare = ItemRarityID.Red;
            Item.UseSound = SoundID.Item95;
            Item.shoot = ModContent.ProjectileType<SulphuricBlast>();
            Item.shootSpeed = 16f;
            Item.Calamity().customRarity = CalamityRarity.PureGreen;
        }

        public override Vector2? HoldoutOffset() => Vector2.UnitX * -15f;
    }
}
