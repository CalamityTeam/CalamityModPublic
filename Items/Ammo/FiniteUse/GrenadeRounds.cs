using CalamityMod.Projectiles.Typeless.FiniteUse;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Ammo.FiniteUse
{
    public class GrenadeRounds : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Grenade Shell");
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
            item.rare = ItemRarityID.Yellow;
            item.shoot = ModContent.ProjectileType<GrenadeRound>();
            item.shootSpeed = 12f;
            item.ammo = ModContent.ItemType<GrenadeRounds>(); // CONSIDER -- Would item.type work here instead of a self reference?
        }
    }
}
