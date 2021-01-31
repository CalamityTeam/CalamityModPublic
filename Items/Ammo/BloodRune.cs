using CalamityMod.Projectiles.Magic;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Ammo
{
    public class BloodRune : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blood Rune");
            Tooltip.SetDefault("Used with the Ice Barrage \n" +
                               "Found in some sort of runic landscape");
        }

        public override void SetDefaults()
        {
            item.damage = 1;
            item.width = 22;
            item.height = 24;
            item.maxStack = 999;
            item.consumable = true;
            item.knockBack = 10f;
            item.value = Item.buyPrice(0, 1, 0, 0);
            item.Calamity().postMoonLordRarity = 14;
            item.shoot = ModContent.ProjectileType<IceBarrageMain>();
            item.shootSpeed = 0f;
            item.ammo = item.type; // CONSIDER -- Would item.type work here instead of a self reference?
        }
    }
}
