using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class DuststormInABottle : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Duststorm in a Bottle");
            Tooltip.SetDefault("Explodes into a dust cloud\n" +
            "Stealth strikes form a more intense and longer lasting dust cloud");
        }

        public override void SafeSetDefaults()
        {
            item.width = 20;
            item.damage = 47;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useAnimation = 25;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 25;
            item.knockBack = 5f;
            item.UseSound = SoundID.Item106;
            item.autoReuse = true;
            item.height = 24;
            item.value = Item.buyPrice(0, 60, 0, 0);
            item.rare = ItemRarityID.Lime;
            item.shoot = ModContent.ProjectileType<DuststormInABottleProj>();
            item.shootSpeed = 12f;
            item.Calamity().rogue = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.Calamity().StealthStrikeAvailable())
            {
                int stealth = Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI);
                if (stealth.WithinBounds(Main.maxProjectiles))
                    Main.projectile[stealth].Calamity().stealthStrike = true;
                return false;
            }
            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.SandstorminaBottle);
            recipe.AddIngredient(ItemID.HolyWater, 20);
            recipe.AddIngredient(ModContent.ItemType<GrandScale>());
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
