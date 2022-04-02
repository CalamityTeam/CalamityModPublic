using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class SquirrelSquireStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Squirrel Squire Staff");
            Tooltip.SetDefault("Summons a squirrel squire to fight for you");
        }

        public override void SetDefaults()
        {
            item.damage = 8;
            item.mana = 10;
            item.width = 52;
            item.height = 52;
            item.useTime = item.useAnimation = 35;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.noMelee = true;
            item.knockBack = 0.5f;
            item.value = Item.buyPrice(0, 0, 50, 0);
            item.rare = ItemRarityID.White;
            item.UseSound = SoundID.Item44;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<SquirrelSquireMinion>();
            item.shootSpeed = 10f;
            item.summon = true;
            item.sentry = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.altFunctionUse != 2)
            {
                position = Main.MouseWorld;
                speedX = 0;
                speedY = 0;
                Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, player.whoAmI, 0f, 30f);
                player.UpdateMaxTurrets();
                //projectile.ai[1] is attack cooldown.  Setting it here prevents immediate attacks
            }
            return false;
        }

        //in case you lose it and want another for some bizzare reason
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Wood, 10);
            recipe.anyWood = true;
            recipe.AddIngredient(ItemID.Acorn);
            recipe.AddTile(TileID.WorkBenches);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
