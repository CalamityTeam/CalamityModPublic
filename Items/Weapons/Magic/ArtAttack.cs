using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class ArtAttack : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";
        public const int MaxDamageBoostTime = 180;
        public const float MaxDamageBoostFactor = 18f;
        public static readonly SoundStyle UseSound = new("CalamityMod/Sounds/Item/ArtAttackCast");
        public override void SetDefaults()
        {
            Item.damage = 80;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 20;
            Item.width = 70;
            Item.height = 70;
            Item.useTime = Item.useAnimation = 24;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2f;
            Item.value = CalamityGlobalItem.Rarity7BuyPrice;
            Item.rare = ItemRarityID.Lime;
            Item.UseSound = null;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.shoot = ModContent.ProjectileType<ArtAttackHoldout>();
            Item.channel = true;
            Item.shootSpeed = 12f;
            Item.Calamity().donorItem = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.RainbowRod).
                AddIngredient(ItemID.LargeRuby).
                AddIngredient(ItemID.CrystalShard).
                AddIngredient<AshesofCalamity>(5).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
