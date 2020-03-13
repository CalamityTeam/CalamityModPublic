using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class CorrosiveSpine : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Corrosive Spine");
            Tooltip.SetDefault("Increased movement speed\n" +
                               "All rogue weapons inflict venom and spawn clouds on enemy hits\n" +
                               "You release a ton of clouds everywhere on hit");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 46;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.rare = 5;
            item.defense = 4;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Calamity().corrosiveSpine = true;
            if (player.immune)
            {
                if (Main.rand.NextBool(9))
                {
                    for (int i = 0; i < 6; i++)
                    {
                        int type = -1;
                        switch (Main.rand.Next(3))
                        {
                            case 0:
                                type = ModContent.ProjectileType<Corrocloud1>();
                                break;
                            case 1:
                                type = ModContent.ProjectileType<Corrocloud2>();
                                break;
                            case 2:
                                type = ModContent.ProjectileType<Corrocloud3>();
                                break;
                        }
                        // Should never happen, but just in case-
                        if (type != -1)
                        {
                            float speed = Main.rand.NextFloat(3f, 11f);
                            Projectile.NewProjectile(player.Center, Vector2.One.RotatedByRandom(MathHelper.TwoPi) * speed,
                                type, 100, 0f, player.whoAmI);
                        }
                    }
                }
            }
        }
    }
}