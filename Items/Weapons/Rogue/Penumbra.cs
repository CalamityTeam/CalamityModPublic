using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Rogue;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class Penumbra : RogueWeapon
    {
        public static float ShootSpeed = 8f;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Penumbra");
            Tooltip.SetDefault("Throws a shadow bomb that explodes into homing souls\n" +
                               "Stealth strikes make the bomb manifest on the cursor and explode into more souls");
        }

        public override void SafeSetDefaults()
        {
            item.width = 46;
            item.height = 32;
            item.autoReuse = true;
            item.noUseGraphic = true;
            item.noMelee = true;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.UseSound = SoundID.Item103;
            item.value = Item.buyPrice(1, 80, 0, 0);
            item.rare = ItemRarityID.Red;

            item.damage = 1008;
            item.useAnimation = 40;
            item.useTime = 40;
            item.knockBack = 8f;
            item.shoot = ModContent.ProjectileType<PenumbraBomb>();
            item.shootSpeed = ShootSpeed;

            item.Calamity().customRarity = CalamityRarity.DarkBlue;
            item.Calamity().rogue = true;
        }

        // Terraria seems to really dislike high crit values in SetDefaults
        public override void GetWeaponCrit(Player player, ref int crit) => crit += 16;

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.Calamity().StealthStrikeAvailable())
            {
                Vector2 vector2 = player.RotatedRelativePoint(player.MountedCenter, true);
                float num78 = Main.mouseX + Main.screenPosition.X - vector2.X;
                float num79 = Main.mouseY + Main.screenPosition.Y - vector2.Y;
                if (player.gravDir == -1f)
                {
                    num79 = Main.screenPosition.Y + Main.screenHeight - Main.mouseY - vector2.Y;
                }
                if ((float.IsNaN(num78) && float.IsNaN(num79)) || (num78 == 0f && num79 == 0f))
                {
                    num78 = player.direction;
                    num79 = 0f;
                }
                vector2 += new Vector2(num78, num79);
                int proj = Projectile.NewProjectile(vector2, new Vector2(0f,-0.5f), ModContent.ProjectileType<PenumbraBomb>(), damage, knockBack, player.whoAmI, 0f, 1f);
                if (proj.WithinBounds(Main.maxProjectiles))
                    Main.projectile[proj].Calamity().stealthStrike = true;
                return false;
            }
            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<RuinousSoul>(), 6);
            recipe.AddIngredient(ModContent.ItemType<CosmiliteBar>(), 8);
            recipe.AddIngredient(ModContent.ItemType<NightmareFuel>(), 20);
            recipe.AddTile(ModContent.TileType<CosmicAnvil>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
