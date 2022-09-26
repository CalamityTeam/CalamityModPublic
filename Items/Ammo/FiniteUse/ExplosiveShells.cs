using CalamityMod.Projectiles.Typeless.FiniteUse;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Ammo.FiniteUse
{
    public class ExplosiveShells : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 20;
            DisplayName.SetDefault("Explosive Shotgun Shell");
        }

        public override void SetDefaults()
        {
            Item.damage = 30;
            Item.width = 18;
            Item.height = 18;
            Item.maxStack = 6;
            Item.consumable = true;
            Item.knockBack = 10f;
            Item.value = 15000;
            Item.rare = ItemRarityID.Yellow;
            Item.shoot = ModContent.ProjectileType<ExplosiveShotgunShell>();
            Item.shootSpeed = 12f;
            Item.ammo = ModContent.ItemType<ExplosiveShells>(); // CONSIDER -- Would item.type work here instead of a self reference?
        }
    }
}
