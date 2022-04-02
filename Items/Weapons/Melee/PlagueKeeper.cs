using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class PlagueKeeper : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Plague Keeper");
            Tooltip.SetDefault("Fires a plague and bee cloud");
        }

        public override void SetDefaults()
        {
            item.width = 74;
            item.damage = 80;
            item.melee = true;
            item.useAnimation = 20;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 20;
            item.useTurn = true;
            item.knockBack = 6f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 90;
            item.value = CalamityGlobalItem.Rarity10BuyPrice;
            item.rare = ItemRarityID.Red;
            item.shoot = ModContent.ProjectileType<PlagueBeeDust>();
            item.shootSpeed = 9f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<VirulentKatana>());
            recipe.AddIngredient(ItemID.BeeKeeper);
            recipe.AddIngredient(ItemID.FragmentSolar, 10);
            recipe.AddIngredient(ModContent.ItemType<InfectedArmorPlating>(), 5);
            recipe.AddIngredient(ItemID.LunarBar, 5);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            if (crit)
                damage /= 2;

            target.AddBuff(ModContent.BuffType<Plague>(), 300);
            for (int i = 0; i < 3; i++)
            {
                int bee = Projectile.NewProjectile(player.Center, Vector2.Zero, player.beeType(),
                    player.beeDamage(damage / 3), player.beeKB(0f), player.whoAmI);
                if (bee.WithinBounds(Main.maxProjectiles))
                {
                    Main.projectile[bee].penetrate = 1;
                    Main.projectile[bee].Calamity().forceMelee = true;
                }
            }
        }

        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
            if (crit)
                damage /= 2;

            target.AddBuff(ModContent.BuffType<Plague>(), 300);
            for (int i = 0; i < 3; i++)
            {
                int bee = Projectile.NewProjectile(player.Center, Vector2.Zero, player.beeType(),
                    player.beeDamage(damage / 3), player.beeKB(0f), player.whoAmI);
                if (bee.WithinBounds(Main.maxProjectiles))
                {
                    Main.projectile[bee].penetrate = 1;
                    Main.projectile[bee].Calamity().forceMelee = true;
                }
            }
        }
    }
}
