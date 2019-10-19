using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class GhoulishGouger : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ghoulish Gouger");
            Tooltip.SetDefault("Throws a ghoulish scythe that ignores immunity frames");
        }

        public override void SafeSetDefaults()
        {
            item.width = 68;
            item.damage = 160;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useAnimation = 12;
            item.useTime = 12;
            item.useStyle = 1;
            item.knockBack = 7.5f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 60;
            item.value = Item.buyPrice(1, 40, 0, 0);
            item.rare = 10;
            item.shoot = ModContent.ProjectileType<GhoulishGouger>();
            item.shootSpeed = 20f;
            item.Calamity().rogue = true;
            item.Calamity().postMoonLordRarity = 13;
        }
    }
}
