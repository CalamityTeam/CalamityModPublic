using Terraria.DataStructures;
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
            Item.damage = 14;
            Item.shootSpeed = 12f;
            Item.shoot = ModContent.ProjectileType<Brick>();
            Item.width = 26;
            Item.height = 20;
            Item.useTime = Item.useAnimation = 40;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 5f;
            Item.value = Item.buyPrice(0, 0, 0, 50);
            Item.rare = ItemRarityID.White;
            Item.maxStack = 999;
            Item.UseSound = SoundID.Item1;
            Item.consumable = true;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.Calamity().rogue = true;
        }

        // Terraria seems to really dislike high crit values in SetDefaults
        public override void ModifyWeaponCrit(Player player, ref int crit) => crit += 20;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            //Check if stealth is full
            if (player.Calamity().StealthStrikeAvailable())
            {
                int p = Projectile.NewProjectile(source, position.X, position.Y, velocity.X, velocity.Y, type, damage, knockback, player.whoAmI, 1);
                if (p.WithinBounds(Main.maxProjectiles))
                    Main.projectile[p].Calamity().stealthStrike = true;
                return false;
            }
            return true;
        }
        public override void AddRecipes()
        {
            CreateRecipe(20).AddIngredient(ItemID.RedBrick, 5).AddTile(TileID.Anvils).Register();
        }
    }
}
