using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Tools
{
    [LegacyName("ChaoswarpedSlashaxe")]
    public class TectonicTruncator : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Tectonic Truncator");
        }

        public override void SetDefaults()
        {
            Item.damage = 68;
            Item.knockBack = 7f;
            Item.useTime = 6;
            Item.useAnimation = 17;
            Item.axe = 190 / 5;
            Item.tileBoost += 2;

            Item.DamageType = DamageClass.Melee;
            Item.width = 50;
            Item.height = 50;
            Item.useTurn = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.value = CalamityGlobalItem.Rarity8BuyPrice;
            Item.rare = ItemRarityID.Yellow;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<ScoriaBar>(9).
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
            target.AddBuff(BuffID.OnFire3, 300);
        }
    }
}
