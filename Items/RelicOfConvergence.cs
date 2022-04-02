using CalamityMod.Projectiles.Typeless;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items
{
    public class RelicOfConvergence : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Relic of Convergence");
            Tooltip.SetDefault("Creates a profaned crystal that charges power\n" +
                               "Holding out the crystal slows the player down\n" +
                               "At the end of its life, the crystal heals the player for 70 HP");
        }

        public override void SetDefaults()
        {
            item.width = 32;
            item.height = 46;
            item.useTime = item.useAnimation = 25;
            item.reuseDelay = 15;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.UseSound = SoundID.DD2_DarkMageCastHeal;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.channel = true;
            item.value = CalamityGlobalItem.Rarity11BuyPrice;
            item.rare = ItemRarityID.Purple;
            item.shoot = ModContent.ProjectileType<RelicOfConvergenceCrystal>();
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[item.shoot] <= 0 && player.ownedProjectileCounts[ModContent.ProjectileType<RelicOfDeliveranceSpear>()] <= 0;
    }
}
