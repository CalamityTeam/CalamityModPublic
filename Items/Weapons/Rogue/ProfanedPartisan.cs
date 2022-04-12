using Terraria.DataStructures;
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
            Item.damage = 322;
            Item.knockBack = 8f;

            Item.width = 56;
            Item.height = 56;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.noUseGraphic = true;

            Item.useTime = 18;
            Item.useAnimation = 18;
            Item.UseSound = SoundID.Item1;
            Item.value = CalamityGlobalItem.Rarity12BuyPrice;
            Item.rare = ItemRarityID.Purple;
            Item.Calamity().customRarity = CalamityRarity.Turquoise;
            Item.Calamity().rogue = true;

            Item.autoReuse = true;
            Item.shootSpeed = 6f;
            Item.shoot = ModContent.ProjectileType<ProfanedPartisanProj>();
        }

        // Terraria seems to really dislike high crit values in SetDefaults
        public override void ModifyWeaponCrit(Player player, ref float crit) => crit += 15;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.Calamity().StealthStrikeAvailable()) //setting the stealth strike
            {
                int stealth = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
                if (stealth.WithinBounds(Main.maxProjectiles))
                    Main.projectile[stealth].Calamity().stealthStrike = true;
                return false;
            }
            return true;
        }

        public override void AddRecipes()
        {

            CreateRecipe(1).AddIngredient(ModContent.ItemType<CrystalPiercer>(), 500).AddIngredient(ModContent.ItemType<UeliaceBar>(), 6).AddIngredient(ModContent.ItemType<DivineGeode>(), 4).AddIngredient(ModContent.ItemType <UnholyEssence>(), 25).AddTile(TileID.LunarCraftingStation).Register();
        }
    }
}
