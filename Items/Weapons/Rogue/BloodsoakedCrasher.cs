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
            Item.damage = 245;
            Item.knockBack = 3f;
            Item.autoReuse = true;
            Item.Calamity().rogue = true;
            Item.useAnimation = Item.useTime = 24;
            Item.shootSpeed = 9f;
            Item.shoot = ModContent.ProjectileType<BloodsoakedCrashax>();

            Item.width = 66;
            Item.height = 64;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;
            Item.value = CalamityGlobalItem.Rarity12BuyPrice;
            Item.rare = ItemRarityID.Purple;
            Item.Calamity().customRarity = CalamityRarity.Turquoise;
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
            CreateRecipe(1).AddIngredient(ModContent.ItemType<CrushsawCrasher>()).AddIngredient(ModContent.ItemType<BloodstoneCore>(), 12).AddTile(TileID.LunarCraftingStation).Register();
        }
    }
}
