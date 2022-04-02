using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class StarofDestruction : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Star of Destruction");
            Tooltip.SetDefault("Fires a huge destructive mine that explodes into destruction bolts\n" +
            "Amount of bolts scales with enemies hit, up to 16\n" +
            "Stealth strikes always explode into the max amount of bolts");
        }

        public override void SafeSetDefaults()
        {
            item.width = item.height = 94;
            item.damage = 150;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useAnimation = 38;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 38;
            item.knockBack = 10f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.value = Item.buyPrice(0, 95, 0, 0);
            item.rare = ItemRarityID.Cyan;
            item.shoot = ModContent.ProjectileType<DestructionStar>();
            item.shootSpeed = 5f;
            item.Calamity().rogue = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.Calamity().StealthStrikeAvailable()) //setting the stealth strike
            {
                int stealth = Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, (int)(damage * 0.8f), knockBack, player.whoAmI, 0f, 1f);
                if (stealth.WithinBounds(Main.maxProjectiles))
                    Main.projectile[stealth].Calamity().stealthStrike = true;
                return false;
            }
            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<MeldiateBar>(), 10);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
