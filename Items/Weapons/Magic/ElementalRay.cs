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
            Item.staff[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.damage = 90;
            item.magic = true;
            item.mana = 18;
            item.width = 62;
            item.height = 62;
            item.useTime = 4;
            item.useAnimation = 16;
            item.reuseDelay = 14;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 7.5f;
            item.value = CalamityGlobalItem.Rarity11BuyPrice;
            item.rare = ItemRarityID.Purple;
            item.UseSound = SoundID.Item60;
            item.autoReuse = true;
            item.shoot = 1;
            item.shootSpeed = 6f;
        }

        public override Vector2? HoldoutOrigin() => new Vector2(15);

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            float offsetAngle = MathHelper.TwoPi * player.itemAnimation / player.itemAnimationMax;
            offsetAngle += MathHelper.PiOver4 + Main.rand.NextFloat(0f, 1.3f);
            float shootSpeed = 1f;

            if (player.itemAnimation == item.useAnimation - 1f)
                type = ModContent.ProjectileType<SolarElementalBeam>();
            else if (player.itemAnimation == item.useAnimation - item.useTime - 1)
            {
                type = ModContent.ProjectileType<NebulaElementalBeam>();
                offsetAngle -= NebulaElementalBeam.UniversalAngularSpeed * 0.5f;
            }
            else if (player.itemAnimation == item.useAnimation - item.useTime * 2 - 1)
            {
                type = ModContent.ProjectileType<VortexElementalBeam>();
                shootSpeed = 2f;
            }
            else if (player.itemAnimation == item.useAnimation - item.useTime * 3 - 1)
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
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<TerraRay>());
            recipe.AddIngredient(ModContent.ItemType<GalacticaSingularity>(), 5);
            recipe.AddIngredient(ItemID.LunarBar, 5);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
