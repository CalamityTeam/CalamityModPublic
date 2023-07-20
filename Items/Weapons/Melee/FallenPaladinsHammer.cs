using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    [LegacyName("TruePaladinsHammer", "TruePaladinsHammerMelee", "TruePaladinsHammerRogue")]
    public class FallenPaladinsHammer : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public static float Speed = 28.8f;
        public override void SetDefaults()

        {
            Item.width = 14;
            Item.damage = 368;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
            Item.useAnimation = Item.useTime = 57;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 20f;
            Item.UseSound = SoundID.Item1;
            Item.DamageType = DamageClass.MeleeNoSpeed;
            Item.height = 28;
            Item.value = CalamityGlobalItem.RarityYellowBuyPrice;
            Item.rare = ItemRarityID.Yellow;
            Item.shoot = ModContent.ProjectileType<FallenPaladinsHammerProj>();
            Item.shootSpeed = Speed;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.PaladinsHammer).
                AddIngredient<Pwnagehammer>().
                AddIngredient<ScoriaBar>(5).
                AddIngredient<AshesofCalamity>(5).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
