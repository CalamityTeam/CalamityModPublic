using Terraria.ModLoader;
using CalamityMod.Projectiles;
namespace CalamityMod.Items
{
    public class GrenadeRounds : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Grenade Shells");
        }

        public override void SetDefaults()
        {
            item.damage = 200;
            item.width = 18;
            item.height = 18;
            item.maxStack = 9;
            item.consumable = true;
            item.knockBack = 10f;
            item.value = 15000;
            item.rare = 8;
            item.shoot = ModContent.ProjectileType<GrenadeRound>();
            item.shootSpeed = 12f;
            item.ammo = ModContent.ItemType<GrenadeRounds>();
        }
    }
}
