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
            item.damage = 144;
            item.ranged = true;
            item.width = 90;
            item.height = 30;
            item.useTime = 18;
            item.useAnimation = 18;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 6f;
            item.autoReuse = true;
            item.value = CalamityGlobalItem.RarityPureGreenBuyPrice;
            item.rare = ItemRarityID.Red;
            item.UseSound = SoundID.Item95;
            item.shoot = ModContent.ProjectileType<SulphuricBlast>();
            item.shootSpeed = 16f;
            item.Calamity().customRarity = CalamityRarity.PureGreen;
        }

        public override Vector2? HoldoutOffset() => Vector2.UnitX * -15f;
    }
}
