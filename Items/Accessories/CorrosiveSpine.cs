using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Accessories
{
    public class CorrosiveSpine : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            DisplayName.SetDefault("Corrosive Spine");
            Tooltip.SetDefault("10% increased movement speed\n" +
                               "All rogue weapons inflict venom and spawn clouds on enemy hits\n" +
                               "You release a ton of clouds everywhere on hit");
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 46;
            Item.value = Item.buyPrice(0, 36, 0, 0);
            Item.rare = ItemRarityID.Pink;
            Item.defense = 4;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.moveSpeed += 0.1f;
            player.Calamity().corrosiveSpine = true;
            if (player.immune)
            {
                if (Main.rand.NextBool(15))
                {
                    var source = player.GetSource_Accessory(Item);
                    int cloudCount = Main.rand.Next(2,5);
                    for (int i = 0; i < cloudCount; i++)
                    {
                        int type = Utils.SelectRandom(Main.rand, new int[]
                        {
                            ModContent.ProjectileType<Corrocloud1>(),
                            ModContent.ProjectileType<Corrocloud2>(),
                            ModContent.ProjectileType<Corrocloud3>()
                        });
                        float speed = Main.rand.NextFloat(3f, 11f);
                        Projectile.NewProjectile(source, player.Center, Vector2.One.RotatedByRandom(MathHelper.TwoPi) * speed,
                            type, (int)(100 * player.RogueDamage()), 0f, player.whoAmI);
                    }
                }
            }
        }
    }
}
