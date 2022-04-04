using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class HeliumFlash : ModItem
    {
        internal const float ExplosionDamageMultiplier = 0.125f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Helium Flash");
            Tooltip.SetDefault("The power of a galaxy, if only for mere moments\n" +
            "Launches volatile star cores which erupt into colossal fusion blasts");
            Item.staff[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 112;
            Item.height = 112;
            Item.DamageType = DamageClass.Magic;
            Item.damage = 2727;
            Item.knockBack = 9.5f;
            Item.mana = 26;
            Item.useAnimation = 40;
            Item.useTime = 40;
            Item.autoReuse = true;
            Item.noMelee = true;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundID.Item73;

            Item.value = CalamityGlobalItem.Rarity15BuyPrice;
            Item.rare = ItemRarityID.Purple;
            Item.Calamity().customRarity = CalamityRarity.Violet;

            Item.shoot = ModContent.ProjectileType<VolatileStarcore>();
            Item.shootSpeed = 15f;
        }

        // TODO -- Fancy visual flare doesn't work with resprited Helium Flash, adjust dust positions
        /*
        // Creates dust at the tip of the staff when used.
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 dir = velocity;
            double angle = Math.Atan2(velocity.Y, velocity.X) + MathHelper.PiOver4;
            dir = dir.SafeNormalize(Vector2.Zero);
            dir *= 64f * 1.4142f; // distance to gleaming point on staff
            Vector2 dustPos = position + dir;

            int dustType = 162;
            int dustCount = 72;
            float minSpeed = 4f;
            float maxSpeed = 11f;
            float minScale = 0.8f;
            float maxScale = 1.4f;
            Vector2 leftVec = new Vector2(-1f, 0f).RotatedBy(angle);
            Vector2 rightVec = new Vector2(1f, 0f).RotatedBy(angle);
            Vector2 upVec = new Vector2(0f, -1f).RotatedBy(angle);
            Vector2 downVec = new Vector2(0f, 1f).RotatedBy(angle);
            for (int i = 0; i < dustCount; i += 4)
            {
                int left = Dust.NewDust(dustPos, 1, 1, dustType, 0f, 0f);
                Main.dust[left].position = dustPos;
                Main.dust[left].velocity = leftVec * Main.rand.NextFloat(minSpeed, maxSpeed);
                Main.dust[left].scale = Main.rand.NextFloat(minScale, maxScale);
                Main.dust[left].noGravity = true;

                int right = Dust.NewDust(dustPos, 1, 1, dustType, 0f, 0f);
                Main.dust[right].position = dustPos;
                Main.dust[right].velocity = rightVec * Main.rand.NextFloat(minSpeed, maxSpeed);
                Main.dust[right].scale = Main.rand.NextFloat(minScale, maxScale);
                Main.dust[right].noGravity = true;

                int up = Dust.NewDust(dustPos, 1, 1, dustType, 0f, 0f);
                Main.dust[up].position = dustPos;
                Main.dust[up].velocity = upVec * Main.rand.NextFloat(minSpeed, maxSpeed);
                Main.dust[up].scale = Main.rand.NextFloat(minScale, maxScale);
                Main.dust[up].noGravity = true;

                int down = Dust.NewDust(dustPos, 1, 1, dustType, 0f, 0f);
                Main.dust[down].position = dustPos;
                Main.dust[down].velocity = downVec * Main.rand.NextFloat(minSpeed, maxSpeed);
                Main.dust[down].scale = Main.rand.NextFloat(minScale, maxScale);
                Main.dust[down].noGravity = true;
            }
            return true;
        }
        */

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ModContent.ItemType<VenusianTrident>()).AddIngredient(ModContent.ItemType<CalamitasInferno>()).AddIngredient(ModContent.ItemType<ForbiddenSun>()).AddIngredient(ItemID.FragmentSolar, 20).AddIngredient(ItemID.FragmentNebula, 5).AddIngredient(ModContent.ItemType<AuricBar>(), 5).AddTile(ModContent.TileType<CosmicAnvil>()).Register();
        }
    }
}
