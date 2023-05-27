using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class PlagueKeeper : ModItem
    {
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

        public override void ModifyHitNPC(Player player, NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.CritDamage *= 0.5f;
        }
        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            var source = player.GetSource_ItemUse(Item);

            target.AddBuff(ModContent.BuffType<Plague>(), 300);
            for (int i = 0; i < 3; i++)
            {
                int bee = Projectile.NewProjectile(source, player.Center, Vector2.Zero, player.beeType(),
                    player.beeDamage(Item.damage / 3), player.beeKB(0f), player.whoAmI);
                if (bee.WithinBounds(Main.maxProjectiles))
                {
                    Main.projectile[bee].penetrate = 1;
                    Main.projectile[bee].DamageType = DamageClass.Melee;
                }
            }
        }

        public override void OnHitPvp(Player player, Player target, Player.HurtInfo hurtInfo)
        {
            var source = player.GetSource_ItemUse(Item);

            target.AddBuff(ModContent.BuffType<Plague>(), 300);
            for (int i = 0; i < 3; i++)
            {
                int bee = Projectile.NewProjectile(source, player.Center, Vector2.Zero, player.beeType(),
                    player.beeDamage(Item.damage / 3), player.beeKB(0f), player.whoAmI);
                if (bee.WithinBounds(Main.maxProjectiles))
                {
                    Main.projectile[bee].penetrate = 1;
                    Main.projectile[bee].DamageType = DamageClass.Melee;
                }
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<Virulence>().
                AddIngredient(ItemID.BeeKeeper).
                AddIngredient(ItemID.LunarBar, 5).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
