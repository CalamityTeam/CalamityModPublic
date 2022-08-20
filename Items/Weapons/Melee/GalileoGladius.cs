using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Ores;
using CalamityMod.Projectiles.Melee.Shortswords;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class GalileoGladius : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Galileo Gladius");
            Tooltip.SetDefault("Don't underestimate the power of small space swords\n" +
                "Shoots a homing crescent moon\n" +
                "Spawns planetoids on enemy hits\n" +
                "Receives 33% benefit from melee speed bonuses");
            SacrificeTotal = 1;
            ItemID.Sets.BonusAttackSpeedMultiplier[Item.type] = 0.33f;
        }

        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.DamageType = TrueMeleeDamageClass.Instance;
            Item.useAnimation = 10;
            Item.useTime = 10;
            Item.width = 44;
            Item.height = 44;
            Item.damage = 110;
            Item.knockBack = 10f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.shoot = ModContent.ProjectileType<GalileoGladiusProj>();
            Item.shootSpeed = 2.4f;

            Item.value = CalamityGlobalItem.Rarity13BuyPrice;
            Item.Calamity().customRarity = CalamityRarity.PureGreen;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Gladius).
                AddIngredient<Lumenyl>(8).
                AddIngredient<RuinousSoul>(5).
                AddIngredient<ExodiumCluster>(15).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
