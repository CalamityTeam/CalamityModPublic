using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class SubsumingVortex : ModItem
    {
        public const int VortexReleaseRate = 32;

        public const int VortexShootDelay = 56;

        public const float SmallVortexTargetRange = 1300f;

        public const float GiantVortexMouseDriftFactor = 0.35f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Subsuming Vortex");
            Tooltip.SetDefault("Casts a gigantic vortex above your head with a bias towards the mouse\n" +
                               "When enemies are near the vortex, it sends multiple fast-moving smaller vortices towards them");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 533;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 12;
            Item.width = 38;
            Item.height = 48;
            Item.UseSound = SoundID.Item84;
            Item.useTime = Item.useAnimation = 60;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 5f;
            Item.rare = ItemRarityID.Red;
            Item.Calamity().customRarity = CalamityRarity.Violet;
            Item.value = CalamityGlobalItem.Rarity15BuyPrice;
            Item.autoReuse = true;
            Item.channel = true;
            Item.shoot = ModContent.ProjectileType<EnormousConsumingVortex>();
            Item.shootSpeed = 7f;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0;

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Item.DrawItemGlowmaskSingleFrame(spriteBatch, rotation, ModContent.Request<Texture2D>("CalamityMod/Items/Weapons/Magic/SubsumingVortexGlow").Value);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<AuguroftheElements>().
                AddIngredient<EventHorizon>().
                AddIngredient<TearsofHeaven>().
                AddIngredient<MiracleMatter>().
                AddTile<DraedonsForge>().
                Register();
        }
    }
}
