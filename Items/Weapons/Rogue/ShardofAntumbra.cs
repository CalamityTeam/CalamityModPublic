using Terraria.DataStructures;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    [LegacyName("XerocPitchfork")]
    public class ShardofAntumbra : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shard of Antumbra");
            Tooltip.SetDefault("Stealth strikes leave homing stars in their wake");
            SacrificeTotal = 99;
        }

        public override void SetDefaults()
        {
            Item.width = 48;
            Item.damage = 280;
            Item.noMelee = true;
            Item.consumable = true;
            Item.noUseGraphic = true;
            Item.useAnimation = 19;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 19;
            Item.knockBack = 8f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.height = 48;
            Item.maxStack = 999;
            Item.value = 10000;
            Item.rare = ItemRarityID.Cyan;
            Item.shoot = ModContent.ProjectileType<AntumbraShardProjectile>();
            Item.shootSpeed = 24f;
            Item.DamageType = RogueDamageClass.Instance;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.Calamity().StealthStrikeAvailable())
            {
                int stealth = Projectile.NewProjectile(source, position, velocity, type, (int)(damage * 1.5f), knockback, player.whoAmI);
                if (stealth.WithinBounds(Main.maxProjectiles))
                    Main.projectile[stealth].Calamity().stealthStrike = true;
            }
            return !player.Calamity().StealthStrikeAvailable();
        }

        public override void AddRecipes()
        {
            CreateRecipe(100).
                AddIngredient<MeldConstruct>().
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
