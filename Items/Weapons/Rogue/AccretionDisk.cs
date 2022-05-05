using Terraria.DataStructures;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class AccretionDisk : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Elemental Disk");
            Tooltip.SetDefault("Throws a disk that has a chance to generate several disks if enemies are near it\n" +
            "Stealth strikes fly slower but travel farther, pierce through enemies, and spawn extra disks more frequently");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SafeSetDefaults()
        {
            Item.width = 38;
            Item.damage = 100;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 15;
            Item.knockBack = 9f;
            Item.UseSound = SoundID.Item1;
            Item.height = 38;
            Item.value = CalamityGlobalItem.Rarity11BuyPrice;
            Item.rare = ItemRarityID.Purple;
            Item.shoot = ModContent.ProjectileType<AccretionDiskProj>();
            Item.shootSpeed = 13f;
            Item.Calamity().rogue = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.Calamity().StealthStrikeAvailable())
            {
                velocity.X *= 0.7f;
                velocity.Y *= 0.7f;
                int proj = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
                if (proj.WithinBounds(Main.maxProjectiles))
                {
                    Main.projectile[proj].Calamity().stealthStrike = true;
                    Main.projectile[proj].timeLeft *= 3;
                    Main.projectile[proj].localNPCHitCooldown *= 2;
                }
                return false;
            }
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<MangroveChakram>().
                AddIngredient<FlameScythe>().
                AddIngredient<TerraDisk>().
                AddIngredient<GalacticaSingularity>(5).
                AddIngredient<BarofLife>(5).
                AddIngredient(ItemID.LunarBar, 5).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
