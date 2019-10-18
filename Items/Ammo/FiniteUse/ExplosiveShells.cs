using CalamityMod.Projectiles.Typeless.FiniteUse;
using Terraria.ModLoader;
namespace CalamityMod.Items.Ammo.FiniteUse
{
    public class ExplosiveShells : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Explosive Shotgun Shell");
        }

        public override void SetDefaults()
        {
            item.damage = 30;
            item.width = 18;
            item.height = 18;
            item.maxStack = 6;
            item.consumable = true;
            item.knockBack = 10f;
            item.value = 15000;
            item.rare = 8;
            item.shoot = ModContent.ProjectileType<ExplosiveShotgunShell>();
            item.shootSpeed = 12f;
            item.ammo = ModContent.ItemType<ExplosiveShells>(); // TODO -- would item.type work here
        }
    }
}
