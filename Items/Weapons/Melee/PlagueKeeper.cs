using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Projectiles.Melee;
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
            item.damage = 90;
            item.melee = true;
            item.useAnimation = 18;
            item.useStyle = 1;
            item.useTime = 18;
            item.useTurn = true;
            item.knockBack = 6f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 90;
            item.value = Item.buyPrice(1, 20, 0, 0);
            item.rare = 10;
            item.shoot = ModContent.ProjectileType<PlagueBeeDust>();
            item.shootSpeed = 9f;
            item.Calamity().postMoonLordRarity = 12;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<VirulentKatana>());
            recipe.AddIngredient(ItemID.BeeKeeper);
            recipe.AddIngredient(ItemID.FragmentSolar, 10);
            recipe.AddIngredient(ItemID.LunarBar, 5);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Plague>(), 300);
            for (int i = 0; i < 3; i++)
            {
                int bee = Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, 0f, player.beeType(),
                    player.beeDamage(item.damage / 3), player.beeKB(0f), player.whoAmI, 0f, 0f);
                Main.projectile[bee].penetrate = 1;
                Main.projectile[bee].Calamity().forceMelee = true;
            }
        }
    }
}
