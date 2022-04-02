using CalamityMod.CalPlayer;
using CalamityMod.Projectiles.Melee;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class BalefulHarvester : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Baleful Harvester");
            Tooltip.SetDefault("Summons flaming pumpkins and skulls that split into homing fire orbs on enemy hits");
        }

        public override void SetDefaults()
        {
            item.damage = 90;
            item.width = 66;
            item.height = 66;
            item.scale = 1.5f;
            item.melee = true;
            item.useAnimation = 22;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 22;
            item.useTurn = true;
            item.knockBack = 8f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.value = CalamityGlobalItem.Rarity10BuyPrice;
            item.rare = ItemRarityID.Red;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.TheHorsemansBlade);
            recipe.AddIngredient(ItemID.BookofSkulls);
            recipe.AddIngredient(ItemID.FragmentSolar, 20);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            if (crit)
                damage /= 2;

            int type = Main.rand.NextBool() ? ModContent.ProjectileType<BalefulHarvesterProjectile>() : ProjectileID.FlamingJack;
            CalamityPlayer.HorsemansBladeOnHit(player, target.whoAmI, (int)(damage * 1.5f), knockback, 0, type);
            target.AddBuff(BuffID.OnFire, 300);
        }

        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
            if (crit)
                damage /= 2;

            int type = Main.rand.NextBool() ? ModContent.ProjectileType<BalefulHarvesterProjectile>() : ProjectileID.FlamingJack;
            CalamityPlayer.HorsemansBladeOnHit(player, -1, (int)(damage * 1.5f), item.knockBack, 0, type);
            target.AddBuff(BuffID.OnFire, 300);
        }
    }
}
