using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Rogue;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class ProfanedPartisan : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Profaned Partisan");
            Tooltip.SetDefault(@"Fires an unholy spear that explodes on death
Stealth strikes spawn smaller spears to fly along side it");
        }

        public override void SafeSetDefaults()
        {
            item.damage = 280;
            item.knockBack = 8f;

            item.width = 56;
            item.height = 56;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.noMelee = true;
            item.noUseGraphic = true;

            item.useTime = 18;
            item.useAnimation = 18;
            item.UseSound = SoundID.Item1;
            item.value = CalamityGlobalItem.Rarity12BuyPrice;
            item.rare = ItemRarityID.Purple;
            item.Calamity().customRarity = CalamityRarity.Turquoise;
            item.Calamity().rogue = true;

            item.autoReuse = true;
            item.shootSpeed = 6f;
            item.shoot = ModContent.ProjectileType<ProfanedPartisanProj>();
        }

        // Terraria seems to really dislike high crit values in SetDefaults
        public override void GetWeaponCrit(Player player, ref int crit) => crit += 15;

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.Calamity().StealthStrikeAvailable()) //setting the stealth strike
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
            recipe.AddIngredient(ModContent.ItemType<CrystalPiercer>(), 200);
            recipe.AddIngredient(ModContent.ItemType<UeliaceBar>(), 6);
            recipe.AddIngredient(ModContent.ItemType<DivineGeode>(), 4);
            recipe.AddIngredient(ModContent.ItemType <UnholyEssence>(), 25);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
