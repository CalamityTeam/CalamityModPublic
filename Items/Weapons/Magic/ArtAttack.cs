using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class ArtAttack : ModItem
    {
        public const int MaxDamageBoostTime = 270;
        public const float MaxDamageBoostFactor = 2.3f;
        public static readonly SoundStyle UseSound = new("CalamityMod/Sounds/Item/ArtAttackCast");
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Art Attack");
            Tooltip.SetDefault("Casts a star that follows the mouse that creates a rainbow trail as it moves\n" +
                "Once a full shape is created with the trail all enemies within it take damage proportional to how long it took to draw the shape\n" +
                "And the audience goes wild!");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 470;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 10;
            Item.width = 70;
            Item.height = 70;
            Item.useTime = Item.useAnimation = 24;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2f;
            Item.value = CalamityGlobalItem.Rarity7BuyPrice;
            Item.rare = ItemRarityID.Lime;
            Item.UseSound = UseSound;
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
