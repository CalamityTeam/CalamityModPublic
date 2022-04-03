using CalamityMod.Projectiles.Typeless.FiniteUse;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Ammo.FiniteUse
{
    public class MagnumRounds : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Magnum Round");
        }

        public override void SetDefaults()
        {
            Item.damage = 80;
            Item.crit += 4;
            Item.width = 18;
            Item.height = 18;
            Item.maxStack = 12;
            Item.consumable = true;
            Item.knockBack = 8f;
            Item.value = 10000;
            Item.rare = ItemRarityID.Yellow;
            Item.shoot = ModContent.ProjectileType<MagnumRound>();
            Item.shootSpeed = 12f;
            Item.ammo = ModContent.ItemType<MagnumRounds>(); // CONSIDER -- Would item.type work here instead of a self reference?
        }
    }
}
