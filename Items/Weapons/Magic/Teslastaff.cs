using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Rarities;
using CalamityMod.Sounds;
using CalamityMod.Systems.Collections;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class Teslastaff : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";
        public override void SetStaticDefaults()
        {
            Item.staff[Type] = true;
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [BuffID.Electrified];
        }

        public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 38;
            Item.damage = 166;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 10;
            Item.useAnimation = Item.useTime = 10;
            Item.reuseDelay = 60;
            Item.knockBack = 0.25f;
            Item.shoot = ModContent.ProjectileType<Teslabeam>();
            Item.shootSpeed = 30f;

            Item.UseSound = CommonCalamitySounds.LightningSound with { Pitch = 1.1f };
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.channel = true;

            Item.value = CalamityGlobalItem.RarityTurquoiseBuyPrice;
            Item.rare = ModContent.RarityType<Turquoise>();
            Item.Calamity().donorItem = true;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0;

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.ThunderStaff).
                AddRecipeGroup("AnyCopperBar", 20).
                AddIngredient<EssenceofSunlight>(6).
                AddIngredient<ArmoredShell>(3).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
