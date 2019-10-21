using CalamityMod.Projectiles.Rogue;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class SnapClam : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Snap Clam");
            Tooltip.SetDefault("Can latch on enemies and deal damage over time");
        }

        public override void SafeSetDefaults()
        {
            item.width = 26;
            item.height = 16;
            item.damage = 15;
            item.thrown = true;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useTime = 25;
            item.useAnimation = 25;
            item.useStyle = 1;
            item.knockBack = 3f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.value = Item.buyPrice(0, 2, 0, 0);
            item.rare = 2;
            item.shoot = ModContent.ProjectileType<SnapClamProj>();
            item.shootSpeed = 12f;
            item.Calamity().rogue = true;
        }
    }
}
