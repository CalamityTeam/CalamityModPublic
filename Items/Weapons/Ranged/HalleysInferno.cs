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

        public static readonly SoundStyle Shoot = new("CalamityMod/Sounds/Item/HalleysInfernoShoot") { Volume = 0.6f };
        public static readonly SoundStyle Hit = new("CalamityMod/Sounds/Item/HalleysInfernoHit") { Volume = 0.5f };
        public override void SetDefaults()
        {
            Item.damage = 1205;
            Item.knockBack = 5.5f;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = Item.useAnimation = 39;
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
