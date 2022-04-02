using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class PwnagehammerMelee : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Pwnagehammer");
            Tooltip.SetDefault("Throws a heavy, gravity-affected hammer that creates a loud blast of hallowed energy when it hits something\n" +
            "There is a 20 percent chance for the hammer to home in on a target\n" +
            "Homing hammers summon an additional spectral hammer on hit and are guaranteed to land a critical hit");
        }

        public override void SetDefaults()
        {
            item.width = 66;
            item.damage = 210;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.autoReuse = true;
            item.useAnimation = item.useTime = 48;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 10f;
            item.UseSound = SoundID.Item1;
            item.melee = true;
            item.height = 66;
            item.value = Item.buyPrice(gold: 48);
            item.rare = ItemRarityID.LightPurple;
            item.shoot = ModContent.ProjectileType<PwnagehammerProj>();
            item.shootSpeed = 24.4f;
        }

        // Terraria seems to really dislike high crit values in SetDefaults
        public override void GetWeaponCrit(Player player, ref int crit) => crit += 10;

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Vector2 speed = new Vector2(speedX, speedY);
            Vector2 yeetOffset = Vector2.Normalize(speed) * 40f;
            if (Collision.CanHit(position, 0, 0, position + yeetOffset, 0, 0))
            {
                position += yeetOffset;
            }
            Projectile.NewProjectile(position, speed, type, damage, knockBack, player.whoAmI, Main.rand.NextBool(5) ? 1f : -1f);
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Pwnhammer);
            recipe.AddIngredient(ItemID.HallowedBar, 7);
            recipe.AddIngredient(ItemID.SoulofMight, 3);
            recipe.AddIngredient(ItemID.SoulofSight, 3);
            recipe.AddIngredient(ItemID.SoulofFright, 3);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
