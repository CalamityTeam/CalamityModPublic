using CalamityMod.Projectiles.Melee;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Items.Tools
{
    public class MarniteObliterator : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Tools";
        public static readonly SoundStyle UseSound = new("CalamityMod/Sounds/Item/MarniteObliteratorUse") { PitchVariance = 0.3f };

        public static int ArmorPenetration = 15;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(ArmorPenetration);

        public override void SetDefaults()
        {
            Item.width = 36;
            Item.height = 18;
            Item.damage = 7;
            Item.DamageType = TrueMeleeNoSpeedDamageClass.Instance;
            Item.ArmorPenetration = ArmorPenetration;
            Item.pick = 59;
            Item.tileBoost = 7;
            Item.useAnimation = 25;
            Item.useTime = 3;
            Item.knockBack = 0.5f;
            Item.shoot = ModContent.ProjectileType<MarniteObliteratorProj>();
            Item.shootSpeed = 40f;

            Item.UseSound = SoundID.Item23;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = true;
            Item.channel = true;
            Item.noMelee = true;
            Item.noUseGraphic = true;

            Item.value = CalamityGlobalItem.RarityBlueBuyPrice;
            Item.rare = ItemRarityID.Blue;
        }

        public override void HoldItem(Player player) => player.Calamity().mouseWorldListener = true;

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Diamond).
                AddRecipeGroup("AnyGoldBar", 3).
                AddIngredient(ItemID.Granite, 5).
                AddIngredient(ItemID.Marble, 5).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
