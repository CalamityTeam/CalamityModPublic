using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Crags;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Rarities;
using CalamityMod.Systems.Collections;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class DevilsSunrise : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public override void SetStaticDefaults()
        {
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [ModContent.BuffType<BrimstoneFlames>()];
        }

        public override void SetDefaults()
        {
            Item.width = 66;
            Item.height = 66;
            Item.damage = 420;
            Item.DamageType = DamageClass.MeleeNoSpeed;
            Item.crit = 10;
            Item.useAnimation = 25;
            Item.useTime = 5;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.channel = true;
            Item.knockBack = 4f;
            Item.autoReuse = false;
            Item.useStyle = ItemUseStyleID.Shoot;

            Item.value = CalamityGlobalItem.RarityTurquoiseBuyPrice;
            Item.rare = ModContent.RarityType<Turquoise>();
            Item.Calamity().donorItem = true;

            Item.shoot = ModContent.ProjectileType<DevilsSunriseProj>();
            Item.shootSpeed = 24f;
        }

        // You can't use the sword if you've thrown it, silly
        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[ModContent.ProjectileType<DevilsSunriseCyclone>()] <= 0;
        public override void HoldItem(Player player)
        {
            player.Calamity().mouseWorldListener = true;
            player.Calamity().rightClickListener = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Terragrim).
                AddIngredient<BloodstoneCore>(25).
                AddIngredient<ScorchedBone>(10).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
