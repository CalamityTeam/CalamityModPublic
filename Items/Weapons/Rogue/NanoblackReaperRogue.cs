using Terraria.DataStructures;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Rogue;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class NanoblackReaperRogue : RogueWeapon
    {
        public static int BaseDamage = 130;
        public static float Knockback = 9f;
        public static float Speed = 16f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Nanoblack Reaper");
            Tooltip.SetDefault("Unleashes a storm of nanoblack energy blades\n" +
                "Blades target bosses whenever possible\n" +
                "Stealth strikes cause the scythe to create a large amount of homing afterimages instead of energy blades\n" +
                "'She smothered them in Her hatred'");
        }

        public override void SafeSetDefaults()
        {
            Item.width = 78;
            Item.height = 64;
            Item.damage = BaseDamage;
            Item.knockBack = Knockback;
            Item.useTime = 6;
            Item.useAnimation = 6;
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.noUseGraphic = true;

            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item18;

            Item.value = CalamityGlobalItem.Rarity16BuyPrice;
            Item.Calamity().customRarity = CalamityRarity.HotPink;
            Item.Calamity().devItem = true;

            Item.Calamity().rogue = true;
            Item.shoot = ModContent.ProjectileType<NanoblackMain>();
            Item.shootSpeed = Speed;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.Calamity().StealthStrikeAvailable())
            {
                damage = (int)(damage * 1.2);
                int stealth = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
                if (stealth.WithinBounds(Main.maxProjectiles))
                    Main.projectile[stealth].Calamity().stealthStrike = player.Calamity().StealthStrikeAvailable();
                return false;
            }
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ModContent.ItemType<GhoulishGouger>()).AddIngredient(ModContent.ItemType<MoltenAmputator>()).AddIngredient(ModContent.ItemType<EndothermicEnergy>(), 40).AddIngredient(ModContent.ItemType<PlagueCellCluster>(), 20).AddIngredient(ItemID.Nanites, 400).AddIngredient(ModContent.ItemType<ShadowspecBar>(), 5).AddTile(ModContent.TileType<DraedonsForge>()).Register();
        }
    }
}
