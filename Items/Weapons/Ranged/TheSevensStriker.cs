using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class TheSevensStriker : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        public static readonly SoundStyle RouletteSound = new("CalamityMod/Sounds/Item/SevensStrikerRoulette") { Volume = 0.6f, SoundLimitBehavior = SoundLimitBehavior.ReplaceOldest};
        public static readonly SoundStyle RouletteTickSound = new("CalamityMod/Sounds/Item/SevensStrikerRouletteTick") { Volume = 0.5f};
        public static readonly SoundStyle BustSound = new("CalamityMod/Sounds/Item/SevensStrikerBust");
        public static readonly SoundStyle DoublesSound = new("CalamityMod/Sounds/Item/SevensStrikerDoubles");
        public static readonly SoundStyle TriplesSound = new("CalamityMod/Sounds/Item/SevensStrikerTriples");
        public static readonly SoundStyle JackpotSound = new("CalamityMod/Sounds/Item/SevensStrikerJackpot");
        public static readonly SoundStyle CoinSound = new("CalamityMod/Sounds/Item/SevensStrikerCoinShot") { MaxInstances = 0, PitchVariance = 0.5f };

        public static int ShotCoin = 0; // projectile ID to use for right click, affects damage multiplier
        public static readonly float RightClickCopperMultiplier = 0.04f;
        public static readonly float RightClickSilverMultiplier = 0.08f;
        public static readonly float RightClickGoldMultiplier   = 0.16f;

        public static readonly float DoublesMultiplier = 2f; // Doubles means double damage!
        public static readonly float TriplesCherryMultiplier = 1f;
        public static readonly float TriplesGrapeMultiplier = 0.333f; // fixed the grapes interfering with each other's iframes
        public static readonly float JackpotMultiplier = 0.5f; // Jackpot fires 49 shots total and thus needs to be reduced somehow

        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 777;
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
            Item.shoot = ProjectileID.PlatinumCoin;
            Item.value = CalamityGlobalItem.Rarity12BuyPrice;
            Item.rare = ModContent.RarityType<Turquoise>();
            Item.Calamity().donorItem = true;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool? UseItem(Player player)
        {
            // Right click has a 20% chance to consume money
            if (player.altFunctionUse == 2)
            {
                bool consumeCoin = Main.rand.NextFloat() > 0.8f;
                long coinCount = Utils.CoinsCount(out bool overflow, player.inventory);
                int price;

                if (overflow || coinCount > 10000)
                {
                    price = 10000;
                    ShotCoin = ProjectileID.GoldCoin;
                }
                else if (coinCount > 100)
                {
                    price = 100;
                    ShotCoin = ProjectileID.SilverCoin;
                }
                else
                {
                    price = 1;
                    ShotCoin = ProjectileID.CopperCoin;
                }

                if (consumeCoin)
                    player.BuyItem(price);
            }

            // Left click does nothing, it just spawns the holdout
            else
                Item.shoot = ModContent.ProjectileType<SevensStrikerHoldout>();

            return base.UseItem(player);
        }

        public override Vector2? HoldoutOffset() => new Vector2(-30, -11);

        // Left click spawns a holdout, so the item must not appear normally.
        // Right click uses the item like a standard gun, so draw it normally.
        public override void UseAnimation(Player player)
        {
            if (player.altFunctionUse == 2)
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

        // Right click fires coins extremely rapidly (3 use time)
        public override float UseTimeMultiplier(Player player) => player.altFunctionUse == 2 ? 0.1f : 1f;

        // Right click consumes coins manually instead of using Terraria's ammo system
        // (otherwise it could shoot Platinum Coins)
        public override bool CanConsumeAmmo(Item ammo, Player player) => player.altFunctionUse == 2 ? false : true;

        public override bool CanUseItem(Player player)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<SevensStrikerHoldout>()] <= 0)
                return true;

            if (player.altFunctionUse == 2)
            {
                Utils.CoinsCount(out bool overflow, player.inventory);
                return overflow;
            }
            return true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            // Right click damage multiplier varies by coin type
            if (player.altFunctionUse == 2)
            {
                Vector2 shootDirection = velocity.SafeNormalize(Vector2.UnitX * player.direction);
                Vector2 gunTip = position + shootDirection * Item.scale * 90f;
                gunTip.Y -= 20f;

                // spread copied from Onyxia, angular conic spread instead of Terraria's square spread
                float randAngle = Main.rand.NextFloat(-0.05f, 0.05f);
                float randVelMultiplier = Main.rand.NextFloat(0.92f, 1.08f);
                Vector2 finalVelocity = velocity.RotatedBy(randAngle) * randVelMultiplier;
                float damageMult = ShotCoin == ProjectileID.GoldCoin
                    ? RightClickGoldMultiplier : ShotCoin == ProjectileID.SilverCoin
                    ? RightClickSilverMultiplier : RightClickCopperMultiplier;
                int finalDamage = (int)(damage * damageMult);
                Projectile.NewProjectile(source, gunTip, finalVelocity, ShotCoin, finalDamage, knockback, player.whoAmI);
            }

            // Left click just spawns the holdout
            else
                Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<SevensStrikerHoldout>(), damage, knockback, player.whoAmI, type, 0f);
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
