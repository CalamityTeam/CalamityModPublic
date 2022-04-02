using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class ThrowingBrick : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Prove its resistance by throwing it upwards and catching it with your face\n" +
                "Throws a brick that shatters if stealth is full.");
            DisplayName.SetDefault("Throwing Brick");
        }
        public override void SafeSetDefaults()
        {
            item.damage = 14;
            item.shootSpeed = 12f;
            item.shoot = ModContent.ProjectileType<Brick>();
            item.width = 26;
            item.height = 20;
            item.useTime = item.useAnimation = 40;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 5f;
            item.value = Item.buyPrice(0, 0, 0, 50);
            item.rare = ItemRarityID.White;
            item.maxStack = 999;
            item.UseSound = SoundID.Item1;
            item.consumable = true;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.Calamity().rogue = true;
        }

        // Terraria seems to really dislike high crit values in SetDefaults
        public override void GetWeaponCrit(Player player, ref int crit) => crit += 20;

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            //Check if stealth is full
            if (player.Calamity().StealthStrikeAvailable())
            {
                int p = Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, player.whoAmI, 1);
                if (p.WithinBounds(Main.maxProjectiles))
                    Main.projectile[p].Calamity().stealthStrike = true;
                return false;
            }
            return true;
        }
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.RedBrick, 5);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this, 20);
            recipe.AddRecipe();
        }
    }
}
