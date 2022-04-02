using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class NastyCholla : RogueWeapon
    {
        public static int BaseDamage = 9;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Nasty Cholla");
            Tooltip.SetDefault(@"Throws a spiky ball that sticks to everything
Explodes into cactus spikes after roughly 3 seconds
Can hurt town NPCs
Stealth strikes throw five at once");
        }

        public override void SafeSetDefaults()
        {
            item.width = 20;
            item.damage = BaseDamage;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.maxStack = 999;
            item.consumable = true;
            item.useAnimation = 20;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 20;
            item.knockBack = 3f;
            item.UseSound = SoundID.Item1;
            item.height = 18;
            item.value = Item.buyPrice(0, 0, 0, 50);
            item.rare = ItemRarityID.White;
            item.shoot = ModContent.ProjectileType<NastyChollaBol>();
            item.shootSpeed = 8f;
            item.Calamity().rogue = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.Calamity().StealthStrikeAvailable()) //setting the stealth strike
            {
                int spread = 3;
                for (int i = 0; i < 5; i++)
                {
                    Vector2 perturbedspeed = new Vector2(speedX + Main.rand.Next(-3,4), speedY + Main.rand.Next(-3,4)).RotatedBy(MathHelper.ToRadians(spread));
                    int proj = Projectile.NewProjectile(position.X, position.Y, perturbedspeed.X, perturbedspeed.Y, type, damage, knockBack, player.whoAmI);
                    if (proj.WithinBounds(Main.maxProjectiles))
                        Main.projectile[proj].Calamity().stealthStrike = true;
                    spread -= Main.rand.Next(1,4);
                }
                return false;
            }
            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Cactus, 3);
            recipe.AddTile(TileID.WorkBenches);
            recipe.SetResult(this, 15);
            recipe.AddRecipe();
        }
    }
}
