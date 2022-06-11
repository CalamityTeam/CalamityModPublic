using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class CrimsonCrusherBlade : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crimson Crusher Blade");
            Tooltip.SetDefault("Inflicts ichor and critical hits lower enemy contact damage");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 40;
            Item.DamageType = DamageClass.Melee;
            Item.width = 70;
            Item.height = 80;
            Item.scale = 1.25f;
            Item.useTime = 28;
            Item.useAnimation = 28;
            Item.useTurn = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 7f;
            Item.value = CalamityGlobalItem.Rarity2BuyPrice;
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(7))
            {
                int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 5);
            }
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Ichor, 120);
            if (target.damage > 0 && crit && !CalamityPlayer.areThereAnyDamnBosses)
            {
                target.damage = target.defDamage - 5;
                if (target.damage < 1)
                    target.damage = 1;
            }
        }

        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Ichor, 120);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<BlightedGel>(15).
                AddIngredient(ItemID.CrimstoneBlock, 50).
                AddIngredient(ItemID.TissueSample, 5).
                AddIngredient(ItemID.IronBar, 4).
                AddTile(TileID.DemonAltar).
                Register();
        }
    }
}
