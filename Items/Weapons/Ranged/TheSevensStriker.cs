using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class TheSevensStriker : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("The Sevens Striker");
            Tooltip.SetDefault("'A gun given to a great gunslinger\n"+
                "Forged by the arms of a man given no name'\n"+
                "Consumes coins to power a slot machine with several different outcomes\n"+
                "Quality of the outcome depends on the coin inputted\n"+
                "Right click to rapidly fire a spread of coins");
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 400;
            Item.knockBack = 9f;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.DamageType = DamageClass.Ranged;
            Item.noMelee = true;
            Item.channel = true;
            Item.width = 70;
            Item.height = 70;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAmmo = AmmoID.Coin;
            Item.shootSpeed = 24f;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.value = CalamityGlobalItem.Rarity12BuyPrice;
            Item.Calamity().customRarity = CalamityRarity.Turquoise;
            Item.Calamity().donorItem = true;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool? UseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                Item.shoot = ProjectileID.PurificationPowder;
            }
            else
            {
                Item.shoot = ModContent.ProjectileType<SevensStrikerHoldout>();
            }
            return base.UseItem(player);
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-30, -11);
        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2f)
            {
                Item.shootSpeed = 20f;
                Item.useTime = 3;
                Item.reuseDelay = 12;
                Item.useAnimation = 31;
                Item.UseSound = SoundID.Item31;
                Item.useTurn = true;
                Item.autoReuse = true;
                Item.noMelee = false;
                Item.noUseGraphic = false;
                Item.channel = false;
            }
            else
            {
                Item.shootSpeed = 24f;
                Item.reuseDelay = 0;
                Item.useTime = Item.useAnimation = 30;
                Item.UseSound = null;
                Item.useTurn = false;
                Item.autoReuse = false;
                Item.noMelee = true;
                Item.noUseGraphic = true;
                Item.channel = true;
            }
            return base.CanUseItem(player);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2f)
            {
                Vector2 shootVelocity = velocity;
                Vector2 shootDirection = shootVelocity.SafeNormalize(Vector2.UnitX * player.direction);
                Vector2 gunTip = position + shootDirection * Item.scale * 90f;
                gunTip.Y -= 20f;

                for (int i = 0; i < 2; i++)
                {
                    float SpeedX = velocity.X + Main.rand.Next(-15, 16) * 0.05f;
                    float SpeedY = velocity.Y + Main.rand.Next(-15, 16) * 0.05f;

                    Projectile.NewProjectile(source, gunTip.X, gunTip.Y, SpeedX, SpeedY, type, (int)(damage * 0.07f), knockback, player.whoAmI);
                }
            }
            else
            {
                Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<SevensStrikerHoldout>(), damage * 7, knockback, player.whoAmI, type, 0f);
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.CoinGun).
                AddIngredient<ClockGatlignum>(1).
                AddIngredient(ItemID.PlatinumCoin, 7).
                AddIngredient(ItemID.GoldCoin, 77).
                AddIngredient<TwistingNether>(2).
                AddIngredient(ItemID.LunarBar, 12).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
