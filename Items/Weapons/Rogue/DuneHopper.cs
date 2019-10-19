using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using CalamityMod.Projectiles.Rogue;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class DuneHopper : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dune Hopper");
            Tooltip.SetDefault("Throws a spear that bounces a lot");
        }

        public override void SafeSetDefaults()
        {
            item.width = 44;
            item.damage = 18;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useAnimation = 22;
            item.useStyle = 1;
            item.useTime = 22;
            item.knockBack = 4f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 44;
            item.value = Item.buyPrice(0, 2, 0, 0);
            item.rare = 2;
            item.shoot = ModContent.ProjectileType<DuneHopperProjectile>();
            item.shootSpeed = 12f;
            item.Calamity().rogue = true;
            item.Calamity().postMoonLordRarity = 22;
        }
    }
}
