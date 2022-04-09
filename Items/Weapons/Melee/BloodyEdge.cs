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
            Item.width = 46;
            Item.damage = 70;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = 23;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 23;
            Item.knockBack = 5.25f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.useTurn = true;
            Item.height = 60;
            Item.scale = 1.25f;
            Item.value = Item.buyPrice(0, 4, 0, 0);
            Item.rare = ItemRarityID.Orange;
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

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.LightsBane).
                AddIngredient(ItemID.Muramasa).
                AddIngredient(ItemID.BladeofGrass).
                AddIngredient(ItemID.FieryGreatsword).
                AddTile(TileID.DemonAltar).
                Register();
            CreateRecipe().
                AddIngredient(ItemID.BloodButcherer).
                AddIngredient(ItemID.Muramasa).
                AddIngredient(ItemID.BladeofGrass).
                AddIngredient(ItemID.FieryGreatsword).
                AddTile(TileID.DemonAltar).
                Register();
        }
    }
}
