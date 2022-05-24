using Terraria.DataStructures;
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
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 10;
            Item.mana = 10;
            Item.width = 28;
            Item.height = 20;
            Item.useTime = Item.useAnimation = 34;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.noMelee = true;
            Item.knockBack = 0.5f;
            Item.value = Item.buyPrice(0, 1, 0, 0);
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = SoundID.Item15; //phaseblade sound effect
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<WulfrumDroid>();
            Item.shootSpeed = 10f;
            Item.DamageType = DamageClass.Summon;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse != 2)
            {
                position = Main.MouseWorld;
                velocity.X = 0;
                velocity.Y = 0;
                int droid = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 0f, 1f);
                if (Main.projectile.IndexInRange(droid))
                    Main.projectile[droid].originalDamage = Item.damage;
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<WulfrumShard>(9).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
