using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Summon.MirrorofKalandraMinions;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class MirrorofKalandra : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Summon";

        public static float TargetDistanceDetection = 2500f;
        public static float IdleDistanceFromPlayer = 250f;
        public static float OscillationSpeed = .025f;
        public static float OscillationRange = 8; // The bigger the number, the smaller the range is.

        public static float Axe_MinRamSpeed = 30f;
        public static float Axe_MaxRamSpeed = 50f;
        public static int Axe_IFrames = 15;
        public static float Axe_SpinSpeed = 25f; // In degrees per frame.

        public static float Purple_MinRamSpeed = 40f;
        public static float Purple_MaxRamSpeed = 60f;
        public static int Purple_IFrames = 15;
        public static float Purple_BlastDMGModifier = 3f;
        public static float Purple_BlastFireRate = 180f; // In frames.
        public static int Purple_BlastSize = 300;
        public static int Purple_BlastChargeTime = 10;
        public static float Purple_SpinSpeed = 25f; // In degrees per frame.

        public static int Scimitar_IFrames = 15;

        public static int Wind_BowChargeTime = 5; // Therefore, the higher the time, the slower the fire rate will be, and viceversa.
        public static float Wind_ArrowSpeed = 5f;
        public static int Wind_ArrowSpeedMult = 10;

        public static int Vile_BowChargeTime = 8;
        public static float Vile_ArrowSpeed = 5f;
        public static int Vile_ArrowSpeedMult = 10;
        public static float Vile_SplitDMGMultiplier = .33f;
        public static int Vile_SplitIFrames = 30;
        public static int Vile_SplitSpreadAngle = 8; // In degrees.

        public override void SetDefaults()
        {
            Item.damage = 380;
            Item.useTime = Item.useAnimation = 30;
            Item.knockBack = 4f;
            Item.mana = 10;
            Item.shoot = ModContent.ProjectileType<AtzirisDisfavor>();

            Item.width = 58;
            Item.height = 50;
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.Calamity().donorItem = true;
            Item.DamageType = DamageClass.Summon;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundID.Item4;
            Item.rare = ModContent.RarityType<DarkBlue>();
            Item.value = CalamityGlobalItem.Rarity14BuyPrice;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-15, 0);

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[ModContent.ProjectileType<HopeShredder>()] != 1;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<AtzirisDisfavor>()] == 1)
            {
                type = ModContent.ProjectileType<HopeShredder>();
                if (player.ownedProjectileCounts[ModContent.ProjectileType<WindRipper>()] != 1)
                    type = ModContent.ProjectileType<WindRipper>();
                if (player.ownedProjectileCounts[ModContent.ProjectileType<Paradoxica>()] != 1)
                    type = ModContent.ProjectileType<Paradoxica>();
                if (player.ownedProjectileCounts[ModContent.ProjectileType<Starforge>()] != 1)
                    type = ModContent.ProjectileType<Starforge>();
            }

            int minion = Projectile.NewProjectile(source, player.Center, Vector2.Zero, type, damage, knockback, player.whoAmI);
            if (Main.projectile.IndexInRange(minion))
                Main.projectile[minion].rotation = -MathHelper.PiOver2;

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.MagicMirror).
                AddIngredient<CosmiliteBar>(10).
                AddIngredient<DivineGeode>(10).
                AddCondition(Condition.NearShimmer).
                Register();
        }
    }
}
