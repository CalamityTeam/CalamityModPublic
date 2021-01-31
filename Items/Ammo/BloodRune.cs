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

            item.value = Item.buyPrice(gold: 1);
            item.Calamity().customRarity = CalamityRarity.DarkBlue;
            item.Calamity().donorItem = true;

            item.shoot = ModContent.ProjectileType<IceBarrageMain>();
            item.shootSpeed = 0f;
            item.ammo = item.type;
        }
    }
}
