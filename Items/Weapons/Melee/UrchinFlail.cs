using CalamityMod.Projectiles.Melee;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class UrchinFlail : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Urchin Flail");
            Tooltip.SetDefault("Launch an urchin ball, which shoots a spike on contact with an enemy");
        }

        public override void SetDefaults()
        {
            item.damage = 33;
            item.melee = true;
            item.width = 44;
            item.height = 36;
            item.useTime = 25;
            item.useAnimation = 25;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.knockBack = 6f;
            item.value = Item.buyPrice(0, 2, 0, 0);
            item.rare = ItemRarityID.Green;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.channel = true;
            item.shoot = ModContent.ProjectileType<UrchinBall>();
            item.shootSpeed = 12f;
        }
    }
}
