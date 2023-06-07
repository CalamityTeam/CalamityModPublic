using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using CalamityMod.Projectiles.Typeless;

namespace CalamityMod.Items.Fishing.SunkenSeaCatches
{
    public class SerpentsBite : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Fishing";
        public override void SetDefaults()
        {
            // Instead of copying these values, we can clone and modify the ones we want to copy
            Item.CloneDefaults(ItemID.AmethystHook);
            Item.shootSpeed = SerpentsBiteHook.LaunchSpeed; // how quickly the hook is shot.
            Item.shoot = ProjectileType<SerpentsBiteHook>();
            Item.value = CalamityGlobalItem.Rarity4BuyPrice;
            Item.rare = ItemRarityID.LightRed;
            Item.width = 30;
            Item.height = 32;
        }
    }
}
