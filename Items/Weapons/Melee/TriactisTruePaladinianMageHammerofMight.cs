using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    [LegacyName("TriactisTruePaladinianMageHammerofMightMelee")]
    public class TriactisTruePaladinianMageHammerofMight : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";

        public override void SetDefaults()
        {
            Item.width = 168;
            Item.height = 168;
            Item.damage = 2000;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
            Item.useAnimation = Item.useTime = 25;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 14f;
            Item.UseSound = new SoundStyle("CalamityMod/Sounds/Item/SwingMid") with { Volume = 0.5f, Pitch = Main.rand.NextFloat(-0.3f, -0.4f) };
            Item.DamageType = DamageClass.MeleeNoSpeed;
            Item.shoot = ModContent.ProjectileType<TriactisHammerProj>();
            Item.shootSpeed = 25f;

            Item.value = CalamityGlobalItem.RarityHotPinkBuyPrice;
            Item.rare = ModContent.RarityType<HotPink>();
            Item.Calamity().devItem = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<GalaxySmasher>().
                AddIngredient<ShadowspecBar>(5).
                AddIngredient<LifeAlloy>(5).
                AddTile<DraedonsForge>().
                Register();
        }
    }
}
