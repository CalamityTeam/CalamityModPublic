using CalamityMod.Projectiles.Typeless;
using CalamityMod.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Items.Tools
{
    public class BobbitHook : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Tools";
        public override void SetDefaults()
        {
            // Instead of copying these values, we can clone and modify the ones we want to copy
            Item.CloneDefaults(ItemID.AmethystHook);
            Item.shootSpeed = BobbitHead.LaunchSpeed; // How quickly the hook is shot.
            Item.shoot = ProjectileType<BobbitHead>();
            Item.value = CalamityGlobalItem.Rarity13BuyPrice;
            Item.rare = ModContent.RarityType<PureGreen>();
            Item.width = 30;
            Item.height = 32;
        }
    }
}
