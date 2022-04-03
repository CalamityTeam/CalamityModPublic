using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace CalamityMod.Items.Weapons.Magic
{
    public class ElementalRay : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Elemental Ray");
            Tooltip.SetDefault("Casts four celestial beams near the player\n" +
                "Solar beams explode into fire on enemy hits\n" +
                "Nebula beams sweep a little bit over time\n" +
                "Vortex beams act like fast lightning and electrify enemies on hit\n" +
                "Stardust beams release small stars that home on enemy hits");
            Item.staff[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 90;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 18;
            Item.width = 62;
            Item.height = 62;
            Item.useTime = 4;
            Item.useAnimation = 16;
            Item.reuseDelay = 14;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 7.5f;
            Item.value = CalamityGlobalItem.Rarity11BuyPrice;
            Item.rare = ItemRarityID.Purple;
            Item.UseSound = SoundID.Item60;
            Item.autoReuse = true;
            Item.shoot = 1;
            Item.shootSpeed = 6f;
        }

        public override Vector2? HoldoutOrigin() => new Vector2(15);

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            float offsetAngle = MathHelper.TwoPi * player.itemAnimation / player.itemAnimationMax;
            offsetAngle += MathHelper.PiOver4 + Main.rand.NextFloat(0f, 1.3f);
            float shootSpeed = 1f;

            if (player.itemAnimation == Item.useAnimation - 1f)
                type = ModContent.ProjectileType<SolarElementalBeam>();
            else if (player.itemAnimation == Item.useAnimation - Item.useTime - 1)
            {
                type = ModContent.ProjectileType<NebulaElementalBeam>();
                offsetAngle -= NebulaElementalBeam.UniversalAngularSpeed * 0.5f;
            }
            else if (player.itemAnimation == Item.useAnimation - Item.useTime * 2 - 1)
            {
                type = ModContent.ProjectileType<VortexElementalBeam>();
                shootSpeed = 2f;
            }
            else if (player.itemAnimation == Item.useAnimation - Item.useTime * 3 - 1)
                type = ModContent.ProjectileType<StardustElementalBeam>();
            else
                return false;

            Vector2 spawnOffset = player.SafeDirectionTo(Main.MouseWorld, Vector2.UnitY).RotatedBy(offsetAngle) * -Main.rand.NextFloat(40f, 96f);
            Vector2 shootDirection = (Main.MouseWorld - (position + spawnOffset)).SafeNormalize(Vector2.UnitX * player.direction);
            int beam = Projectile.NewProjectile(position + spawnOffset, shootDirection * shootSpeed, type, damage, knockBack, player.whoAmI);

            // Define specific values for fired lightning.
            if (type == ModContent.ProjectileType<VortexElementalBeam>())
            {
                Main.projectile[beam].ai[0] = shootDirection.ToRotation();
                Main.projectile[beam].ai[1] = Main.rand.Next(100);
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ModContent.ItemType<TerraRay>()).AddIngredient(ModContent.ItemType<GalacticaSingularity>(), 5).AddIngredient(ItemID.LunarBar, 5).AddTile(TileID.LunarCraftingStation).Register();
        }
    }
}
