using Microsoft.Xna.Framework;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class IronFrancisca : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Iron Francisca");
            Tooltip.SetDefault("The franciscas do more damage for a short time when initially thrown\n" +
                               "Stealth Strike: The franciscas have infinite piercing");
        }

        public override void SafeSetDefaults()
        {
            item.width = 40;
            item.damage = 7;
            item.noMelee = true;
            item.consumable = true;
            item.noUseGraphic = true;
            item.useAnimation = 15;
            item.useStyle = 1;
            item.useTime = 15;
            item.knockBack = 1f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 36;
            item.maxStack = 999;
            item.value = 1000;
            item.rare = 0;
            item.shoot = ModContent.ProjectileType<Projectiles.IronFrancisca>();
            item.shootSpeed = 12f;
            item.Calamity().rogue = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.Calamity().StealthStrikeAvailable())
            {
                int p = Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, player.whoAmI, 0f, 1f);
                Main.projectile[p].Calamity().stealthStrike = true;
                return false;
            }
            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Wood);
            recipe.anyWood = true;
            recipe.AddIngredient(ItemID.IronBar);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this, 50);
            recipe.AddRecipe();
        }
    }
}
