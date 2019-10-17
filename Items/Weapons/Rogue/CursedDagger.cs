using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items
{
    public class CursedDagger : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cursed Dagger");
            Tooltip.SetDefault("Throws bouncing daggers");
        }

        public override void SafeSetDefaults()
        {
            item.width = 34;
            item.damage = 34;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useAnimation = 16;
            item.useStyle = 1;
            item.useTime = 16;
            item.knockBack = 4.5f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 34;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.rare = 5;
            item.shoot = ModContent.ProjectileType<Projectiles.CursedDagger>();
            item.shootSpeed = 12f;
            item.Calamity().rogue = true;
        }
    }
}
