using CalamityMod.Projectiles.Magic;
using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Ammo
{
    public class BloodRune : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 99;
            DisplayName.SetDefault("Blood Rune");
            Tooltip.SetDefault("Used with the Ice Barrage \n" +
                "Found in some sort of runic landscape");
        }

        public override void SetDefaults()
        {
            Item.damage = 1;
            Item.width = 22;
            Item.height = 24;
            Item.maxStack = 999;
            Item.consumable = true;
            Item.knockBack = 10f;
            Item.value = Item.buyPrice(gold: 1);
            Item.Calamity().customRarity = CalamityRarity.DarkBlue;
            Item.Calamity().donorItem = true;
            Item.shoot = ModContent.ProjectileType<IceBarrageMain>();
            Item.shootSpeed = 0f;
            Item.ammo = Item.type;
        }
    }
}
