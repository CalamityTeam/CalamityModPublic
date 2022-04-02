using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class SepticSkewer : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Septic Skewer");
            Tooltip.SetDefault("Launches a spiky harpoon infested with toxins\n" +
                "Releases bacteria when returning to the player");
        }

        public override void SetDefaults()
        {
            item.damage = 272;
            item.ranged = true;
            item.width = 46;
            item.height = 24;
            item.useTime = 12;
            item.useAnimation = 12;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 7.5f;
            item.UseSound = SoundID.Item10;
            item.autoReuse = true;
            item.shootSpeed = 20f;
            item.shoot = ModContent.ProjectileType<SepticSkewerHarpoon>();

            item.value = CalamityGlobalItem.Rarity13BuyPrice;
            item.Calamity().customRarity = CalamityRarity.PureGreen;
            item.Calamity().canFirePointBlankShots = true;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-10, 0);
    }
}
