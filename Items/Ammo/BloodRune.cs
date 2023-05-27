using CalamityMod.Projectiles.Magic;
using CalamityMod.Rarities;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Ammo
{
    public class BloodRune : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 99;
                   }

        public override void SetDefaults()
        {
            Item.damage = 1;
            Item.width = 22;
            Item.height = 24;
            Item.maxStack = 9999;
            Item.consumable = true;
            Item.knockBack = 10f;
            Item.value = Item.buyPrice(gold: 1);
            Item.rare = ModContent.RarityType<DarkBlue>();
            Item.Calamity().donorItem = true;
            Item.shoot = ModContent.ProjectileType<IceBarrageMain>();
            Item.shootSpeed = 0f;
            Item.ammo = Item.type;
        }
    }
}
