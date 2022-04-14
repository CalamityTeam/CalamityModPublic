using CalamityMod.Projectiles.Typeless;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

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
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 46;
            Item.useTime = Item.useAnimation = 25;
            Item.reuseDelay = 15;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundID.DD2_DarkMageCastHeal;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.channel = true;
            Item.value = CalamityGlobalItem.Rarity11BuyPrice;
            Item.rare = ItemRarityID.Purple;
            Item.shoot = ModContent.ProjectileType<RelicOfConvergenceCrystal>();
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0 && player.ownedProjectileCounts[ModContent.ProjectileType<RelicOfDeliveranceSpear>()] <= 0;
    }
}
