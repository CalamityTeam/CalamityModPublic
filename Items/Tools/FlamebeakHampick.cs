using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Tools
{
    public class FlamebeakHampick : ModItem
    {
        private const int PickPower = 210;
        private const int HammerPower = 95;

        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            DisplayName.SetDefault("Seismic Hampick");
            Tooltip.SetDefault(@"Capable of mining Lihzahrd Bricks
Left click to use as a pickaxe
Right click to use as a hammer");
        }

        public override void SetDefaults()
        {
            Item.damage = 58;
            Item.knockBack = 8f;
            Item.useTime = 6;
            Item.useAnimation = 15;
            Item.pick = PickPower;
            Item.hammer = HammerPower;
            Item.tileBoost += 2;

            Item.DamageType = DamageClass.Melee;
            Item.width = 52;
            Item.height = 50;
            Item.useTurn = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.value = Item.buyPrice(0, 80, 0, 0);
            Item.rare = ItemRarityID.Yellow;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                Item.pick = 0;
                Item.hammer = HammerPower;
            }
            else
            {
                Item.pick = PickPower;
                Item.hammer = 0;
            }
            return base.CanUseItem(player);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<CruptixBar>(7).
                AddTile(TileID.MythrilAnvil).
                Register();
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(5))
            {
                Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, Main.rand.NextBool(3) ? 16 : 127);
            }
            if (Main.rand.NextBool(5) && Main.netMode != NetmodeID.Server)
            {
                int smoke = Gore.NewGore(player.GetSource_ItemUse(Item), new Vector2(hitbox.X, hitbox.Y), default, Main.rand.Next(375, 378), 0.75f);
                Main.gore[smoke].behindTiles = true;
            }
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 300);
        }
    }
}
