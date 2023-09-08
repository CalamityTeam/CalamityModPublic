using CalamityMod.Balancing;
using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Items.Accessories
{
    // TODO -- this item includes a dodge accessory, Brain of Cthulhu
    public class TheAmalgam : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(9, 6));
            ItemID.Sets.AnimatesAsSoul[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 34;
            Item.accessory = true;
            Item.value = CalamityGlobalItem.Rarity14BuyPrice;
            Item.rare = RarityType<DarkBlue>();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.amalgam = true;
            player.brainOfConfusionItem = Item;
            player.GetDamage<GenericDamageClass>() += 0.15f;

            if (player.immune)
            {
                var source = player.GetSource_Accessory(Item);
                if (player.miscCounter % 6 == 0)
                {
                    if (player.whoAmI == Main.myPlayer)
                    {
                        int damage = (int)player.GetBestClassDamage().ApplyTo(300);
                        Projectile rain = CalamityUtils.ProjectileRain(source, player.Center, 400f, 100f, 500f, 800f, 22f, ProjectileType<AuraRain>(), damage, 2f, player.whoAmI);
                        if (rain.whoAmI.WithinBounds(Main.maxProjectiles))
                        {
                            rain.DamageType = DamageClass.Generic;
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
                AddIngredient<SulphuricScale>(15).
                AddIngredient<PlagueCellCanister>(15).
                AddIngredient<CosmiliteBar>(5).
                AddIngredient<AscendantSpiritEssence>(4).
                AddTile<CosmicAnvil>().
                Register();
        }
    }
}
