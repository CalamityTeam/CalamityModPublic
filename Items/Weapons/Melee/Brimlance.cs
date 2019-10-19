using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using CalamityMod.Projectiles.Melee;

namespace CalamityMod.Items.Weapons.Melee
{
    public class Brimlance : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Brimlance");
            Tooltip.SetDefault("Enemies killed by the spear explode into brimstone flames");
        }

        public override void SetDefaults()
        {
            item.width = 56;
            item.damage = 75;
            item.melee = true;
            item.noMelee = true;
            item.useTurn = true;
            item.noUseGraphic = true;
            item.useAnimation = 19;
            item.useStyle = 5;
            item.useTime = 19;
            item.knockBack = 7.5f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = false;
            item.height = 56;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.rare = 5;
            item.shoot = ModContent.ProjectileType<BrimlanceProj>();
            item.shootSpeed = 12f;
        }
    }
}
