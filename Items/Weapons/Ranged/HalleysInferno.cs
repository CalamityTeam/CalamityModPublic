using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Ores;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class HalleysInferno : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";

        public static readonly SoundStyle Shoot = new("CalamityMod/Sounds/Item/HalleysInfernoShoot") { Volume = 0.68f };
        public static readonly SoundStyle Hit = new("CalamityMod/Sounds/Item/HalleysInfernoHit") { Volume = 0.75f };
        public override void SetDefaults()
        {
            Item.damage = 444;
            Item.knockBack = 5.5f;
            Item.DamageType = DamageClass.Ranged;

            // Burst of 5, one every 5 frames for 25 total. Cooldown of 39 frames.
            Item.useTime = 5;
            Item.useAnimation = 25;
            Item.reuseDelay = 39;
            Item.useLimitPerAnimation = 5;
            Item.autoReuse = true;

            Item.useAmmo = AmmoID.Gel;
            Item.shootSpeed = 12f;
            Item.shoot = ModContent.ProjectileType<HalleysComet>();

            Item.width = 84;
            Item.height = 34;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.UseSound = Shoot;
            Item.value = CalamityGlobalItem.Rarity13BuyPrice;
            Item.rare = ModContent.RarityType<PureGreen>();
        }

        // Terraria seems to really dislike high crit values in SetDefaults
        public override void ModifyWeaponCrit(Player player, ref float crit) => crit += 20;

        public override Vector2? HoldoutOffset() => new Vector2(-15, 0);

        public override bool CanConsumeAmmo(Item ammo, Player player) => Main.rand.Next(100) >= 50;

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.RifleScope).
                AddIngredient<Lumenyl>(6).
                AddIngredient<RuinousSoul>(4).
                AddIngredient<ExodiumCluster>(12).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
