using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Tools
{
    public class FlamebeakHampick : ModItem
    {
        private static int PickPower = 210;
        private static int HammerPower = 130;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Flamebeak Hampick");
            Tooltip.SetDefault(@"Capable of mining Lihzahrd Bricks
Left click to use as a pickaxe
Right click to use as a hammer");
        }

        public override void SetDefaults()
        {
            item.damage = 58;
            item.melee = true;
            item.width = 52;
            item.height = 50;
            item.useTime = 6;
            item.useAnimation = 15;
            item.useTurn = true;
            item.pick = PickPower;
            item.hammer = HammerPower;
            item.useStyle = 1;
            item.knockBack = 3.5f;
            item.value = Item.buyPrice(0, 80, 0, 0);
            item.rare = 8;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.tileBoost += 2;
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                item.pick = 0;
				item.hammer = HammerPower;
            }
            else
            {
                item.pick = PickPower;
				item.hammer = 0;
            }
            return base.CanUseItem(player);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<CruptixBar>(), 7);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(5))
            {
                int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 127);
            }
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 300);
        }
    }
}
