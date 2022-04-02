using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class BloodyEdge : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bloody Edge");
            Tooltip.SetDefault("Chance to heal the player on enemy hits\n" +
                "Inflicts Burning Blood");
        }

        public override void SetDefaults()
        {
            item.width = 46;
            item.damage = 70;
            item.melee = true;
            item.useAnimation = 23;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 23;
            item.knockBack = 5.25f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.useTurn = true;
            item.height = 60;
            item.scale = 1.25f;
            item.value = Item.buyPrice(0, 4, 0, 0);
            item.rare = ItemRarityID.Orange;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.LightsBane);
            recipe.AddIngredient(ItemID.Muramasa);
            recipe.AddIngredient(ItemID.BladeofGrass);
            recipe.AddIngredient(ItemID.FieryGreatsword);
            recipe.AddTile(TileID.DemonAltar);
            recipe.SetResult(this);
            recipe.AddRecipe();
            recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.BloodButcherer);
            recipe.AddIngredient(ItemID.Muramasa);
            recipe.AddIngredient(ItemID.BladeofGrass);
            recipe.AddIngredient(ItemID.FieryGreatsword);
            recipe.AddTile(TileID.DemonAltar);
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
            target.AddBuff(ModContent.BuffType<BurningBlood>(), 60);

            if (!target.canGhostHeal || player.moonLeech)
                return;

            int healAmount = Main.rand.Next(2) + 2;
            player.statLife += healAmount;
            player.HealEffect(healAmount);
        }

        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<BurningBlood>(), 60);

            int healAmount = Main.rand.Next(2) + 2;
            if (!player.moonLeech)
            {
                player.statLife += healAmount;
                player.HealEffect(healAmount);
            }
        }
    }
}
