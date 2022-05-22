using CalamityMod.CalPlayer;
using CalamityMod.Projectiles.Magic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Accessories
{
    // TODO -- this item includes a dodge accessory, Brain of Cthulhu
    public class AmalgamatedBrain : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            DisplayName.SetDefault("Amalgamated Brain");
            Tooltip.SetDefault("10% increased damage\n" +
                               "Shade rains down when you are hit\n" +
                               "You will confuse nearby enemies when you are struck");
        }

        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 34;
            Item.value = CalamityGlobalItem.Rarity4BuyPrice;
            Item.rare = ItemRarityID.LightRed;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.aBrain = true;
            if (player.immune)
            {
                var source = player.GetSource_Accessory(Item);
                if (player.miscCounter % 6 == 0)
                {
                    if (player.whoAmI == Main.myPlayer)
                    {
                        int damage = (int)player.GetBestClassDamage().ApplyTo(60);
                        Projectile rain = CalamityUtils.ProjectileRain(source, player.Center, 400f, 100f, 500f, 800f, 22f, ModContent.ProjectileType<AuraRain>(), damage, 2f, player.whoAmI);
                        if (rain.whoAmI.WithinBounds(Main.maxProjectiles))
                        {
                            rain.Calamity().forceClassless = true;
                            rain.tileCollide = false;
                            rain.penetrate = 1;
                        }
                    }
                }
            }
            player.GetDamage<GenericDamageClass>() += 0.1f;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<RottenBrain>().
                AddIngredient(ItemID.BrainOfConfusion).
                AddIngredient(ItemID.SoulofNight, 3).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
