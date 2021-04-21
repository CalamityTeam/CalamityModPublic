using CalamityMod.CalPlayer;
using CalamityMod.Projectiles.Typeless;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class GoldBurdenBreaker : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Burden Breaker");
            Tooltip.SetDefault("The good time\n" +
				"Go fast\n" +
				"WARNING: May have disastrous results\n" +
				"Increases horizontal movement speed beyond comprehension\n" +
				"Does not work while a boss is alive");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 28;
            item.value = CalamityGlobalItem.Rarity5BuyPrice;
            item.rare = ItemRarityID.Pink;
            item.accessory = true;
			item.Calamity().challengeDrop = true;
		}

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (CalamityPlayer.areThereAnyDamnBosses)
            { return; }
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.dashMod = modPlayer.dashMod == 7 ? 0 : modPlayer.dashMod; //statis belt memes for projectile spam :feelsgreat:
            modPlayer.burdenBreakerYeet = true;
            // Completely remove movement restrictions if you're yeeting with the profaned spear
            if (player.ownedProjectileCounts[ModContent.ProjectileType<RelicOfDeliveranceSpear>()] <= 0)
            {
                if (player.velocity.X > 5f)
                {
                    player.velocity.X *= 1.025f;
                    if (player.velocity.X >= 500f)
                    {
                        player.velocity.X = 0f;
                    }
                }
                else if (player.velocity.X < -5f)
                {
                    player.velocity.X *= 1.025f;
                    if (player.velocity.X <= -500f)
                    {
                        player.velocity.X = 0f;
                    }
                }
            }
        }
    }
}
