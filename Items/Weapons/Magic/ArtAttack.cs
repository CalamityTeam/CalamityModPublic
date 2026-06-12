using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using Terraria;
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
            Item.width = 70;
            Item.height = 70;
            Item.damage = 120;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 60;
            Item.useAnimation = Item.useTime = 24;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2f;
            Item.value = CalamityGlobalItem.RarityLimeBuyPrice;
            Item.rare = ItemRarityID.Lime;
            Item.UseSound = null;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.shoot = ModContent.ProjectileType<ArtAttackHoldout>();
            Item.channel = true;
            Item.shootSpeed = 12f;
            Item.Calamity().donorItem = true;
        }

        // Cancels out the mana used to summon the holdout
        public override void OnConsumeMana(Player player, int manaConsumed)
        {
            if (player.ownedProjectileCounts[Item.shoot] <= 0)
                player.statMana += manaConsumed;
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
