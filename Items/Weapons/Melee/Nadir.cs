using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee.Spears;
using CalamityMod.Rarities;
using CalamityMod.Systems.Collections;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class Nadir : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public static float ProjShootSpeed = 20f;
        public static int FadeoutSpeed = 20;

        public override void SetStaticDefaults()
        {
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [ModContent.BuffType<Voidfrost>()];
            ItemID.Sets.Spears[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 144;
            Item.height = 144;
            Item.noUseGraphic = true;
            Item.DamageType = DamageClass.Melee;
            Item.damage = 365;
            Item.knockBack = 8f;
            Item.useAnimation = 18;
            Item.useTime = 18;
            Item.autoReuse = true;
            Item.noMelee = true;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundID.Item1;

            Item.value = CalamityGlobalItem.RarityVioletBuyPrice;
            Item.rare = ModContent.RarityType<BurnishedAuric>();
            Item.Calamity().donorItem = true;

            Item.shoot = ModContent.ProjectileType<NadirSpear>();
            Item.shootSpeed = 12f; // This isn't the projectile's speed, it's the spear's.
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0;

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<VanishingPoint>().
                AddIngredient<AuricBar>(5).
                AddIngredient<DarksunFragment>(8).
                AddIngredient<TwistingNether>(5).
                AddTile<CosmicAnvil>().
                Register();
        }
    }
}
