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
            Item.damage = 74;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useAnimation = 28;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 28;
            Item.knockBack = 4f;
            Item.UseSound = SoundID.Item106;
            Item.autoReuse = true;
            Item.height = 24;
            Item.value = CalamityGlobalItem.Rarity7BuyPrice;
            Item.rare = ItemRarityID.Lime;
            Item.shoot = ModContent.ProjectileType<DuststormInABottleProj>();
            Item.shootSpeed = 14f;
            Item.DamageType = RogueDamageClass.Instance;
        }

        public static float StealthDmgMult = 0.6f; //So I can edit it directly via DragonLens instead of having to do math with CalTestHelpers
        public override float StealthDamageMultiplier => StealthDmgMult;

        public static int CloudLifetime = 200;
        public static float DustRadius = 11f;
        public static int StealthIframes = 9;

        //Cloud hitbox size manipulation
        public static float MaxSize = 3.2f;
        public static float MaxSizeStealth = 3.6f;
        public static float GrowthRate = 0.025f;
        public static float StealthGrowhRate = 0.035f;

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
                AddIngredient<AncientBoneDust>(10).
                AddIngredient<GrandScale>().
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
