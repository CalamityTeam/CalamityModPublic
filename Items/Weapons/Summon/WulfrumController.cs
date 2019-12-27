using CalamityMod.Projectiles.Summon;
using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class WulfrumController : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Wulfrum Controller");
            Tooltip.SetDefault("Summons a wulfrum droid to fight for you");
        }

        public override void SetDefaults()
        {
            item.damage = 10;
            item.mana = 10;
            item.width = 28;
            item.height = 20;
            item.useTime = 36;
            item.useAnimation = 36;
            item.useStyle = 4;
            item.noMelee = true;
            item.knockBack = 0.5f;
            item.value = Item.buyPrice(0, 1, 0, 0);
            item.rare = 1;
            item.UseSound = SoundID.Item15; //phaseblade sound effect
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<WulfrumDroid>();
            item.shootSpeed = 10f;
            item.summon = true;
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.altFunctionUse != 2)
            {
                position = Main.MouseWorld;
                speedX = 0;
                speedY = 0;
                Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, player.whoAmI);
            }
            return false;
        }

        public override bool UseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                player.MinionNPCTargetAim();
            }
            return base.UseItem(player);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<WulfrumShard>(), 9);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
