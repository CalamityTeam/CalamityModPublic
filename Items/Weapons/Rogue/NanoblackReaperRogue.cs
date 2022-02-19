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
            item.width = 78;
            item.height = 64;
            item.damage = BaseDamage;
            item.knockBack = Knockback;
            item.useTime = 6;
            item.useAnimation = 6;
            item.autoReuse = true;
            item.noMelee = true;
            item.noUseGraphic = true;

            item.useStyle = ItemUseStyleID.SwingThrow;
            item.UseSound = SoundID.Item18;

            item.value = CalamityGlobalItem.Rarity16BuyPrice;
            item.Calamity().customRarity = CalamityRarity.HotPink;
            item.Calamity().devItem = true;

            item.Calamity().rogue = true;
            item.shoot = ModContent.ProjectileType<NanoblackMain>();
            item.shootSpeed = Speed;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.Calamity().StealthStrikeAvailable())
            {
                damage = (int)(damage * 1.2);
                int stealth = Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI);
                if (stealth.WithinBounds(Main.maxProjectiles))
                    Main.projectile[stealth].Calamity().stealthStrike = player.Calamity().StealthStrikeAvailable();
                return false;
            }
            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe r = new ModRecipe(mod);
            r.SetResult(this);
            r.AddIngredient(ModContent.ItemType<GhoulishGouger>());
            r.AddIngredient(ModContent.ItemType<MoltenAmputator>());
            r.AddIngredient(ModContent.ItemType<EndothermicEnergy>(), 40);
            r.AddIngredient(ModContent.ItemType<PlagueCellCluster>(), 20);
            r.AddIngredient(ItemID.Nanites, 400);
            r.AddIngredient(ModContent.ItemType<ShadowspecBar>(), 5);
            r.AddTile(ModContent.TileType<DraedonsForge>());
            r.AddRecipe();
        }
    }
}
