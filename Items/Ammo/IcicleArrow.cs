using CalamityMod.Projectiles.Ranged;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Ammo
{
    public class IcicleArrow : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 99;
            DisplayName.SetDefault("Icicle Arrow");
            Tooltip.SetDefault("Shatters into shards on impact");
        }

        public override void SetDefaults()
        {
            Item.damage = 15;
            Item.DamageType = DamageClass.Ranged;
            Item.consumable = true;
            Item.width = 18;
            Item.height = 50;
            Item.knockBack = 2.5f;
            Item.value = Item.buyPrice(0, 0, 0, 80);
            Item.rare = ItemRarityID.LightPurple;
            Item.shoot = ModContent.ProjectileType<IcicleArrowProj>();
            Item.shootSpeed = 1.0f;
            Item.ammo = AmmoID.Arrow;
            Item.maxStack = 999;
        }
    }
}
