using Terraria.DataStructures;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class DuststormInABottle : RogueWeapon
    {
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.damage = 64;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 30;
            Item.knockBack = 5f;
            Item.UseSound = SoundID.Item106;
            Item.autoReuse = true;
            Item.height = 24;
            Item.value = CalamityGlobalItem.Rarity7BuyPrice;
            Item.rare = ItemRarityID.Lime;
            Item.shoot = ModContent.ProjectileType<DuststormInABottleProj>();
            Item.shootSpeed = 12f;
            Item.DamageType = RogueDamageClass.Instance;
        }

        public static float StealthDmgMult = 1f; //So I can edit it directly via DragonLens instead of having to do math with CalTestHelpers
        public override float StealthDamageMultiplier => StealthDmgMult;

        public static double StealthCloudAmountMult = 2.5; //Seems like DragonLens does not detect doubles

        public static int CloudLifetime = 200;
        public static float DustRadius = 16f;

        //Cloud hitbox size manipulation
        public static float MaxSize = 3f;
        public static float MaxSizeStealth = 3.5f;
        public static float GrowthRate = 0.02f;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.Calamity().StealthStrikeAvailable())
            {
                int stealth = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
                if (stealth.WithinBounds(Main.maxProjectiles))
                    Main.projectile[stealth].Calamity().stealthStrike = true;
                return false;
            }
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.SandstorminaBottle).
                AddIngredient<AncientBoneDust>(50).
                AddIngredient<GrandScale>().
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
