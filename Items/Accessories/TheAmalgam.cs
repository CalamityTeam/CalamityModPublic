using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Items.Accessories
{
    public class TheAmalgam : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Amalgam");
            Tooltip.SetDefault("Extends the duration of potion buffs by 100% and potion buffs remain active even after you die\n" +
                            "15% increased damage\n" +
                            "Shade rains down when you are hit\n" +
                            "Nearby enemies receive a variety of debuffs when you are hit");
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(9, 6));
        }

        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 34;
            Item.accessory = true;
            Item.value = CalamityGlobalItem.Rarity14BuyPrice;
            Item.Calamity().customRarity = CalamityRarity.DarkBlue;
            Item.rare = ItemRarityID.Purple;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.amalgam = true;
            player.GetDamage<GenericDamageClass>() += 0.15f;

            if (player.immune)
            {
                var source = player.GetProjectileSource_Accessory(Item);
                if (player.miscCounter % 6 == 0)
                {
                    if (player.whoAmI == Main.myPlayer)
                    {
                        Projectile rain = CalamityUtils.ProjectileRain(source, player.Center, 400f, 100f, 500f, 800f, 22f, ProjectileType<AuraRain>(), (int)(300 * player.AverageDamage()), 2f, player.whoAmI);
                        if (rain.whoAmI.WithinBounds(Main.maxProjectiles))
                        {
                            rain.Calamity().forceTypeless = true;
                            rain.tileCollide = false;
                            rain.penetrate = 1;
                        }
                    }
                }
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<AmalgamatedBrain>().
                AddIngredient<UnholyCore>(5).
                AddIngredient<MolluskHusk>(10).
                AddIngredient<SulfuricScale>(15).
                AddIngredient<PlagueCellCluster>(15).
                AddIngredient<CosmiliteBar>(5).
                AddIngredient<AscendantSpiritEssence>(4).
                AddTile<CosmicAnvil>().
                Register();
        }
    }
}
