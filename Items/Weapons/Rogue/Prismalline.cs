using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class Prismalline : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Prismalline");
            Tooltip.SetDefault("Throws daggers that split after a while\n" +
            "Stealth strikes additionally explode into prism shards and briefly stun enemies");
        }

        public override void SafeSetDefaults()
        {
            item.width = 46;
            item.damage = 24;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useAnimation = 16;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 16;
            item.knockBack = 5f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 46;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.rare = ItemRarityID.Pink;
            item.shoot = ModContent.ProjectileType<PrismallineProj>();
            item.shootSpeed = 16f;
            item.Calamity().rogue = true;
        }

        // Terraria seems to really dislike high crit values in SetDefaults
        public override void GetWeaponCrit(Player player, ref int crit) => crit += 4;

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.Calamity().StealthStrikeAvailable())
            {
                int proj = Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, (int)(damage * 1.15f), knockBack, player.whoAmI);
                if (proj.WithinBounds(Main.maxProjectiles))
                    Main.projectile[proj].Calamity().stealthStrike = true;
                return false;
            }
            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<Crystalline>());
            recipe.AddIngredient(ModContent.ItemType<MolluskHusk>(), 5);
            recipe.AddIngredient(ModContent.ItemType<SeaPrism>(), 5);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
