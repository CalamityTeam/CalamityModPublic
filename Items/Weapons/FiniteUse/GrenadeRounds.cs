using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.FiniteUse
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
            item.shoot = mod.ProjectileType("GrenadeRound");
            item.shootSpeed = 12f;
            item.ammo = mod.ItemType("GrenadeRounds");
        }
    }
}
