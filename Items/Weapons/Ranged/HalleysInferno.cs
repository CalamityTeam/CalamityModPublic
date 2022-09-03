using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Ores;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class HalleysInferno : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Halley's Inferno");
            Tooltip.SetDefault("Halley came sooner than expected\n" +
            "Fires a flaming comet\n" +
            "50% chance to not consume gel\n" +
            "Right click to zoom out");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 1350;
            Item.knockBack = 5f;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = Item.useAnimation = 30;
            Item.autoReuse = true;
            Item.useAmmo = AmmoID.Gel;
            Item.shootSpeed = 14.6f;
            Item.shoot = ModContent.ProjectileType<HalleysComet>();

            Item.width = 84;
            Item.height = 34;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.UseSound = SoundID.Item34;
            Item.value = CalamityGlobalItem.Rarity14BuyPrice;
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
