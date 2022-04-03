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
            Item.damage = 466;
            Item.knockBack = 10f;

            Item.width = 56;
            Item.height = 56;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.noUseGraphic = true;

            Item.useTime = 21;
            Item.useAnimation = 21;
            Item.value = CalamityGlobalItem.Rarity12BuyPrice;
            Item.rare = ItemRarityID.Purple;
            Item.Calamity().customRarity = CalamityRarity.Turquoise;
            Item.Calamity().rogue = true;

            Item.autoReuse = true;
            Item.shootSpeed = 13.69f;
            Item.shoot = ModContent.ProjectileType<DeificThunderboltProj>();
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
            CreateRecipe(1).AddIngredient(ModContent.ItemType<StormfrontRazor>()).AddIngredient(ModContent.ItemType<ArmoredShell>(), 8).AddIngredient(ModContent.ItemType<UnholyEssence>(), 15).AddIngredient(ModContent.ItemType<CoreofCinder>(), 5).AddTile(TileID.LunarCraftingStation).Register();
        }
    }
}
