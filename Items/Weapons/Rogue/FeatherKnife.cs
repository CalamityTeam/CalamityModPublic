using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class FeatherKnife : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Feather Knife");
            Tooltip.SetDefault(@"Throws a knife which summons homing feathers
Stealth strike throws a volley of knives");
        }

        public override void SafeSetDefaults()
        {
            item.width = 18;
            item.damage = 25;
            item.noMelee = true;
            item.consumable = true;
            item.noUseGraphic = true;
            item.useAnimation = 11;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 18;
            item.knockBack = 2f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 32;
            item.maxStack = 999;
            item.shoot = ModContent.ProjectileType<FeatherKnifeProjectile>();
            item.shootSpeed = 25f;
            item.Calamity().rogue = true;

            item.value = Item.sellPrice(copper: 60);
            item.rare = ItemRarityID.Orange;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<AerialiteBar>());
            recipe.AddTile(TileID.SkyMill);
            recipe.SetResult(this, 100);
            recipe.AddRecipe();
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.Calamity().StealthStrikeAvailable()) //setting the stealth strike
            {
                int spread = 6;
                for (int i = 0; i < 5; i++)
                {
                    Vector2 perturbedspeed = new Vector2(speedX + Main.rand.Next(-3, 4), speedY + Main.rand.Next(-3, 4)).RotatedBy(MathHelper.ToRadians(spread));
                    int proj = Projectile.NewProjectile(position, perturbedspeed, type, damage, knockBack, player.whoAmI);
                    if (proj.WithinBounds(Main.maxProjectiles))
                    {
                        Main.projectile[proj].Calamity().stealthStrike = true;
                        Main.projectile[proj].noDropItem = true;
                    }
                    spread -= Main.rand.Next(2, 6);
                }
                return false;
            }
            return true;
        }
    }
}
