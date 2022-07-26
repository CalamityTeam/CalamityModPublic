using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using System.Linq;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class TheSevensStriker : ModItem
    {
        public static readonly SoundStyle RouletteSound = new("CalamityMod/Sounds/Item/SevensStrikerRoulette") { Volume = 0.6f, SoundLimitBehavior = SoundLimitBehavior.ReplaceOldest};
        public static readonly SoundStyle RouletteTickSound = new("CalamityMod/Sounds/Item/SevensStrikerRouletteTick") { Volume = 0.5f};

        public static readonly SoundStyle BustSound = new("CalamityMod/Sounds/Item/SevensStrikerBust");
        public static readonly SoundStyle DoublesSound = new("CalamityMod/Sounds/Item/SevensStrikerDoubles");
        public static readonly SoundStyle TriplesSound = new("CalamityMod/Sounds/Item/SevensStrikerTriples");
        public static readonly SoundStyle JackpotSound = new("CalamityMod/Sounds/Item/SevensStrikerJackpot");
        public static readonly SoundStyle CoinSound = new("CalamityMod/Sounds/Item/SevensStrikerCoinShot") { MaxInstances = 0, PitchVariance = 0.5f };

        public static int ShotCoin = 0;

        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("The Sevens Striker");
            Tooltip.SetDefault("'A gun given to a great gunslinger\n"+
                "Forged by the arms of a man given no name'\n"+
                "Consumes coins to power a slot machine with several different outcomes\n"+
                "Quality of the outcome depends on the coin inputted\n"+
                "Right click to rapidly fire a spread of coins\n"+
                "The gun refuses to fire platinum coins for its right click");
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
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.width = 170;
            Item.height = 56;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAmmo = AmmoID.Coin;
            Item.shootSpeed = 24f;
            Item.reuseDelay = 0;
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
                long coinCount = Utils.CoinsCount(out bool overflow, player.inventory);

                if (overflow || coinCount > 10000)
                {
                    player.BuyItem(10000);
                    ShotCoin = ProjectileID.GoldCoin;
                }
                else if (coinCount > 100)
                {
                    player.BuyItem(100);
                    ShotCoin = ProjectileID.SilverCoin;
                }
                else
                {
                    player.BuyItem(1);
                    ShotCoin = ProjectileID.CopperCoin;
                }
            }
            else
            {
                Item.shoot = ModContent.ProjectileType<SevensStrikerHoldout>();
            }
            return base.UseItem(player);
        }
        public override bool CanConsumeAmmo(Item ammo, Player player) => player.altFunctionUse == 2 ? false : true;

        public override Vector2? HoldoutOffset() => new Vector2(-30, -11);


        public override void UseAnimation(Player player)
        {
            if (player.altFunctionUse == 2f)
            {
                Item.UseSound = CoinSound;
                Item.noUseGraphic = false;
            }
            else
            {
                Item.UseSound = null;
                Item.noUseGraphic = true;
            }
        }

        public override float UseTimeMultiplier(Player player)
        {
            if (player.altFunctionUse == 2f)
                return 0.1f;
            
            return 1f;
        }

        public override bool CanUseItem(Player player)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<SevensStrikerHoldout>()] <= 0)
            {
                return true;
            }
            if (player.altFunctionUse == 2)
            {
                Utils.CoinsCount(out bool overflow, player.inventory);
                return overflow;
            }
            return true;
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

                    Projectile.NewProjectile(source, gunTip.X, gunTip.Y, SpeedX, SpeedY, ShotCoin, (int)(damage * 0.07f), knockback, player.whoAmI);
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
