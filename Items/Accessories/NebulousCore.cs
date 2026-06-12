using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.CalPlayer;
using CalamityMod.Projectiles.Typeless;
using CalamityMod.Rarities;
using CalamityMod.Systems.Collections;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class NebulousCore : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public override void SetStaticDefaults()
        {
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [ModContent.BuffType<GodSlayerInferno>()];
        }
        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 14;
            Item.accessory = true;
            Item.value = CalamityGlobalItem.RarityDarkBlueBuyPrice;
            Item.rare = ModContent.RarityType<CosmicPurple>();
            Item.expert = true;
        }

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            float projLighting = Main.rand.Next(90, 111) * 0.01f;
            projLighting *= Main.essScale;
            Lighting.AddLight((int)((Item.position.X + (float)(Item.width / 2)) / 16f), (int)((Item.position.Y + (float)(Item.height / 2)) / 16f), 0.35f * projLighting, 0.05f * projLighting, 0.35f * projLighting);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.nebulousCore = true;
            player.GetDamage<GenericDamageClass>() += 0.1f;

            // Spawn nebula stars
            if (Main.rand.NextBool(15))
            {
                // Count the number of current active nebula stars; if this is at least 10, no more can spawn
                int numProj = 0;
                foreach (Projectile p in Main.ActiveProjectiles)
                {
                    if (p.owner == player.whoAmI && p.type == ModContent.ProjectileType<NebulaStar>())
                    {
                        numProj++;
                    }
                }
                if (Main.rand.Next(15) >= numProj && numProj < 10)
                {
                    int spawnRadius = 24;
                    for (int j = 0; j < 50; j++) // Attempt to spawn the star randomly around the player
                    {
                        float randomProjOffset = Main.rand.NextFloat(200 - j * 2, 400 + j * 2);
                        Vector2 center = player.Center;
                        center.X += Main.rand.NextFloat(-randomProjOffset, randomProjOffset + 1);
                        center.Y += Main.rand.NextFloat(-randomProjOffset, randomProjOffset + 1);
                        // Ensure we are not trying to spawn the star on top of a solid tile or liquid
                        if (!Collision.SolidCollision(center, spawnRadius, spawnRadius) && !Collision.WetCollision(center, spawnRadius, spawnRadius))
                        {
                            center.X += (float)(spawnRadius / 2);
                            center.Y += (float)(spawnRadius / 2);
                            // Ensure the star's spawn point has line-of-sight with the player
                            if (Collision.CanHit(player.Center, 1, 1, center, 1, 1) || Collision.CanHit(player.Center - new Vector2(0f, 50f), 1, 1, center, 1, 1))
                            {
                                if (Main.rand.NextBool(3) && Main.myPlayer == player.whoAmI)
                                {
                                    var source = player.GetSource_Accessory(Item);
                                    int damage = (int)player.GetBestClassDamage().ApplyTo(250);
                                    float knockBack = 3f;

                                    Projectile.NewProjectile(source, center, Vector2.Zero, ModContent.ProjectileType<NebulaStar>(), damage, knockBack, player.whoAmI);
                                    return;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
