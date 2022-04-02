using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class BloodsoakedCrasher : RogueWeapon //This weapon has been coded by Ben || Termi
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bloodsoaked Crasher");
            Tooltip.SetDefault("Slows down when hitting an enemy. Speeds up otherwise\n" +
            "Heals on enemy hits\n" +
            "Stealth strikes spawn homing blood on enemy hits");
        }

        public override void SafeSetDefaults()
        {
            item.damage = 245;
            item.knockBack = 3f;
            item.autoReuse = true;
            item.Calamity().rogue = true;
            item.useAnimation = item.useTime = 24;
            item.shootSpeed = 9f;
            item.shoot = ModContent.ProjectileType<BloodsoakedCrashax>();

            item.width = 66;
            item.height = 64;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.UseSound = SoundID.Item1;
            item.value = CalamityGlobalItem.Rarity12BuyPrice;
            item.rare = ItemRarityID.Purple;
            item.Calamity().customRarity = CalamityRarity.Turquoise;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            int proj = Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI);
            if (proj.WithinBounds(Main.maxProjectiles))
                Main.projectile[proj].Calamity().stealthStrike = player.Calamity().StealthStrikeAvailable();
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<CrushsawCrasher>());
            recipe.AddIngredient(ModContent.ItemType<BloodstoneCore>(), 12);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
