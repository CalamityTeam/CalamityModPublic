using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Rogue;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class DeificThunderbolt : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Deific Thunderbolt");
            Tooltip.SetDefault(@"Fires a lightning bolt to electrocute enemies
The lightning bolt travels faster while it is raining
Summons lightning from the sky on impact
Stealth strikes summon more lightning and travel faster");
        }

        public override void SafeSetDefaults()
        {
            item.damage = 407;
            item.knockBack = 10f;

            item.width = 56;
            item.height = 56;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.noMelee = true;
            item.noUseGraphic = true;

            item.useTime = 21;
            item.useAnimation = 21;
            item.value = CalamityGlobalItem.Rarity12BuyPrice;
            item.rare = ItemRarityID.Purple;
            item.Calamity().customRarity = CalamityRarity.Turquoise;
            item.Calamity().rogue = true;

            item.autoReuse = true;
            item.shootSpeed = 13.69f;
            item.shoot = ModContent.ProjectileType<DeificThunderboltProj>();
        }

        // Terraria seems to really dislike high crit values in SetDefaults
        public override void GetWeaponCrit(Player player, ref int crit) => crit += 12;

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            float stealthSpeedMult = 1f;
            if (player.Calamity().StealthStrikeAvailable())
                stealthSpeedMult = 1.5f;
            float rainSpeedMult = 1f;
            if (Main.raining)
                rainSpeedMult = 1.5f;

            int thunder = Projectile.NewProjectile(position.X, position.Y, speedX * rainSpeedMult * stealthSpeedMult, speedY * rainSpeedMult * stealthSpeedMult, type, damage, knockBack, player.whoAmI, 0f, 0f);
            if (player.Calamity().StealthStrikeAvailable() && thunder.WithinBounds(Main.maxProjectiles)) //setting the stealth strike
            {
                Main.projectile[thunder].Calamity().stealthStrike = true;
            }
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<StormfrontRazor>());
            recipe.AddIngredient(ModContent.ItemType<ArmoredShell>(), 8);
            recipe.AddIngredient(ModContent.ItemType<UnholyEssence>(), 15);
            recipe.AddIngredient(ModContent.ItemType<CoreofCinder>(), 5);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
