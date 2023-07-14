using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Rogue;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class SearedPan : RogueWeapon
    {
        public static readonly SoundStyle SmashSound = new("CalamityMod/Sounds/Item/SearedPanSmash");

        // Attacks must be within 40 frames of each other to count as "consecutive" hits
        // This is a little less than double the use time
        public static int ConsecutiveHitOpening = 40;
        public override void SetDefaults()
        {
            Item.width = 60;
            Item.height = 36;
            Item.damage = 2222;
            Item.DamageType = RogueDamageClass.Instance;
            Item.knockBack = 10f;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = Item.useAnimation = 25;
            Item.reuseDelay = 1;
            Item.useLimitPerAnimation = 1;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.value = CalamityGlobalItem.Rarity15BuyPrice;
            Item.rare = ModContent.RarityType<Violet>();
            Item.shoot = ModContent.ProjectileType<SearedPanProjectile>();
            Item.shootSpeed = 15f;
            Item.Calamity().donorItem = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float mode = 1f;
            if (player.Calamity().searedPanCounter >= 3)
            {
                player.Calamity().searedPanCounter = 0;
                mode = 2f;
            }
            if (player.Calamity().StealthStrikeAvailable())
            {
                player.Calamity().searedPanCounter = 0;
                mode = 3f;
            }
            int pan = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, mode);
            if (mode > 1f && pan.WithinBounds(Main.maxProjectiles))
            {
                Main.projectile[pan].extraUpdates++;
                if (mode == 3f)
                    Main.projectile[pan].Calamity().stealthStrike = true;
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<UtensilPoker>().
                AddIngredient<AuricBar>(5).
                AddIngredient(ItemID.LifeCrystal).
                AddIngredient(ItemID.Bone, 92).
                // AddIngredient(ItemID.Steak).
                AddTile<CosmicAnvil>().
                Register();
        }
    }
}
