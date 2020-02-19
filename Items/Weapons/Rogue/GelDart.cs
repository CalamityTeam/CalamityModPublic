using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Rogue;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class GelDart : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gel Dart");
            Tooltip.SetDefault("Throws bouncing darts");
        }

        public override void SafeSetDefaults()
        {
            item.width = 14;
            item.damage = 28;
            item.noMelee = true;
            item.consumable = true;
            item.noUseGraphic = true;
            item.useAnimation = 11;
            item.useStyle = 1;
            item.useTime = 11;
            item.knockBack = 2.5f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 28;
            item.maxStack = 999;
            item.value = Item.buyPrice(0, 0, 2, 50);
            item.rare = 4;
            item.shoot = ModContent.ProjectileType<GelDartProjectile>();
            item.shootSpeed = 14f;
            item.Calamity().rogue = true;
        }
    }
}
