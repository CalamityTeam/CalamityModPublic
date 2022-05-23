using Terraria.DataStructures;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class NastyCholla : ModItem
    {
        public static int BaseDamage = 9;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Nasty Cholla");
            Tooltip.SetDefault(@"Throws a spiky ball that sticks to everything
Explodes into cactus spikes after roughly 3 seconds
Can hurt town NPCs
Stealth strikes throw five at once");
            SacrificeTotal = 99;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.damage = BaseDamage;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.maxStack = 999;
            Item.consumable = true;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 20;
            Item.knockBack = 3f;
            Item.UseSound = SoundID.Item1;
            Item.height = 18;
            Item.value = Item.buyPrice(0, 0, 0, 50);
            Item.rare = ItemRarityID.White;
            Item.shoot = ModContent.ProjectileType<NastyChollaBol>();
            Item.shootSpeed = 8f;
            Item.DamageType = RogueDamageClass.Instance;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.Calamity().StealthStrikeAvailable()) //setting the stealth strike
            {
                int spread = 3;
                for (int i = 0; i < 5; i++)
                {
                    Vector2 perturbedspeed = new Vector2(velocity.X + Main.rand.Next(-3,4), velocity.Y + Main.rand.Next(-3,4)).RotatedBy(MathHelper.ToRadians(spread));
                    int proj = Projectile.NewProjectile(source, position.X, position.Y, perturbedspeed.X, perturbedspeed.Y, type, damage, knockback, player.whoAmI);
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
            CreateRecipe(15).
                AddIngredient(ItemID.Cactus, 3).
                AddTile(TileID.WorkBenches).
                Register();
        }
    }
}
