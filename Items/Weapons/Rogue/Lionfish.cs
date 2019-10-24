using CalamityMod.Projectiles.Rogue;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class Lionfish : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lionfish");
            Tooltip.SetDefault("Sticks to enemies and injects a potent toxin");
        }

        public override void SafeSetDefaults()
        {
            item.width = 40;
            item.damage = 45;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useAnimation = 22;
            item.useStyle = 1;
            item.useTime = 22;
            item.knockBack = 2.5f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 40;
            item.value = Item.buyPrice(0, 4, 0, 0);
            item.rare = 3;
            item.shoot = ModContent.ProjectileType<LionfishProj>();
            item.shootSpeed = 12f;
            item.Calamity().rogue = true;
        }
    }
}
