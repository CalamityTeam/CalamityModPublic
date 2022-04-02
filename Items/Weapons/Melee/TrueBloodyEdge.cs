using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class TrueBloodyEdge : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("True Bloody Edge");
            Tooltip.SetDefault("Chance to heal the player on enemy hits\n" +
                "Inflicts Burning Blood\n" +
                "Fires a bloody blade");
        }

        public override void SetDefaults()
        {
            item.width = 48;
            item.damage = 75;
            item.melee = true;
            item.useAnimation = 20;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 20;
            item.knockBack = 6f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 64;
            item.scale = 1.5f;
            item.value = Item.buyPrice(0, 80, 0, 0);
            item.rare = ItemRarityID.Yellow;
            item.shoot = ModContent.ProjectileType<BloodyBlade>();
            item.shootSpeed = 11f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<BloodyEdge>());
            recipe.AddIngredient(ItemID.BrokenHeroSword);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(5))
                Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 5);
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Ichor, 60);
            target.AddBuff(ModContent.BuffType<BurningBlood>(), 60);
            OnHitEffects(player);
        }

        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Ichor, 60);
            target.AddBuff(ModContent.BuffType<BurningBlood>(), 60);
            OnHitEffects(player);
        }

        private void OnHitEffects(Player player)
        {
            if (player.moonLeech)
                return;

            int healAmount = Main.rand.Next(3) + 3;
            if (Main.rand.NextBool(2))
            {
                player.statLife += healAmount;
                player.HealEffect(healAmount);
            }
        }
    }
}
