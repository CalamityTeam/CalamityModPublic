using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class XerocPitchfork : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shard of Antumbra");
            Tooltip.SetDefault("Stealth strikes leave homing stars in their wake");
        }

        public override void SafeSetDefaults()
        {
            item.width = 48;
            item.damage = 280;
            item.noMelee = true;
            item.consumable = true;
            item.noUseGraphic = true;
            item.useAnimation = 19;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 19;
            item.knockBack = 8f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 48;
            item.maxStack = 999;
            item.value = 10000;
            item.rare = ItemRarityID.Cyan;
            item.shoot = ModContent.ProjectileType<AntumbraShardProjectile>();
            item.shootSpeed = 24f;
            item.Calamity().rogue = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.Calamity().StealthStrikeAvailable())
            {
                int stealth = Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, (int)(damage * 1.5f), knockBack, player.whoAmI);
                if (stealth.WithinBounds(Main.maxProjectiles))
                    Main.projectile[stealth].Calamity().stealthStrike = true;
            }
            return !player.Calamity().StealthStrikeAvailable();
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<MeldiateBar>());
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this, 100);
            recipe.AddRecipe();
        }
    }
}
