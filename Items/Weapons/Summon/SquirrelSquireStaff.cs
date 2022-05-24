using Terraria.DataStructures;
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
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 8;
            Item.mana = 10;
            Item.width = 52;
            Item.height = 52;
            Item.useTime = Item.useAnimation = 35;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.knockBack = 0.5f;
            Item.value = Item.buyPrice(0, 0, 50, 0);
            Item.rare = ItemRarityID.White;
            Item.UseSound = SoundID.Item44;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<SquirrelSquireMinion>();
            Item.shootSpeed = 10f;
            Item.DamageType = DamageClass.Summon;
            Item.sentry = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse != 2)
            {
                position = Main.MouseWorld;
                velocity.X = 0;
                velocity.Y = 0;
                int p = Projectile.NewProjectile(source, position.X, position.Y, velocity.X, velocity.Y, type, damage, knockback, player.whoAmI, 0f, 30f);
                if (Main.projectile.IndexInRange(p))
                    Main.projectile[p].originalDamage = Item.damage;

                player.UpdateMaxTurrets();
                //projectile.ai[1] is attack cooldown.  Setting it here prevents immediate attacks
            }
            return false;
        }

        //in case you lose it and want another for some bizzare reason
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Wood, 10).
                AddIngredient(ItemID.Acorn).
                AddTile(TileID.WorkBenches).
                Register();
        }
    }
}
