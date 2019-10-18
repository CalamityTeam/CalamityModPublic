using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using CalamityMod.Projectiles.Ranged;

namespace CalamityMod.Items.Ammo
{
    public class IcicleArrow : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Icicle Arrow");
            Tooltip.SetDefault("Shatters into shards on impact");
        }

        public override void SetDefaults()
        {
            item.damage = 14;
            item.ranged = true;
            item.consumable = true;
            item.width = 14;
            item.height = 50;
            item.knockBack = 2.5f;
            item.value = Item.buyPrice(0, 0, 0, 15);
            item.rare = 3;
            item.shoot = ModContent.ProjectileType<IcicleArrowProj>();
            item.shootSpeed = 1.0f;
            item.ammo = AmmoID.Arrow;
            item.maxStack = 999;
        }
    }
}
