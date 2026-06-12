using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class TheSevensStriker : ModItem, ILocalizedModType, IHoldShiftTooltipItem
    {
        public bool ShowExtensionIndicator => false;
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        public static readonly SoundStyle RouletteSound = new("CalamityMod/Sounds/Item/SevensStrikerRoulette") { Volume = 0.6f, SoundLimitBehavior = SoundLimitBehavior.ReplaceOldest };
        public static readonly SoundStyle RouletteTickSound = new("CalamityMod/Sounds/Item/SevensStrikerRouletteTick") { Volume = 0.5f };
        public static readonly SoundStyle BustSound = new("CalamityMod/Sounds/Item/SevensStrikerBust");
        public static readonly SoundStyle BustGFB = new("CalamityMod/Sounds/Item/SevensStrikerBustGFB");
        public static readonly SoundStyle DoublesSound = new("CalamityMod/Sounds/Item/SevensStrikerDoubles");
        public static readonly SoundStyle TriplesSound = new("CalamityMod/Sounds/Item/SevensStrikerTriples");
        public static readonly SoundStyle JackpotSound = new("CalamityMod/Sounds/Item/SevensStrikerJackpot");
        public static readonly SoundStyle JackpotGFB = new("CalamityMod/Sounds/Item/SevensStrikerJackpotGFB");
        public static readonly SoundStyle CoinSound = new("CalamityMod/Sounds/Item/SevensStrikerCoinShot") { MaxInstances = 0, PitchVariance = 0.5f };

        public static readonly float DoublesMultiplier = 1f; // Unfortunately doubles can't be doubles. Balancing!
        public static readonly float TriplesCherryMultiplier = 1f;
        public static readonly float TriplesCherrySplitMultiplier = 0.333f;
        public static readonly float TriplesGrapeMultiplier = 0.333f; // fixed the grapes interfering with each other's iframes
        public static readonly float JackpotMultiplier = 0.5f; // Jackpot fires 49 shots total and thus needs to be reduced somehow...
        public static readonly float JackpotMultiplierGFB = 7f; // ...Unless you simply cannot stop winning

        public override void SetDefaults()
        {
            Item.width = 170;
            Item.height = 56;
            Item.damage = 777;
            Item.knockBack = 9f;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.DamageType = DamageClass.Ranged;
            Item.noMelee = true;
            Item.channel = true;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.UseSound = null;
            Item.noUseGraphic = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAmmo = AmmoID.Coin;
            Item.shoot = ModContent.ProjectileType<SevensStrikerHoldout>();
            Item.shootSpeed = 24f;
            Item.shoot = ProjectileID.PlatinumCoin;
            Item.value = CalamityGlobalItem.RarityTurquoiseBuyPrice;
            Item.rare = ModContent.RarityType<Turquoise>();
            Item.Calamity().donorItem = true;
        }
        public override Vector2? HoldoutOffset() => new Vector2(-30, -11);
        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile holdout = Projectile.NewProjectileDirect(source, player.MountedCenter, Vector2.Zero, ModContent.ProjectileType<SevensStrikerHoldout>(), damage, knockback, player.whoAmI);

            // We set the rotation to the direction to the mouse so the first frame doesn't appear bugged out.
            holdout.velocity = (player.Calamity().mouseWorld - player.MountedCenter).SafeNormalize(Vector2.Zero);
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.CoinGun).
                AddIngredient(ItemID.PlatinumCoin, 7).
                AddIngredient(ItemID.GoldCoin, 77).
                AddIngredient(ItemID.LunarBar, 12).
                AddIngredient<TwistingNether>(3).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
