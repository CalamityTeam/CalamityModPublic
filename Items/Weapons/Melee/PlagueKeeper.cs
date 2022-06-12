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
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 74;
            Item.damage = 80;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 20;
            Item.useTurn = true;
            Item.knockBack = 6f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.height = 90;
            Item.value = CalamityGlobalItem.Rarity10BuyPrice;
            Item.rare = ItemRarityID.Red;
            Item.shoot = ModContent.ProjectileType<VirulentBeeWave>();
            Item.shootSpeed = 9f;
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            var source = player.GetSource_ItemUse(Item);
            if (crit)
                damage /= 2;

            target.AddBuff(ModContent.BuffType<Plague>(), 300);
            for (int i = 0; i < 3; i++)
            {
                int bee = Projectile.NewProjectile(source, player.Center, Vector2.Zero, player.beeType(),
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
            var source = player.GetSource_ItemUse(Item);
            if (crit)
                damage /= 2;

            target.AddBuff(ModContent.BuffType<Plague>(), 300);
            for (int i = 0; i < 3; i++)
            {
                int bee = Projectile.NewProjectile(source, player.Center, Vector2.Zero, player.beeType(),
                    player.beeDamage(damage / 3), player.beeKB(0f), player.whoAmI);
                if (bee.WithinBounds(Main.maxProjectiles))
                {
                    Main.projectile[bee].penetrate = 1;
                    Main.projectile[bee].Calamity().forceMelee = true;
                }
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<Virulence>().
                AddIngredient(ItemID.BeeKeeper).
                AddIngredient(ItemID.FragmentSolar, 10).
                AddIngredient<InfectedArmorPlating>(5).
                AddIngredient(ItemID.LunarBar, 5).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
