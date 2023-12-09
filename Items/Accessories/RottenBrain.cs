using CalamityMod.CalPlayer;
using CalamityMod.Projectiles.Magic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class RottenBrain : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 34;
            Item.value = CalamityGlobalItem.Rarity3BuyPrice;
            Item.rare = ItemRarityID.Orange;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.immune)
            {
                var source = player.GetSource_Accessory(Item);
                if (player.miscCounter % 6 == 0)
                {
                    if (player.whoAmI == Main.myPlayer)
                    {
                        int damage = (int)player.GetBestClassDamage().ApplyTo(18);
                        damage = player.ApplyArmorAccDamageBonusesTo(damage);

                        Projectile rain = CalamityUtils.ProjectileRain(source, player.Center, 400f, 100f, 500f, 800f, 22f, ModContent.ProjectileType<AuraRain>(), damage, 2f, player.whoAmI);
                        if (rain.whoAmI.WithinBounds(Main.maxProjectiles))
                        {
                            rain.DamageType = DamageClass.Generic;
                            rain.tileCollide = false;
                            rain.penetrate = 1;
                        }
                    }
                }
            }
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.rBrain = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<BloodyWormTooth>().
                AddTile(TileID.TinkerersWorkbench).
                AddCondition(Condition.InGraveyard).
                Register();
        }
    }
}
